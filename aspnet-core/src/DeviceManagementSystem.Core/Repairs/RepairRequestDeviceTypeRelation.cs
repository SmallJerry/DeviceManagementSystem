using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Repairs
{
    /// <summary>
    /// 维修申报与设备类型关系表
    /// </summary>
    [Table("RepairRequestDeviceTypeRelation")]
    public class RepairRequestDeviceTypeRelation : Entity<Guid>
    {
        /// <summary>
        /// 维修申报ID
        /// </summary>
        public Guid RepairRequestId { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid DeviceTypeId { get; set; }
    }
}
