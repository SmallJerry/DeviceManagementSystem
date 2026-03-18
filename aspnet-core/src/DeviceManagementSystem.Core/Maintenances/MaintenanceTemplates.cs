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
    /// 保养模板表
    /// </summary>
    [Table("MaintenanceTemplate")]
    public class MaintenanceTemplates : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        [StringLength(100)]
        public string TemplateName { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid DeviceTypeId { get; set; }

        /// <summary>
        /// 保养等级（月度、季度、半年度、年度）
        /// </summary>
        [StringLength(20)]
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 模板描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 状态（启用/停用）
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
