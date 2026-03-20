using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceManagementSystem.Systems
{

    /// <summary>
    /// 系统配置项
    /// </summary>
    [Table("SystemConfig")]
    public class SystemConfig : FullAuditedEntity<Guid>
    {

        /// <summary>
        /// 配置键
        /// </summary>
        [Required]
        [StringLength(64)]
        public string ConfigKey { get; set; }


        /// <summary>
        /// 配置值
        /// </summary>
        public string ConfigValue { get; set; }


        /// <summary>
        /// 分类
        /// </summary>
        [StringLength(64)]
        public string Category { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


        /// <summary>
        /// 排序码
        /// </summary>
        [Range(1, 200)]
        public int? SortCode { get; set; }


        /// <summary>
        /// 扩展信息
        /// </summary>
        [StringLength(255)]
        public string ExtJson { get; set; }


    }
}
