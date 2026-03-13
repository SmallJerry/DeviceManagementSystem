using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceManagementSystem.System
{
    /// <summary>
    /// 数据字典
    /// </summary>
    [Table("Dict")]
    public class Dict : FullAuditedEntity<Guid>
    {

        /// <summary>
        /// 父id
        /// </summary>
        public Guid? ParentId { get; set; } = Guid.Empty;


        /// <summary>
        /// 字典名称
        /// </summary>
        [StringLength(64)]
        public string DictLabel { get; set; }



        /// <summary>
        /// 字典值
        /// </summary>
        public string DictValue { get; set; }



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
        /// 扩展信息
        /// </summary>
        [StringLength(255)]
        public string ExtJson { get; set; }

    }
}
