using DeviceManagementSystem.Repairs.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Repairs.Interface
{
    /// <summary>
    /// 维修验收服务接口
    /// </summary>
    public interface IRepairAcceptanceAppService
    {
        /// <summary>
        /// 获取验收记录
        /// </summary>
        Task<CommonResult<RepairAcceptanceDto>> GetByTaskId(Guid repairTaskId);

        /// <summary>
        /// 创建验收记录
        /// </summary>
        Task<CommonResult<Guid>> Create(RepairAcceptanceInput input);

        /// <summary>
        /// 更新验收记录
        /// </summary>
        Task<CommonResult> Update(RepairAcceptanceInput input);

        /// <summary>
        /// 获取待验收工单列表（按设备创建人）
        /// </summary>
        Task<CommonResult<Page<RepairTaskDto>>> GetPendingAcceptanceTasks([FromQuery] RepairTaskPageInput input);
    }
}