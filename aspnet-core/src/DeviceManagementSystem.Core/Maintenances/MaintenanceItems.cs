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
    /// 保养项目表
    /// </summary>
    [Table("MaintenanceItem")]
    public class MaintenanceItems : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// 分组ID
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// 点检序号（①、②、③...）
        /// </summary>
        [MaxLength(10)]
        public string PointNo { get; set; }

        /// <summary>
        /// 点检项目名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string PointName { get; set; }

        /// <summary>
        /// 点检内容
        /// </summary>
        [MaxLength(500)]
        public string InspectionContent { get; set; }

        /// <summary>
        /// 点检方法（JSON数组存储，例如：["清洁","目测"]）
        /// </summary>
        [MaxLength(200)]
        public string InspectionMethod { get; set; }

        /// <summary>
        /// 分组排序（用于组间排序）
        /// </summary>
        public int? GroupSortOrder { get; set; }

        /// <summary>
        /// 项目排序（用于组内排序）
        /// </summary>
        public int? ItemSortOrder { get; set; }



    }
}
