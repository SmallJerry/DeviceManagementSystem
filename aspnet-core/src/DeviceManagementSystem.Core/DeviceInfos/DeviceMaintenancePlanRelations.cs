using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.DeviceInfos
{
    /// <summary>
    /// 设备与保养计划关系
    /// </summary>
    [Table("DeviceMaintenancePlanRelation")]
    public class DeviceMaintenancePlanRelations : Entity<Guid>
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 保养计划Id
        /// </summary>
        public Guid MaintenancePlanId { get; set; }

        /// <summary>
        /// 周期类型（年度、季度、月度）
        /// </summary>
        [StringLength(20)]
        public string CycleType { get; set; }
    }
}
