using Abp.Auditing;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using DeviceManagementSystem.BasicDataManagement;
using DeviceManagementSystem.DeviceInfos;
using DeviceManagementSystem.Maintenances.Constant;
using DeviceManagementSystem.Maintenances.Dto;
using DeviceManagementSystem.Maintenances.Interface;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances
{
    /// <summary>
    /// 保养计划服务实现
    /// </summary>
    [Audited]
    [AbpAuthorize]
    public class MaintenancePlanAppService : DeviceManagementSystemAppServiceBase, IMaintenancePlanAppService
    {
        private readonly IRepository<MaintenancePlans, Guid> _planRepository;
        private readonly IRepository<MaintenanceTemplates, Guid> _templateRepository;
        private readonly IRepository<DeviceMaintenancePlanRelation, Guid> _devicePlanRelationRepository;
        private readonly IRepository<Devices, Guid> _deviceRepository;
        private readonly IRepository<MaintenanceTasks, Guid> _taskRepository; 

        public MaintenancePlanAppService(
            IRepository<MaintenancePlans, Guid> planRepository,
            IRepository<MaintenanceTemplates, Guid> templateRepository,
            IRepository<DeviceMaintenancePlanRelation, Guid> devicePlanRelationRepository,
            IRepository<Devices, Guid> deviceRepository,
            IRepository<MaintenanceTasks, Guid> taskRepository)
        {
            _planRepository = planRepository;
            _templateRepository = templateRepository;
            _devicePlanRelationRepository = devicePlanRelationRepository;
            _deviceRepository = deviceRepository;
            _taskRepository = taskRepository;
        }

        #region 查询方法

        /// <summary>
        /// 获取保养计划分页列表
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<Page<MaintenancePlanDto>>> GetPageList([FromQuery] MaintenancePlanPageInput input)
        {
            try
            {
                if (input.Size > 100) input.Size = 100;

                var query = from p in _planRepository.GetAll().AsNoTracking()
                            join d in _deviceRepository.GetAll().AsNoTracking() on p.DeviceId equals d.Id into deviceJoin
                            from d in deviceJoin.DefaultIfEmpty()
                            join t in _templateRepository.GetAll().AsNoTracking() on p.TemplateId equals t.Id into templateJoin
                            from t in templateJoin.DefaultIfEmpty()
                            select new { Plan = p, Device = d, Template = t };

                // 应用过滤条件
                if (!string.IsNullOrWhiteSpace(input.SearchKey))
                {
                    query = query.Where(x =>
                        x.Plan.PlanName.Contains(input.SearchKey) ||
                        (x.Device != null && (x.Device.DeviceName.Contains(input.SearchKey) || x.Device.DeviceCode.Contains(input.SearchKey))));
                }

                if (!string.IsNullOrWhiteSpace(input.MaintenanceLevel))
                {
                    query = query.Where(x => x.Plan.MaintenanceLevel == input.MaintenanceLevel);
                }

                if (!string.IsNullOrWhiteSpace(input.Status))
                {
                    query = query.Where(x => x.Plan.Status == input.Status);
                }

                if (input.NextDateBegin.HasValue)
                {
                    var begin = input.NextDateBegin.Value.Date;
                    query = query.Where(x => x.Plan.NextMaintenanceDate >= begin);
                }

                if (input.NextDateEnd.HasValue)
                {
                    var end = input.NextDateEnd.Value.Date.AddDays(1).AddSeconds(-1);
                    query = query.Where(x => x.Plan.NextMaintenanceDate <= end);
                }

                var total = await query.CountAsync();

                var items = await query
                    .OrderBy(x => x.Plan.NextMaintenanceDate)
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var result = items.Select(x => MapToDto(x.Plan, x.Device, x.Template)).ToList();

                var page = new Page<MaintenancePlanDto>(input.Current, input.Size, total)
                {
                    Records = result
                };

                return CommonResult<Page<MaintenancePlanDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取保养计划分页列表失败", ex);
                return CommonResult<Page<MaintenancePlanDto>>.Error($"获取保养计划分页列表失败: {ex.Message}");
            }
        }


        /// <summary>
        /// 获取保养计划详情
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<MaintenancePlanDto>> GetById(Guid id)
        {
            try
            {
                var plan = await _planRepository.FirstOrDefaultAsync(id);
                if (plan == null)
                {
                    return CommonResult<MaintenancePlanDto>.Error("保养计划不存在");
                }

                var device = await _deviceRepository.FirstOrDefaultAsync(plan.DeviceId ?? Guid.Empty);
                var template = await _templateRepository.FirstOrDefaultAsync(plan.TemplateId);

                var dto = MapToDto(plan, device, template);

                return CommonResult<MaintenancePlanDto>.Success(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("获取保养计划详情失败", ex);
                return CommonResult<MaintenancePlanDto>.Error($"获取保养计划详情失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取设备保养计划列表
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<MaintenancePlanDto>>> GetPlansByDeviceId(Guid deviceId)
        {
            try
            {
                var relations = await _devicePlanRelationRepository.GetAll()
                    .Where(x => x.DeviceId == deviceId)
                    .Select(x => x.MaintenancePlanId)
                    .ToListAsync();

                var plans = await _planRepository.GetAll()
                    .Where(x => relations.Contains(x.Id))
                    .ToListAsync();

                var device = await _deviceRepository.FirstOrDefaultAsync(deviceId);
                var result = new List<MaintenancePlanDto>();

                foreach (var plan in plans)
                {
                    var template = await _templateRepository.FirstOrDefaultAsync(plan.TemplateId);
                    result.Add(MapToDto(plan, device, template));
                }

                return CommonResult<List<MaintenancePlanDto>>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取设备保养计划失败", ex);
                return CommonResult<List<MaintenancePlanDto>>.Error($"获取设备保养计划失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取设备保养计划（按等级）
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<DeviceMaintenancePlansDto>> GetDevicePlans(Guid deviceId)
        {
            try
            {
                var plans = await GetPlansByDeviceId(deviceId);
                if (!plans.IsSuccess)
                {
                    return CommonResult<DeviceMaintenancePlansDto>.Error(plans.Message);
                }

                var result = new DeviceMaintenancePlansDto();

                foreach (var plan in plans.Data)
                {
                    var planInput = new MaintenancePlanInput
                    {
                        Id = plan.Id,
                        DeviceId = plan.DeviceId,
                        TemplateId = plan.TemplateId,
                        MaintenanceLevel = plan.MaintenanceLevel,
                        FirstMaintenanceDate = plan.FirstMaintenanceDate,
                        Remark = plan.Remark
                    };

                    switch (plan.MaintenanceLevel)
                    {
                        case "月度":
                            result.Monthly = planInput;
                            break;
                        case "季度":
                            result.Quarterly = planInput;
                            break;
                        case "半年度":
                            result.HalfYearly = planInput;
                            break;
                        case "年度":
                            result.Annual = planInput;
                            break;
                    }
                }

                return CommonResult<DeviceMaintenancePlansDto>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取设备保养计划失败", ex);
                return CommonResult<DeviceMaintenancePlansDto>.Error($"获取设备保养计划失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取所有计划（简版，用于选择器）
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<MaintenancePlanSimpleDto>>> GetSimpleList()
        {
            try
            {
                var plans = await _planRepository.GetAll()
                    .Where(x => x.Status == "启用")
                    .OrderBy(x => x.NextMaintenanceDate)
                    .ToListAsync();

                var result = new List<MaintenancePlanSimpleDto>();

                foreach (var plan in plans)
                {
                    var device = await _deviceRepository.FirstOrDefaultAsync(plan.DeviceId ?? Guid.Empty);
                    result.Add(new MaintenancePlanSimpleDto
                    {
                        Id = plan.Id,
                        PlanName = plan.PlanName,
                        MaintenanceLevel = plan.MaintenanceLevel,
                        DeviceName = device?.DeviceName,
                        NextMaintenanceDate = plan.NextMaintenanceDate
                    });
                }

                return CommonResult<List<MaintenancePlanSimpleDto>>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取计划简表失败", ex);
                return CommonResult<List<MaintenancePlanSimpleDto>>.Error($"获取计划简表失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取统计信息
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<MaintenancePlanStatisticsDto>> GetStatistics()
        {
            try
            {
                var today = DateTime.Today;
                var nextWeek = today.AddDays(7);
                var lastWeek = today.AddDays(-7);

                var plans = await _planRepository.GetAll()
                    .Where(x => x.Status == "启用")
                    .ToListAsync();

                var stats = new MaintenancePlanStatisticsDto
                {
                    ThisWeek = plans.Count(x => x.NextMaintenanceDate >= today && x.NextMaintenanceDate <= nextWeek && !x.HasGeneratedTask),
                    NextWeek = plans.Count(x => x.NextMaintenanceDate > nextWeek && x.NextMaintenanceDate <= nextWeek.AddDays(7) && !x.HasGeneratedTask),
                    Overdue = plans.Count(x => x.NextMaintenanceDate < today && !x.HasGeneratedTask),
                    Generated = plans.Count(x => x.HasGeneratedTask)
                };

                return CommonResult<MaintenancePlanStatisticsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                Logger.Error("获取统计信息失败", ex);
                return CommonResult<MaintenancePlanStatisticsDto>.Error($"获取统计信息失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取待生成工单的计划
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<MaintenancePlanDto>>> GetPlansNeedGenerateTask()
        {
            try
            {
                // 查找7天内需要执行且未生成工单的计划
                var today = DateTime.Today;
                var nextWeek = today.AddDays(7);

                var plans = await _planRepository.GetAll()
                    .Where(x => x.Status == "启用" &&
                               !x.HasGeneratedTask &&
                               x.NextMaintenanceDate >= today &&
                               x.NextMaintenanceDate <= nextWeek)
                    .ToListAsync();

                var result = new List<MaintenancePlanDto>();

                foreach (var plan in plans)
                {
                    var device = await _deviceRepository.FirstOrDefaultAsync(plan.DeviceId ?? Guid.Empty);
                    var template = await _templateRepository.FirstOrDefaultAsync(plan.TemplateId);

                    result.Add(new MaintenancePlanDto
                    {
                        Id = plan.Id,
                        PlanName = plan.PlanName,
                        DeviceId = plan.DeviceId,
                        DeviceCode = device?.DeviceCode,
                        DeviceName = device?.DeviceName,
                        TemplateId = plan.TemplateId,
                        TemplateName = template?.TemplateName,
                        MaintenanceLevel = plan.MaintenanceLevel,
                        MaintenanceLevelText = GetMaintenanceLevelText(plan.MaintenanceLevel),
                        NextMaintenanceDate = plan.NextMaintenanceDate,
                        Status = plan.Status
                    });
                }

                return CommonResult<List<MaintenancePlanDto>>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取待生成工单计划失败", ex);
                return CommonResult<List<MaintenancePlanDto>>.Error($"获取待生成工单计划失败: {ex.Message}");
            }
        }

        #endregion

        #region 操作方法

        /// <summary>
        /// 创建保养计划
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult<Guid>> Create(MaintenancePlanInput input)
        {
            try
            {
                // 验证设备
                var device = await _deviceRepository.FirstOrDefaultAsync(input.DeviceId ?? Guid.Empty);
                if (device == null)
                {
                    return CommonResult<Guid>.Error("设备不存在");
                }

                // 验证模板
                var template = await _templateRepository.FirstOrDefaultAsync(input.TemplateId);
                if (template == null)
                {
                    return CommonResult<Guid>.Error("保养模板不存在");
                }

                // 验证保养等级一致性
                if (template.MaintenanceLevel != input.MaintenanceLevel)
                {
                    return CommonResult<Guid>.Error($"模板的保养等级为{template.MaintenanceLevel}，与选择的{input.MaintenanceLevel}不一致");
                }

                // 计算下次保养日期
                var nextDate = CalculateNextMaintenanceDate(input.FirstMaintenanceDate, input.MaintenanceLevel);

                // 创建计划
                var plan = new MaintenancePlans
                {
                    PlanName = $"{template.TemplateName} - {input.MaintenanceLevel}保养计划",
                    DeviceId = input.DeviceId,
                    TemplateId = input.TemplateId,
                    MaintenanceLevel = input.MaintenanceLevel,
                    CycleType = MaintenanceCycleConstants.GetCycleType(input.MaintenanceLevel),
                    CycleDays = MaintenanceCycleConstants.GetCycleDays(input.MaintenanceLevel),
                    FirstMaintenanceDate = input.FirstMaintenanceDate,
                    NextMaintenanceDate = nextDate,
                    Status = "启用",
                    Remark = input.Remark
                };

                var planId = await _planRepository.InsertAndGetIdAsync(plan);
                await CurrentUnitOfWork.SaveChangesAsync();

                // 创建关联关系
                var relation = new DeviceMaintenancePlanRelation
                {
                    DeviceId = input.DeviceId.Value,
                    MaintenancePlanId = planId,
                    MaintenanceLevel = input.MaintenanceLevel,
                    TemplateId = input.TemplateId
                };
                await _devicePlanRelationRepository.InsertAsync(relation);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult<Guid>.Success(planId);
            }
            catch (Exception ex)
            {
                Logger.Error("创建保养计划失败", ex);
                return CommonResult<Guid>.Error($"创建保养计划失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新保养计划
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Update(MaintenancePlanInput input)
        {
            try
            {
                if (!input.Id.HasValue)
                {
                    return CommonResult.Error("计划ID不能为空");
                }

                var plan = await _planRepository.FirstOrDefaultAsync(input.Id.Value);
                if (plan == null)
                {
                    return CommonResult.Error("保养计划不存在");
                }

                // 验证设备
                var device = await _deviceRepository.FirstOrDefaultAsync(input.DeviceId ?? Guid.Empty);
                if (device == null)
                {
                    return CommonResult.Error("设备不存在");
                }

                // 验证模板
                var template = await _templateRepository.FirstOrDefaultAsync(input.TemplateId);
                if (template == null)
                {
                    return CommonResult.Error("保养模板不存在");
                }

                // 验证保养等级一致性
                if (template.MaintenanceLevel != input.MaintenanceLevel)
                {
                    return CommonResult.Error($"模板的保养等级为{template.MaintenanceLevel}，与选择的{input.MaintenanceLevel}不一致");
                }

                // 计算下次保养日期
                var nextDate = CalculateNextMaintenanceDate(input.FirstMaintenanceDate, input.MaintenanceLevel);

                // 更新计划
                plan.PlanName = $"{template.TemplateName} - {input.MaintenanceLevel}保养计划";
                plan.DeviceId = input.DeviceId;
                plan.TemplateId = input.TemplateId;
                plan.MaintenanceLevel = input.MaintenanceLevel;
                plan.CycleType = MaintenanceCycleConstants.GetCycleType(input.MaintenanceLevel);
                plan.CycleDays = MaintenanceCycleConstants.GetCycleDays(input.MaintenanceLevel);
                plan.FirstMaintenanceDate = input.FirstMaintenanceDate;
                plan.NextMaintenanceDate = nextDate;
                plan.Remark = input.Remark;

                await _planRepository.UpdateAsync(plan);
                await CurrentUnitOfWork.SaveChangesAsync();

                // 更新关联关系
                var relation = await _devicePlanRelationRepository.FirstOrDefaultAsync(x => x.MaintenancePlanId == plan.Id);
                if (relation != null)
                {
                    relation.TemplateId = input.TemplateId;
                    await _devicePlanRelationRepository.UpdateAsync(relation);
                }

                return CommonResult.Ok("更新成功");
            }
            catch (Exception ex)
            {
                Logger.Error("更新保养计划失败", ex);
                return CommonResult.Error($"更新保养计划失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除保养计划
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Delete(Guid id)
        {
            try
            {
                // 直接使用注入的仓储查询工单
                var hasTasks = await _taskRepository.GetAll().AnyAsync(x => x.PlanId == id);
                if (hasTasks)
                {
                    return CommonResult.Error("该计划已生成工单，不能删除");
                }

                // 删除关联关系
                await _devicePlanRelationRepository.DeleteAsync(x => x.MaintenancePlanId == id);

                // 删除计划
                await _planRepository.DeleteAsync(id);

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("删除成功");
            }
            catch (Exception ex)
            {
                Logger.Error("删除保养计划失败", ex);
                return CommonResult.Error($"删除保养计划失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 批量删除保养计划
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> BatchDelete([FromBody] List<Guid> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的计划");
                }

                foreach (var id in ids)
                {
                    await Delete(id);
                }

                return CommonResult.Ok("批量删除成功");
            }
            catch (Exception ex)
            {
                Logger.Error("批量删除保养计划失败", ex);
                return CommonResult.Error($"批量删除保养计划失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新计划状态
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult> UpdatePlanStatus(Guid planId, string status)
        {
            try
            {
                var plan = await _planRepository.FirstOrDefaultAsync(planId);
                if (plan == null)
                {
                    return CommonResult.Error("计划不存在");
                }

                plan.Status = status;
                await _planRepository.UpdateAsync(plan);

                return CommonResult.Ok("更新成功");
            }
            catch (Exception ex)
            {
                Logger.Error("更新计划状态失败", ex);
                return CommonResult.Error($"更新计划状态失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新计划状态（接口输入）
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult> UpdateStatus(UpdatePlanStatusInput input)
        {
            return await UpdatePlanStatus(input.Id, input.Status);
        }

        /// <summary>
        /// 手动生成工单
        /// </summary>
        public async Task<CommonResult> GenerateTask(Guid planId)
        {
            try
            {
                var plan = await _planRepository.FirstOrDefaultAsync(planId);
                if (plan == null)
                {
                    return CommonResult.Error("计划不存在");
                }

                if (plan.HasGeneratedTask)
                {
                    return CommonResult.Error("该计划已生成工单");
                }

                // 直接创建工单，而不是调用服务
                var task = new MaintenanceTasks
                {
                    PlanId = planId,
                    TaskName = $"{plan.PlanName}-{DateTime.Now:yyyyMMdd}",
                    DeviceId = (Guid)plan.DeviceId,
                    TemplateId = plan.TemplateId,
                    MaintenanceLevel = plan.MaintenanceLevel,
                    PlanStartDate = plan.NextMaintenanceDate,
                    Status = "待执行",
                    CreateType = "自动"
                };

                await _taskRepository.InsertAsync(task);

                // 标记已生成
                plan.HasGeneratedTask = true;
                await _planRepository.UpdateAsync(plan);

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("工单生成成功");
            }
            catch (Exception ex)
            {
                Logger.Error("手动生成工单失败", ex);
                return CommonResult.Error($"手动生成工单失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 记录保养完成（更新下次保养日期）
        /// </summary>
        public async Task<CommonResult> RecordMaintenanceCompleted(Guid planId, DateTime? actualDate = null)
        {
            try
            {
                var plan = await _planRepository.FirstOrDefaultAsync(planId);
                if (plan == null)
                {
                    return CommonResult.Error("计划不存在");
                }

                var completeDate = actualDate ?? DateTime.Today;

                plan.LastMaintenanceDate = completeDate;
                plan.NextMaintenanceDate = CalculateNextMaintenanceDate(completeDate, plan.MaintenanceLevel);
                plan.HasGeneratedTask = false; // 重置，等待下次生成

                await _planRepository.UpdateAsync(plan);

                return CommonResult.Ok("更新成功");
            }
            catch (Exception ex)
            {
                Logger.Error("记录保养完成失败", ex);
                return CommonResult.Error($"记录保养完成失败: {ex.Message}");
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 映射为DTO
        /// </summary>
        private MaintenancePlanDto MapToDto(MaintenancePlans plan, Devices device, MaintenanceTemplates template)
        {
            var dto = new MaintenancePlanDto
            {
                Id = plan.Id,
                PlanName = plan.PlanName,
                DeviceId = plan.DeviceId,
                DeviceCode = device?.DeviceCode,
                DeviceName = device?.DeviceName,
                TemplateId = plan.TemplateId,
                TemplateName = template?.TemplateName,
                MaintenanceLevel = plan.MaintenanceLevel,
                MaintenanceLevelText = GetMaintenanceLevelText(plan.MaintenanceLevel),
                CycleType = plan.CycleType,
                CycleDays = plan.CycleDays,
                FirstMaintenanceDate = plan.FirstMaintenanceDate,
                NextMaintenanceDate = plan.NextMaintenanceDate,
                LastMaintenanceDate = plan.LastMaintenanceDate,
                Status = plan.Status,
                HasGeneratedTask = plan.HasGeneratedTask,
                Remark = plan.Remark
            };

            return dto;
        }

        /// <summary>
        /// 计算下次保养日期（跳过节假日）
        /// </summary>
        private DateTime CalculateNextMaintenanceDate(DateTime lastDate, string level, bool skipHoliday = true)
        {
            int days = MaintenanceCycleConstants.GetCycleDays(level);
            DateTime nextDate = lastDate.AddDays(days);

            if (skipHoliday)
            {
                // 如果是周末或节假日，顺延到下一个工作日
                while (IsHoliday(nextDate))
                {
                    nextDate = nextDate.AddDays(1);
                }
            }

            return nextDate;
        }

        /// <summary>
        /// 判断是否为节假日（调用公共API）
        /// </summary>
        private bool IsHoliday(DateTime date)
        {
            // 周末判断
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }

            // TODO: 调用节假日API判断
            // 示例：http://timor.tech/api/holiday/info/{date:yyyy-MM-dd}
            // 如果返回 holiday 不为 null 且 holiday.holiday == true，则是节假日

            return false;
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