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
    /// 节点任务表
    /// </summary>
    [Table("FlowNodeTask")]
    public class FlowNodeTasks : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 流程实例ID
        /// </summary>
        public Guid FlowInstanceId { get; set; }

        /// <summary>
        /// 节点ID
        /// </summary>
        [StringLength(100)]
        public string NodeId { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        [StringLength(200)]
        public string NodeName { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public int NodeType { get; set; }

        /// <summary>
        /// 审批人ID
        /// </summary>
        public long? AssigneeId { get; set; }

        /// <summary>
        /// 审批人姓名
        /// </summary>
        [StringLength(50)]
        public string AssigneeName { get; set; }

        /// <summary>
        /// 任务状态：0-待处理，1-已处理，2-已跳过
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// 多人审批方式：1-会签，2-或签，3-依次审批
        /// </summary>
        public int? MultiInstanceType { get; set; }

        /// <summary>
        /// 处理顺序
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// 创建时间（任务生成时间）
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? HandleTime { get; set; }

        /// <summary>
        /// 操作指令
        /// </summary>
        public int? HandleCmd { get; set; }

        /// <summary>
        /// 操作意见
        /// </summary>
        [StringLength(500)]
        public string HandleComment { get; set; }

        /// <summary>
        /// 表单权限JSON
        /// </summary>
        public string FormAuths { get; set; }
    }
}
