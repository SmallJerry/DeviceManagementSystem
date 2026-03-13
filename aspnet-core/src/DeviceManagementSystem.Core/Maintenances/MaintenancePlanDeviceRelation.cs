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
    /// 保养计划与设备关系表（审核通过后关联）
    /// </summary>
    [Table("MaintenancePlanDeviceRelation")]
    public class MaintenancePlanDeviceRelation : Entity<Guid>
    {
        /// <summary>
        /// 保养计划Id
        /// </summary>
        public Guid PlanId { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid DeviceId { get; set; }
    }
}
