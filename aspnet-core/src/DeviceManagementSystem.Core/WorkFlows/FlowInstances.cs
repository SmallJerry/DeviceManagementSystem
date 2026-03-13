using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.WorkFlows
{
    /// <summary>
    /// 流程实例
    /// </summary>
    [Table("FlowInstance")]
    public class FlowInstances : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 实例编号
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Code { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [StringLength(200)]
        public string FlowName { get; set; }

        /// <summary>
        /// 流程定义ID
        /// </summary>
        [Required]
        public Guid FlowDefinitionId { get; set; }

        /// <summary>
        /// 业务ID（关联的设备变更申请ID）
        /// </summary>
        [Required]
        public Guid BusinessId { get; set; }

        /// <summary>
        /// 业务类型：DeviceChange
        /// </summary>
        [Required]
        [StringLength(50)]
        public string BusinessType { get; set; }

        /// <summary>
        /// 发起人ID
        /// </summary>
        public long InitiatorId { get; set; }

        /// <summary>
        /// 发起人姓名
        /// </summary>
        [StringLength(50)]
        public string InitiatorName { get; set; }

        /// <summary>
        /// 当前节点ID
        /// </summary>
        [StringLength(100)]
        public string CurrentNodeId { get; set; }

        /// <summary>
        /// 当前节点名称
        /// </summary>
        [StringLength(200)]
        public string CurrentNodeName { get; set; }

        /// <summary>
        /// 当前节点类型：1-审批，2-抄送，5-办理
        /// </summary>
        public int? CurrentNodeType { get; set; }

        /// <summary>
        /// 当前任务ID（多个审批人时的具体任务）
        /// </summary>
        [StringLength(100)]
        public string CurrentTaskId { get; set; }

        /// <summary>
        /// 当前审批人ID（多个审批人时的具体任务人）
        /// </summary>
        public long? CurrentAssigneeId { get; set; }

        /// <summary>
        /// 当前审批人姓名
        /// </summary>
        [StringLength(50)]
        public string CurrentAssigneeName { get; set; }

        /// <summary>
        /// 流程状态：0-审批中，1-已通过，2-不通过，3-已撤销
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// 流程开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 流程结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 节点配置JSON
        /// </summary>
        public string NodeConfig { get; set; }


        /// <summary>
        /// 表单数据JSON
        /// </summary>
        public string FormData { get; set; }

        /// <summary>
        /// 是否可撤销
        /// </summary>
        public bool Cancelable { get; set; } = true;
    }
}
