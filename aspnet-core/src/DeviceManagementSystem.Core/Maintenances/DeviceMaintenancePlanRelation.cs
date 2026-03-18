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
    /// 设备与保养计划关系表
    /// </summary>
    [Table("DeviceMaintenancePlanRelation")]
    public class DeviceMaintenancePlanRelation : Entity<Guid>
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 保养计划ID
        /// </summary>
        public Guid MaintenancePlanId { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        [MaxLength(20)]
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid TemplateId { get; set; }
    }
}
