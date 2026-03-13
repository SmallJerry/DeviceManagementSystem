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
        [MaxLength(100)]
        public string TemplateName { get; set; }

        /// <summary>
        /// 模板编号
        /// </summary>
        [MaxLength(50)]
        public string TemplateCode { get; set; }

        /// <summary>
        /// 保养等级（月度：Monthly/季度：Quarter/半年度：Half-Yearly/年度：Annual）
        /// </summary>
        [MaxLength(50)]
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 模板描述
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 状态（启用/停用）
        /// </summary>
        public bool? IsActive { get; set; } = true;

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; } = 1;
    }
}
