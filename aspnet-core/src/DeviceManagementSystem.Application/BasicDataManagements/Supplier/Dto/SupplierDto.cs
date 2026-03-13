using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.BasicDataManagements.Supplier.Dto
{
    /// <summary>
    /// 供应商DTO
    /// </summary>
    public class SupplierDto
    {
        /// <summary>
        /// 供应商ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 供应商编码
        /// </summary>
        public string SupplierCode { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string UnifiedSocialCreditCode { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactPerson { get; set; }

        /// <summary>
        /// 服务热线
        /// </summary>
        public string ServiceHotline { get; set; }

        /// <summary>
        /// 供应商等级
        /// </summary>
        public string SupplierLevel { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

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
    /// 供应商分页查询参数
    /// </summary>
    public class SupplierPageInput
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 供应商等级
        /// </summary>
        public string SupplierLevel { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? Status { get; set; }

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
        /// 当前页
        /// </summary>
        public int Current { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public int Size { get; set; } = 10;
    }

    /// <summary>
    /// 添加供应商参数
    /// </summary>
    public class SupplierAddInput
    {
        /// <summary>
        /// 供应商名称
        /// </summary>
        [Required(ErrorMessage = "供应商名称不能为空")]
        [StringLength(100, ErrorMessage = "供应商名称长度不能超过100个字符")]
        public string SupplierName { get; set; }

        /// <summary>
        /// 供应商编码
        /// </summary>
        [StringLength(100, ErrorMessage = "供应商编码长度不能超过100个字符")]
        public string SupplierCode { get; set; }

        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        [Required(ErrorMessage = "统一社会信用代码不能为空")]
        [StringLength(18, MinimumLength = 18, ErrorMessage = "统一社会信用代码必须是18位")]
        [RegularExpression(@"^[0-9A-Z]{18}$", ErrorMessage = "统一社会信用代码格式不正确")]
        public string UnifiedSocialCreditCode { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Required(ErrorMessage = "地址不能为空")]
        [StringLength(200, ErrorMessage = "地址长度不能超过200个字符")]
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [Required(ErrorMessage = "联系人不能为空")]
        [StringLength(50, ErrorMessage = "联系人长度不能超过50个字符")]
        public string ContactPerson { get; set; }

        /// <summary>
        /// 服务热线
        /// </summary>
        [Required(ErrorMessage = "服务热线不能为空")]
        [StringLength(20, ErrorMessage = "服务热线长度不能超过20个字符")]
        [RegularExpression(@"^(\d{3,4}-)?\d{7,8}$|^1[3-9]\d{9}$", ErrorMessage = "服务热线格式不正确")]
        public string ServiceHotline { get; set; }

        /// <summary>
        /// 供应商等级
        /// </summary>
        [Required(ErrorMessage = "供应商等级不能为空")]
        [RegularExpression(@"^[ABC]$", ErrorMessage = "供应商等级只能是A、B、C")]
        public string SupplierLevel { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// 拓展信息
        /// </summary>
        [StringLength(1000, ErrorMessage = "拓展信息长度不能超过1000个字符")]
        public string ExtendInfo { get; set; }
    }

    /// <summary>
    /// 编辑供应商参数
    /// </summary>
    public class SupplierEditInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [Required(ErrorMessage = "供应商名称不能为空")]
        [StringLength(100, ErrorMessage = "供应商名称长度不能超过100个字符")]
        public string SupplierName { get; set; }

        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        [Required(ErrorMessage = "统一社会信用代码不能为空")]
        [StringLength(18, MinimumLength = 18, ErrorMessage = "统一社会信用代码必须是18位")]
        [RegularExpression(@"^[0-9A-Z]{18}$", ErrorMessage = "统一社会信用代码格式不正确")]
        public string UnifiedSocialCreditCode { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Required(ErrorMessage = "地址不能为空")]
        [StringLength(200, ErrorMessage = "地址长度不能超过200个字符")]
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [Required(ErrorMessage = "联系人不能为空")]
        [StringLength(50, ErrorMessage = "联系人长度不能超过50个字符")]
        public string ContactPerson { get; set; }

        /// <summary>
        /// 服务热线
        /// </summary>
        [Required(ErrorMessage = "服务热线不能为空")]
        [StringLength(20, ErrorMessage = "服务热线长度不能超过20个字符")]
        [RegularExpression(@"^(\d{3,4}-)?\d{7,8}$|^1[3-9]\d{9}$", ErrorMessage = "服务热线格式不正确")]
        public string ServiceHotline { get; set; }

        /// <summary>
        /// 供应商等级
        /// </summary>
        [Required(ErrorMessage = "供应商等级不能为空")]
        [RegularExpression(@"^[ABC]$", ErrorMessage = "供应商等级只能是A、B、C")]
        public string SupplierLevel { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// 拓展信息
        /// </summary>
        [StringLength(1000, ErrorMessage = "拓展信息长度不能超过1000个字符")]
        public string ExtendInfo { get; set; }
    }

    /// <summary>
    /// 供应商ID参数
    /// </summary>
    public class SupplierIdInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }
    }
}
