using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.WorkFlows.FlowInstance.Dto
{
    /// <summary>
    /// 流程实例DTO
    /// </summary>
    public class FlowInstanceDto
    {
        /// <summary>
        /// 实例ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 实例编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName { get; set; }

        /// <summary>
        /// 流程定义ID
        /// </summary>
        public Guid FlowDefinitionId { get; set; }

        /// <summary>
        /// 业务ID
        /// </summary>
        public Guid BusinessId { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BusinessType { get; set; }

        /// <summary>
        /// 发起人ID
        /// </summary>
        public long InitiatorId { get; set; }

        /// <summary>
        /// 发起人姓名
        /// </summary>
        public string InitiatorName { get; set; }

        /// <summary>
        /// 当前节点ID
        /// </summary>
        public string CurrentNodeId { get; set; }

        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string CurrentNodeName { get; set; }

        /// <summary>
        /// 当前节点类型
        /// </summary>
        public int? CurrentNodeType { get; set; }

        /// <summary>
        /// 当前任务ID
        /// </summary>
        public string CurrentTaskId { get; set; }

        /// <summary>
        /// 流程状态：0-审批中，1-已通过，2-不通过，3-已撤销
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 状态名称
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 是否可取消
        /// </summary>
        public bool Cancelable { get; set; }

        /// <summary>
        /// 当前节点是否可编辑表单
        /// </summary>
        public bool Editable { get; set; }
    }

    /// <summary>
    /// 流程节点记录DTO
    /// </summary>
    public class FlowNodeRecordDto
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public int NodeType { get; set; }


        /// <summary>
        /// 节点类型名称
        /// </summary>
        public string NodeTypeName { get; set; }

        /// <summary>
        /// 操作指令
        /// </summary>
        public int FlowCmd { get; set; }

        /// <summary>
        /// 操作指令名称
        /// </summary>
        public string FlowCmdName { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public long? OperatorId { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperateTime { get; set; }

        /// <summary>
        /// 操作意见
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 是否正在进行中
        /// </summary>
        public bool Underway { get; set; }

        /// <summary>
        /// 任务人列表
        /// </summary>
        public List<long> UserIds { get; set; }

        /// <summary>
        /// 审批人姓名列表
        /// </summary>
        public List<string> UserNames { get; set; } // 新增字段，用于显示审批人姓名
    }

    /// <summary>
    /// 流程待办查询参数
    /// </summary>
    public class FlowPendingInput
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BusinessType { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int Current { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public int Size { get; set; } = 10;
    }

    /// <summary>
    /// 流程待办项DTO
    /// </summary>
    public class FlowPendingItemDto
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 流程实例ID
        /// </summary>
        public Guid FlowInstanceId { get; set; }

        /// <summary>
        /// 实例编号
        /// </summary>
        public string FlowCode { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName { get; set; }

        /// <summary>
        /// 业务ID
        /// </summary>
        public Guid BusinessId { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BusinessType { get; set; }

        /// <summary>
        /// 业务名称（设备名称）
        /// </summary>
        public string BusinessName { get; set; }

        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public int NodeType { get; set; }

        /// <summary>
        /// 发起人姓名
        /// </summary>
        public string InitiatorName { get; set; }

        /// <summary>
        /// 发起时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 任务生成时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 流程已办查询参数
    /// </summary>
    public class FlowProcessedInput
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BusinessType { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int Current { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public int Size { get; set; } = 10;
    }

    /// <summary>
    /// 流程已办项DTO
    /// </summary>
    public class FlowProcessedItemDto
    {
        /// <summary>
        /// 流程实例ID
        /// </summary>
        public Guid FlowInstanceId { get; set; }

        /// <summary>
        /// 实例编号
        /// </summary>
        public string FlowCode { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName { get; set; }

        /// <summary>
        /// 业务ID
        /// </summary>
        public Guid BusinessId { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string BusinessType { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        public string BusinessName { get; set; }

        /// <summary>
        /// 发起人姓名
        /// </summary>
        public string InitiatorName { get; set; }

        /// <summary>
        /// 发起时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 状态名称
        /// </summary>
        public string StatusName { get; set; }
    }

    /// <summary>
    /// 流程操作输入
    /// </summary>
    public class FlowActionInput
    {
        /// <summary>
        /// 流程实例ID
        /// </summary>
        public Guid FlowInstanceId { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid? TaskId { get; set; }

        /// <summary>
        /// 操作指令：3-拒绝，4-通过，7-回退
        /// </summary>
        public int FlowCmd { get; set; }

        /// <summary>
        /// 操作意见
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 回退到的节点ID
        /// </summary>
        public string BackNodeId { get; set; }

        /// <summary>
        /// 表单数据（编辑后）
        /// </summary>
        public string FormData { get; set; }
    }

    /// <summary>
    /// 流程详情DTO
    /// </summary>
    public class FlowDetailDto
    {
        /// <summary>
        /// 流程实例信息
        /// </summary>
        public FlowInstanceDto FlowInstance { get; set; }

        /// <summary>
        /// 节点记录列表
        /// </summary>
        public List<FlowNodeRecordDto> NodeRecords { get; set; }

        /// <summary>
        /// 待处理节点列表
        /// </summary>
        public List<FlowNodeRecordDto> FutureNodes { get; set; }

        /// <summary>
        /// 业务数据
        /// </summary>
        public object BusinessData { get; set; }
    }


    /// <summary>
    /// 审批通过输入参数
    /// </summary>
    public class ApproveInput
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 表单数据（如果编辑了表单）
        /// </summary>
        public string FormData { get; set; }
    }

    /// <summary>
    /// 审批拒绝输入参数
    /// </summary>
    public class RejectInput
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 拒绝意见
        /// </summary>
        public string Comment { get; set; }
    }

    /// <summary>
    /// 回退输入参数
    /// </summary>
    public class BackInput
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 回退意见
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 回退到的节点ID
        /// </summary>
        public string BackNodeId { get; set; }
    }

    /// <summary>
    /// 撤销流程输入参数
    /// </summary>
    public class CancelFlowInput
    {
        /// <summary>
        /// 流程实例ID
        /// </summary>
        public Guid FlowInstanceId { get; set; }

        /// <summary>
        /// 撤销原因
        /// </summary>
        public string Reason { get; set; }
    }


    /// <summary>
    /// 流程完成回调输入参数
    /// </summary>
    public class ProcessCompletedInput
    {
        /// <summary>
        /// 流程实例ID
        /// </summary>
        public Guid FlowInstanceId { get; set; }

        /// <summary>
        /// 流程状态：1-已通过，2-已拒绝，3-已撤销
        /// </summary>
        public int ProcessStatus { get; set; }
    }

}
