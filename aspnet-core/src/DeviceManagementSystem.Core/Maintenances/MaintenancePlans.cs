using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances
{
    /// <summary>
    /// 保养计划表
    /// </summary>
    [Table("MaintenancePlan")]
    public class MaintenancePlans : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 计划编号
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string PlanCode { get; set; }

        /// <summary>
        /// 计划名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string PlanName { get; set; }

        /// <summary>
        /// 申请ID（设备建档时的变更申请ID）
        /// </summary>
        public Guid? ChangeApplyId { get; set; }

        /// <summary>
        /// 保养等级（月度/季度/半年度/年度）
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 周期类型（Monthly/Quarterly/HalfYearly/Annual）
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string CycleType { get; set; }

        /// <summary>
        /// 周期值（固定为1）
        /// </summary>
        public int CycleValue { get; set; } = 1;

        /// <summary>
        /// 周期天数（缓存计算值）
        /// </summary>
        public int CycleDays { get; set; }

        /// <summary>
        /// 首次保养日期（设备启用日期）
        /// </summary>
        public DateTime FirstMaintenanceDate { get; set; }

        /// <summary>
        /// 上次保养日期
        /// </summary>
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// 下次保养日期
        /// </summary>
        public DateTime? NextMaintenanceDate { get; set; }

        /// <summary>
        /// 计划状态（启用/停用）
        /// </summary>
        [MaxLength(10)]
        public string PlanStatus { get; set; } = "启用";

        /// <summary>
        /// 计划描述
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 提前提醒天数
        /// </summary>
        public int RemindDays { get; set; } = 7;

        /// <summary>
        /// 最后执行人
        /// </summary>
        [MaxLength(200)]
        public string LastExecutor { get; set; }

        /// <summary>
        /// 最后执行时间
        /// </summary>
        public DateTime? LastExecuteTime { get; set; }
    }
}
