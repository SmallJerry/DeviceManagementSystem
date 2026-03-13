using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.BasicDataManagement
{
    /// <summary>
    /// 工厂建模实体
    /// </summary>
    [Table("FactoryNode")]
    public class FactoryNodes : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 节点类型    工厂用Factory命名、车间用Workshop命名、产线用ProductionLine命名、工位用Workstation命名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string NodeType { get; set; }

        /// <summary>
        /// 节点编码
        /// </summary>
        [StringLength(100)]
        public string Code { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// 地址 -用于工厂节点
        /// </summary>
        [StringLength(500)]
        public string Address { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Enabled";

        /// <summary>
        /// 排序码
        /// </summary>
        public int SortCode { get; set; } = 0;

        /// <summary>
        /// 拓展信息
        /// </summary>
        public string ExtendInfo { get; set; }



        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }
        }
}
