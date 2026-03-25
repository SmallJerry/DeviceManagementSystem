using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using DeviceManagementSystem.Attachment;
using DeviceManagementSystem.Attachment.Dto;
using DeviceManagementSystem.BasicDataManagement;
using DeviceManagementSystem.Configuration;
using DeviceManagementSystem.DeviceInfos;
using DeviceManagementSystem.Email;
using DeviceManagementSystem.Email.Dto;
using DeviceManagementSystem.Maintenances.Constant;
using DeviceManagementSystem.Maintenances.Dto;
using DeviceManagementSystem.Maintenances.Interface;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly IRepository<MaintenancePlans, Guid> _planRepository;
        private readonly IRepository<MaintenanceTemplates, Guid> _templateRepository;
        private readonly IRepository<Devices, Guid> _deviceRepository;
        private readonly IRepository<DeviceUserRelations, Guid> _deviceUserRelationRepository;
        private readonly IUserAppService _userAppService;
        private readonly IRepository<MaintenanceItems, Guid> _itemRepository;
        private readonly IRepository<Types, Guid> _typeRepository;
        private readonly IAttachmentAppService _attachmentAppService;
        private readonly IEmailAppService _emailAppService;
        private readonly IRepository<DeviceTypeRelations, Guid> _deviceTypeRelationRepository;
        private readonly IConfigurationRoot _configuration;

        // 工单状态常量
        private const string TASK_STATUS_PLAN = "计划";
        private const string TASK_STATUS_PENDING = "待执行";
        private const string TASK_STATUS_EXECUTING = "执行中";
        private const string TASK_STATUS_COMPLETED = "已完成";
        private const string TASK_STATUS_CANCELLED = "已取消";
        private const string TASK_STATUS_DELEGATED = "已委派";
        private const string TASK_STATUS_OVERDUE = "逾期";

        public MaintenanceTaskAppService(
            IRepository<MaintenanceTasks, Guid> taskRepository,
            IRepository<MaintenanceTaskItems, Guid> taskItemRepository,
            IRepository<MaintenancePlans, Guid> planRepository,
            IRepository<MaintenanceTemplates, Guid> templateRepository,
            IRepository<Devices, Guid> deviceRepository,
            IRepository<DeviceUserRelations, Guid> deviceUserRelationRepository,
            IUserAppService userAppService,
            IRepository<DeviceTypeRelations, Guid> deviceTypeRelationRepository,
            IRepository<MaintenanceItems, Guid> itemRepository,
            IRepository<Types, Guid> typeRepository,
            IAttachmentAppService attachmentAppService,
            IEmailAppService emailAppService,
            IWebHostEnvironment env)
        {
            _taskRepository = taskRepository;
            _taskItemRepository = taskItemRepository;
            _planRepository = planRepository;
            _templateRepository = templateRepository;
            _deviceRepository = deviceRepository;
            _deviceUserRelationRepository = deviceUserRelationRepository;
            _userAppService = userAppService;
            _itemRepository = itemRepository;
            _typeRepository = typeRepository;
            _attachmentAppService = attachmentAppService;
            _emailAppService = emailAppService;
            _deviceTypeRelationRepository = deviceTypeRelationRepository;
            _configuration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, env.IsDevelopment());
        }

        #region 查询方法

        /// <summary>
        /// 获取工单分页列表（全部工单）
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
                            join dtr in _deviceTypeRelationRepository.GetAll().AsNoTracking() on d.Id equals dtr.DeviceId into deviceTypeJoin
                            from dtr in deviceTypeJoin.DefaultIfEmpty()
                            join dt in _typeRepository.GetAll().AsNoTracking() on dtr.TypeId equals dt.Id into typeJoin
                            from dt in typeJoin.DefaultIfEmpty()
                            select new { Task = t, Device = d, Plan = p, Template = tm, DeviceType = dt };

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


                if (input.DeviceTypeId.HasValue)
                {
                    query = query.Where(x => x.DeviceType != null && x.DeviceType.Id == input.DeviceTypeId.Value);
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
        /// 获取我的待办工单（按状态分类）
        /// </summary>
        public async Task<CommonResult<MyTaskStatisticsDto>> GetMyTasks(long? executorId = null)
        {
            try
            {
                var userId = executorId ?? AbpSession.UserId.Value;
                var userIdStr = userId.ToString();

                // 获取用户相关的所有工单
                var tasks = await _taskRepository.GetAll()
                    .Where(x => x.ExecutorIds.Contains(userIdStr))
                    .OrderByDescending(x => x.PlanStartDate)
                    .ToListAsync();

                var result = new MyTaskStatisticsDto
                {
                    AllCount = tasks.Count,
                    PendingCount = tasks.Count(x => x.Status == TASK_STATUS_PENDING),
                    PlanCount = tasks.Count(x => x.Status == TASK_STATUS_PLAN),
                    ExecutingCount = tasks.Count(x => x.Status == TASK_STATUS_EXECUTING),
                    CompletedCount = tasks.Count(x => x.Status == TASK_STATUS_COMPLETED),
                    OverdueCount = tasks.Count(x => x.Status == TASK_STATUS_OVERDUE),
                    Tasks = new List<MaintenanceTaskDto>()
                };

                foreach (var task in tasks)
                {
                    var device = await _deviceRepository.FirstOrDefaultAsync(task.DeviceId);
                    var template = await _templateRepository.FirstOrDefaultAsync(task.TemplateId);
                    var dto = await MapToDto(task, device, template);

                    // 获取工单项目（包含来源周期信息）
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
                            SortOrder = x.SortOrder,
                            SourcePlanId = x.SourcePlanId,
                            SourceMaintenanceLevel = x.SourceMaintenanceLevel
                        })
                        .ToListAsync();

                    dto.Items = items;
                    result.Tasks.Add(dto);
                }

                return CommonResult<MyTaskStatisticsDto>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取我的待办工单失败", ex);
                return CommonResult<MyTaskStatisticsDto>.Error($"获取我的待办工单失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 按状态获取我的待办工单
        /// </summary>
        public async Task<CommonResult<List<MaintenanceTaskDto>>> GetMyTasksByStatus(string status, long? executorId = null)
        {
            try
            {
                var userId = executorId ?? AbpSession.UserId.Value;
                var userIdStr = userId.ToString();

                var query = _taskRepository.GetAll()
                    .Where(x => x.ExecutorIds.Contains(userIdStr));

                if (!string.IsNullOrWhiteSpace(status) && status != "全部待办")
                {
                    query = query.Where(x => x.Status == status);
                }

                var tasks = await query
                    .OrderByDescending(x => x.PlanStartDate)
                    .ToListAsync();

                var result = new List<MaintenanceTaskDto>();

                foreach (var task in tasks)
                {
                    var device = await _deviceRepository.FirstOrDefaultAsync(task.DeviceId);
                    var template = await _templateRepository.FirstOrDefaultAsync(task.TemplateId);
                    var dto = await MapToDto(task, device, template);

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
                            SortOrder = x.SortOrder,
                            SourcePlanId = x.SourcePlanId,
                            SourceMaintenanceLevel = x.SourceMaintenanceLevel
                        })
                        .ToListAsync();

                    dto.Items = items;
                    result.Add(dto);
                }

                return CommonResult<List<MaintenanceTaskDto>>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取我的待办工单失败", ex);
                return CommonResult<List<MaintenanceTaskDto>>.Error($"获取我的待办工单失败: {ex.Message}");
            }
        }


        /// <summary>
        /// 获取工单详情
        /// </summary>
        public async Task<CommonResult<MaintenanceTaskDto>> GetByPlanId(Guid planId)
        {
            try
            {
                var task = await _taskRepository.FirstOrDefaultAsync(it => string.Equals(it.PlanId,planId));
                if (task == null)
                {
                    return CommonResult<MaintenanceTaskDto>.Error("工单不存在");
                }

                var device = await _deviceRepository.FirstOrDefaultAsync(task.DeviceId);
                var template = await _templateRepository.FirstOrDefaultAsync(task.TemplateId);
                var dto = await MapToDto(task, device, template);

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
                        SortOrder = x.SortOrder,
                        SourcePlanId = x.SourcePlanId,
                        SourceMaintenanceLevel = x.SourceMaintenanceLevel
                    })
                    .ToListAsync();

                dto.Items = items;

                // 获取附件列表
                var attachments = await _attachmentAppService.GetBusinessAttachments(
                    new GetBusinessAttachmentsInput
                    {
                        BusinessId = task.Id,
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
                        SortOrder = x.SortOrder,
                        SourcePlanId = x.SourcePlanId,
                        SourceMaintenanceLevel = x.SourceMaintenanceLevel
                    })
                    .ToListAsync();

                dto.Items = items;

                // 获取附件列表
                var attachments = await _attachmentAppService.GetBusinessAttachments(
                    new GetBusinessAttachmentsInput
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

                var result = new List<PendingRemindTaskDto>();

                foreach (var task in tasks)
                {
                    var device = await _deviceRepository.FirstOrDefaultAsync(task.DeviceId);
                    var dto = new PendingRemindTaskDto
                    {
                        TaskId = task.Id,
                        TaskNo = task.TaskNo,
                        TaskName = task.TaskName,
                        DeviceId = task.DeviceId,
                        DeviceName = device?.DeviceName,
                        DeviceCode = device?.DeviceCode,
                        MaintenanceLevel = task.MaintenanceLevel,
                        PlanStartDate = task.PlanStartDate,
                        PlanEndDate = task.PlanEndDate,
                        RemindDate = task.RemindDate.Value,
                        ExecutorIds = task.ExecutorIds,
                        ExecutorNames = task.ExecutorNames,
                        IsMergedTask = task.IsMergedTask,
                        Status = task.Status
                    };
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
        /// 生成下周保养工单（智能合并）
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult<int>> GenerateNextWeekTasks()
        {
            try
            {
                var today = DateTime.Today;
                var nextTwoWeeks = today.AddDays(14);

                // 获取所有启用的计划（按设备分组）
                var plans = await _planRepository.GetAll()
                    .Where(x => x.Status == "启用" &&
                               !x.HasGeneratedTask &&
                               x.DeviceId.HasValue &&
                               x.NextMaintenanceDate >= today &&
                               x.NextMaintenanceDate <= nextTwoWeeks)
                    .OrderBy(x => x.DeviceId)
                    .ThenBy(x => x.NextMaintenanceDate)
                    .ToListAsync();

                var generatedPlans = new List<MaintenancePlanDto>();
                var plansByDevice = plans.GroupBy(x => x.DeviceId.Value);
                var generatedCount = 0;

                foreach (var devicePlans in plansByDevice)
                {
                    var deviceId = devicePlans.Key;
                    var device = await _deviceRepository.FirstOrDefaultAsync(deviceId);
                    if (device == null) continue;

                    var planList = devicePlans.OrderBy(p => p.NextMaintenanceDate).ToList();
                    var processedIndex = 0;

                    while (processedIndex < planList.Count)
                    {
                        var currentPlan = planList[processedIndex];
                        var baseDate = currentPlan.NextMaintenanceDate;

                        var plansToMerge = new List<MaintenancePlans>();
                        var mergeEndIndex = processedIndex;

                        while (mergeEndIndex < planList.Count)
                        {
                            var checkPlan = planList[mergeEndIndex];
                            var daysDiff = (checkPlan.NextMaintenanceDate - baseDate).Days;

                            if (daysDiff <= 7)
                            {
                                plansToMerge.Add(checkPlan);
                                mergeEndIndex++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (plansToMerge.Count > 1)
                        {
                            await GenerateMergedTask(plansToMerge, device);
                            foreach (var plan in plansToMerge)
                            {
                                plan.HasGeneratedTask = true;
                                await _planRepository.UpdateAsync(plan);

                                // 记录生成的计划信息
                                generatedPlans.Add(new MaintenancePlanDto
                                {
                                    Id = plan.Id,
                                    PlanName = plan.PlanName,
                                    DeviceName = device.DeviceName,
                                    DeviceCode = device.DeviceCode,
                                    MaintenanceLevel = plan.MaintenanceLevel,
                                    NextMaintenanceDate = plan.NextMaintenanceDate
                                });
                            }
                            generatedCount++;
                            processedIndex = mergeEndIndex;
                        }
                        else
                        {
                            await GenerateSingleTask(currentPlan, device);
                            currentPlan.HasGeneratedTask = true;
                            await _planRepository.UpdateAsync(currentPlan);

                            generatedPlans.Add(new MaintenancePlanDto
                            {
                                Id = currentPlan.Id,
                                PlanName = currentPlan.PlanName,
                                DeviceName = device.DeviceName,
                                DeviceCode = device.DeviceCode,
                                MaintenanceLevel = currentPlan.MaintenanceLevel,
                                NextMaintenanceDate = currentPlan.NextMaintenanceDate
                            });

                            generatedCount++;
                            processedIndex++;
                        }
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                // 发送邮件通知（异步）
                if (generatedPlans.Any())
                {
                    await SendNextWeekTasksEmail(generatedPlans);
                }

                return CommonResult<int>.Success(
                    $"成功生成 {generatedCount} 个工单（其中合并工单 {plansByDevice.Sum(g => g.Count(p => p.HasGeneratedTask && g.Count() > 1))} 个）",
                    generatedCount);
            }
            catch (Exception ex)
            {
                Logger.Error("生成下周保养工单失败", ex);
                return CommonResult<int>.Error($"生成下周保养工单失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 发送下周保养计划邮件
        /// </summary>
        private async Task SendNextWeekTasksEmail(List<MaintenancePlanDto> plans)
        {
            try
            {
                // 获取收件人列表（所有保养人员）
                var maintainers = await _deviceUserRelationRepository.GetAll()
                    .Where(x => x.UserType == "保养人员")
                    .Select(x => x.UserId)
                    .Distinct()
                    .ToListAsync();

                var recipientEmails = new List<string>();
                foreach (var userId in maintainers)
                {
                    var email = await GetUserEmail(userId);
                    if (!string.IsNullOrEmpty(email))
                    {
                        recipientEmails.Add(email);
                    }
                }

                if (!recipientEmails.Any()) return;

                var emailContent = await BuildNextWeekTasksEmailContent(plans);
                var emailRequest = new EmailNotificationRequest
                {
                    Subject = $"【保养计划】下周共有 {plans.Count} 个保养计划待执行",
                    Body = emailContent,
                    MailTo = recipientEmails,
                };

                await _emailAppService.SendEmailForNotificationAsync(emailRequest);
                Logger.Info($"已发送下周保养计划邮件，收件人 {recipientEmails.Count} 人");
            }
            catch (Exception ex)
            {
                Logger.Error("发送下周保养计划邮件失败", ex);
            }
        }


     

       



        /// <summary>
        /// 获取所有保养人员邮箱
        /// </summary>
        private async Task<List<string>> GetAllMaintainerEmails()
        {
            var maintainerUserIds = await _deviceUserRelationRepository.GetAll()
                .Where(x => x.UserType == "保养人员")
                .Select(x => x.UserId)
                .Distinct()
                .ToListAsync();
            var emails = new List<string>();
            foreach (var userId in maintainerUserIds)
            {
                var email = await GetUserEmail(userId);
                if (!string.IsNullOrEmpty(email)) emails.Add(email);
            }
            return emails;
        }





        /// <summary>
        /// 手动生成工单
        /// </summary>
        public async Task<CommonResult> GenerateManualWithDate(GenerateTaskInput input)
        {
            try
            {
                // 1. 验证执行日期不能早于今天
                if (input.ExecuteDate < DateTime.Today)
                {
                    return CommonResult.Error("执行日期不能早于今天");
                }

                // 2. 获取计划并验证
                var plan = await _planRepository.FirstOrDefaultAsync(input.PlanId);
                if (plan == null)
                {
                    return CommonResult.Error("计划不存在");
                }

                // 3. 获取设备
                var device = await _deviceRepository.FirstOrDefaultAsync(plan.DeviceId ?? Guid.Empty);
                if (device == null)
                {
                    return CommonResult.Error("设备不存在");
                }

                // 4. 临时修改计划的下次保养日期，复用原有的生成方法
                var originalNextDate = plan.NextMaintenanceDate;
                var originalRemindDate = plan.NextMaintenanceDate.AddDays(-7);

                try
                {
                    // 临时设置新的执行日期
                    plan.NextMaintenanceDate = (DateTime)input.ExecuteDate;
                    await _planRepository.UpdateAsync(plan);

                    // 复用原有的生成单个工单方法
                    await GenerateSingleTask(plan, device);

                    // 标记已生成
                    plan.HasGeneratedTask = true;
                    await _planRepository.UpdateAsync(plan);
                }
                finally
                {
                    // 恢复原来的日期（如果需要保留原始计划数据）
                    // 注意：如果计划的下次保养日期不应该被修改，可以在这里恢复
                    // 根据业务需求决定是否恢复
                    if (plan.NextMaintenanceDate != originalNextDate)
                    {
                        plan.NextMaintenanceDate = originalNextDate;
                        await _planRepository.UpdateAsync(plan);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("工单生成成功");
            }
            catch (Exception ex)
            {
                Logger.Error($"为计划 {input.PlanId} 生成工单失败", ex);
                return CommonResult.Error($"生成工单失败: {ex.Message}");
            }
        }


        private async Task GenerateMergedTask(List<MaintenancePlans> plans, Devices device)
        {
            var remindDate = plans.Min(p => p.NextMaintenanceDate).AddDays(-7);
            if (remindDate < DateTime.Today) remindDate = DateTime.Today;

            var allItems = new List<(MaintenanceItems Item, MaintenancePlans Plan)>();
            var levelNames = string.Join("+", plans.Select(p => p.MaintenanceLevel).Distinct());

            foreach (var plan in plans)
            {
                var template = await _templateRepository.FirstOrDefaultAsync(plan.TemplateId);
                var items = await _itemRepository.GetAll()
                    .Where(x => x.TemplateId == plan.TemplateId)
                    .OrderBy(x => x.GroupSortOrder)
                    .ThenBy(x => x.ItemSortOrder)
                    .ToListAsync();

                foreach (var item in items)
                {
                    allItems.Add((item, plan));
                }
            }

            var distinctItems = allItems
                .GroupBy(x => x.Item.Id)
                .Select(g => g.First())
                .ToList();

            var executorIds = await GetDeviceMaintainers(device.Id);
            var executorNames = await GetUserNames(executorIds);

            var taskNo = $"MT-{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";
            var taskName = $"{device.DeviceName} - 合并保养({levelNames})";
            var maxDate = plans.Max(p => p.NextMaintenanceDate);

            var task = new MaintenanceTasks
            {
                TaskNo = taskNo,
                TaskName = taskName,
                DeviceId = device.Id,
                PlanId = plans.First().Id,
                TemplateId = Guid.Empty,
                MaintenanceLevel = levelNames,
                Status = TASK_STATUS_PLAN,
                PlanStartDate = remindDate,
                PlanEndDate = maxDate,
                RemindDate = remindDate,
                ExecutorIds = string.Join(",", executorIds),
                ExecutorNames = string.Join(",", executorNames),
                CreateType = "自动",
                IsMergedTask = true,
                MergedPlanIds = string.Join(",", plans.Select(p => p.Id.ToString()))
            };

            var taskId = await _taskRepository.InsertAndGetIdAsync(task);

            var sortOrder = 1;
            foreach (var item in distinctItems)
            {
                var taskItem = new MaintenanceTaskItems
                {
                    TaskId = taskId,
                    ItemId = item.Item.Id,
                    ItemName = item.Item.PointName,
                    MaintenanceMethod = string.Join("、", item.Item.InspectionMethod),
                    Content = item.Item.InspectionContent,
                    StandardValue = null,
                    SortOrder = sortOrder++,
                    SourcePlanId = item.Plan.Id,
                    SourceMaintenanceLevel = item.Plan.MaintenanceLevel
                };
                await _taskItemRepository.InsertAsync(taskItem);
            }
        }

        private async Task GenerateSingleTask(MaintenancePlans plan, Devices device)
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
            var taskName = $"{device.DeviceName} - {template?.TemplateName} ({plan.MaintenanceLevel})";

            var task = new MaintenanceTasks
            {
                TaskNo = taskNo,
                TaskName = taskName,
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
                IsMergedTask = false
            };

            var taskId = await _taskRepository.InsertAndGetIdAsync(task);

            var sortOrder = 1;
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
                    SortOrder = sortOrder++,
                    SourcePlanId = plan.Id,
                    SourceMaintenanceLevel = plan.MaintenanceLevel
                };
                await _taskItemRepository.InsertAsync(taskItem);
            }
        }

        #endregion

        #region 工单执行

        /// <summary>
        /// 开始执行工单
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

                if (task.Status != TASK_STATUS_PENDING && task.Status != TASK_STATUS_PLAN && task.Status != TASK_STATUS_DELEGATED)
                {
                    return CommonResult.Error($"当前状态({task.Status})不可开始");
                }

                task.Status = TASK_STATUS_EXECUTING;
                task.ActualStartTime = DateTime.Now;

                await _taskRepository.UpdateAsync(task);
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
        /// 保存工单执行进度
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

                task.Summary = input.Summary;
                task.CompletionRemark = input.CompletionRemark;
                await _taskRepository.UpdateAsync(task);

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

                if (input.AttachmentIds != null && input.AttachmentIds.Any())
                {
                    await _attachmentAppService.SetBusinessAttachments(new SetBusinessAttachmentInput
                    {
                        BusinessId = input.TaskId,
                        BusinessType = "MaintenanceTask",
                        AttachmentIds = input.AttachmentIds
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
        /// 完成保养工单
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

                task.Status = TASK_STATUS_COMPLETED;
                task.ActualEndTime = input.EndTime ?? DateTime.Now;
                task.Summary = input.Summary;
                task.CompletionRemark = input.CompletionRemark;
                await _taskRepository.UpdateAsync(task);

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

                if (input.AttachmentIds != null && input.AttachmentIds.Any())
                {
                    await _attachmentAppService.SetBusinessAttachments(new SetBusinessAttachmentInput
                    {
                        BusinessId = input.TaskId,
                        BusinessType = "MaintenanceTask",
                        AttachmentIds = input.AttachmentIds
                    });
                }

                if (task.IsMergedTask && !string.IsNullOrEmpty(task.MergedPlanIds))
                {
                    var planIds = task.MergedPlanIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => Guid.Parse(id)).ToList();

                    foreach (var planId in planIds)
                    {
                        var plan = await _planRepository.FirstOrDefaultAsync(planId);
                        if (plan != null)
                        {
                            await RecordMaintenanceCompleted(plan.Id, task.ActualEndTime.Value);
                        }
                    }
                }
                else if (task.PlanId != Guid.Empty)
                {
                    await RecordMaintenanceCompleted(task.PlanId, task.ActualEndTime.Value);
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
        /// 委派工单
        /// </summary>
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

                task.OriginalExecutorIds = task.ExecutorNames;

                var newExecutorNames = new List<string>();
                foreach (var userId in input.NewExecutorIds)
                {
                    var userName = await _userAppService.GetNameByUserId(userId);
                    newExecutorNames.Add(userName);
                }

                task.ExecutorIds = string.Join(",", input.NewExecutorIds);
                task.ExecutorNames = string.Join(",", newExecutorNames);
                task.DelegatorId = AbpSession.UserId;
                task.DelegatorName = await _userAppService.GetNameByUserId(AbpSession.UserId.Value);
                task.DelegateReason = input.Reason;
                task.Status = TASK_STATUS_DELEGATED;

                await _taskRepository.UpdateAsync(task);
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
                await CurrentUnitOfWork.SaveChangesAsync();

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
        /// 发送提醒（包含邮件提醒）
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

                var tasks = remindResult.Data;
                int remindCount = 0;
                var tasksByExecutor = new Dictionary<long, List<PendingRemindTaskDto>>();

                foreach (var task in tasks)
                {
                    var taskEntity = await _taskRepository.GetAsync(task.TaskId);
                    taskEntity.IsReminded = true;
                    taskEntity.Status = TASK_STATUS_PENDING;
                    await _taskRepository.UpdateAsync(taskEntity);
                    remindCount++;

                    if (!string.IsNullOrEmpty(taskEntity.ExecutorIds))
                    {
                        var executorIds = taskEntity.ExecutorIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => long.Parse(x)).ToList();

                        foreach (var executorId in executorIds)
                        {
                            if (!tasksByExecutor.ContainsKey(executorId))
                            {
                                tasksByExecutor[executorId] = new List<PendingRemindTaskDto>();
                            }
                            tasksByExecutor[executorId].Add(task);
                        }
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                // 发送邮件提醒（异步，不阻塞主流程）
                await  SendReminderEmails(tasksByExecutor);

                return CommonResult<int>.Success($"成功发送 {remindCount} 个提醒", remindCount);
            }
            catch (Exception ex)
            {
                Logger.Error("发送提醒失败", ex);
                return CommonResult<int>.Error($"发送提醒失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 发生提醒邮件
        /// </summary>
        /// <param name="tasksByExecutor"></param>
        /// <returns></returns>
        private async Task SendReminderEmails(Dictionary<long, List<PendingRemindTaskDto>> tasksByExecutor)
        {
            try
            {
                foreach (var kvp in tasksByExecutor)
                {
                    var executorId = kvp.Key;
                    var executorTasks = kvp.Value;
                    var executorEmail = await GetUserEmail(executorId);

                    if (string.IsNullOrEmpty(executorEmail)) continue;

                    var emailContent = BuildReminderEmailContent(executorTasks,executorId);
                    var emailRequest = new EmailRequest
                    {
                        Subject = $"【待办提醒】您有 {executorTasks.Count} 个保养工单待执行",
                        Body = emailContent,
                        MailTo = new List<string> { executorEmail }
                    };

                    await _emailAppService.SendEmailAsync(emailRequest);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("发送提醒邮件失败", ex);
            }
        }

        /// <summary>
        /// 建立邮件内容
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="executorId"></param>
        /// <returns></returns>
        private string BuildReminderEmailContent(List<PendingRemindTaskDto> tasks, long executorId)
        {
            var executorName = _userAppService.GetNameByUserId(executorId).Result;
            var today = DateTime.Today;

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("<!DOCTYPE html>");
            strBuilder.AppendLine("<html>");
            strBuilder.AppendLine("<head>");
            strBuilder.AppendLine("<meta charset='UTF-8'>");
            strBuilder.AppendLine("<style>");
            strBuilder.AppendLine("body { font-family: 'Microsoft YaHei', Arial, sans-serif; line-height: 1.6; }");
            strBuilder.AppendLine(".container { max-width: 800px; margin: 0 auto; padding: 20px; }");
            strBuilder.AppendLine(".header { background: linear-gradient(135deg, #1890ff 0%, #096dd9 100%); color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }");
            strBuilder.AppendLine(".content { background: #fff; padding: 20px; border: 1px solid #e8e8e8; border-top: none; border-radius: 0 0 8px 8px; }");
            strBuilder.AppendLine("h2 { color: #1890ff; margin-top: 0; }");
            strBuilder.AppendLine("table { border-collapse: collapse; width: 100%; margin: 20px 0; }");
            strBuilder.AppendLine("th { background: #f5f5f5; padding: 12px; text-align: left; font-weight: 500; }");
            strBuilder.AppendLine("td { padding: 10px 12px; border-bottom: 1px solid #f0f0f0; }");
            strBuilder.AppendLine(".btn { display: inline-block; padding: 10px 20px; background: #1890ff; color: white; text-decoration: none; border-radius: 4px; }");
            strBuilder.AppendLine(".footer { margin-top: 20px; padding-top: 20px; border-top: 1px solid #f0f0f0; color: #999; font-size: 12px; text-align: center; }");
            strBuilder.AppendLine("</style>");
            strBuilder.AppendLine("</head>");
            strBuilder.AppendLine("<body>");
            strBuilder.AppendLine("<div class='container'>");
            strBuilder.AppendLine("<div class='header'>");
            strBuilder.AppendLine("<h1>🔔 保养工单待办提醒</h1>");
            strBuilder.AppendLine("</div>");
            strBuilder.AppendLine("<div class='content'>");
            strBuilder.AppendLine($"<h2>您好，{executorName}</h2>");
            strBuilder.AppendLine($"<p>您有 <strong style='color: #1890ff;'>{tasks.Count}</strong> 个保养工单即将开始执行，请及时处理。</p>");
            strBuilder.AppendLine("<table>");
            strBuilder.AppendLine("<thead><tr><th>工单编号</th><th>工单名称</th><th>设备名称</th><th>保养等级</th><th>计划开始</th><th>计划完成</th></tr></thead>");
            strBuilder.AppendLine("<tbody>");
            foreach (var task in tasks)
            {
                strBuilder.AppendLine("<tr>");
                strBuilder.AppendLine($"<td>{task.TaskNo}</td>");
                strBuilder.AppendLine($"<td>{task.TaskName}</td>");
                strBuilder.AppendLine($"<td>{task.DeviceName}</td>");
                strBuilder.AppendLine($"<td>{task.MaintenanceLevel}</td>");
                strBuilder.AppendLine($"<td>{task.PlanStartDate:yyyy-MM-dd}</td>");
                strBuilder.AppendLine($"<td>{task.PlanEndDate:yyyy-MM-dd}</td>");
                strBuilder.AppendLine("</tr>");
            }
            strBuilder.AppendLine("</tbody></table>");
            strBuilder.AppendLine($"<p style='text-align: center;'><a href='{_configuration["WebStation:Host"]}/#/biz/device-maintenance/task?status=待执行' class='btn'>立即处理待办工单</a></p>");
            strBuilder.AppendLine("<div class='footer'>此邮件由系统自动发送，请勿回复。如有疑问，请联系管理员。</div>");
            strBuilder.AppendLine("</div></div></body></html>");
            return strBuilder.ToString();
        }




        /// <summary>
        /// 构建下周保养计划邮件内容
        /// </summary>
        private async Task<string> BuildNextWeekTasksEmailContent(List<MaintenancePlanDto> plans)
        {
            var webHost = _configuration["WebStation:Host"];

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("<!DOCTYPE html>");
            strBuilder.AppendLine("<html>");
            strBuilder.AppendLine("<head>");
            strBuilder.AppendLine("<meta charset='UTF-8'>");
            strBuilder.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            strBuilder.AppendLine("<style>");
            strBuilder.AppendLine("body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; line-height: 1.6; margin: 0; padding: 0; background-color: #f5f7fa; }");
            strBuilder.AppendLine(".email-container { max-width: 700px; margin: 0 auto; background: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }");
            strBuilder.AppendLine(".email-header { background: linear-gradient(135deg, #52c41a 0%, #389e0d 100%); padding: 32px 24px; text-align: center; }");
            strBuilder.AppendLine(".email-header h1 { color: #ffffff; margin: 0; font-size: 24px; font-weight: 500; }");
            strBuilder.AppendLine(".email-header p { color: rgba(255,255,255,0.9); margin: 8px 0 0; }");
            strBuilder.AppendLine(".email-body { padding: 32px 24px; }");
            strBuilder.AppendLine(".greeting { margin-bottom: 24px; }");
            strBuilder.AppendLine(".greeting h2 { color: #333; margin: 0 0 8px; font-size: 20px; }");
            strBuilder.AppendLine(".stats-card { background: #f6ffed; border-radius: 12px; padding: 20px; margin-bottom: 24px; text-align: center; border: 1px solid #b7eb8f; }");
            strBuilder.AppendLine(".stats-number { font-size: 48px; font-weight: 600; color: #52c41a; line-height: 1; }");
            strBuilder.AppendLine(".stats-label { color: #666; margin-top: 8px; }");
            strBuilder.AppendLine("table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
            strBuilder.AppendLine("th { background: #f5f5f5; padding: 12px; text-align: left; font-weight: 500; color: #333; border-bottom: 2px solid #e8e8e8; }");
            strBuilder.AppendLine("td { padding: 12px; border-bottom: 1px solid #f0f0f0; color: #666; }");
            strBuilder.AppendLine(".level-badge { display: inline-block; padding: 2px 8px; border-radius: 4px; font-size: 12px; }");
            strBuilder.AppendLine(".level-month { background: #e6f7ff; color: #1890ff; }");
            strBuilder.AppendLine(".level-quarter { background: #f6ffed; color: #52c41a; }");
            strBuilder.AppendLine(".level-half { background: #fff7e6; color: #fa8c16; }");
            strBuilder.AppendLine(".level-year { background: #f9f0ff; color: #722ed1; }");
            strBuilder.AppendLine(".btn { display: inline-block; padding: 12px 28px; background: #52c41a; color: #ffffff; text-decoration: none; border-radius: 8px; font-weight: 500; }");
            strBuilder.AppendLine(".btn:hover { background: #389e0d; }");
            strBuilder.AppendLine(".email-footer { background: #fafafa; padding: 20px 24px; text-align: center; border-top: 1px solid #f0f0f0; color: #999; font-size: 12px; }");
            strBuilder.AppendLine("</style>");
            strBuilder.AppendLine("</head>");
            strBuilder.AppendLine("<body>");
            strBuilder.AppendLine($"<div class='email-container'>");
            strBuilder.AppendLine($"<div class='email-header'>");
            strBuilder.AppendLine($"<h1>📋 下周保养计划通知</h1>");
            strBuilder.AppendLine($"<p>系统自动生成，请提前安排</p>");
            strBuilder.AppendLine($"</div>");
            strBuilder.AppendLine($"<div class='email-body'>");
            strBuilder.AppendLine($"<div class='greeting'>");
            strBuilder.AppendLine($"<h2>您好</h2>");
            strBuilder.AppendLine($"<p>以下是下周需要执行的保养计划，系统已自动生成对应工单，请相关执行人提前做好准备。</p>");
            strBuilder.AppendLine($"</div>");
            strBuilder.AppendLine($"<div class='stats-card'>");
            strBuilder.AppendLine($"<div class='stats-number'>{plans.Count}</div>");
            strBuilder.AppendLine($"<div class='stats-label'>个下周保养计划</div>");
            strBuilder.AppendLine($"</div>");
            strBuilder.AppendLine("<table>");
            strBuilder.AppendLine("<thead><tr><th>计划名称</th><th>设备名称</th><th>保养等级</th><th>计划执行日期</th></tr></thead>");
            strBuilder.AppendLine("<tbody>");
            foreach (var plan in plans)
            {
                var levelClass = plan.MaintenanceLevel switch
                {
                    "月度" => "level-month",
                    "季度" => "level-quarter",
                    "半年度" => "level-half",
                    "年度" => "level-year",
                    _ => ""
                };
                strBuilder.AppendLine("<tr>");
                strBuilder.AppendLine($"<td>{plan.PlanName}</td>");
                strBuilder.AppendLine($"<td>{plan.DeviceName}<br/><span style='font-size:11px;color:#999;'>{plan.DeviceCode}</span></td>");
                strBuilder.AppendLine($"<td><span class='level-badge {levelClass}'>{plan.MaintenanceLevelText}</span></td>");
                strBuilder.AppendLine($"<td>{plan.NextMaintenanceDate:yyyy-MM-dd}</td>");
                strBuilder.AppendLine("</tr>");
            }
            strBuilder.AppendLine("</tbody>");
            strBuilder.AppendLine("</table>");
            strBuilder.AppendLine($"<div style='text-align: center; margin: 32px 0;'>");
            strBuilder.AppendLine($"<a href='{webHost}/#/biz/device-maintenance/plan' class='btn'>查看全部保养计划</a>");
            strBuilder.AppendLine($"</div>");
            strBuilder.AppendLine($"</div>");
            strBuilder.AppendLine($"<div class='email-footer'>");
            strBuilder.AppendLine($"<p>此邮件由设备保养管理系统自动发送，请勿回复。</p>");
            strBuilder.AppendLine($"<p>如有疑问，请联系系统管理员。</p>");
            strBuilder.AppendLine($"<p>&copy; {DateTime.Now.Year} 设备保养管理系统</p>");
            strBuilder.AppendLine($"</div>");
            strBuilder.AppendLine($"</div>");
            strBuilder.AppendLine("</body>");
            strBuilder.AppendLine("</html>");

            return strBuilder.ToString();
        }

        #endregion


        #region 统计分析

        /// <summary>
        /// 获取保养统计看板数据（使用联表查询）
        /// </summary>
        public async Task<CommonResult<MaintenanceStatisticsDto>> GetMaintenanceStatistics([FromQuery] MaintenanceStatisticsInput input)
        {
            try
            {
                var today = DateTime.Today;
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                // 使用联表查询：工单 -> 设备 -> 模板 -> 设备类型
                var query = from t in _taskRepository.GetAll().AsNoTracking()
                            join d in _deviceRepository.GetAll().AsNoTracking() on t.DeviceId equals d.Id into deviceJoin
                            from d in deviceJoin.DefaultIfEmpty()
                            join tm in _templateRepository.GetAll().AsNoTracking() on t.TemplateId equals tm.Id into templateJoin
                            from tm in templateJoin.DefaultIfEmpty()
                            join dtr in _deviceTypeRelationRepository.GetAll().AsNoTracking() on d.Id equals dtr.DeviceId into deviceTypeJoin
                            from dtr in deviceTypeJoin.DefaultIfEmpty()
                            join dt in _typeRepository.GetAll().AsNoTracking() on dtr.TypeId equals dt.Id into typeJoin
                            from dt in typeJoin.DefaultIfEmpty()
                            select new
                            {
                                Task = t,
                                Device = d,
                                Template = tm,
                                DeviceType = dt
                            };

                // 应用设备名称筛选
                if (!string.IsNullOrWhiteSpace(input.DeviceName))
                {
                    var keywords = input.DeviceName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var keyword in keywords)
                    {
                        query = query.Where(x => x.Device != null && x.Device.DeviceName != null && x.Device.DeviceName.Contains(keyword));
                    }
                }

                // 应用设备编码筛选
                if (!string.IsNullOrWhiteSpace(input.DeviceCode))
                {
                    query = query.Where(x => x.Device != null && x.Device.DeviceCode != null && x.Device.DeviceCode.Contains(input.DeviceCode));
                }

                // 应用设备类型筛选
                if (input.DeviceTypeIds != null && input.DeviceTypeIds.Any())
                {
                    query = query.Where(x => x.DeviceType != null && input.DeviceTypeIds.Contains(x.DeviceType.Id));
                }

                // 应用保养等级筛选
                if (input.MaintenanceLevels != null && input.MaintenanceLevels.Any())
                {
                    query = query.Where(x => x.Task != null && x.Task.MaintenanceLevel != null &&
                                            input.MaintenanceLevels.Contains(x.Task.MaintenanceLevel));
                }

                // 应用保养频次筛选
                if (input.MaintenanceFrequencies != null && input.MaintenanceFrequencies.Any())
                {
                    query = query.Where(x => x.Task != null && x.Task.MaintenanceLevel != null &&
                                            input.MaintenanceFrequencies.Contains(GetMaintenanceFrequency(x.Task.MaintenanceLevel)));
                }

                // 应用日期筛选
                if (input.StartDate.HasValue)
                {
                    query = query.Where(x => x.Task.PlanEndDate >= input.StartDate.Value);
                }

                if (input.EndDate.HasValue)
                {
                    var endDate = input.EndDate.Value.AddDays(1).AddSeconds(-1);
                    query = query.Where(x => x.Task.PlanEndDate <= endDate);
                }

                var allData = await query.ToListAsync();

                // 获取设备类型选项（修复空引用问题）
                var deviceTypeOptions = new List<DeviceTypeOptionDto>();
                try
                {
                    deviceTypeOptions = await _typeRepository.GetAll().AsNoTracking()
                        .Select(x => new DeviceTypeOptionDto
                        {
                            Id = x.Id,
                            TypeName = x.TypeName ?? "未分类"
                        })
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    Logger.Error("获取设备类型选项失败", ex);
                }

                // 获取设备编码选项
                var deviceCodeOptions = new List<string>();
                try
                {
                    deviceCodeOptions = await _deviceRepository.GetAll()
                        .Select(x => x.DeviceCode)
                        .Where(x => x != null && !string.IsNullOrEmpty(x))
                        .Distinct()
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    Logger.Error("获取设备编码选项失败", ex);
                }

                // 1. 今日待完成任务
                var todayPending = allData.Where(x =>
                    x.Task != null &&
                    x.Task.PlanEndDate.Date == today &&
                    x.Task.Status != TASK_STATUS_COMPLETED).ToList();

                // 2. 今日已完成任务
                var todayCompleted = allData.Where(x =>
                    x.Task != null &&
                    x.Task.ActualEndTime.HasValue &&
                    x.Task.ActualEndTime.Value.Date == today &&
                    x.Task.Status == TASK_STATUS_COMPLETED).ToList();

                // 3. 本月已完成任务
                var monthCompleted = allData.Where(x =>
                    x.Task != null &&
                    x.Task.ActualEndTime.HasValue &&
                    x.Task.ActualEndTime.Value >= monthStart &&
                    x.Task.ActualEndTime.Value <= monthEnd &&
                    x.Task.Status == TASK_STATUS_COMPLETED).ToList();

                // 4. 本月未完成任务
                var monthPending = allData.Where(x =>
                    x.Task != null &&
                    x.Task.PlanEndDate >= monthStart &&
                    x.Task.PlanEndDate <= monthEnd &&
                    x.Task.Status != TASK_STATUS_COMPLETED).ToList();

                // 5. 本月待完成任务等级分布
                var monthLevelDistribution = monthPending
                    .Where(x => x.Task != null && !string.IsNullOrEmpty(x.Task.MaintenanceLevel))
                    .GroupBy(x => x.Task.MaintenanceLevel)
                    .Select(g => new LevelDistributionDto
                    {
                        Level = g.Key ?? "未分类",
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();

                // 6. 保养完成率分析（按设备+月份统计）
                var completionRates = new List<CompletionRateDto>();
                var deviceGroups = allData.Where(x => x.Device != null).GroupBy(x => x.Device.Id);

                foreach (var deviceGroup in deviceGroups)
                {
                    var device = deviceGroup.FirstOrDefault()?.Device;
                    if (device == null) continue;

                    var deviceType = deviceGroup.FirstOrDefault()?.DeviceType;
                    var months = deviceGroup
                        .Where(x => x.Task != null)
                        .Select(x => x.Task.PlanEndDate.ToString("yyyy-MM"))
                        .Distinct()
                        .OrderByDescending(m => m)
                        .Take(12);

                    foreach (var month in months)
                    {
                        var monthTasks = deviceGroup.Where(x => x.Task != null && x.Task.PlanEndDate.ToString("yyyy-MM") == month).ToList();
                        if (!monthTasks.Any()) continue;

                        var totalCount = monthTasks.Count;
                        var completedCount = monthTasks.Count(x => x.Task.Status == TASK_STATUS_COMPLETED);

                        // 获取该设备的保养等级分布（取最常见的等级）
                        var commonLevel = monthTasks
                            .Where(x => !string.IsNullOrEmpty(x.Task.MaintenanceLevel))
                            .GroupBy(x => x.Task.MaintenanceLevel)
                            .OrderByDescending(g => g.Count())
                            .FirstOrDefault()?.Key ?? "日常保养";

                        completionRates.Add(new CompletionRateDto
                        {
                            DeviceName = device.DeviceName ?? "未知设备",
                            DeviceCode = device.DeviceCode ?? "未知编码",
                            DeviceTypeName = deviceType?.TypeName ?? "未分类",
                            MaintenanceLevel = commonLevel,
                            MaintenanceFrequency = GetMaintenanceFrequency(commonLevel),
                            StatisticsDate = DateTime.ParseExact(month, "yyyy-MM", null),
                            TotalCount = totalCount,
                            CompletedCount = completedCount,
                            CompletionRate = totalCount > 0 ? Math.Round((decimal)completedCount / totalCount * 100, 2) : 0
                        });
                    }
                }

                // 7. 保养次数统计（按设备分组）
                var maintenanceCounts = allData
                    .Where(x => x.Device != null && !string.IsNullOrEmpty(x.Device.DeviceName))
                    .GroupBy(x => x.Device.Id)
                    .Select(g => new DeviceMaintenanceCountDto
                    {
                        DeviceName = g.FirstOrDefault()?.Device?.DeviceName ?? "未知设备",
                        DeviceCode = g.FirstOrDefault()?.Device?.DeviceCode ?? "未知编码",
                        DeviceTypeName = g.FirstOrDefault()?.DeviceType?.TypeName ?? "未分类",
                        TotalCount = g.Count()
                    })
                    .OrderByDescending(x => x.TotalCount)
                    .Take(10)
                    .ToList();

                // 8. 保养次数时间趋势（按月统计）
                var maintenanceTrends = allData
                    .Where(x => x.Device != null && !string.IsNullOrEmpty(x.Device.DeviceName) && x.Task != null)
                    .GroupBy(x => new {
                        DeviceName = x.Device.DeviceName,
                        Month = x.Task.PlanEndDate.ToString("yyyy-MM")
                    })
                    .Select(g => new MaintenanceTrendDto
                    {
                        Month = DateTime.ParseExact(g.Key.Month, "yyyy-MM", null),
                        DeviceName = g.Key.DeviceName ?? "未知设备",
                        Count = g.Count()
                    })
                    .OrderBy(t => t.Month)
                    .ThenBy(t => t.DeviceName)
                    .ToList();

                // 9. 已完成保养次数与等级分布
                var completedDistributions = allData
                    .Where(x => x.Device != null && !string.IsNullOrEmpty(x.Device.DeviceName) && x.Task != null)
                    .GroupBy(x => new {
                        DeviceId = x.Device.Id,
                        DeviceName = x.Device.DeviceName,
                        DeviceCode = x.Device.DeviceCode,
                        DeviceTypeName = x.DeviceType != null ? x.DeviceType.TypeName : "未分类",
                        MaintenanceLevel = x.Task.MaintenanceLevel ?? "未分类"
                    })
                    .Select(g => new CompletedMaintenanceDistributionDto
                    {
                        DeviceName = g.Key.DeviceName ?? "未知设备",
                        DeviceCode = g.Key.DeviceCode ?? "未知编码",
                        DeviceTypeName = g.Key.DeviceTypeName,
                        MaintenanceLevel = g.Key.MaintenanceLevel,
                        MaintenanceFrequency = GetMaintenanceFrequency(g.Key.MaintenanceLevel),
                        TotalCount = g.Count(),
                        CompletedCount = g.Count(x => x.Task.Status == TASK_STATUS_COMPLETED),
                        PendingCount = g.Count(x => x.Task.Status != TASK_STATUS_COMPLETED)
                    })
                    .OrderBy(x => x.DeviceName)
                    .ThenBy(x => x.MaintenanceLevel)
                    .ToList();

                // 10. 保养记录明细（分页）
                var records = allData
                    .Where(x => x.Task != null && x.Device != null)
                    .OrderByDescending(x => x.Task.PlanEndDate)
                    .Take(20)
                    .Select(x => new MaintenanceRecordDto
                    {
                        Id = x.Task.Id,
                        TaskNo = x.Task.TaskNo ?? "未知",
                        DeviceCode = x.Device?.DeviceCode ?? "未知",
                        DeviceName = x.Device?.DeviceName ?? "未知设备",
                        DeviceTypeName = x.DeviceType?.TypeName ?? "未分类",
                        MaintenanceResult = x.Task.Status == TASK_STATUS_COMPLETED ? "已完成" : "未完成",
                        MaintenanceTime = x.Task.ActualEndTime ?? x.Task.PlanEndDate,
                        ExecutorName = x.Task.ExecutorNames ?? "未知",
                        MaintenanceLevel = x.Task.MaintenanceLevel ?? "未分类",
                        DeviceStatus = x.Task.Status == TASK_STATUS_COMPLETED ? "正常" : "待保养"
                    })
                    .ToList();

                var result = new MaintenanceStatisticsDto
                {
                    TodayPendingCount = todayPending.Count,
                    TodayCompletedCount = todayCompleted.Count,
                    MonthCompletedCount = monthCompleted.Count,
                    MonthPendingCount = monthPending.Count,
                    MonthLevelDistribution = monthLevelDistribution,
                    CompletionRates = completionRates,
                    MaintenanceCounts = maintenanceCounts,
                    MaintenanceTrends = maintenanceTrends,
                    CompletedDistributions = completedDistributions,
                    Records = records,
                    TotalRecords = allData.Count,
                    DeviceTypeOptions = deviceTypeOptions,
                    DeviceCodeOptions = deviceCodeOptions
                };

                return CommonResult<MaintenanceStatisticsDto>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error($"获取保养统计看板数据失败: {ex.Message}", ex);
                Logger.Error($"异常堆栈: {ex.StackTrace}");
                return CommonResult<MaintenanceStatisticsDto>.Error($"获取保养统计看板数据失败: {ex.Message}");
            }
        }


        /// <summary>
        /// 获取保养频次文本
        /// </summary>
        private string GetMaintenanceFrequency(string level)
        {
            return level switch
            {
                "日常保养" => "每天一次",
                "月度" => "每月一次",
                "季度" => "每季度一次",
                "半年度" => "每半年一次",
                "年度" => "每年一次",
                _ => level
            };
        }
        #endregion



        #region 辅助方法

        private async Task RecordMaintenanceCompleted(Guid planId, DateTime actualDate)
        {
            var plan = await _planRepository.FirstOrDefaultAsync(planId);
            if (plan == null) return;

            plan.LastMaintenanceDate = actualDate;
            plan.NextMaintenanceDate = CalculateNextMaintenanceDate(actualDate, plan.MaintenanceLevel);
            plan.HasGeneratedTask = false;
            await _planRepository.UpdateAsync(plan);
        }

        private DateTime CalculateNextMaintenanceDate(DateTime lastDate, string level)
        {
            int days = MaintenanceCycleConstants.GetCycleDays(level);
            DateTime today = DateTime.Today;

            DateTime nextDate;

            if (lastDate < today)
            {
                int daysPassed = (today - lastDate).Days;
                int cyclesNeeded = (int)Math.Ceiling((double)daysPassed / days);
                cyclesNeeded = Math.Max(1, cyclesNeeded);
                nextDate = lastDate.AddDays(days * cyclesNeeded);
                while (nextDate <= today)
                {
                    nextDate = nextDate.AddDays(days);
                }
            }
            else
            {
                nextDate = lastDate.AddDays(days);
            }

            while (!WorkdayHelper.IsWorkday(nextDate))
            {
                nextDate = nextDate.AddDays(1);
            }

            return nextDate;
        }

        private async Task<List<long>> GetDeviceMaintainers(Guid deviceId)
        {
            return await _deviceUserRelationRepository.GetAll()
                .Where(x => x.DeviceId == deviceId && x.UserType == "保养人员")
                .Select(x => x.UserId)
                .ToListAsync();
        }

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

        private async Task<string> GetUserEmail(long userId)
        {
            try
            {
                var user = await _userAppService.GetByIdAsync(new Abp.Application.Services.Dto.EntityDto<long>(userId));
                return user.Data.EmailAddress;
            }
            catch
            {
                return null;
            }
        }

        private async Task<MaintenanceTaskDto> MapToDto(MaintenanceTasks task, Devices device, MaintenanceTemplates template)
        {
            var dto = new MaintenanceTaskDto
            {
                Id = task.Id,
                TaskNo = task.TaskNo,
                TaskName = task.TaskName,
                DeviceId = task.DeviceId,
                DeviceCode = device?.DeviceCode,
                DeviceName = device?.DeviceName,
                DeviceLocation = device?.Location,
                DeviceTypeName = device != null ? await GetDeviceTypeName(device.Id) : null,
                PlanId = task.PlanId,
                TemplateId = task.TemplateId,
                TemplateName = template?.TemplateName,
                MaintenanceLevel = task.MaintenanceLevel,
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
                DelegateReason = task.DelegateReason,
                IsMergedTask = task.IsMergedTask,
                MergedPlanIds = task.MergedPlanIds,
                RemainingDays = (task.PlanEndDate - DateTime.Today).Days,
                IsOverdue = task.PlanEndDate < DateTime.Today && task.Status != TASK_STATUS_COMPLETED
            };

            if (!string.IsNullOrEmpty(task.ExecutorIds))
            {
                dto.ExecutorIds = task.ExecutorIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => long.Parse(x)).ToList();
            }

            if (!string.IsNullOrEmpty(task.ExecutorNames))
            {
                dto.ExecutorNames = task.ExecutorNames.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            return dto;
        }

        private async Task<string> GetDeviceTypeName(Guid deviceId)
        {
            var typeRelation = await _deviceTypeRelationRepository.FirstOrDefaultAsync(x => x.DeviceId == deviceId);
            if (typeRelation != null)
            {
                var type = await _typeRepository.FirstOrDefaultAsync(typeRelation.TypeId);
                return type?.TypeName;
            }
            return null;
        }

        #endregion
    }
}