// MaintenanceTaskAppService.cs
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using DeviceManagementSystem.Attachment;
using DeviceManagementSystem.Attachment.Dto;
using DeviceManagementSystem.BasicDataManagement;
using DeviceManagementSystem.DeviceInfos;
using DeviceManagementSystem.Maintenances.Dto;
using DeviceManagementSystem.Maintenances.Interface;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances
{
    /// <summary>
    /// 保养工单服务实现
    /// </summary>
    public class MaintenanceTaskAppService : DeviceManagementSystemAppServiceBase, IMaintenanceTaskAppService
    {
        private readonly IRepository<MaintenanceTasks, Guid> _taskRepository;
        private readonly IRepository<MaintenanceTaskItems, Guid> _taskItemRepository;
        private readonly IRepository<MaintenanceTaskGroups, Guid> _taskGroupRepository;
        private readonly IRepository<MaintenancePlans, Guid> _planRepository;
        private readonly IRepository<MaintenanceTemplates, Guid> _templateRepository;
        private readonly IRepository<MaintenanceStandards, Guid> _standardRepository;
        private readonly IRepository<Devices, Guid> _deviceRepository;
        private readonly IRepository<DeviceUserRelations, Guid> _deviceUserRelationRepository;
        private readonly IUserAppService _userAppService;
        private readonly IRepository<MaintenanceItems, Guid> _itemRepository;
        private readonly IRepository<Types, Guid> _typeRepository;
        private readonly IAttachmentAppService _attachmentAppService;

        // 工单状态常量
        private const string TASK_STATUS_PLAN = "计划";
        private const string TASK_STATUS_PENDING = "待执行";
        private const string TASK_STATUS_EXECUTING = "执行中";
        private const string TASK_STATUS_COMPLETED = "已完成";
        private const string TASK_STATUS_CANCELLED = "已取消";
        private const string TASK_STATUS_DELEGATED = "已委派";


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="taskRepository"></param>
        /// <param name="taskItemRepository"></param>
        /// <param name="taskGroupRepository"></param>
        /// <param name="planRepository"></param>
        /// <param name="templateRepository"></param>
        /// <param name="standardRepository"></param>
        /// <param name="deviceRepository"></param>
        /// <param name="deviceUserRelationRepository"></param>
        /// <param name="userAppService"></param>
        /// <param name="itemRepository"></param>
        /// <param name="typeRepository"></param>
        /// <param name="attachmentAppService"></param>
        public MaintenanceTaskAppService(
            IRepository<MaintenanceTasks, Guid> taskRepository,
            IRepository<MaintenanceTaskItems, Guid> taskItemRepository,
            IRepository<MaintenanceTaskGroups, Guid> taskGroupRepository,
            IRepository<MaintenancePlans, Guid> planRepository,
            IRepository<MaintenanceTemplates, Guid> templateRepository,
            IRepository<MaintenanceStandards, Guid> standardRepository,
            IRepository<Devices, Guid> deviceRepository,
            IRepository<DeviceUserRelations, Guid> deviceUserRelationRepository,
            IUserAppService userAppService,
            IRepository<MaintenanceItems, Guid> itemRepository,
            IRepository<Types, Guid> typeRepository,
            IAttachmentAppService attachmentAppService)
        {
            _taskRepository = taskRepository;
            _taskItemRepository = taskItemRepository;
            _taskGroupRepository = taskGroupRepository;
            _planRepository = planRepository;
            _templateRepository = templateRepository;
            _standardRepository = standardRepository;
            _deviceRepository = deviceRepository;
            _deviceUserRelationRepository = deviceUserRelationRepository;
            _userAppService = userAppService;
            _itemRepository = itemRepository;
            _typeRepository = typeRepository;
            _attachmentAppService = attachmentAppService;
        }

        #region 查询方法

        /// <summary>
        /// 获取工单分页列表
        /// </summary>
        public async Task<CommonResult<Page<MaintenanceTaskDto>>> GetPageList([FromQuery] MaintenanceTaskPageInput input)
        {
            try
            {
                if (input.Size > 100) input.Size = 100;

                var query = from t in _taskRepository.GetAll().AsNoTracking()
                            join d in _deviceRepository.GetAll().AsNoTracking() on t.DeviceId equals d.Id into deviceJoin
                            from d in deviceJoin.DefaultIfEmpty()
                            join p in _planRepository.GetAll().AsNoTracking() on t.PlanId equals p.Id into planJoin
                            from p in planJoin.DefaultIfEmpty()
                            join tm in _templateRepository.GetAll().AsNoTracking() on t.TemplateId equals tm.Id into templateJoin
                            from tm in templateJoin.DefaultIfEmpty()
                            select new { Task = t, Device = d, Plan = p, Template = tm };

                // 应用过滤条件
                if (!string.IsNullOrWhiteSpace(input.SearchKey))
                {
                    query = query.Where(x =>
                        (x.Device != null && (x.Device.DeviceName.Contains(input.SearchKey) || x.Device.DeviceCode.Contains(input.SearchKey))) ||
                        x.Task.TaskNo.Contains(input.SearchKey) ||
                        x.Task.TaskName.Contains(input.SearchKey));
                }

                if (input.DeviceId.HasValue)
                {
                    query = query.Where(x => x.Task.DeviceId == input.DeviceId.Value);
                }

                if (!string.IsNullOrWhiteSpace(input.MaintenanceLevel))
                {
                    query = query.Where(x => x.Task.MaintenanceLevel == input.MaintenanceLevel);
                }

                if (!string.IsNullOrWhiteSpace(input.Status))
                {
                    query = query.Where(x => x.Task.Status == input.Status);
                }

                if (input.StartDateBegin.HasValue)
                {
                    query = query.Where(x => x.Task.PlanStartDate >= input.StartDateBegin.Value);
                }

                if (input.StartDateEnd.HasValue)
                {
                    var endDate = input.StartDateEnd.Value.AddDays(1).AddSeconds(-1);
                    query = query.Where(x => x.Task.PlanStartDate <= endDate);
                }

                if (input.OnlyMyPending == true)
                {
                    var userId = AbpSession.UserId.Value;
                    query = query.Where(x => x.Task.ExecutorIds.Contains(userId.ToString()) &&
                                            (x.Task.Status == TASK_STATUS_PENDING ||
                                             x.Task.Status == TASK_STATUS_EXECUTING ||
                                             x.Task.Status == TASK_STATUS_PLAN));
                }

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Task.PlanStartDate)
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var result = new List<MaintenanceTaskDto>();

                foreach (var item in items)
                {
                    var dto = await MapToDto(item.Task, item.Device, item.Template);
                    result.Add(dto);
                }

                var page = new Page<MaintenanceTaskDto>(input.Current, input.Size, total)
                {
                    Records = result
                };

                return CommonResult<Page<MaintenanceTaskDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取工单分页列表失败", ex);
                return CommonResult<Page<MaintenanceTaskDto>>.Error($"获取工单分页列表失败: {ex.Message}");
            }
        }







        /// <summary>
        /// 获取我的待办工单（按组整合）
        /// </summary>
        public async Task<CommonResult<List<PendingTaskGroupDto>>> GetMyPendingTasks(long? executorId = null)
        {
            try
            {
                var userId = executorId ?? AbpSession.UserId.Value;
                var userIdStr = userId.ToString();

                // 获取用户待办工单
                var tasks = await _taskRepository.GetAll()
                    .Where(x => x.ExecutorIds.Contains(userIdStr) &&
                               (x.Status == TASK_STATUS_PLAN ||
                                x.Status == TASK_STATUS_PENDING ||
                                x.Status == TASK_STATUS_EXECUTING))
                    .OrderBy(x => x.PlanStartDate)
                    .ToListAsync();

                // 按组ID分组
                var groupedTasks = tasks
                    .Where(x => x.GroupId.HasValue)
                    .GroupBy(x => x.GroupId.Value)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var ungroupedTasks = tasks.Where(x => !x.GroupId.HasValue).ToList();

                var result = new List<PendingTaskGroupDto>();

                // 处理有组的任务
                foreach (var group in groupedTasks)
                {
                    var groupInfo = await _taskGroupRepository.FirstOrDefaultAsync(group.Key);
                    if (groupInfo == null) continue;

                    var groupDto = new PendingTaskGroupDto
                    {
                        GroupId = group.Key,
                        GroupNo = groupInfo.GroupNo,
                        GroupName = groupInfo.GroupName,
                        PlanStartDate = groupInfo.PlanStartDate,
                        PlanEndDate = groupInfo.PlanEndDate,
                        DeviceCount = group.Value.Select(t => t.DeviceId).Distinct().Count(),
                        Tasks = new List<MaintenanceTaskDto>()
                    };

                    foreach (var task in group.Value)
                    {
                        var device = await _deviceRepository.FirstOrDefaultAsync(task.DeviceId);
                        var template = await _templateRepository.FirstOrDefaultAsync(task.TemplateId);
                        var taskDto = await MapToDto(task, device, template);

                        // 获取工单项目
                        var items = await _taskItemRepository.GetAll()
                            .Where(x => x.TaskId == task.Id)
                            .OrderBy(x => x.SortOrder)
                            .Select(x => new MaintenanceTaskItemDto
                            {
                                Id = x.Id,
                                TaskId = x.TaskId,
                                ItemId = x.ItemId,
                                ItemName = x.ItemName,
                                MaintenanceMethod = x.MaintenanceMethod,
                                Content = x.Content,
                                StandardValue = x.StandardValue,
                                Result = x.Result,
                                ActualValue = x.ActualValue,
                                Remark = x.Remark,
                                SortOrder = x.SortOrder
                            })
                            .ToListAsync();

                        taskDto.Items = items;
                        groupDto.Tasks.Add(taskDto);
                    }

                    result.Add(groupDto);
                }

                // 处理无组的任务
                if (ungroupedTasks.Any())
                {
                    var ungroupedGroup = new PendingTaskGroupDto
                    {
                        GroupId = Guid.Empty,
                        GroupNo = "SINGLE",
                        GroupName = "独立任务",
                        PlanStartDate = ungroupedTasks.Min(x => x.PlanStartDate),
                        PlanEndDate = ungroupedTasks.Max(x => x.PlanEndDate),
                        DeviceCount = ungroupedTasks.Select(t => t.DeviceId).Distinct().Count(),
                        Tasks = new List<MaintenanceTaskDto>()
                    };

                    foreach (var task in ungroupedTasks)
                    {
                        var device = await _deviceRepository.FirstOrDefaultAsync(task.DeviceId);
                        var template = await _templateRepository.FirstOrDefaultAsync(task.TemplateId);
                        var taskDto = await MapToDto(task, device, template);

                        var items = await _taskItemRepository.GetAll()
                            .Where(x => x.TaskId == task.Id)
                            .OrderBy(x => x.SortOrder)
                            .Select(x => new MaintenanceTaskItemDto
                            {
                                Id = x.Id,
                                TaskId = x.TaskId,
                                ItemId = x.ItemId,
                                ItemName = x.ItemName,
                                MaintenanceMethod = x.MaintenanceMethod,
                                Content = x.Content,
                                StandardValue = x.StandardValue,
                                Result = x.Result,
                                ActualValue = x.ActualValue,
                                Remark = x.Remark,
                                SortOrder = x.SortOrder
                            })
                            .ToListAsync();

                        taskDto.Items = items;
                        ungroupedGroup.Tasks.Add(taskDto);
                    }

                    result.Add(ungroupedGroup);
                }

                return CommonResult<List<PendingTaskGroupDto>>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取我的待办工单失败", ex);
                return CommonResult<List<PendingTaskGroupDto>>.Error($"获取我的待办工单失败: {ex.Message}");
            }
        }


        /// <summary>
        /// 获取工单详情
        /// </summary>
        public async Task<CommonResult<MaintenanceTaskDto>> GetById(Guid id)
        {
            try
            {
                var task = await _taskRepository.FirstOrDefaultAsync(id);
                if (task == null)
                {
                    return CommonResult<MaintenanceTaskDto>.Error("工单不存在");
                }

                var device = await _deviceRepository.FirstOrDefaultAsync(task.DeviceId);
                var template = await _templateRepository.FirstOrDefaultAsync(task.TemplateId);
                var dto = await MapToDto(task, device, template);

                // 获取工单项目
                var items = await _taskItemRepository.GetAll()
                    .Where(x => x.TaskId == id)
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new MaintenanceTaskItemDto
                    {
                        Id = x.Id,
                        TaskId = x.TaskId,
                        ItemId = x.ItemId,
                        ItemName = x.ItemName,
                        MaintenanceMethod = x.MaintenanceMethod,
                        Content = x.Content,
                        StandardValue = x.StandardValue,
                        Result = x.Result,
                        ActualValue = x.ActualValue,
                        Remark = x.Remark,
                        SortOrder = x.SortOrder
                    })
                    .ToListAsync();

                dto.Items = items;


                // 获取附件列表
                var attachments = await _attachmentAppService.GetBusinessAttachments(
                    new Attachment.Dto.GetBusinessAttachmentsInput
                    {
                        BusinessId = id,
                        BusinessType = "MaintenanceTask"
                    });

                if (attachments.IsSuccess && attachments.Data != null)
                {
                    dto.Attachments = attachments.Data.Select(a => new AttachmentDto
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FileSize = a.FileSize,
                        AttachmentType = a.AttachmentType,
                        LinkUrl = a.LinkUrl,
                        FilePath = a.FilePath,
                        CreationTime = a.CreationTime
                    }).ToList();
                }



                // 获取设备位置
                if (device != null)
                {
                    dto.DeviceLocation = device.Location;
                }

                return CommonResult<MaintenanceTaskDto>.Success(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("获取工单详情失败", ex);
                return CommonResult<MaintenanceTaskDto>.Error($"获取工单详情失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取今日需提醒任务
        /// </summary>
        public async Task<CommonResult<List<PendingRemindTaskDto>>> GetTodayRemindTasks()
        {
            try
            {
                var today = DateTime.Today;

                var tasks = await _taskRepository.GetAll()
                    .Where(x => x.RemindDate.HasValue &&
                               x.RemindDate.Value.Date == today &&
                               !x.IsReminded &&
                               x.Status == TASK_STATUS_PLAN)
                    .ToListAsync();

                // 按组分组
                var groups = new Dictionary<Guid?, List<MaintenanceTasks>>();

                foreach (var task in tasks)
                {
                    var key = task.GroupId;
                    if (!groups.ContainsKey(key))
                    {
                        groups[key] = new List<MaintenanceTasks>();
                    }
                    groups[key].Add(task);
                }

                var result = new List<PendingRemindTaskDto>();

                foreach (var group in groups)
                {
                    var groupId = group.Key;
                    var groupTasks = group.Value;

                    MaintenanceTaskGroups groupInfo = null;
                    if (groupId.HasValue)
                    {
                        groupInfo = await _taskGroupRepository.FirstOrDefaultAsync(groupId.Value);
                    }

                    var dto = new PendingRemindTaskDto
                    {
                        GroupId = groupId,
                        GroupName = groupInfo?.GroupName ?? "待办任务",
                        RemindDate = today,
                        PlanStartDate = groupTasks.Min(x => x.PlanStartDate),
                        PlanEndDate = groupTasks.Max(x => x.PlanEndDate),
                        ExecutorNames = groupInfo?.ExecutorNames ?? string.Join(",", groupTasks.Select(x => x.ExecutorNames).Distinct()),
                        Tasks = new List<MaintenanceTaskDto>(),
                        DeviceCount = groupTasks.Select(t => t.DeviceId).Distinct().Count()
                    };

                    foreach (var task in groupTasks)
                    {
                        var device = await _deviceRepository.FirstOrDefaultAsync(task.DeviceId);
                        var template = await _templateRepository.FirstOrDefaultAsync(task.TemplateId);
                        var taskDto = await MapToDto(task, device, template);
                        dto.Tasks.Add(taskDto);
                    }

                    result.Add(dto);
                }

                return CommonResult<List<PendingRemindTaskDto>>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取今日需提醒任务失败", ex);
                return CommonResult<List<PendingRemindTaskDto>>.Error($"获取今日需提醒任务失败: {ex.Message}");
            }
        }



        #endregion

        #region 工单生成

        /// <summary>
        /// 生成下周保养工单（带合并执行逻辑）
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult<int>> GenerateNextWeekTasks()
        {
            try
            {
                var today = DateTime.Today;
                var nextWeek = today.AddDays(7);
                var nextTwoWeeks = today.AddDays(14);

                // 获取所有启用的计划
                var plans = await _planRepository.GetAll()
                    .Where(x => x.Status == "启用" &&
                               !x.HasGeneratedTask &&
                               x.DeviceId.HasValue)
                    .ToListAsync();

                // 按设备分组计划
                var plansByDevice = plans.GroupBy(x => x.DeviceId.Value);

                var generatedTasks = new List<MaintenanceTasks>();
                var taskGroups = new Dictionary<string, List<MaintenancePlans>>();

                foreach (var devicePlans in plansByDevice)
                {
                    var deviceId = devicePlans.Key;
                    var device = await _deviceRepository.FirstOrDefaultAsync(deviceId);
                    if (device == null) continue;

                    // 获取设备的下次保养日期
                    var deviceNextDates = devicePlans
                        .Where(p => p.NextMaintenanceDate >= today && p.NextMaintenanceDate <= nextTwoWeeks)
                        .GroupBy(p => p.NextMaintenanceDate.Date)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    foreach (var dateGroup in deviceNextDates)
                    {
                        var date = dateGroup.Key;
                        var plansOnDate = dateGroup.Value;

                        // 检查是否有多个计划在同一天
                        if (plansOnDate.Count > 1)
                        {
                            // 多个计划在同一天 - 需要合并执行
                            var groupKey = $"{deviceId}_{date:yyyyMMdd}_merged";

                            if (!taskGroups.ContainsKey(groupKey))
                            {
                                taskGroups[groupKey] = new List<MaintenancePlans>();
                            }

                            taskGroups[groupKey].AddRange(plansOnDate);
                        }
                        else
                        {
                            // 单个计划 - 单独生成工单
                            foreach (var plan in plansOnDate)
                            {
                                await GenerateSingleTask(plan, device);
                                plan.HasGeneratedTask = true;
                                generatedTasks.Add(new MaintenanceTasks { Id = Guid.NewGuid() });
                            }
                        }
                    }
                }

                // 处理需要合并的计划组
                foreach (var group in taskGroups)
                {
                    var groupPlans = group.Value;
                    var firstPlan = groupPlans.First();
                    var device = await _deviceRepository.FirstOrDefaultAsync(firstPlan.DeviceId.Value);

                    if (device == null) continue;

                    // 创建合并工单组
                    var groupEntity = await CreateMergedTaskGroup(groupPlans, device);

                    // 为每个计划创建工单（但关联到同一组）
                    foreach (var plan in groupPlans)
                    {
                        var task = await CreateTaskForPlan(plan, device, groupEntity.Id);
                        plan.HasGeneratedTask = true;
                        generatedTasks.Add(task);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult<int>.Success($"成功生成 {generatedTasks.Count} 个工单（含合并执行）",
                    generatedTasks.Count);
            }
            catch (Exception ex)
            {
                Logger.Error("生成下周保养工单失败", ex);
                return CommonResult<int>.Error($"生成下周保养工单失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 为指定计划生成工单
        /// </summary>
        public async Task<CommonResult> GenerateTaskForPlan(Guid planId)
        {
            try
            {
                var plan = await _planRepository.FirstOrDefaultAsync(planId);
                if (plan == null)
                {
                    return CommonResult.Error("计划不存在");
                }

                var device = await _deviceRepository.FirstOrDefaultAsync(plan.DeviceId ?? Guid.Empty);
                if (device == null)
                {
                    return CommonResult.Error("设备不存在");
                }

                await GenerateSingleTask(plan, device);
                plan.HasGeneratedTask = true;
                await _planRepository.UpdateAsync(plan);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("工单生成成功");
            }
            catch (Exception ex)
            {
                Logger.Error($"为计划 {planId} 生成工单失败", ex);
                return CommonResult.Error($"生成工单失败: {ex.Message}");
            }
        }


        /// <summary>
        /// 生成单个工单
        /// </summary>
        private async Task GenerateSingleTask(MaintenancePlans plan, Devices device)
        {
            var remindDate = plan.NextMaintenanceDate.AddDays(-7);
            if (remindDate < DateTime.Today) remindDate = DateTime.Today;

            var template = await _templateRepository.FirstOrDefaultAsync(plan.TemplateId);

            // 获取模板的保养项目
            var items = await _itemRepository.GetAll()
                .Where(x => x.TemplateId == plan.TemplateId)
                .OrderBy(x => x.GroupSortOrder)
                .ThenBy(x => x.ItemSortOrder)
                .ToListAsync();

            var executorIds = await GetDeviceMaintainers(device.Id);
            var executorNames = await GetUserNames(executorIds);

            var taskNo = $"MT-{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";

            var task = new MaintenanceTasks
            {
                TaskNo = taskNo,
                TaskName = $"{device.DeviceName} - {template?.TemplateName} ({plan.MaintenanceLevel})",
                DeviceId = device.Id,
                PlanId = plan.Id,
                TemplateId = plan.TemplateId,
                MaintenanceLevel = plan.MaintenanceLevel,
                Status = TASK_STATUS_PLAN,
                PlanStartDate = remindDate,
                PlanEndDate = plan.NextMaintenanceDate,
                RemindDate = remindDate,
                ExecutorIds = string.Join(",", executorIds),
                ExecutorNames = string.Join(",", executorNames),
                CreateType = "自动"
            };

            var taskId = await _taskRepository.InsertAndGetIdAsync(task);

            // 创建工单项目
            foreach (var item in items)
            {
                var taskItem = new MaintenanceTaskItems
                {
                    TaskId = taskId,
                    ItemId = item.Id,
                    ItemName = item.PointName,
                    MaintenanceMethod = string.Join("、", item.InspectionMethod),
                    Content = item.InspectionContent,
                    StandardValue = null,
                    SortOrder = (int)item.ItemSortOrder
                };
                await _taskItemRepository.InsertAsync(taskItem);
            }
        }

        /// <summary>
        /// 为计划创建工单（可指定组ID）
        /// </summary>
        private async Task<MaintenanceTasks> CreateTaskForPlan(MaintenancePlans plan, Devices device, Guid? groupId = null)
        {
            var remindDate = plan.NextMaintenanceDate.AddDays(-7);
            if (remindDate < DateTime.Today) remindDate = DateTime.Today;

            var template = await _templateRepository.FirstOrDefaultAsync(plan.TemplateId);
            var items = await _itemRepository.GetAll()
                .Where(x => x.TemplateId == plan.TemplateId)
                .OrderBy(x => x.GroupSortOrder)
                .ThenBy(x => x.ItemSortOrder)
                .ToListAsync();

            var executorIds = await GetDeviceMaintainers(device.Id);
            var executorNames = await GetUserNames(executorIds);

            var taskNo = $"MT-{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";

            var task = new MaintenanceTasks
            {
                TaskNo = taskNo,
                TaskName = $"{device.DeviceName} - {template?.TemplateName} ({plan.MaintenanceLevel})",
                DeviceId = device.Id,
                PlanId = plan.Id,
                TemplateId = plan.TemplateId,
                MaintenanceLevel = plan.MaintenanceLevel,
                Status = TASK_STATUS_PLAN,
                PlanStartDate = remindDate,
                PlanEndDate = plan.NextMaintenanceDate,
                RemindDate = remindDate,
                ExecutorIds = string.Join(",", executorIds),
                ExecutorNames = string.Join(",", executorNames),
                CreateType = "自动",
                GroupId = groupId
            };

            var taskId = await _taskRepository.InsertAndGetIdAsync(task);

            // 创建工单项目
            foreach (var item in items)
            {
                var taskItem = new MaintenanceTaskItems
                {
                    TaskId = taskId,
                    ItemId = item.Id,
                    ItemName = item.PointName,
                    MaintenanceMethod = string.Join("、", item.InspectionMethod),
                    Content = item.InspectionContent,
                    StandardValue = null,
                    SortOrder = (int)item.ItemSortOrder
                };
                await _taskItemRepository.InsertAsync(taskItem);
            }

            return task;
        }

        /// <summary>
        /// 创建合并工单组
        /// </summary>
        private async Task<MaintenanceTaskGroups> CreateMergedTaskGroup(List<MaintenancePlans> plans, Devices device)
        {
            var firstPlan = plans.First();
            var remindDate = firstPlan.NextMaintenanceDate.AddDays(-7);
            if (remindDate < DateTime.Today) remindDate = DateTime.Today;

            // 获取所有模板名称
            var templateIds = plans.Select(p => p.TemplateId).Distinct().ToList();
            var templates = await _templateRepository.GetAll()
                .Where(t => templateIds.Contains(t.Id))
                .ToDictionaryAsync(t => t.Id, t => t.TemplateName);

            var levelNames = string.Join("+", plans.Select(p => p.MaintenanceLevel).Distinct());
            var groupName = $"{device.DeviceName} - 合并保养({levelNames})";

            var executorIds = await GetDeviceMaintainers(device.Id);
            var executorNames = await GetUserNames(executorIds);

            var groupNo = $"MG-{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";

            var group = new MaintenanceTaskGroups
            {
                GroupNo = groupNo,
                GroupName = groupName,
                RemindDate = remindDate,
                PlanStartDate = remindDate,
                PlanEndDate = firstPlan.NextMaintenanceDate,
                ExecutorIds = string.Join(",", executorIds),
                ExecutorNames = string.Join(",", executorNames),
                Status = TASK_STATUS_PLAN
            };

            var groupId = await _taskGroupRepository.InsertAndGetIdAsync(group);
            return group;
        }


        #endregion

        #region 工单执行
        /// <summary>
        /// 开始执行工单（接单）
        /// </summary>
        public async Task<CommonResult> StartTask(Guid taskId)
        {
            try
            {
                var task = await _taskRepository.FirstOrDefaultAsync(taskId);
                if (task == null)
                {
                    return CommonResult.Error("工单不存在");
                }

                if (task.Status != TASK_STATUS_PENDING && task.Status != TASK_STATUS_PLAN)
                {
                    return CommonResult.Error($"当前状态({task.Status})不可开始");
                }

                task.Status = TASK_STATUS_EXECUTING;
                task.ActualStartTime = DateTime.Now;

                await _taskRepository.UpdateAsync(task);

                // 如果属于组任务，更新组状态
                if (task.GroupId.HasValue)
                {
                    var group = await _taskGroupRepository.FirstOrDefaultAsync(task.GroupId.Value);
                    if (group != null && group.Status != TASK_STATUS_EXECUTING)
                    {
                        group.Status = TASK_STATUS_EXECUTING;
                        await _taskGroupRepository.UpdateAsync(group);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("开始执行");
            }
            catch (Exception ex)
            {
                Logger.Error("开始执行工单失败", ex);
                return CommonResult.Error($"开始执行工单失败: {ex.Message}");
            }
        }



        /// <summary>
        /// 保存工单执行进度（保持执行中状态）
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> SaveTaskExecution(SaveMaintenanceTaskExecutionInput input)
        {
            try
            {
                var task = await _taskRepository.FirstOrDefaultAsync(input.TaskId);
                if (task == null)
                {
                    return CommonResult.Error("工单不存在");
                }

                if (task.Status != TASK_STATUS_EXECUTING)
                {
                    return CommonResult.Error($"当前状态({task.Status})不可保存执行进度");
                }

                // 更新工单信息
                task.Summary = input.Summary;
                task.CompletionRemark = input.CompletionRemark;

                await _taskRepository.UpdateAsync(task);

                // 更新项目执行结果
                if (input.Items != null && input.Items.Any())
                {
                    foreach (var itemInput in input.Items)
                    {
                        var taskItem = await _taskItemRepository.FirstOrDefaultAsync(x =>
                            x.TaskId == input.TaskId && x.ItemId == itemInput.ItemId);

                        if (taskItem != null)
                        {
                            taskItem.Result = itemInput.Result;
                            taskItem.ActualValue = itemInput.ActualValue;
                            taskItem.Remark = itemInput.Remark;
                            await _taskItemRepository.UpdateAsync(taskItem);
                        }
                    }
                }

                // 保存附件关系
                if (input.AttachmentIds != null && input.AttachmentIds.Any())
                {
                    await _attachmentAppService.SetBusinessAttachments(new Attachment.Dto.SetBusinessAttachmentInput
                    {
                        BusinessId = input.TaskId,
                        BusinessType = "MaintenanceTask",
                        AttachmentIds = input.AttachmentIds
                    });
                }
                else
                {
                    await _attachmentAppService.SetBusinessAttachments(new Attachment.Dto.SetBusinessAttachmentInput
                    {
                        BusinessId = input.TaskId,
                        BusinessType = "MaintenanceTask",
                        AttachmentIds = new List<Guid>() // 清空附件关系
                    });
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("保存成功");
            }
            catch (Exception ex)
            {
                Logger.Error("保存工单执行进度失败", ex);
                return CommonResult.Error($"保存工单执行进度失败: {ex.Message}");
            }
        }



        /// <summary>
        /// 完成保养工单（带结束时间）
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> CompleteTask(CompleteMaintenanceTaskInput input)
        {
            try
            {
                var task = await _taskRepository.FirstOrDefaultAsync(input.TaskId);
                if (task == null)
                {
                    return CommonResult.Error("工单不存在");
                }

                if (task.Status != TASK_STATUS_EXECUTING)
                {
                    return CommonResult.Error($"当前状态({task.Status})不可完成");
                }

                // 更新工单
                task.Status = TASK_STATUS_COMPLETED;
                task.ActualEndTime = input.EndTime ?? DateTime.Now;
                task.Summary = input.Summary;
                task.CompletionRemark = input.CompletionRemark;

                await _taskRepository.UpdateAsync(task);

                // 更新项目执行结果
                if (input.Items != null && input.Items.Any())
                {
                    foreach (var itemInput in input.Items)
                    {
                        var taskItem = await _taskItemRepository.FirstOrDefaultAsync(x =>
                            x.TaskId == input.TaskId && x.ItemId == itemInput.ItemId);

                        if (taskItem != null)
                        {
                            taskItem.Result = itemInput.Result;
                            taskItem.ActualValue = itemInput.ActualValue;
                            taskItem.Remark = itemInput.Remark;
                            await _taskItemRepository.UpdateAsync(taskItem);
                        }
                    }
                }

                // 保存附件关系
                if (input.AttachmentIds != null && input.AttachmentIds.Any())
                {
                    await _attachmentAppService.SetBusinessAttachments(new Attachment.Dto.SetBusinessAttachmentInput
                    {
                        BusinessId = input.TaskId,
                        BusinessType = "MaintenanceTask",
                        AttachmentIds = input.AttachmentIds
                    });
                }
                else
                {
                    await _attachmentAppService.SetBusinessAttachments(new Attachment.Dto.SetBusinessAttachmentInput
                    {
                        BusinessId = input.TaskId,
                        BusinessType = "MaintenanceTask",
                        AttachmentIds = new List<Guid>() // 清空附件关系
                    });
                }

                // 记录保养完成，更新下次保养日期
                var plan = await _planRepository.FirstOrDefaultAsync(task.PlanId);
                if (plan != null)
                {
                    var planService = IocManager.Instance.Resolve<IMaintenancePlanAppService>();
                    await planService.RecordMaintenanceCompleted(plan.Id, task.ActualEndTime.Value);
                }

                // 检查是否同一组的所有工单都已完成
                if (task.GroupId.HasValue)
                {
                    var groupTasks = await _taskRepository.GetAll()
                        .Where(x => x.GroupId == task.GroupId.Value)
                        .ToListAsync();

                    if (groupTasks.All(x => x.Status == TASK_STATUS_COMPLETED))
                    {
                        var group = await _taskGroupRepository.GetAsync(task.GroupId.Value);
                        group.Status = TASK_STATUS_COMPLETED;
                        await _taskGroupRepository.UpdateAsync(group);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("完成保养");
            }
            catch (Exception ex)
            {
                Logger.Error("完成保养工单失败", ex);
                return CommonResult.Error($"完成保养工单失败: {ex.Message}");
            }
        }



        /// <summary>
        /// 执行保养工单
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> ExecuteTask(ExecuteMaintenanceTaskInput input)
        {
            try
            {
                var task = await _taskRepository.FirstOrDefaultAsync(input.TaskId);
                if (task == null)
                {
                    return CommonResult.Error("工单不存在");
                }

                if (task.Status != TASK_STATUS_EXECUTING && task.Status != TASK_STATUS_PENDING)
                {
                    return CommonResult.Error($"当前状态({task.Status})不可执行");
                }

                // 更新工单
                task.Status = TASK_STATUS_COMPLETED;
                task.ActualEndTime = DateTime.Now;
                task.Summary = input.Summary;
                task.CompletionRemark = input.CompletionRemark;

                await _taskRepository.UpdateAsync(task);

                // 更新项目执行结果
                if (input.Items != null && input.Items.Any())
                {
                    foreach (var itemInput in input.Items)
                    {
                        var taskItem = await _taskItemRepository.FirstOrDefaultAsync(x => x.TaskId == input.TaskId && x.ItemId == itemInput.ItemId);
                        if (taskItem != null)
                        {
                            taskItem.Result = itemInput.Result;
                            taskItem.ActualValue = itemInput.ActualValue;
                            taskItem.Remark = itemInput.Remark;
                            await _taskItemRepository.UpdateAsync(taskItem);
                        }
                    }
                }

                // 保存附件关系
                if (input.AttachmentIds != null && input.AttachmentIds.Any())
                {
                    await _attachmentAppService.SetBusinessAttachments(new Attachment.Dto.SetBusinessAttachmentInput
                    {
                        BusinessId = input.TaskId,
                        BusinessType = "MaintenanceTask",
                        AttachmentIds = input.AttachmentIds
                    });
                }
                else
                {
                    await _attachmentAppService.SetBusinessAttachments(new Attachment.Dto.SetBusinessAttachmentInput
                    {
                        BusinessId = input.TaskId,
                        BusinessType = "MaintenanceTask",   
                        AttachmentIds = new List<Guid>() // 清空附件关系
                    });
                }

                // 记录保养完成，更新下次保养日期
                var plan = await _planRepository.FirstOrDefaultAsync(task.PlanId);
                if (plan != null)
                {
                    var planService = IocManager.Instance.Resolve<IMaintenancePlanAppService>();
                    await planService.RecordMaintenanceCompleted(plan.Id, task.ActualEndTime);
                }

                // 检查是否同一组的所有工单都已完成
                if (task.GroupId.HasValue)
                {
                    var groupTasks = await _taskRepository.GetAll()
                        .Where(x => x.GroupId == task.GroupId.Value)
                        .ToListAsync();

                    if (groupTasks.All(x => x.Status == TASK_STATUS_COMPLETED))
                    {
                        var group = await _taskGroupRepository.GetAsync(task.GroupId.Value);
                        group.Status = TASK_STATUS_COMPLETED;
                        await _taskGroupRepository.UpdateAsync(group);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("执行成功");
            }
            catch (Exception ex)
            {
                Logger.Error("执行保养工单失败", ex);
                return CommonResult.Error($"执行保养工单失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 委派工单
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> DelegateTask(DelegateMaintenanceTaskInput input)
        {
            try
            {
                var task = await _taskRepository.FirstOrDefaultAsync(input.TaskId);
                if (task == null)
                {
                    return CommonResult.Error("工单不存在");
                }

                if (task.Status != TASK_STATUS_PENDING && task.Status != TASK_STATUS_PLAN)
                {
                    return CommonResult.Error($"当前状态({task.Status})不可委派");
                }

                // 保存原执行人
                task.OriginalExecutorIds = task.ExecutorIds;

                // 获取新执行人姓名
                var newExecutorNames = new List<string>();
                foreach (var userId in input.NewExecutorIds)
                {
                    var userName = await _userAppService.GetNameByUserId(userId);
                    newExecutorNames.Add(userName);
                }

                // 更新执行人
                task.ExecutorIds = string.Join(",", input.NewExecutorIds);
                task.ExecutorNames = string.Join(",", newExecutorNames);
                task.DelegatorId = AbpSession.UserId;
                task.DelegatorName = await _userAppService.GetNameByUserId(AbpSession.UserId.Value);
                task.DelegateReason = input.Reason;
                task.Status = TASK_STATUS_DELEGATED;

                await _taskRepository.UpdateAsync(task);

                // 如果属于组任务，更新组的执行人
                if (task.GroupId.HasValue)
                {
                    var group = await _taskGroupRepository.FirstOrDefaultAsync(task.GroupId.Value);
                    if (group != null)
                    {
                        var groupTasks = await _taskRepository.GetAll()
                            .Where(x => x.GroupId == task.GroupId.Value)
                            .ToListAsync();

                        var allExecutorIds = new HashSet<long>();
                        var allExecutorNames = new HashSet<string>();

                        foreach (var t in groupTasks)
                        {
                            var ids = t.ExecutorIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => long.Parse(x));
                            foreach (var id in ids)
                            {
                                allExecutorIds.Add(id);
                            }

                            var names = t.ExecutorNames.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (var name in names)
                            {
                                allExecutorNames.Add(name);
                            }
                        }

                        group.ExecutorIds = string.Join(",", allExecutorIds);
                        group.ExecutorNames = string.Join(",", allExecutorNames);
                        await _taskGroupRepository.UpdateAsync(group);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("委派成功");
            }
            catch (Exception ex)
            {
                Logger.Error("委派工单失败", ex);
                return CommonResult.Error($"委派工单失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 取消工单
        /// </summary>
        public async Task<CommonResult> CancelTask(Guid taskId, string reason)
        {
            try
            {
                var task = await _taskRepository.FirstOrDefaultAsync(taskId);
                if (task == null)
                {
                    return CommonResult.Error("工单不存在");
                }

                if (task.Status == TASK_STATUS_COMPLETED)
                {
                    return CommonResult.Error("已完成工单不能取消");
                }

                task.Status = TASK_STATUS_CANCELLED;
                task.CompletionRemark = reason;

                await _taskRepository.UpdateAsync(task);

                return CommonResult.Ok("取消成功");
            }
            catch (Exception ex)
            {
                Logger.Error("取消工单失败", ex);
                return CommonResult.Error($"取消工单失败: {ex.Message}");
            }
        }


        #endregion

        #region 提醒功能

        /// <summary>
        /// 发送提醒（定时任务调用）
        /// </summary>
        public async Task<CommonResult<int>> SendReminders()
        {
            try
            {
                var remindResult = await GetTodayRemindTasks();
                if (!remindResult.IsSuccess || remindResult.Data == null || !remindResult.Data.Any())
                {
                    return CommonResult<int>.Success("没有需要提醒的任务", 0);
                }

                var groups = remindResult.Data;
                int remindCount = 0;

                foreach (var group in groups)
                {
                    Logger.Info($"发送提醒: {group.GroupName}, 包含 {group.Tasks.Count} 个工单");

                    foreach (var task in group.Tasks)
                    {
                        var taskEntity = await _taskRepository.GetAsync(task.Id);
                        taskEntity.IsReminded = true;
                        taskEntity.Status = TASK_STATUS_PENDING;
                        await _taskRepository.UpdateAsync(taskEntity);
                        remindCount++;
                    }

                    if (group.GroupId.HasValue && group.GroupId.Value != Guid.Empty)
                    {
                        var groupEntity = await _taskGroupRepository.GetAsync(group.GroupId.Value);
                        groupEntity.Status = TASK_STATUS_PENDING;
                        await _taskGroupRepository.UpdateAsync(groupEntity);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult<int>.Success($"成功发送 {remindCount} 个提醒", remindCount);
            }
            catch (Exception ex)
            {
                Logger.Error("发送提醒失败", ex);
                return CommonResult<int>.Error($"发送提醒失败: {ex.Message}");
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取设备保养人员
        /// </summary>
        private async Task<List<long>> GetDeviceMaintainers(Guid deviceId)
        {
            return await _deviceUserRelationRepository.GetAll()
                .Where(x => x.DeviceId == deviceId && x.UserType == "保养人员")
                .Select(x => x.UserId)
                .ToListAsync();
        }

        /// <summary>
        /// 获取用户名称
        /// </summary>
        private async Task<List<string>> GetUserNames(List<long> userIds)
        {
            var names = new List<string>();
            foreach (var userId in userIds)
            {
                var name = await _userAppService.GetNameByUserId(userId);
                names.Add(name);
            }
            return names;
        }

        /// <summary>
        /// 映射为DTO
        /// </summary>
        private async Task<MaintenanceTaskDto> MapToDto(MaintenanceTasks task, Devices device, MaintenanceTemplates template)
        {
            var dto = new MaintenanceTaskDto
            {
                Id = task.Id,
                TaskNo = task.TaskNo,
                TaskName = task.TaskName,
                GroupId = task.GroupId,
                DeviceId = task.DeviceId,
                DeviceCode = device?.DeviceCode,
                DeviceName = device?.DeviceName,
                DeviceLocation = device?.Location,
                DeviceTypeName = device != null ? await GetDeviceTypeName(device.Id) : null,
                PlanId = task.PlanId,
                TemplateId = task.TemplateId,
                TemplateName = template?.TemplateName,
                MaintenanceLevel = task.MaintenanceLevel,
                MaintenanceLevelText = GetMaintenanceLevelText(task.MaintenanceLevel),
                Status = task.Status,
                PlanStartDate = task.PlanStartDate,
                PlanEndDate = task.PlanEndDate,
                RemindDate = task.RemindDate,
                ActualStartTime = task.ActualStartTime,
                ActualEndTime = task.ActualEndTime,
                Summary = task.Summary,
                CompletionRemark = task.CompletionRemark,
                CreateType = task.CreateType,
                OriginalExecutorIds = task.OriginalExecutorIds,
                DelegatorId = task.DelegatorId,
                DelegatorName = task.DelegatorName,
                DelegateReason = task.DelegateReason
            };

            // 解析执行人
            if (!string.IsNullOrEmpty(task.ExecutorIds))
            {
                dto.ExecutorIds = task.ExecutorIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => long.Parse(x)).ToList();
            }

            if (!string.IsNullOrEmpty(task.ExecutorNames))
            {
                dto.ExecutorNames = task.ExecutorNames.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            // 获取组信息
            if (task.GroupId.HasValue)
            {
                var group = await _taskGroupRepository.FirstOrDefaultAsync(task.GroupId.Value);
                dto.GroupName = group?.GroupName;
            }

            return dto;
        }


        /// <summary>
        /// 获取设备类型名称
        /// </summary>
        private async Task<string> GetDeviceTypeName(Guid deviceId)
        {
            var typeRepository = IocManager.Instance.Resolve<IRepository<DeviceTypeRelations, Guid>>();
            var typeRelation = await typeRepository.FirstOrDefaultAsync(x => x.DeviceId == deviceId);
            if (typeRelation != null)
            {
                var type = await _typeRepository.FirstOrDefaultAsync(typeRelation.TypeId);
                return type?.TypeName;
            }
            return null;
        }

        /// <summary>
        /// 获取保养等级显示文本
        /// </summary>
        private string GetMaintenanceLevelText(string level)
        {
            return level switch
            {
                "月度" => "月度保养",
                "季度" => "季度保养",
                "半年度" => "半年度保养",
                "年度" => "年度保养",
                _ => level
            };
        }

        #endregion
    }
}