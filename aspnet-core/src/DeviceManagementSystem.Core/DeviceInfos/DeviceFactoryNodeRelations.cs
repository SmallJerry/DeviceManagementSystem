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
    /// 设备与工厂节点关系
    /// </summary>
    [Table("DeviceFactoryNodeRelation")]
    public class DeviceFactoryNodeRelations : Entity<Guid>
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 工厂节点Id
        /// </summary>
        public Guid FactoryNodeId { get; set; }

        /// <summary>
        /// 节点类型（工厂、车间、产线、工位）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string NodeType { get; set; }
    }
}
