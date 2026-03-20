using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceManagementSystem.Systems
{
    /// <summary>
    /// 访问资源实体
    /// </summary>
    [Table("Resources")]
    public class Resource : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 父id
        /// </summary>
        public Guid? ParentId { get; set; }


        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(255)]
        public string Title { get; set; }


        /// <summary>
        /// 别名
        /// </summary>
        [StringLength(255)]
        public string Name { get; set; }


        /// <summary>
        /// 分类
        /// </summary>
        [StringLength(64)]
        public string Category { get; set; }



        /// <summary>
        /// 模块
        /// </summary>
        [StringLength(255)]
        public string Module { get; set; }


        /// <summary>
        /// 菜单类型
        /// </summary>
        [StringLength(255)]
        public string MenuType { get; set; }


        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }


        /// <summary>
        /// 组件
        /// </summary>
        [StringLength(255)]
        public string Component { get; set; }


        /// <summary>
        /// 图标
        /// </summary>
        [StringLength(255)]
        public string Icon { get; set; }


        /// <summary>
        /// 颜色
        /// </summary>
        [StringLength(255)]
        public string Color { get; set; }


        /// <summary>
        /// 是否可见
        /// </summary>
        [StringLength(100)]
        public string Visible { get; set; }



        /// <summary>
        /// 显示布局
        /// </summary>
        [StringLength(50)]
        public string DisplayLayout { get; set; }


        /// <summary>
        /// 页签缓存
        /// </summary>
        [StringLength(50)]
        public string KeepLive { get; set; }


        /// <summary>
        /// 排序码
        /// </summary>
        [Range(1, 200)]
        public int? SortOrder { get; set; }


        /// <summary>
        /// 扩展信息
        /// </summary>
        public string ExtJson { get; set; }


    }
}
