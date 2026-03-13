using Abp.Application.Services.Dto;
using DeviceManagementSystem.Utils.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Organizations.Dto
{
    /// <summary>
    /// 组织DTO
    /// </summary>
    public class OrganizationDto
    {
        /// <summary>
        /// 组织Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 父id
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int SortCode { get; set; }


        /// <summary>
        /// 拓展字段
        /// </summary>
        public string ExtJson { get; set; }


    }






    /// <summary>
    /// 组织分页查询参数
    /// </summary>
    public class OrganizationPageInput
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentId { get; set; }

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
    /// 添加组织参数
    /// </summary>
    public class OrganizationAddInput
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        [Required(ErrorMessage = "组织名称不能为空")]
        [StringLength(50, ErrorMessage = "组织名称长度不能超过50个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 组织分类
        /// </summary>
        [Required(ErrorMessage = "组织分类不能为空")]
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
    /// 编辑组织参数
    /// </summary>
    public class OrganizationEditInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        [Required(ErrorMessage = "父级ID不能为空")]
        public Guid ParentId { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        [Required(ErrorMessage = "组织名称不能为空")]
        [StringLength(50, ErrorMessage = "组织名称长度不能超过50个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 组织分类
        /// </summary>
        [Required(ErrorMessage = "组织分类不能为空")]
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
    /// 组织ID参数
    /// </summary>
    public class OrganizationIdInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }
    }



    /// <summary>
    /// 组织选择器用户查询参数
    /// </summary>
    public class OrganizationSelectorUserInput
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



    /// <summary>
    /// 组织选择器组织列表查询参数
    /// </summary>
    public class OrganizationSelectorOrgListInput
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentId { get; set; }

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
