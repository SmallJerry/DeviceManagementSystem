using Abp.Domain.Entities.Auditing;
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
    /// 页面资源表
    /// </summary>
    [Table("Resource")]
    public class Resource : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 父id
        /// </summary>
        public Guid? ParentId { get; set; }


        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(100)]
        public string Title { get; set; }



        /// <summary>
        /// 编码
        /// </summary>
        [StringLength(50)]
        public string Code { get; set; }




        /// <summary>
        /// 别名
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; }


        /// <summary>
        /// 分类
        /// </summary>
        [StringLength(100)]
        public string Category { get; set; }


        /// <summary>
        /// 模块
        /// </summary>
        public Guid? Module { get; set; }


        /// <summary>
        /// 菜单类型
        /// </summary>
        [StringLength(50)]
        public string MenuType { get; set; }


        /// <summary>
        /// 路径
        /// </summary>
        [StringLength(100)]
        public string Path { get; set; }


        /// <summary>
        /// 组件     
        /// </summary>
        [StringLength(100)]
        public string Component { get; set; }


        /// <summary>
        /// 图标
        /// </summary>
        [StringLength(100)]
        public string Icon { get; set; }


        /// <summary>
        /// 颜色
        /// </summary>
        [StringLength(50)]
        public string Color { get; set; }



        /// <summary>
        /// 是否可见
        /// </summary>
        public bool? Visible { get; set; }



        /// <summary>
        /// 显示布局
        /// </summary>
        [StringLength(20)]
        public string DisplayLayout { get; set; }


        /// <summary>
        /// 页面缓存
        /// </summary>
        [StringLength(20)]
        public string KeppLive { get; set; }


        /// <summary>
        /// 排序码
        /// </summary>
        [StringLength(20)]
        public int? SortCode { get; set; }


        /// <summary>
        /// 拓展信息
        /// </summary>
        public string ExtJson { get; set; }


    }
}
