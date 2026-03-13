using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.WorkFlows.BusinessForm.Dto
{
    /// <summary>
    /// 业务表单DTO
    /// </summary>
    public class BusinessFormDto
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// 表单编码
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 对应数据库表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 关联的流程定义ID
        /// </summary>
        public Guid? FlowDefId { get; set; }

        /// <summary>
        /// 关联的流程名称
        /// </summary>
        public string FlowName { get; set; }

        /// <summary>
        /// 表单描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 是否已绑定流程
        /// </summary>
        public bool IsBound => FlowDefId.HasValue;
    }

    /// <summary>
    /// 业务表单分页查询参数
    /// </summary>
    public class BusinessFormPageInput
    {
        /// <summary>
        /// 搜索关键字（表单名称/编码/表名）
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? Status { get; set; }

        /// <summary>
        /// 是否已绑定流程
        /// </summary>
        public bool? IsBound { get; set; }

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
    /// 添加业务表单参数
    /// </summary>
    public class BusinessFormAddInput
    {
        /// <summary>
        /// 表单名称
        /// </summary>
        [Required(ErrorMessage = "表单名称不能为空")]
        [StringLength(100, ErrorMessage = "表单名称长度不能超过100个字符")]
        public string FormName { get; set; }

        /// <summary>
        /// 表单编码
        /// </summary>
        [Required(ErrorMessage = "表单编码不能为空")]
        [StringLength(50, ErrorMessage = "表单编码长度不能超过50个字符")]
        [RegularExpression(@"^[A-Za-z0-9_]+$", ErrorMessage = "表单编码只能包含字母、数字和下划线")]
        public string FormCode { get; set; }

        /// <summary>
        /// 对应数据库表名
        /// </summary>
        [Required(ErrorMessage = "数据库表名不能为空")]
        [StringLength(100, ErrorMessage = "数据库表名长度不能超过100个字符")]
        public string TableName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// 表单描述
        /// </summary>
        [StringLength(200, ErrorMessage = "表单描述长度不能超过200个字符")]
        public string Description { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
        public string Remark { get; set; }
    }

    /// <summary>
    /// 编辑业务表单参数
    /// </summary>
    public class BusinessFormEditInput
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        [Required(ErrorMessage = "表单ID不能为空")]
        public Guid Id { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        [Required(ErrorMessage = "表单名称不能为空")]
        [StringLength(100, ErrorMessage = "表单名称长度不能超过100个字符")]
        public string FormName { get; set; }

        /// <summary>
        /// 表单编码
        /// </summary>
        [Required(ErrorMessage = "表单编码不能为空")]
        [StringLength(50, ErrorMessage = "表单编码长度不能超过50个字符")]
        [RegularExpression(@"^[A-Za-z0-9_]+$", ErrorMessage = "表单编码只能包含字母、数字和下划线")]
        public string FormCode { get; set; }

        /// <summary>
        /// 对应数据库表名
        /// </summary>
        [Required(ErrorMessage = "数据库表名不能为空")]
        [StringLength(100, ErrorMessage = "数据库表名长度不能超过100个字符")]
        public string TableName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// 表单描述
        /// </summary>
        [StringLength(200, ErrorMessage = "表单描述长度不能超过200个字符")]
        public string Description { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
        public string Remark { get; set; }
    }

    /// <summary>
    /// 业务表单ID参数
    /// </summary>
    public class BusinessFormIdInput
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        [Required(ErrorMessage = "表单ID不能为空")]
        public Guid Id { get; set; }
    }

    /// <summary>
    /// 批量删除业务表单参数
    /// </summary>
    public class BusinessFormBatchDeleteInput
    {
        /// <summary>
        /// 表单ID列表
        /// </summary>
        [Required(ErrorMessage = "请选择要删除的表单")]
        public List<Guid> Ids { get; set; }
    }

    /// <summary>
    /// 更改表单状态参数
    /// </summary>
    public class BusinessFormChangeStatusInput
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        [Required(ErrorMessage = "表单ID不能为空")]
        public Guid Id { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
    }

    /// <summary>
    /// 业务表单选择器DTO
    /// </summary>
    public class BusinessFormSelectorDto
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// 表单编码
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 对应数据库表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表单描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否已绑定流程
        /// </summary>
        public bool IsBound { get; set; }

        /// <summary>
        /// 绑定的流程ID
        /// </summary>
        public Guid? FlowDefId { get; set; }

        /// <summary>
        /// 绑定的流程名称
        /// </summary>
        public string FlowName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
    }

    /// <summary>
    /// 业务表单选择器查询参数
    /// </summary>
    public class BusinessFormSelectorInput
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 是否只显示未绑定的表单
        /// </summary>
        public bool OnlyUnbound { get; set; } = true;

        /// <summary>
        /// 排除当前流程ID（编辑时使用）
        /// </summary>
        public Guid? ExcludeFlowId { get; set; }
    }
}
