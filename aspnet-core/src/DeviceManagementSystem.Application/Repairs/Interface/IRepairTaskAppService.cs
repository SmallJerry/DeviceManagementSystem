using DeviceManagementSystem.Repairs.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Repairs.Interface
{
    /// <summary>
    /// 维修工单服务接口
    /// </summary>
    public interface IRepairTaskAppService
    {
        /// <summary>
        /// 获取维修工单分页列表
        /// </summary>
        Task<CommonResult<Page<RepairTaskDto>>> GetPageList([FromQuery] RepairTaskPageInput input);

        /// <summary>
        /// 获取维修工单详情
        /// </summary>
        Task<CommonResult<RepairTaskDto>> GetById(Guid id);

        /// <summary>
        /// 派单（分配维修人员）
        /// </summary>
        Task<CommonResult> Dispatch(RepairTaskInput input);

        /// <summary>
        /// 接单
        /// </summary>
        Task<CommonResult> Accept(AcceptRepairTaskInput input);

        /// <summary>
        /// 开始维修
        /// </summary>
        Task<CommonResult> Start(StartRepairTaskInput input);

        /// <summary>
        /// 保存维修执行记录（草稿）
        /// </summary>
        Task<CommonResult> SaveExecution(RepairTaskExecutionInput input);

        /// <summary>
        /// 完成维修
        /// </summary>
        Task<CommonResult> Complete(CompleteRepairTaskInput input);

        /// <summary>
        /// 取消工单
        /// </summary>
        Task<CommonResult> Cancel(Guid taskId, string reason);

        /// <summary>
        /// 获取我的待接单工单
        /// </summary>
        Task<CommonResult<Page<RepairTaskDto>>> GetMyPendingTasks([FromQuery] RepairTaskPageInput input);

        /// <summary>
        /// 获取我的进行中工单
        /// </summary>
        Task<CommonResult<Page<RepairTaskDto>>> GetMyOngoingTasks([FromQuery] RepairTaskPageInput input);

        /// <summary>
        /// 获取设备故障原因历史选项
        /// </summary>
        Task<CommonResult<List<FaultReasonOptionDto>>> GetFaultReasonOptions(Guid deviceId);

        /// <summary>
        /// 获取设备维修方法历史选项
        /// </summary>
        Task<CommonResult<List<RepairMethodOptionDto>>> GetRepairMethodOptions(Guid deviceId);

        /// <summary>
        /// 获取设备绑定的保养计划选项
        /// </summary>
        Task<CommonResult<List<MaintenancePlanOptionDto>>> GetMaintenancePlanOptions(Guid deviceId);
    }
}