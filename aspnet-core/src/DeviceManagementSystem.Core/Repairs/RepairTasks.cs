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
    /// 维修工单表
    /// </summary>
    [Table("RepairTask")]
    public class RepairTasks : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 工单编号（格式：RT+年月日+流水号）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TaskNo { get; set; }

        /// <summary>
        /// 关联的维修申报ID
        /// </summary>
        public Guid RepairRequestId { get; set; }

        /// <summary>
        /// 申报人ID
        /// </summary>
        [MaxLength(20)]
        public long RequesterId { get; set; }

        /// <summary>
        /// 申报人姓名
        /// </summary>
        [StringLength(50)]
        public string RequesterName { get; set; }

        /// <summary>
        /// 报修时间
        /// </summary>
        public DateTime RequestTime { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 维修人员ID（多个用逗号分隔）
        /// </summary>
        [StringLength(500)]
        public string RepairerIds { get; set; }

        /// <summary>
        /// 维修人员姓名（多个用逗号分隔）
        /// </summary>
        [StringLength(1000)]
        public string RepairerNames { get; set; }

        /// <summary>
        /// 接单时间
        /// </summary>
        public DateTime? AcceptTime { get; set; }

        /// <summary>
        /// 维修开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 维修结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 维修时长（分钟）
        /// </summary>
        public int? DurationMinutes { get; set; }

        /// <summary>
        /// 工单状态：0-待接单，1-维修中，2-待验收，3-已完成，4-已取消
        /// </summary>
        [MaxLength(20)]
        public int TaskStatus { get; set; } = 0;

        /// <summary>
        /// 超时标记：0-未超时，1-已超时
        /// </summary>
        [MaxLength(20)]
        public int IsOverdue { get; set; } = 0;

        /// <summary>
        /// 故障原因
        /// </summary>
        [StringLength(1000)]
        public string FaultReason { get; set; }

        /// <summary>
        /// 对产品品质影响分析及处理方案
        /// </summary>
        [StringLength(2000)]
        public string QualityImpactAnalysis { get; set; }

        /// <summary>
        /// 维修方法及结果
        /// </summary>
        [StringLength(2000)]
        public string RepairMethodResult { get; set; }

        /// <summary>
        /// 是否通知保养：0-否，1-是
        /// </summary>
        [MaxLength(20)]
        public int NotifyMaintenance { get; set; } = 0;

        /// <summary>
        /// 保养计划ID（当NotifyMaintenance=1时）
        /// </summary>
        public Guid? MaintenancePlanId { get; set; }

        /// <summary>
        /// 保养工单ID（当NotifyMaintenance=1时）
        /// </summary>
        public Guid? MaintenanceTaskId { get; set; }

        /// <summary>
        /// 是否已通知验收：0-否，1-是
        /// </summary>
        [MaxLength(20)]
        public int NotifiedAcceptance { get; set; } = 0;

        /// <summary>
        /// 验收人ID
        /// </summary>
        public long? AcceptorId { get; set; }

        /// <summary>
        /// 验收人姓名
        /// </summary>
        [StringLength(50)]
        public string AcceptorName { get; set; }

        /// <summary>
        /// 验收时间
        /// </summary>
        public DateTime? AcceptanceTime { get; set; }
    }
}
