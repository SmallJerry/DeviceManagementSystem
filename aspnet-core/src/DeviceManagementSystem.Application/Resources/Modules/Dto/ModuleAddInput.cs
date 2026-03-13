using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Resources.Modules.Dto
{
    /// <summary>
    /// 添加模块参数
    /// </summary>
    public class ModuleAddInput
    {

        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "标题不能为空")]
        [StringLength(100, ErrorMessage = "标题长度不能超过100个字符")]
        public string Title { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [Required(ErrorMessage = "图标不能为空")]
        [StringLength(100, ErrorMessage = "图标长度不能超过100个字符")]
        public string Icon { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        [Required(ErrorMessage = "颜色不能为空")]
        [StringLength(50, ErrorMessage = "颜色长度不能超过50个字符")]
        public string Color { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Range(0, 999, ErrorMessage = "排序码范围0-999")]
        public int? SortCode { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public string ExtJson { get; set; }

    }



    /// <summary>
    /// 编辑模块参数
    /// </summary>
    public class ModuleEditInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "标题不能为空")]
        [StringLength(100, ErrorMessage = "标题长度不能超过100个字符")]
        public string Title { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [Required(ErrorMessage = "图标不能为空")]
        [StringLength(100, ErrorMessage = "图标长度不能超过100个字符")]
        public string Icon { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        [Required(ErrorMessage = "颜色不能为空")]
        [StringLength(50, ErrorMessage = "颜色长度不能超过50个字符")]
        public string Color { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Range(0, 999, ErrorMessage = "排序码范围0-999")]
        public int? SortCode { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public string ExtJson { get; set; }
    }


    /// <summary>
    /// 模块ID参数
    /// </summary>
    public class ModuleIdInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }
    }

    /// <summary>
    /// 模块选择器项
    /// </summary>
    public class ModuleSelectorItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }



    /// <summary>
    /// 模块分页查询参数
    /// </summary>
    public class ModulePageInput
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }

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


}
