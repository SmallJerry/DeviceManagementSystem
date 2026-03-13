using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Authorization.Resources
{
    /// <summary>
    /// 系统关系表
    /// </summary>
    [Table("SystemRelation")]
    public class SystemRelation : Entity<Guid>
    {

        /// <summary>
        /// 对象ID
        /// </summary>
        public string ObjectId { get; set; }


        /// <summary>
        /// 目标
        /// </summary>
        public string Target { get; set; }


        /// <summary>
        /// 分类
        /// </summary>
        [StringLength(50)]
        public string Category { get; set; }


        /// <summary>
        /// 拓展信息
        /// </summary>
        public string ExtJson { get; set; }
    }
}
