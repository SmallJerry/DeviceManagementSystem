using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances
{
    /// <summary>
    /// 保养计划与模板关系表
    /// </summary>
    [Table("MaintenancePlanTemplateRelation")]
    public class MaintenancePlanTemplateRelation : Entity<Guid>
    {
        /// <summary>
        /// 保养计划Id
        /// </summary>
        public Guid PlanId { get; set; }

        /// <summary>
        /// 保养模板Id
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// 模板版本号（记录创建时的模板版本）
        /// </summary>
        public int TemplateVersion { get; set; }
    }
}
