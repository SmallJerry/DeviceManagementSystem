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
    /// 设备与用户关系
    /// </summary>
    [Table("DeviceUserRelation")]
    public class DeviceUserRelations : Entity<Guid>
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户类型（维修人员/保养人员）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserType { get; set; }
    }
}
