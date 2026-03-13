using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Resources.Menus.Dto
{
    /// <summary>
    /// 添加菜单参数
    /// </summary>
    public class MenuAddInput
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "标题不能为空")]
        [StringLength(100, ErrorMessage = "标题长度不能超过100个字符")]
        public string Title { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        [Required(ErrorMessage = "菜单类型不能为空")]
        public string MenuType { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        [Required(ErrorMessage = "模块不能为空")]
        public Guid? Module { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [Required(ErrorMessage = "路径不能为空")]
        [StringLength(200, ErrorMessage = "路径长度不能超过200个字符")]
        public string Path { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Required(ErrorMessage = "排序码不能为空")]
        [Range(0, 9999, ErrorMessage = "排序码范围0-9999")]
        public int SortCode { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [StringLength(100, ErrorMessage = "别名长度不能超过100个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 组件路径
        /// </summary>
        [StringLength(200, ErrorMessage = "组件路径长度不能超过200个字符")]
        public string Component { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [StringLength(100, ErrorMessage = "图标长度不能超过100个字符")]
        public string Icon { get; set; }

        /// <summary>
        /// 是否可见（YES/NO）
        /// </summary>
        [StringLength(10, ErrorMessage = "是否可见长度不能超过10个字符")]
        public string Visible { get; set; }

        /// <summary>
        /// 显示布局
        /// </summary>
        [StringLength(20, ErrorMessage = "显示布局长度不能超过20个字符")]
        public string DisplayLayout { get; set; }

        /// <summary>
        /// 扩展信息（JSON格式）
        /// </summary>
        public string ExtJson { get; set; }
    }




    /// <summary>
    /// 菜单分页查询参数
    /// </summary>
    public class MenuPageInput
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        public Guid? Module { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// 排序方式：ASCEND/DESCEND
        /// </summary>
        public string SortOrder { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int Current { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public int Size { get; set; } = 10;
    }



    /// <summary>
    /// 菜单树查询参数
    /// </summary>
    public class MenuTreeInput
    {
        /// <summary>
        /// 模块ID
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }
    }



    /// <summary>
    /// 编辑菜单参数
    /// </summary>
    public class MenuEditInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }

        /// <summary>
        /// 父id
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "标题不能为空")]
        [StringLength(100, ErrorMessage = "标题长度不能超过100个字符")]
        public string Title { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        [Required(ErrorMessage = "菜单类型不能为空")]
        public string MenuType { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        public Guid? Module { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [Required(ErrorMessage = "路径不能为空")]
        [StringLength(200, ErrorMessage = "路径长度不能超过200个字符")]
        public string Path { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Required(ErrorMessage = "排序码不能为空")]
        [Range(0, 9999, ErrorMessage = "排序码范围0-9999")]
        public int SortCode { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [StringLength(100, ErrorMessage = "别名长度不能超过100个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 组件路径
        /// </summary>
        [StringLength(200, ErrorMessage = "组件路径长度不能超过200个字符")]
        public string Component { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [StringLength(100, ErrorMessage = "图标长度不能超过100个字符")]
        public string Icon { get; set; }

        /// <summary>
        /// 是否可见（YES/NO）
        /// </summary>
        [StringLength(10, ErrorMessage = "是否可见长度不能超过10个字符")]
        public string Visible { get; set; }

        /// <summary>
        /// 显示布局
        /// </summary>
        [StringLength(20, ErrorMessage = "显示布局长度不能超过20个字符")]
        public string DisplayLayout { get; set; }

        /// <summary>
        /// 页面缓存
        /// </summary>
        [StringLength(20, ErrorMessage = "页面缓存长度不能超过20个字符")]
        public string KeepLive { get; set; }

        /// <summary>
        /// 扩展信息（JSON格式）
        /// </summary>
        public string ExtJson { get; set; }
    }

    /// <summary>
    /// 更改菜单所属模块参数
    /// </summary>
    public class MenuChangeModuleInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        public Guid? Module { get; set; }
    }

    /// <summary>
    /// 菜单ID参数
    /// </summary>
    public class MenuIdInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }
    }

    /// <summary>
    /// 模块选择器参数
    /// </summary>
    public class MenuSelectorModuleInput
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }
    }

    /// <summary>
    /// 菜单树选择器参数
    /// </summary>
    public class MenuSelectorMenuInput
    {
        /// <summary>
        /// 模块ID
        /// </summary>
        public Guid? Module { get; set; }
    }

    /// <summary>
    /// 生成菜单参数
    /// </summary>
    public class GenerateMenuInput
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        [Required(ErrorMessage = "业务名称不能为空")]
        [StringLength(100, ErrorMessage = "业务名称长度不能超过100个字符")]
        public string BusinessName { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "标题不能为空")]
        [StringLength(100, ErrorMessage = "标题长度不能超过100个字符")]
        public string Title { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        public Guid? Module { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [Required(ErrorMessage = "路径不能为空")]
        [StringLength(200, ErrorMessage = "路径长度不能超过200个字符")]
        public string Path { get; set; }
    }
}
