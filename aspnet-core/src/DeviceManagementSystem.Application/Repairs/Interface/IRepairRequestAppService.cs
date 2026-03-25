using DeviceManagementSystem.Repairs.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Repairs.Interface
{
    /// <summary>
    /// 维修申报服务接口
    /// </summary>
    public interface IRepairRequestAppService
    {
        /// <summary>
        /// 获取维修申报分页列表
        /// </summary>
        Task<CommonResult<Page<RepairRequestDto>>> GetPageList([FromQuery] RepairRequestPageInput input);

        /// <summary>
        /// 获取维修申报详情
        /// </summary>
        Task<CommonResult<RepairRequestDto>> GetById(Guid id);

        /// <summary>
        /// 创建维修申报
        /// </summary>
        Task<CommonResult<Guid>> Create(RepairRequestInput input);

        /// <summary>
        /// 更新维修申报
        /// </summary>
        Task<CommonResult> Update(RepairRequestInput input);

        /// <summary>
        /// 删除维修申报
        /// </summary>
        Task<CommonResult> Delete(Guid id);

        /// <summary>
        /// 取消维修申报
        /// </summary>
        Task<CommonResult> Cancel(Guid id, string reason);

        /// <summary>
        /// 获取设备故障现象历史选项
        /// </summary>
        Task<CommonResult<List<FaultDescriptionOptionDto>>> GetFaultDescriptionOptions(Guid deviceId);

        /// <summary>
        /// 获取我的维修申报列表
        /// </summary>
        Task<CommonResult<Page<RepairRequestDto>>> GetMyRequests([FromQuery] RepairRequestPageInput input);
    }
}