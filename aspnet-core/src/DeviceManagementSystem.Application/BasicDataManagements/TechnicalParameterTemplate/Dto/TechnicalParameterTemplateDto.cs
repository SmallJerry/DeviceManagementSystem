using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.BasicDataManagements.TechnicalParameterTemplate.Dto
{
    /// <summary>
    /// 技术参数模板DTO
    /// </summary>
    public class TechnicalParameterTemplateDto
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 对应的设备类型Id
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// 设备类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 设备类型编码
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 技术参数列表的Json字符串
        /// </summary>
        public string ParameterJson { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }

    /// <summary>
    /// 技术参数模板分页查询参数
    /// </summary>
    public class TechnicalParameterTemplatePageInput
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid TypeId { get; set; } = Guid.Empty;

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
    /// 选择器查询参数
    /// </summary>
    public class TechnicalParameterTemplateSelectorInput
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid TypeId { get; set; } = Guid.Empty;
    }

    /// <summary>
    /// 添加技术参数模板参数
    /// </summary>
    public class TechnicalParameterTemplateAddInput
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        [Required(ErrorMessage = "模板名称不能为空")]
        [StringLength(100, ErrorMessage = "模板名称长度不能超过100个字符")]
        public string TemplateName { get; set; }

        /// <summary>
        /// 对应的设备类型Id
        /// </summary>
        [Required(ErrorMessage = "设备类型不能为空")]
        public Guid TypeId { get; set; } = Guid.Empty;

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// 技术参数列表的Json字符串
        /// </summary>
        [Required(ErrorMessage = "技术参数不能为空")]
        public string ParameterJson { get; set; }

        /// <summary>
        /// 验证参数JSON格式
        /// </summary>
        public bool IsValidParameterJson()
        {
            if (string.IsNullOrWhiteSpace(ParameterJson))
                return false;

            try
            {
                // 验证是否为有效的JSON数组格式
                var parameters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TechnicalParameter>>(ParameterJson);
                return parameters != null && parameters.Any();
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 单个技术参数项
    /// </summary>
    public class TechnicalParameter
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// 参数描述
        /// </summary>
        public string ParameterValue { get; set; }
    }

    /// <summary>
    /// 编辑技术参数模板参数
    /// </summary>
    public class TechnicalParameterTemplateEditInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        [Required(ErrorMessage = "模板名称不能为空")]
        [StringLength(100, ErrorMessage = "模板名称长度不能超过100个字符")]
        public string TemplateName { get; set; }

        /// <summary>
        /// 对应的设备类型Id
        /// </summary>
        [Required(ErrorMessage = "设备类型不能为空")]
        public Guid TypeId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// 技术参数列表的Json字符串
        /// </summary>
        [Required(ErrorMessage = "技术参数不能为空")]
        public string ParameterJson { get; set; }
    }

    /// <summary>
    /// 技术参数模板ID参数
    /// </summary>
    public class TechnicalParameterTemplateIdInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }
    }

    /// <summary>
    /// 选择器返回DTO
    /// </summary>
    public class TechnicalParameterTemplateSelectorDto
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// 设备类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 技术参数数量
        /// </summary>
        public int ParameterCount { get; set; }


        /// <summary>
        /// 技术参数列表（解析后的对象）
        /// </summary>
        public List<TechnicalParameter> Parameters { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
