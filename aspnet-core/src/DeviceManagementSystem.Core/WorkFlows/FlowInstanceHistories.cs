using Abp.Domain.Entities;
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
    /// 流程实例历史记录
    /// </summary>
    [Table("FlowInstanceHistory")]
    public class FlowInstanceHistories : FullAuditedEntity<Guid>
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
        /// 操作指令：0-发起，1-自动拒绝，2-自动通过，3-拒绝，4-通过，5-撤销，7-回退
        /// </summary>
        [Range(0,10)]
        public int FlowCmd { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public long? OperatorId { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        [StringLength(50)]
        public string OperatorName { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperateTime { get; set; }

        /// <summary>
        /// 操作意见
        /// </summary>
        [StringLength(500)]
        public string Comment { get; set; }

        /// <summary>
        /// 是否可编辑表单
        /// </summary>
        public bool Editable { get; set; } = false;

        /// <summary>
        /// 操作前的表单数据JSON
        /// </summary>
        public string BeforeFormData { get; set; }

        /// <summary>
        /// 操作后的表单数据JSON
        /// </summary>
        public string AfterFormData { get; set; }
    }
}
