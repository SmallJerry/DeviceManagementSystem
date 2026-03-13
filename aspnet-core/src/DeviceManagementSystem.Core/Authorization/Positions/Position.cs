using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceManagementSystem.Authorization.Positions
{
    [Table("Position")]
    public class Position : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 组织id
        /// </summary>
        public Guid? OrgId { get; set; }


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
        /// 拓展信息
        /// </summary>
        public string ExtJson { get; set; }


    }
}
