using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeviceManagementSystem.Resources.Buttons.Dto
{
    /// <summary>
    /// 添加按钮参数
    /// </summary>
    public class ButtonAddInput
    {

        /// <summary>
        /// 父级ID
        /// </summary>
        [Required(ErrorMessage = "父级ID不能为空")]
        public string ParentId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "标题不能为空")]
        [StringLength(100, ErrorMessage = "标题长度不能超过100个字符")]
        public string Title { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Required(ErrorMessage = "编码不能为空")]
        [StringLength(50, ErrorMessage = "编码长度不能超过50个字符")]
        public string Code { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Required(ErrorMessage = "排序码不能为空")]
        [Range(0, 9999, ErrorMessage = "排序码范围0-9999")]
        public int SortCode { get; set; }

        /// <summary>
        /// 扩展信息（JSON格式）
        /// </summary>
        public string ExtJson { get; set; }


    }



    /// <summary>
    /// 按钮分页查询参数
    /// </summary>
    public class ButtonPageInput
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public string ParentId { get; set; }

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



    /// <summary>
    /// 编辑按钮参数
    /// </summary>
    public class ButtonEditInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public string Id { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        [Required(ErrorMessage = "父级ID不能为空")]
        public string ParentId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required(ErrorMessage = "标题不能为空")]
        [StringLength(100, ErrorMessage = "标题长度不能超过100个字符")]
        public string Title { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Required(ErrorMessage = "编码不能为空")]
        [StringLength(50, ErrorMessage = "编码长度不能超过50个字符")]
        public string Code { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Required(ErrorMessage = "排序码不能为空")]
        [Range(0, 9999, ErrorMessage = "排序码范围0-9999")]
        public int SortCode { get; set; }

        /// <summary>
        /// 扩展信息（JSON格式）
        /// </summary>
        public string ExtJson { get; set; }
    }

    /// <summary>
    /// 按钮ID参数
    /// </summary>
    public class ButtonIdInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public string Id { get; set; }
    }

    /// <summary>
    /// 生成按钮参数
    /// </summary>
    public class GenerateButtonInput
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        [Required(ErrorMessage = "菜单ID不能为空")]
        public string MenuId { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        [Required(ErrorMessage = "类名不能为空")]
        [StringLength(100, ErrorMessage = "类名长度不能超过100个字符")]
        public string ClassName { get; set; }

        /// <summary>
        /// 功能名称
        /// </summary>
        [Required(ErrorMessage = "功能名称不能为空")]
        [StringLength(100, ErrorMessage = "功能名称长度不能超过100个字符")]
        public string FunctionName { get; set; }
    }

    /// <summary>
    /// 关系扩展JSON按钮信息
    /// </summary>
    public class RelationExtJsonButtonInfo
    {
        /// <summary>
        /// 按钮信息列表
        /// </summary>
        public List<string> ButtonInfo { get; set; } = new List<string>();
    }


}
