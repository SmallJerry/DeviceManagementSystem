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
    /// 保养工单与设备关系表（支持多设备关联同一工单）
    /// </summary>
    [Table("MaintenanceWorkOrderDeviceRelation")]
    public class MaintenanceWorkOrderDeviceRelation : Entity<Guid>
    {
        /// <summary>
        /// 保养工单Id
        /// </summary>
        public Guid WorkOrderId { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 保养计划Id（关联具体的保养计划）
        /// </summary>
        public Guid? PlanId { get; set; }
    }
}
