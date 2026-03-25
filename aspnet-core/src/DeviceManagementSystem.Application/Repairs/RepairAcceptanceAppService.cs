using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using DeviceManagementSystem.DeviceInfos;
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
    /// 维修验收服务实现
    /// </summary>
    [AbpAuthorize]
    [Audited]
    public class RepairAcceptanceAppService : DeviceManagementSystemAppServiceBase, IRepairAcceptanceAppService
    {
        private readonly IRepository<RepairAcceptances, Guid> _acceptanceRepository;
        private readonly IRepository<RepairTasks, Guid> _repairTaskRepository;
        private readonly IRepository<RepairRequests, Guid> _repairRequestRepository;
        private readonly IRepository<Devices, Guid> _deviceRepository;
        private readonly IUserAppService _userAppService;

        // 工单状态常量
        private const int TASK_STATUS_PENDING_ACCEPTANCE = 2;
        private const int TASK_STATUS_COMPLETED = 3;

        // 申报状态常量
        private const int REQUEST_STATUS_COMPLETED = 3;

        public RepairAcceptanceAppService(
            IRepository<RepairAcceptances, Guid> acceptanceRepository,
            IRepository<RepairTasks, Guid> repairTaskRepository,
            IRepository<RepairRequests, Guid> repairRequestRepository,
            IRepository<Devices, Guid> deviceRepository,
            IUserAppService userAppService)
        {
            _acceptanceRepository = acceptanceRepository;
            _repairTaskRepository = repairTaskRepository;
            _repairRequestRepository = repairRequestRepository;
            _deviceRepository = deviceRepository;
            _userAppService = userAppService;
        }

        /// <summary>
        /// 获取验收记录
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<RepairAcceptanceDto>> GetByTaskId(Guid repairTaskId)
        {
            try
            {
                var acceptance = await _acceptanceRepository
                    .FirstOrDefaultAsync(x => x.RepairTaskId == repairTaskId);
                if (acceptance == null)
                {
                    return CommonResult<RepairAcceptanceDto>.Error("验收记录不存在");
                }

                var device = await _deviceRepository.FirstOrDefaultAsync(acceptance.DeviceId);

                var dto = await MapToDto(acceptance, device);
                return CommonResult<RepairAcceptanceDto>.Success(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("获取验收记录失败", ex);
                return CommonResult<RepairAcceptanceDto>.Error($"获取验收记录失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建验收记录
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult<Guid>> Create(RepairAcceptanceInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                var userName = await _userAppService.GetNameByUserId(userId);

                var task = await _repairTaskRepository.FirstOrDefaultAsync(input.RepairTaskId);
                if (task == null)
                {
                    return CommonResult<Guid>.Error("维修工单不存在");
                }

                // 验证验收权限
                if (task.AcceptorId != userId)
                {
                    return CommonResult<Guid>.Error("您不是该工单的验收人");
                }

                if (task.TaskStatus != TASK_STATUS_PENDING_ACCEPTANCE)
                {
                    return CommonResult<Guid>.Error($"当前状态({GetTaskStatusText(task.TaskStatus)})不可验收");
                }

                var acceptance = new RepairAcceptances
                {
                    RepairTaskId = input.RepairTaskId,
                    RepairRequestId = task.RepairRequestId,
                    DeviceId = task.DeviceId,
                    AcceptanceCriteriaJson = JsonConvert.SerializeObject(input.AcceptanceCriteria),
                    BeforeRepairParams = input.BeforeRepairParams,
                    AfterRepairParams = input.AfterRepairParams,
                    AcceptanceConclusion = input.AcceptanceConclusion,
                    AcceptanceOpinion = input.AcceptanceOpinion,
                    AcceptorId = userId,
                    AcceptorName = userName,
                    AcceptanceTime = DateTime.Now
                };

                var acceptanceId = await _acceptanceRepository.InsertAndGetIdAsync(acceptance);

                // 更新工单状态
                task.TaskStatus = TASK_STATUS_COMPLETED;
                task.AcceptanceTime = DateTime.Now;
                task.AcceptorId = userId;
                task.AcceptorName = userName;
                await _repairTaskRepository.UpdateAsync(task);

                // 更新申报状态
                var request = await _repairRequestRepository.FirstOrDefaultAsync(task.RepairRequestId);
                if (request != null)
                {
                    request.RequestStatus = REQUEST_STATUS_COMPLETED;
                    await _repairRequestRepository.UpdateAsync(request);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult<Guid>.Success(acceptanceId);
            }
            catch (Exception ex)
            {
                Logger.Error("创建验收记录失败", ex);
                return CommonResult<Guid>.Error($"创建验收记录失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新验收记录
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Update(RepairAcceptanceInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;

                var acceptance = await _acceptanceRepository
                    .FirstOrDefaultAsync(x => x.RepairTaskId == input.RepairTaskId);
                if (acceptance == null)
                {
                    return CommonResult.Error("验收记录不存在");
                }

                if (acceptance.AcceptorId != userId)
                {
                    return CommonResult.Error("您不是该验收记录的创建人");
                }

                acceptance.AcceptanceCriteriaJson = JsonConvert.SerializeObject(input.AcceptanceCriteria);
                acceptance.BeforeRepairParams = input.BeforeRepairParams;
                acceptance.AfterRepairParams = input.AfterRepairParams;
                acceptance.AcceptanceConclusion = input.AcceptanceConclusion;
                acceptance.AcceptanceOpinion = input.AcceptanceOpinion;

                await _acceptanceRepository.UpdateAsync(acceptance);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("更新成功");
            }
            catch (Exception ex)
            {
                Logger.Error("更新验收记录失败", ex);
                return CommonResult.Error($"更新验收记录失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取待验收工单列表（按设备创建人）
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<Page<RepairTaskDto>>> GetPendingAcceptanceTasks([FromQuery] RepairTaskPageInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;

                if (input.Size > 100) input.Size = 100;

                var query = from t in _repairTaskRepository.GetAll().AsNoTracking()
                            where t.TaskStatus == TASK_STATUS_PENDING_ACCEPTANCE &&
                                  t.AcceptorId == userId
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
                    var dto = new RepairTaskDto
                    {
                        Id = item.Task.Id,
                        TaskNo = item.Task.TaskNo,
                        RepairRequestId = item.Task.RepairRequestId,
                        RequestNo = item.Request?.RequestNo,
                        RequesterId = item.Task.RequesterId,
                        RequesterName = item.Task.RequesterName,
                        RequestTime = item.Task.RequestTime,
                        DeviceId = item.Task.DeviceId,
                        DeviceCode = item.Device?.DeviceCode,
                        DeviceName = item.Device?.DeviceName,
                        AcceptTime = item.Task.AcceptTime,
                        StartTime = item.Task.StartTime,
                        EndTime = item.Task.EndTime,
                        DurationMinutes = item.Task.DurationMinutes,
                        TaskStatus = item.Task.TaskStatus,
                        TaskStatusText = GetTaskStatusText(item.Task.TaskStatus),
                        IsOverdue = item.Task.IsOverdue,
                        FaultReason = item.Task.FaultReason,
                        QualityImpactAnalysis = item.Task.QualityImpactAnalysis,
                        RepairMethodResult = item.Task.RepairMethodResult
                    };

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
                Logger.Error("获取待验收工单列表失败", ex);
                return CommonResult<Page<RepairTaskDto>>.Error($"获取待验收工单列表失败: {ex.Message}");
            }
        }

        #region 辅助方法

        /// <summary>
        /// 映射为DTO
        /// </summary>
        private async Task<RepairAcceptanceDto> MapToDto(RepairAcceptances acceptance, Devices device)
        {
            var dto = new RepairAcceptanceDto
            {
                Id = acceptance.Id,
                RepairTaskId = acceptance.RepairTaskId,
                RepairRequestId = acceptance.RepairRequestId,
                DeviceId = acceptance.DeviceId,
                DeviceCode = device?.DeviceCode,
                DeviceName = device?.DeviceName,
                AcceptanceCriteria = string.IsNullOrEmpty(acceptance.AcceptanceCriteriaJson)
                    ? new List<AcceptanceCriteriaItemDto>()
                    : JsonConvert.DeserializeObject<List<AcceptanceCriteriaItemDto>>(acceptance.AcceptanceCriteriaJson),
                BeforeRepairParams = acceptance.BeforeRepairParams,
                AfterRepairParams = acceptance.AfterRepairParams,
                AcceptanceConclusion = acceptance.AcceptanceConclusion,
                AcceptanceConclusionText = acceptance.AcceptanceConclusion == 0 ? "正常" : "不正常",
                AcceptanceOpinion = acceptance.AcceptanceOpinion,
                AcceptorId = acceptance.AcceptorId,
                AcceptorName = acceptance.AcceptorName,
                AcceptanceTime = acceptance.AcceptanceTime
            };

            return dto;
        }

        /// <summary>
        /// 获取工单状态文本
        /// </summary>
        private string GetTaskStatusText(int status)
        {
            return status switch
            {
                TASK_STATUS_PENDING_ACCEPTANCE => "待验收",
                TASK_STATUS_COMPLETED => "已完成",
                _ => "未知"
            };
        }

        #endregion
    }
}