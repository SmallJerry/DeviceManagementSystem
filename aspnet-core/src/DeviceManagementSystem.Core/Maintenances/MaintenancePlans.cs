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
        /// 计划名称
        /// </summary>
        [StringLength(100)]
        public string PlanName { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// 保养等级（月度、季度、半年度、年度）
        /// </summary>
        [StringLength(20)]
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 周期类型（日、周、月、季度、半年、年）
        /// </summary>
        [StringLength(20)]
        public string CycleType { get; set; }

        /// <summary>
        /// 周期值（天数）
        /// </summary>
        public int CycleDays { get; set; }

        /// <summary>
        /// 首次保养日期
        /// </summary>
        public DateTime FirstMaintenanceDate { get; set; }

        /// <summary>
        /// 下次保养日期
        /// </summary>
        public DateTime NextMaintenanceDate { get; set; }

        /// <summary>
        /// 上次保养日期
        /// </summary>
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// 计划状态（启用/停用）
        /// </summary>
        [StringLength(20)]
        public string Status { get; set; } = "启用";

        /// <summary>
        /// 是否已生成工单
        /// </summary>
        public bool HasGeneratedTask { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
    }
}
