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
    /// 维修工单执行记录表（保存草稿）
    /// </summary>
    [Table("RepairTaskExecutionRecord")]
    public class RepairTaskExecutionRecords : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 维修工单ID
        /// </summary>
        public Guid RepairTaskId { get; set; }

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
        /// 保存类型：0-草稿，1-正式提交
        /// </summary>
        [MaxLength(20)]
        public int SaveType { get; set; } = 0;
    }
}
