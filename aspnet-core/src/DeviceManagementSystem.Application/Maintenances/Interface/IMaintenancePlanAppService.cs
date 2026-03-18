using Abp.Domain.Uow;
using DeviceManagementSystem.Maintenances.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances.Interface
{
    /// <summary>
    /// 保养计划服务接口
    /// </summary>
    public interface IMaintenancePlanAppService
    {

        /// <summary>
        /// 获取保养计划分页列表
        /// </summary>
        Task<CommonResult<Page<MaintenancePlanDto>>> GetPageList([FromQuery] MaintenancePlanPageInput input);



        /// <summary>
        /// 获取保养计划详情
        /// </summary>
        Task<CommonResult<MaintenancePlanDto>> GetById(Guid id);


        /// <summary>
        /// 获取设备保养计划列表
        /// </summary>
        Task<CommonResult<List<MaintenancePlanDto>>> GetPlansByDeviceId(Guid deviceId);

        /// <summary>
        /// 获取设备保养计划（按等级）
        /// </summary>
        Task<CommonResult<DeviceMaintenancePlansDto>> GetDevicePlans(Guid deviceId);

        /// <summary>
        /// 获取待生成工单的计划
        /// </summary>
        Task<CommonResult<List<MaintenancePlanDto>>> GetPlansNeedGenerateTask();

        /// <summary>
        /// 创建保养计划
        /// </summary>
        Task<CommonResult<Guid>> Create(MaintenancePlanInput input);

        /// <summary>
        /// 更新计划状态
        /// </summary>
        Task<CommonResult> UpdatePlanStatus(Guid planId, string status);

        /// <summary>
        /// 记录保养完成（更新下次保养日期）
        /// </summary>
        Task<CommonResult> RecordMaintenanceCompleted(Guid planId, DateTime? actualDate = null);




        /// <summary>
        /// 获取统计信息（本周、下周、逾期、已生成）
        /// </summary>
        Task<CommonResult<MaintenancePlanStatisticsDto>> GetStatistics();

        /// <summary>
        /// 获取所有计划（简版，用于选择器）
        /// </summary>
        Task<CommonResult<List<MaintenancePlanSimpleDto>>> GetSimpleList();


        /// <summary>
        /// 手动生成工单
        /// </summary>
        Task<CommonResult> GenerateTask(Guid planId);



        /// <summary>
        /// 更新计划状态（接口输入）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> UpdateStatus(UpdatePlanStatusInput input);
    }
}
