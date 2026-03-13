using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceManagementSystem.Authorization.Organizations
{
    [Table("Organization")]
    public class Organization : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 父id
        /// </summary>
        public Guid? ParentId { get; set; }


        /// <summary>
        /// 名称
        /// </summary>
        [StringLength(64)]
        public string Name { get; set; }


        /// <summary>
        /// 分类
        /// </summary>
        [StringLength(64)]
        public string Category { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Range(1, 200)]
        public int? SortCode { get; set; }


        /// <summary>
        /// 拓展字段
        /// </summary>
        public string ExtJson { get; set; }

    }
}
