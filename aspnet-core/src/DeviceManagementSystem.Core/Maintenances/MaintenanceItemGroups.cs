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
    /// 保养项目分组（点检部位）
    /// </summary>
    [Table("MaintenanceItemGroup")]
    public class MaintenanceItemGroups : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// 分组名称（例如：清洗工作腔）
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string GroupName { get; set; }

        /// <summary>
        /// 点检部位（与GroupName相同，用于兼容）
        /// </summary>
        [MaxLength(100)]
        public string PointType { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int? SortOrder { get; set; }
    }
}
