using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Positions.Dto
{
    /// <summary>
    /// 添加职位参数
    /// </summary>
    public class PositionAddInput
    {
        /// <summary>
        /// 组织ID
        /// </summary>
        [Required(ErrorMessage = "组织ID不能为空")]
        public Guid OrgId { get; set; }

        /// <summary>
        /// 职位名称
        /// </summary>
        [Required(ErrorMessage = "职位名称不能为空")]
        [StringLength(50, ErrorMessage = "职位名称长度不能超过50个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 职位分类
        /// </summary>
        [Required(ErrorMessage = "职位分类不能为空")]
        public string Category { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Range(0, 9999, ErrorMessage = "排序码范围0-9999")]
        public int SortCode { get; set; } = 99;

        /// <summary>
        /// 扩展信息
        /// </summary>
        public string ExtJson { get; set; }
    }



    /// <summary>
    /// 职位分页查询参数
    /// </summary>
    public class PositionPageInput
    {
        /// <summary>
        /// 组织ID
        /// </summary>
        public Guid? OrgId { get; set; }

        /// <summary>
        /// 职位分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// 排序方式：ASC/DESC
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
    /// 编辑职位参数
    /// </summary>
    public class PositionEditInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        [Required(ErrorMessage = "组织ID不能为空")]
        public Guid OrgId { get; set; }

        /// <summary>
        /// 职位名称
        /// </summary>
        [Required(ErrorMessage = "职位名称不能为空")]
        [StringLength(50, ErrorMessage = "职位名称长度不能超过50个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 职位分类
        /// </summary>
        [Required(ErrorMessage = "职位分类不能为空")]
        public string Category { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Range(0, 9999, ErrorMessage = "排序码范围0-9999")]
        public int SortCode { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public string ExtJson { get; set; }
    }

    /// <summary>
    /// 职位ID参数
    /// </summary>
    public class PositionIdInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }
    }

    /// <summary>
    /// 职位选择器查询参数
    /// </summary>
    public class PositionSelectorPositionInput
    {
        /// <summary>
        /// 组织ID
        /// </summary>
        public Guid? OrgId { get; set; }

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }

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
