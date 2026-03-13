using DeviceManagementSystem.Utils.Common;
using DeviceManagementSystem.WorkFlows.FlowInstance.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManagementSystem.WorkFlows.FlowInstance
{
    /// <summary>
    /// 流程实例服务接口
    /// </summary>
    public interface IFlowInstanceAppService
    {
        /// <summary>
        /// 启动流程
        /// </summary>
        Task<Guid> StartProcessAsync(Guid flowDefId, Guid businessId, string businessType, long initiatorId, string formData);

        /// <summary>
        /// 处理任务
        /// </summary>
        Task<bool> HandleTaskAsync(Guid taskId, int flowCmd, long operatorId, string comment, string formData, string backNodeId = null);

        /// <summary>
        /// 撤销流程
        /// </summary>
        Task<bool> CancelProcessAsync(Guid flowInstanceId, long operatorId, string reason);

        /// <summary>
        /// 获取待办任务列表
        /// </summary>
        Task<Page<FlowPendingItemDto>> GetPendingTasksAsync(string businessType, string searchKey, int current, int size);

        /// <summary>
        /// 获取我已审批列表
        /// </summary>
        Task<Page<FlowProcessedItemDto>> GetMyApprovedAsync(string businessType, string searchKey, int? status, int current, int size);

        /// <summary>
        /// 获取我收到的列表（抄送）
        /// </summary>
        Task<Page<FlowProcessedItemDto>> GetMyInitiatedAsync(string businessType, string searchKey, int? status, int current, int size);


        /// <summary>
        /// 获取流程详情
        /// </summary>
        Task<FlowDetailDto> GetFlowDetailAsync(Guid flowInstanceId);

        /// <summary>
        /// 获取节点表单权限
        /// </summary>
        Task<string> GetNodeFormAuthsAsync(Guid taskId);

        /// <summary>
        /// 获取可回退节点列表
        /// </summary>
        Task<List<dynamic>> GetBackableNodesAsync(Guid flowInstanceId, string currentNodeId);

        /// <summary>
        /// 获取流程实例
        /// </summary>
        Task<FlowInstances> GetFlowInstanceAsync(Guid flowInstanceId);

        /// <summary>
        /// 根据变更申请ID获取流程实例
        /// </summary>
        Task<FlowInstances> GetFlowInstanceByApplyIdAsync(Guid applyId);


        /// <summary>
        /// 预览流程节点图表
        /// </summary>
        Task<CommonResult<List<FlowNodePreviewDto>>> ViewProcessChartAsync(string formCode);
    }
}