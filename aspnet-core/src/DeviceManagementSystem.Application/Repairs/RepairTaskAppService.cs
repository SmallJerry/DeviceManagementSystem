using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using DeviceManagementSystem.Attachment;
using DeviceManagementSystem.Attachment.Dto;
using DeviceManagementSystem.BasicDataManagement;
using DeviceManagementSystem.DeviceInfos;
using DeviceManagementSystem.Maintenances;
using DeviceManagementSystem.Maintenances.Dto;
using DeviceManagementSystem.Maintenances.Interface;
using DeviceManagementSystem.Repairs.Dto;
using DeviceManagementSystem.Repairs.Interface;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Repairs
{
    /// <summary>
    /// 维修工单服务实现
    /// </summary>
    [AbpAuthorize]
    [Audited]
    public class RepairTaskAppService : DeviceManagementSystemAppServiceBase, IRepairTaskAppService
    {
        private readonly IRepository<RepairTasks, Guid> _repairTaskRepository;
        private readonly IRepository<RepairRequests, Guid> _repairRequestRepository;
        private readonly IRepository<RepairTaskExecutionRecords, Guid> _executionRecordRepository;
        private readonly IRepository<Devices, Guid> _deviceRepository;
        private readonly IRepository<DeviceUserRelations, Guid> _deviceUserRelationRepository;
        private readonly IRepository<DeviceTypeRelations, Guid> _deviceTypeRelationRepository;
        private readonly IRepository<Types, Guid> _typeRepository;
        private readonly IUserAppService _userAppService;
        private readonly IAttachmentAppService _attachmentAppService;
        private readonly IMaintenanceTaskAppService _maintenanceTaskAppService;
        private readonly IRepository<MaintenancePlans, Guid> _maintenancePlanRepository;

        // 工单状态常量
        private const int TASK_STATUS_PENDING = 0;      // 待接单
        private const int TASK_STATUS_REPAIRING = 1;    // 维修中
        private const int TASK_STATUS_PENDING_ACCEPTANCE = 2;  // 待验收
        private const int TASK_STATUS_COMPLETED = 3;    // 已完成
        private const int TASK_STATUS_CANCELLED = 4;    // 已取消

        // 申报状态常量
        private const int REQUEST_STATUS_PENDING_DISPATCH = 0;
        private const int REQUEST_STATUS_DISPATCHED = 1;
        private const int REQUEST_STATUS_REPAIRING = 2;
        private const int REQUEST_STATUS_COMPLETED = 3;
        private const int REQUEST_STATUS_CANCELLED = 4;

        public RepairTaskAppService(
            IRepository<RepairTasks, Guid> repairTaskRepository,
            IRepository<RepairRequests, Guid> repairRequestRepository,
            IRepository<RepairTaskExecutionRecords, Guid> executionRecordRepository,
            IRepository<Devices, Guid> deviceRepository,
            IRepository<DeviceUserRelations, Guid> deviceUserRelationRepository,
            IRepository<DeviceTypeRelations, Guid> deviceTypeRelationRepository,
            IRepository<Types, Guid> typeRepository,
            IUserAppService userAppService,
            IAttachmentAppService attachmentAppService,
            IMaintenanceTaskAppService maintenanceTaskAppService,
            IRepository<MaintenancePlans, Guid> maintenancePlanRepository)
        {
            _repairTaskRepository = repairTaskRepository;
            _repairRequestRepository = repairRequestRepository;
            _executionRecordRepository = executionRecordRepository;
            _deviceRepository = deviceRepository;
            _deviceUserRelationRepository = deviceUserRelationRepository;
            _deviceTypeRelationRepository = deviceTypeRelationRepository;
            _typeRepository = typeRepository;
            _userAppService = userAppService;
            _attachmentAppService = attachmentAppService;
            _maintenanceTaskAppService = maintenanceTaskAppService;
            _maintenancePlanRepository = maintenancePlanRepository;
        }

        /// <summary>
        /// 获取维修工单分页列表
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<Page<RepairTaskDto>>> GetPageList([FromQuery] RepairTaskPageInput input)
        {
            try
            {
                if (input.Size > 100) input.Size = 100;

                var query = from t in _repairTaskRepository.GetAll().AsNoTracking()
                            join r in _repairRequestRepository.GetAll().AsNoTracking() on t.RepairRequestId equals r.Id
                            join d in _deviceRepository.GetAll().AsNoTracking() on t.DeviceId equals d.Id into deviceJoin
                            from d in deviceJoin.DefaultIfEmpty()
                            join dtr in _deviceTypeRelationRepository.GetAll().AsNoTracking() on d.Id equals dtr.DeviceId into deviceTypeJoin
                            from dtr in deviceTypeJoin.DefaultIfEmpty()
                            join dt in _typeRepository.GetAll().AsNoTracking() on dtr.TypeId equals dt.Id into typeJoin
                            from dt in typeJoin.DefaultIfEmpty()
                            select new { Task = t, Request = r, Device = d, DeviceType = dt };

                // 应用过滤条件
                if (!string.IsNullOrWhiteSpace(input.SearchKey))
                {
                    query = query.Where(x =>
                        x.Task.TaskNo.Contains(input.SearchKey) ||
                        (x.Device != null && x.Device.DeviceName.Contains(input.SearchKey)) ||
                        (x.Device != null && x.Device.DeviceCode.Contains(input.SearchKey)));
                }

                if (input.TaskStatus.HasValue)
                {
                    query = query.Where(x => x.Task.TaskStatus == input.TaskStatus.Value);
                }

                if (input.DeviceId.HasValue)
                {
                    query = query.Where(x => x.Task.DeviceId == input.DeviceId.Value);
                }

                if (input.RepairerId.HasValue)
                {
                    query = query.Where(x => x.Task.RepairerIds.Contains(input.RepairerId.Value.ToString()));
                }

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Task.CreationTime)
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var result = new List<RepairTaskDto>();

                foreach (var item in items)
                {
                    var dto = await MapToDto(item.Task, item.Request, item.Device, item.DeviceType);
                    result.Add(dto);
                }

                var page = new Page<RepairTaskDto>(input.Current, input.Size, total)
                {
                    Records = result
                };

                return CommonResult<Page<RepairTaskDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取维修工单分页列表失败", ex);
                return CommonResult<Page<RepairTaskDto>>.Error($"获取维修工单分页列表失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取维修工单详情
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<RepairTaskDto>> GetById(Guid id)
        {
            try
            {
                var task = await _repairTaskRepository.FirstOrDefaultAsync(id);
                if (task == null)
                {
                    return CommonResult<RepairTaskDto>.Error("维修工单不存在");
                }

                var request = await _repairRequestRepository.FirstOrDefaultAsync(task.RepairRequestId);
                var device = await _deviceRepository.FirstOrDefaultAsync(task.DeviceId);

                Guid? deviceTypeId = null;
                string deviceTypeName = null;

                if (device != null)
                {
                    var deviceTypeRelation = await _deviceTypeRelationRepository
                        .FirstOrDefaultAsync(x => x.DeviceId == device.Id);
                    if (deviceTypeRelation != null)
                    {
                        deviceTypeId = deviceTypeRelation.TypeId;
                        var type = await _typeRepository.FirstOrDefaultAsync(deviceTypeId.Value);
                        deviceTypeName = type?.TypeName;
                    }
                }

                var dto = await MapToDto(task, request, device, null);
                dto.DeviceTypeId = deviceTypeId;
                dto.DeviceTypeName = deviceTypeName;

                return CommonResult<RepairTaskDto>.Success(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("获取维修工单详情失败", ex);
                return CommonResult<RepairTaskDto>.Error($"获取维修工单详情失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 派单（分配维修人员）
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Dispatch(RepairTaskInput input)
        {
            try
            {
                if (!input.Id.HasValue)
                {
                    return CommonResult.Error("工单ID不能为空");
                }

                var task = await _repairTaskRepository.FirstOrDefaultAsync(input.Id.Value);
                if (task == null)
                {
                    return CommonResult.Error("维修工单不存在");
                }

                if (task.TaskStatus != TASK_STATUS_PENDING)
                {
                    return CommonResult.Error($"当前状态({GetTaskStatusText(task.TaskStatus)})不可派单");
                }

                if (input.RepairerIds == null || !input.RepairerIds.Any())
                {
                    return CommonResult.Error("请选择维修人员");
                }

                var repairerNames = new List<string>();
                foreach (var userId in input.RepairerIds)
                {
                    var userName = await _userAppService.GetNameByUserId(userId);
                    repairerNames.Add(userName);
                }

                task.RepairerIds = string.Join(",", input.RepairerIds);
                task.RepairerNames = string.Join(",", repairerNames);
                task.TaskStatus = TASK_STATUS_REPAIRING;
                task.AcceptTime = DateTime.Now;

                await _repairTaskRepository.UpdateAsync(task);

                // 更新申报状态
                var request = await _repairRequestRepository.FirstOrDefaultAsync(task.RepairRequestId);
                if (request != null)
                {
                    request.RequestStatus = REQUEST_STATUS_REPAIRING;
                    await _repairRequestRepository.UpdateAsync(request);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                // TODO: 发送通知给维修人员

                return CommonResult.Ok("派单成功");
            }
            catch (Exception ex)
            {
                Logger.Error("派单失败", ex);
                return CommonResult.Error($"派单失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 接单
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Accept(AcceptRepairTaskInput input)
        {
            try
            {
                var task = await _repairTaskRepository.FirstOrDefaultAsync(input.TaskId);
                if (task == null)
                {
                    return CommonResult.Error("维修工单不存在");
                }

                var userId = AbpSession.UserId.Value;
                if (!task.RepairerIds.Contains(userId.ToString()))
                {
                    return CommonResult.Error("您不是该工单的维修人员");
                }

                if (task.TaskStatus != TASK_STATUS_PENDING)
                {
                    return CommonResult.Error($"当前状态({GetTaskStatusText(task.TaskStatus)})不可接单");
                }

                task.AcceptTime = input.AcceptTime ?? DateTime.Now;
                task.TaskStatus = TASK_STATUS_REPAIRING;

                await _repairTaskRepository.UpdateAsync(task);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("接单成功");
            }
            catch (Exception ex)
            {
                Logger.Error("接单失败", ex);
                return CommonResult.Error($"接单失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 开始维修
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Start(StartRepairTaskInput input)
        {
            try
            {
                var task = await _repairTaskRepository.FirstOrDefaultAsync(input.TaskId);
                if (task == null)
                {
                    return CommonResult.Error("维修工单不存在");
                }

                var userId = AbpSession.UserId.Value;
                if (!task.RepairerIds.Contains(userId.ToString()))
                {
                    return CommonResult.Error("您不是该工单的维修人员");
                }

                if (task.TaskStatus != TASK_STATUS_REPAIRING)
                {
                    return CommonResult.Error($"当前状态({GetTaskStatusText(task.TaskStatus)})不可开始维修");
                }

                task.StartTime = input.StartTime ?? DateTime.Now;

                await _repairTaskRepository.UpdateAsync(task);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("开始维修成功");
            }
            catch (Exception ex)
            {
                Logger.Error("开始维修失败", ex);
                return CommonResult.Error($"开始维修失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 保存维修执行记录（草稿）
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> SaveExecution(RepairTaskExecutionInput input)
        {
            try
            {
                var task = await _repairTaskRepository.FirstOrDefaultAsync(input.TaskId);
                if (task == null)
                {
                    return CommonResult.Error("维修工单不存在");
                }

                var userId = AbpSession.UserId.Value;
                if (!task.RepairerIds.Contains(userId.ToString()))
                {
                    return CommonResult.Error("您不是该工单的维修人员");
                }

                // 保存或更新执行记录
                var existingRecord = await _executionRecordRepository
                    .FirstOrDefaultAsync(x => x.RepairTaskId == input.TaskId && x.SaveType == input.SaveType);

                if (existingRecord != null)
                {
                    existingRecord.FaultReason = input.FaultReason;
                    existingRecord.QualityImpactAnalysis = input.QualityImpactAnalysis;
                    existingRecord.RepairMethodResult = input.RepairMethodResult;
                    await _executionRecordRepository.UpdateAsync(existingRecord);
                }
                else
                {
                    var record = new RepairTaskExecutionRecords
                    {
                        RepairTaskId = input.TaskId,
                        FaultReason = input.FaultReason,
                        QualityImpactAnalysis = input.QualityImpactAnalysis,
                        RepairMethodResult = input.RepairMethodResult,
                        SaveType = input.SaveType
                    };
                    await _executionRecordRepository.InsertAsync(record);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("保存成功");
            }
            catch (Exception ex)
            {
                Logger.Error("保存维修执行记录失败", ex);
                return CommonResult.Error($"保存维修执行记录失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 完成维修
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Complete(CompleteRepairTaskInput input)
        {
            try
            {
                var task = await _repairTaskRepository.FirstOrDefaultAsync(input.TaskId);
                if (task == null)
                {
                    return CommonResult.Error("维修工单不存在");
                }

                var userId = AbpSession.UserId.Value;
                if (!task.RepairerIds.Contains(userId.ToString()))
                {
                    return CommonResult.Error("您不是该工单的维修人员");
                }

                if (task.TaskStatus != TASK_STATUS_REPAIRING)
                {
                    return CommonResult.Error($"当前状态({GetTaskStatusText(task.TaskStatus)})不可完成维修");
                }

                // 保存执行记录
                task.FaultReason = input.FaultReason;
                task.QualityImpactAnalysis = input.QualityImpactAnalysis;
                task.RepairMethodResult = input.RepairMethodResult;
                task.EndTime = input.EndTime ?? DateTime.Now;

                // 计算维修时长（分钟）
                var startTime = task.StartTime ?? task.AcceptTime ?? task.CreationTime;
                task.DurationMinutes = (int)(task.EndTime.Value - startTime).TotalMinutes;

                // 检查是否超时
                var request = await _repairRequestRepository.FirstOrDefaultAsync(task.RepairRequestId);
                if (request != null && task.EndTime.Value > request.ExpectedCompleteTime)
                {
                    task.IsOverdue = 1;
                }

                // 设置是否通知保养
                task.NotifyMaintenance = input.NotifyMaintenance;

                if (input.NotifyMaintenance == 1 && input.MaintenancePlanId.HasValue)
                {
                    task.MaintenancePlanId = input.MaintenancePlanId.Value;

                    // 生成临时保养工单
                    var plan = await _maintenancePlanRepository.FirstOrDefaultAsync(input.MaintenancePlanId.Value);
                    if (plan != null)
                    {
                        var generateResult = await _maintenanceTaskAppService.GenerateManualWithDate(
                            new GenerateTaskInput
                            {
                                PlanId = plan.Id,
                                ExecuteDate = DateTime.Today
                            });

                        if (generateResult.IsSuccess)
                        {
                            // 获取生成的保养工单ID
                            var newTask = await _maintenanceTaskAppService.GetByPlanId(plan.Id);

                            if (newTask.IsSuccess)
                            {
                                task.MaintenanceTaskId = newTask.Data.Id;
                            }
                        }
                    }
                }

                // 更新工单状态为待验收
                task.TaskStatus = TASK_STATUS_PENDING_ACCEPTANCE;

                // 更新申报状态
                if (request != null)
                {
                    request.RequestStatus = REQUEST_STATUS_REPAIRING;
                    await _repairRequestRepository.UpdateAsync(request);
                }

                await _repairTaskRepository.UpdateAsync(task);
                await CurrentUnitOfWork.SaveChangesAsync();

                // 通知设备创建人进行验收
                var device = await _deviceRepository.FirstOrDefaultAsync(task.DeviceId);
                if (device != null && device.CreatorUserId.HasValue)
                {
                    task.AcceptorId = device.CreatorUserId.Value;
                    task.AcceptorName = await _userAppService.GetNameByUserId(device.CreatorUserId.Value);
                    task.NotifiedAcceptance = 1;
                    await _repairTaskRepository.UpdateAsync(task);
                    await CurrentUnitOfWork.SaveChangesAsync();

                    // TODO: 发送验收通知
                }

                return CommonResult.Ok("维修完成");
            }
            catch (Exception ex)
            {
                Logger.Error("完成维修失败", ex);
                return CommonResult.Error($"完成维修失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 取消工单
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Cancel(Guid taskId, string reason)
        {
            try
            {
                var task = await _repairTaskRepository.FirstOrDefaultAsync(taskId);
                if (task == null)
                {
                    return CommonResult.Error("维修工单不存在");
                }

                if (task.TaskStatus == TASK_STATUS_COMPLETED)
                {
                    return CommonResult.Error("已完成工单不能取消");
                }

                task.TaskStatus = TASK_STATUS_CANCELLED;

                // 更新申报状态
                var request = await _repairRequestRepository.FirstOrDefaultAsync(task.RepairRequestId);
                if (request != null)
                {
                    request.RequestStatus = REQUEST_STATUS_CANCELLED;
                    await _repairRequestRepository.UpdateAsync(request);
                }

                await _repairTaskRepository.UpdateAsync(task);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("取消成功");
            }
            catch (Exception ex)
            {
                Logger.Error("取消工单失败", ex);
                return CommonResult.Error($"取消工单失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取我的待接单工单
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<Page<RepairTaskDto>>> GetMyPendingTasks([FromQuery] RepairTaskPageInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                var userIdStr = userId.ToString();

                if (input.Size > 100) input.Size = 100;

                var query = from t in _repairTaskRepository.GetAll().AsNoTracking()
                            where t.TaskStatus == TASK_STATUS_PENDING &&
                                  t.RepairerIds.Contains(userIdStr)
                            join r in _repairRequestRepository.GetAll().AsNoTracking() on t.RepairRequestId equals r.Id
                            join d in _deviceRepository.GetAll().AsNoTracking() on t.DeviceId equals d.Id into deviceJoin
                            from d in deviceJoin.DefaultIfEmpty()
                            select new { Task = t, Request = r, Device = d };

                // 应用过滤条件
                if (!string.IsNullOrWhiteSpace(input.SearchKey))
                {
                    query = query.Where(x =>
                        x.Task.TaskNo.Contains(input.SearchKey) ||
                        (x.Device != null && x.Device.DeviceName.Contains(input.SearchKey)));
                }

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Task.CreationTime)
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var result = new List<RepairTaskDto>();

                foreach (var item in items)
                {
                    var dto = await MapToDto(item.Task, item.Request, item.Device, null);
                    result.Add(dto);
                }

                var page = new Page<RepairTaskDto>(input.Current, input.Size, total)
                {
                    Records = result
                };

                return CommonResult<Page<RepairTaskDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取我的待接单工单失败", ex);
                return CommonResult<Page<RepairTaskDto>>.Error($"获取我的待接单工单失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取我的进行中工单
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<Page<RepairTaskDto>>> GetMyOngoingTasks([FromQuery] RepairTaskPageInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                var userIdStr = userId.ToString();

                if (input.Size > 100) input.Size = 100;

                var query = from t in _repairTaskRepository.GetAll().AsNoTracking()
                            where (t.TaskStatus == TASK_STATUS_REPAIRING ||
                                   t.TaskStatus == TASK_STATUS_PENDING_ACCEPTANCE) &&
                                  t.RepairerIds.Contains(userIdStr)
                            join r in _repairRequestRepository.GetAll().AsNoTracking() on t.RepairRequestId equals r.Id
                            join d in _deviceRepository.GetAll().AsNoTracking() on t.DeviceId equals d.Id into deviceJoin
                            from d in deviceJoin.DefaultIfEmpty()
                            select new { Task = t, Request = r, Device = d };

                // 应用过滤条件
                if (!string.IsNullOrWhiteSpace(input.SearchKey))
                {
                    query = query.Where(x =>
                        x.Task.TaskNo.Contains(input.SearchKey) ||
                        (x.Device != null && x.Device.DeviceName.Contains(input.SearchKey)));
                }

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Task.CreationTime)
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var result = new List<RepairTaskDto>();

                foreach (var item in items)
                {
                    var dto = await MapToDto(item.Task, item.Request, item.Device, null);
                    result.Add(dto);
                }

                var page = new Page<RepairTaskDto>(input.Current, input.Size, total)
                {
                    Records = result
                };

                return CommonResult<Page<RepairTaskDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取我的进行中工单失败", ex);
                return CommonResult<Page<RepairTaskDto>>.Error($"获取我的进行中工单失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取设备故障原因历史选项
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<FaultReasonOptionDto>>> GetFaultReasonOptions(Guid deviceId)
        {
            try
            {
                var tasks = await _repairTaskRepository.GetAll()
                    .Where(x => x.DeviceId == deviceId && !string.IsNullOrEmpty(x.FaultReason))
                    .GroupBy(x => x.FaultReason)
                    .Select(g => new FaultReasonOptionDto
                    {
                        FaultReason = g.Key,
                        OccurrenceCount = g.Count(),
                        LastOccurrenceTime = g.Max(x => x.CreationTime)
                    })
                    .OrderByDescending(x => x.OccurrenceCount)
                    .Take(10)
                    .ToListAsync();

                return CommonResult<List<FaultReasonOptionDto>>.Success(tasks);
            }
            catch (Exception ex)
            {
                Logger.Error("获取故障原因历史选项失败", ex);
                return CommonResult<List<FaultReasonOptionDto>>.Error($"获取故障原因历史选项失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取设备维修方法历史选项
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<RepairMethodOptionDto>>> GetRepairMethodOptions(Guid deviceId)
        {
            try
            {
                var tasks = await _repairTaskRepository.GetAll()
                    .Where(x => x.DeviceId == deviceId && !string.IsNullOrEmpty(x.RepairMethodResult))
                    .GroupBy(x => x.RepairMethodResult)
                    .Select(g => new RepairMethodOptionDto
                    {
                        RepairMethodResult = g.Key,
                        UsageCount = g.Count(),
                        LastUsageTime = g.Max(x => x.CreationTime)
                    })
                    .OrderByDescending(x => x.UsageCount)
                    .Take(10)
                    .ToListAsync();

                return CommonResult<List<RepairMethodOptionDto>>.Success(tasks);
            }
            catch (Exception ex)
            {
                Logger.Error("获取维修方法历史选项失败", ex);
                return CommonResult<List<RepairMethodOptionDto>>.Error($"获取维修方法历史选项失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取设备绑定的保养计划选项
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<MaintenancePlanOptionDto>>> GetMaintenancePlanOptions(Guid deviceId)
        {
            try
            {
                var plans = await _maintenancePlanRepository.GetAll()
                    .Where(x => x.DeviceId == deviceId && x.Status == "启用")
                    .Select(x => new MaintenancePlanOptionDto
                    {
                        Id = x.Id,
                        PlanName = x.PlanName,
                        MaintenanceLevel = x.MaintenanceLevel,
                        MaintenanceLevelText = GetMaintenanceLevelText(x.MaintenanceLevel)
                    })
                    .ToListAsync();

                return CommonResult<List<MaintenancePlanOptionDto>>.Success(plans);
            }
            catch (Exception ex)
            {
                Logger.Error("获取保养计划选项失败", ex);
                return CommonResult<List<MaintenancePlanOptionDto>>.Error($"获取保养计划选项失败: {ex.Message}");
            }
        }

        #region 辅助方法

        /// <summary>
        /// 生成工单编号
        /// </summary>
        private async Task<string> GenerateTaskNo()
        {
            var today = DateTime.Now.ToString("yyyyMMdd");
            var lastTask = await _repairTaskRepository.GetAll()
                .Where(x => x.TaskNo.StartsWith($"RT{today}"))
                .OrderByDescending(x => x.TaskNo)
                .FirstOrDefaultAsync();

            if (lastTask != null && lastTask.TaskNo.Length >= 14)
            {
                var lastSeq = int.Parse(lastTask.TaskNo.Substring(11, 3));
                var newSeq = (lastSeq + 1).ToString("D3");
                return $"RT{today}{newSeq}";
            }

            return $"RT{today}001";
        }

        /// <summary>
        /// 映射为DTO
        /// </summary>
        private async Task<RepairTaskDto> MapToDto(RepairTasks task, RepairRequests request, Devices device, Types deviceType)
        {
            var dto = new RepairTaskDto
            {
                Id = task.Id,
                TaskNo = task.TaskNo,
                RepairRequestId = task.RepairRequestId,
                RequestNo = request?.RequestNo,
                RequesterId = task.RequesterId,
                RequesterName = task.RequesterName,
                RequestTime = task.RequestTime,
                DeviceId = task.DeviceId,
                DeviceCode = device?.DeviceCode,
                DeviceName = device?.DeviceName,
                DeviceTypeName = deviceType?.TypeName,
                AcceptTime = task.AcceptTime,
                StartTime = task.StartTime,
                EndTime = task.EndTime,
                DurationMinutes = task.DurationMinutes,
                TaskStatus = task.TaskStatus,
                TaskStatusText = GetTaskStatusText(task.TaskStatus),
                IsOverdue = task.IsOverdue,
                FaultReason = task.FaultReason,
                QualityImpactAnalysis = task.QualityImpactAnalysis,
                RepairMethodResult = task.RepairMethodResult,
                NotifyMaintenance = task.NotifyMaintenance,
                MaintenancePlanId = task.MaintenancePlanId,
                MaintenanceTaskId = task.MaintenanceTaskId,
                NotifiedAcceptance = task.NotifiedAcceptance,
                AcceptorId = task.AcceptorId,
                AcceptorName = task.AcceptorName,
                AcceptanceTime = task.AcceptanceTime
            };

            // 解析维修人员
            if (!string.IsNullOrEmpty(task.RepairerIds))
            {
                dto.RepairerIds = task.RepairerIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => long.Parse(x)).ToList();
            }

            if (!string.IsNullOrEmpty(task.RepairerNames))
            {
                dto.RepairerNames = task.RepairerNames.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            return dto;
        }

        /// <summary>
        /// 获取工单状态文本
        /// </summary>
        private string GetTaskStatusText(int status)
        {
            return status switch
            {
                TASK_STATUS_PENDING => "待接单",
                TASK_STATUS_REPAIRING => "维修中",
                TASK_STATUS_PENDING_ACCEPTANCE => "待验收",
                TASK_STATUS_COMPLETED => "已完成",
                TASK_STATUS_CANCELLED => "已取消",
                _ => "未知"
            };
        }

        /// <summary>
        /// 获取保养等级文本
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