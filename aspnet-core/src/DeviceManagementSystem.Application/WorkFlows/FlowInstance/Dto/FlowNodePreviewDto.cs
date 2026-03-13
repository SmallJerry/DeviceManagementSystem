using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.WorkFlows.FlowInstance.Dto
{
    /// <summary>
    /// 流程节点预览DTO
    /// </summary>
    public class FlowNodePreviewDto
    {
        /// <summary>
        /// 节点唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 节点ID（用于前端标识）
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// 节点类型：0-开始，1-审批，2-抄送，3-条件，4-排他网关，5-办理，9-结束
        /// </summary>
        public int NodeType { get; set; }

        /// <summary>
        /// 审批类型：0-人工审批，1-自动通过，2-自动拒绝
        /// </summary>
        public int? ApprovalType { get; set; }

        /// <summary>
        /// 多人审批方式：1-会签，2-或签，3-依次审批
        /// </summary>
        public int? MultiInstanceApprovalType { get; set; }

        /// <summary>
        /// 审批人为空时的处理方式：0-自动通过，1-指定人员审批
        /// </summary>
        public int? FlowNodeNoAuditorType { get; set; }

        /// <summary>
        /// 审批人为空时的指定人员ID
        /// </summary>
        public string FlowNodeNoAuditorAssignee { get; set; }

        /// <summary>
        /// 审批人用户ID列表
        /// </summary>
        public List<string> UserIds { get; set; } = new List<string>();

        /// <summary>
        /// 审批人角色ID列表
        /// </summary>
        public List<string> RoleIds { get; set; } = new List<string>();

        /// <summary>
        /// 是否发起人自选
        /// </summary>
        public bool InitiatorChoice { get; set; }
    }
}
