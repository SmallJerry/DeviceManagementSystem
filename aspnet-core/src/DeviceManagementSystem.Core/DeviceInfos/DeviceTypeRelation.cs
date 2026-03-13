using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.DeviceInfos
{
    /// <summary>
    /// 设备与供应商关系
    /// </summary>
    [Table("DeviceTypeRelation")]
    public class DeviceTypeRelations : Entity<Guid>
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 类型Id
        /// </summary>
        public Guid TypeId { get; set; }
    }
}
