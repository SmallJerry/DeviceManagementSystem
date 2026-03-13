using Abp.Domain.Entities;
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
    /// 保养计划与项目关系表
    /// </summary>
    [Table("MaintenancePlanItemRelation")]
    public class MaintenancePlanItemRelation : Entity<Guid>
    {
        /// <summary>
        /// 保养计划Id
        /// </summary>
        public Guid PlanId { get; set; }

        /// <summary>
        /// 保养项目Id
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 项目来源（Template/Custom）
        /// </summary>
        [MaxLength(20)]
        public string Source { get; set; }
    }
}
