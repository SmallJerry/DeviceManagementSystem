using DeviceManagementSystem.DeviceInfos.Dto;
using DeviceManagementSystem.Utils.Common;
using DeviceManagementSystem.WorkFlows.FlowInstance.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.DeviceInfos
{
    /// <summary>
    /// 设备管理服务接口
    /// </summary>
    public interface IDeviceAppService
    {
        /// <summary>
        /// 获取设备分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<Page<DeviceDto>>> GetPageList([FromQuery] DevicePageInput input);


        /// <summary>
        /// 获取设备简单列表（仅ID和名称）
        /// </summary>
        /// <returns></returns>
        Task<CommonResult<List<DeviceSimpleDto>>> GetSimpleList();

        /// <summary>
        /// 获取设备详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CommonResult<DeviceDto>> GetById(Guid id);


        /// <summary>
        /// 保存草稿
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<Guid>> SaveDraft(DeviceEditInput input);

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> SubmitApply(DeviceSubmitInput input);

        /// <summary>
        /// 撤销申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> CancelApply(CancelApplyInput input);



        /// <summary>
        /// 获取设备二维码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CommonResult<string>> GetQrCode(Guid id);



        /// <summary>
        /// 获取草稿详情
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        Task<CommonResult<ChangeApplyDetailDto>> GetDraftById(Guid applyId);


        /// <summary>
        /// 删除草稿
        /// </summary>
        /// <param name="applyIds"></param>
        /// <returns></returns>
        Task<CommonResult> DeleteDraft([FromBody] List<Guid> applyIds);



        /// <summary>
        /// 流程完成后处理
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> OnProcessCompleted(ProcessCompletedInput input);



        /// <summary>
        /// 检查设备编码是否唯一
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<bool>> CheckDeviceCodeUnique([FromQuery] CheckDeviceCodeUniqueInput input);


        /// <summary>
        /// 获取变更申请详情
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        Task<CommonResult<ChangeApplyDetailDto>> GetChangeApplyDetail(Guid applyId);



        /// <summary>
        /// 获取当前登录用户草稿列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<Page<DeviceDto>>> GetMyDraftList(DevicePageInput input);


        /// <summary>
        /// 更新草稿
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> UpdateDraft(UpdateDraftInput input);

        /// <summary>
        /// 获取当前登录用户已提交列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<Page<DeviceDto>>> GetMySubmittedList(DevicePageInput input);

        /// <summary>
        /// 提交删除申请
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> SubmitDeleteApply(DeleteApplyInput input);


        /// <summary>
        /// 获取变更申请详情（包含流程相关操作权限）
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        Task<CommonResult<ChangeApplyDetailDto>> GetApplyDetailWithActions(Guid applyId);



        /// <summary>
        /// 删除申请记录（草稿/已撤销/已拒绝）
        /// </summary>
        Task<CommonResult> DeleteApply([FromBody] List<Guid> applyIds);

    }
}
