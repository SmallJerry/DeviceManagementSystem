using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.BasicDataManagements.Type.Dto
{
    /// <summary>
    /// 类型DTO
    /// </summary>
    public class TypeDto
    {
        /// <summary>
        /// 类型Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 类型编码
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// SVG图标ID
        /// </summary>
        public Guid? Icon { get; set; }

        /// <summary>
        /// SVG图标地址
        /// </summary>
        public string SvgIconUrl { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 类型描述
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 拓展信息
        /// </summary>
        public string ExtendInfo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }
    }


    /// <summary>
    /// 类型分页查询参数
    /// </summary>
    public class TypePageInput
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 创建时间开始
        /// </summary>
        public DateTime? CreationTimeBegin { get; set; }

        /// <summary>
        /// 创建时间结束
        /// </summary>
        public DateTime? CreationTimeEnd { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// 排序方式：ASC/DESC
        /// </summary>
        public string SortOrder { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        public bool? Status { get; set; }



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
    /// 添加类型参数
    /// </summary>
    public class TypeAddInput
    {
        /// <summary>
        /// 类型编码
        /// </summary>
        [Required(ErrorMessage = "类型编码不能为空")]
        [StringLength(50, ErrorMessage = "类型编码长度不能超过50个字符")]
        public string TypeCode { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        [Required(ErrorMessage = "类型名称不能为空")]
        [StringLength(100, ErrorMessage = "类型名称长度不能超过100个字符")]
        public string TypeName { get; set; }

        /// <summary>
        /// SVG图标地址
        /// </summary>
        public string SvgIconUrl { get; set; }

        /// <summary>
        /// 类型描述
        /// </summary>
        [StringLength(500, ErrorMessage = "类型描述长度不能超过500个字符")]
        public string Remark { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Range(0, 9999, ErrorMessage = "排序码范围0-9999")]
        public int? SortCode { get; set; } = 1;

        /// <summary>
        /// 拓展信息
        /// </summary>
        public string ExtendInfo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } = true;
    }

    /// <summary>
    /// 编辑类型参数
    /// </summary>
    public class TypeEditInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        [Required(ErrorMessage = "类型名称不能为空")]
        [StringLength(100, ErrorMessage = "类型名称长度不能超过100个字符")]
        public string TypeName { get; set; }

        /// <summary>
        /// SVG图标地址
        /// </summary>
        public string SvgIconUrl { get; set; }

        /// <summary>
        /// 类型描述
        /// </summary>
        [StringLength(500, ErrorMessage = "类型描述长度不能超过500个字符")]
        public string Remark { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Range(0, 9999, ErrorMessage = "排序码范围0-9999")]
        public int? SortCode { get; set; }

        /// <summary>
        /// 拓展信息
        /// </summary>
        public string ExtendInfo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } = true;
    }

    /// <summary>
    /// 类型ID参数
    /// </summary>
    public class TypeIdInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }
    }

    /// <summary>
    /// 上传SVG图标参数
    /// </summary>
    public class UploadSvgIconInput
    {
        /// <summary>
        /// 类型ID
        /// </summary>
        public Guid? TypeId { get; set; }
    }
}
