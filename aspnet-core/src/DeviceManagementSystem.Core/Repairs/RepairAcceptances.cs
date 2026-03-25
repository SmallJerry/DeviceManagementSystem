using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Repairs
{
    /// <summary>
    /// 维修验收记录表
    /// </summary>
    [Table("RepairAcceptance")]
    public class RepairAcceptances : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 维修工单ID
        /// </summary>
        public Guid RepairTaskId { get; set; }

        /// <summary>
        /// 维修申报ID
        /// </summary>
        public Guid RepairRequestId { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 验收依据（JSON数组存储）
        /// </summary>
        [StringLength(2000)]
        public string AcceptanceCriteriaJson { get; set; }

        /// <summary>
        /// 维修前参数（JSON格式）
        /// </summary>
        [StringLength(2000)]
        public string BeforeRepairParams { get; set; }

        /// <summary>
        /// 维修后参数（JSON格式）
        /// </summary>
        [StringLength(2000)]
        public string AfterRepairParams { get; set; }

        /// <summary>
        /// 验收结论：0-正常，1-不正常
        /// </summary>
        public int AcceptanceConclusion { get; set; }

        /// <summary>
        /// 验收意见/预防措施
        /// </summary>
        [StringLength(1000)]
        public string AcceptanceOpinion { get; set; }

        /// <summary>
        /// 验收人ID
        /// </summary>
        [MaxLength(20)]
        public long AcceptorId { get; set; }

        /// <summary>
        /// 验收人姓名
        /// </summary>
        [StringLength(50)]
        public string AcceptorName { get; set; }

        /// <summary>
        /// 验收时间
        /// </summary>
        public DateTime AcceptanceTime { get; set; }
    }
}
