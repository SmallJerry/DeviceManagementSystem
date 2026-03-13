using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.WorkFlows.FlowDefinition.Dto
{
    /// <summary>
    /// 流程定义DTO
    /// </summary>
    public class FlowDefinitionDto
    {
        /// <summary>
        /// 流程ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 状态 0-启用 1-停用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 流程说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否可取消
        /// </summary>
        public int Cancelable { get; set; }

        /// <summary>
        /// 展现在工作台权限
        /// </summary>
        public bool? ShowInWorkbench { get; set; }

        /// <summary>
        /// 关联的业务表单ID
        /// </summary>
        public Guid FormId { get; set; }

        /// <summary>
        /// 关联的业务表单名称
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// 关联的业务表单编码
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 发起人类型 0-全员 1-指定成员 2-均不可
        /// </summary>
        public int InitiatorType { get; set; }

        /// <summary>
        /// 发起人列表JSON
        /// </summary>
        public string FlowInitiators { get; set; }

        /// <summary>
        /// 节点配置JSON
        /// </summary>
        public string NodeConfig { get; set; }

        /// <summary>
        /// 流程权限配置JSON
        /// </summary>
        public string FlowPermission { get; set; }

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool Editable { get; set; } = true;

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
    /// 流程定义分页查询参数
    /// </summary>
    public class FlowDefinitionPageInput
    {
        /// <summary>
        /// 搜索关键字（流程名称）
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 关联表单ID
        /// </summary>
        public Guid? FormId { get; set; }

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
    /// 添加流程定义参数
    /// </summary>
    public class FlowDefinitionAddInput
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        [Required(ErrorMessage = "流程名称不能为空")]
        [StringLength(100, ErrorMessage = "流程名称长度不能超过100个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 流程说明
        /// </summary>
        [StringLength(200, ErrorMessage = "流程说明长度不能超过200个字符")]
        public string Remark { get; set; }

        /// <summary>
        /// 是否可取消 0-否 1-是
        /// </summary>
        public int Cancelable { get; set; } = 1;

        /// <summary>
        /// 展现在工作台权限
        /// </summary>
        public bool? ShowInWorkbench { get; set; } = true;

        /// <summary>
        /// 关联的业务表单ID
        /// </summary>
        [Required(ErrorMessage = "请选择关联表单")]
        public Guid FormId { get; set; }

        /// <summary>
        /// 发起人类型 0-全员 1-指定成员 2-均不可
        /// </summary>
        public int InitiatorType { get; set; } = 0;

        /// <summary>
        /// 发起人列表JSON
        /// </summary>
        public string FlowInitiators { get; set; }

        /// <summary>
        /// 节点配置JSON
        /// </summary>
        [Required(ErrorMessage = "节点配置不能为空")]
        public string NodeConfig { get; set; }

        /// <summary>
        /// 流程权限配置JSON
        /// </summary>
        [Required(ErrorMessage = "流程权限配置不能为空")]
        public string FlowPermission { get; set; }
    }

    /// <summary>
    /// 编辑流程定义参数
    /// </summary>
    public class FlowDefinitionEditInput
    {
        /// <summary>
        /// 流程ID
        /// </summary>
        [Required(ErrorMessage = "流程ID不能为空")]
        public Guid Id { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [Required(ErrorMessage = "流程名称不能为空")]
        [StringLength(100, ErrorMessage = "流程名称长度不能超过100个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 流程说明
        /// </summary>
        [StringLength(200, ErrorMessage = "流程说明长度不能超过200个字符")]
        public string Remark { get; set; }

        /// <summary>
        /// 是否可取消 0-否 1-是
        /// </summary>
        public int Cancelable { get; set; } = 1;

        /// <summary>
        /// 展现在工作台权限
        /// </summary>
        public bool? ShowInWorkbench { get; set; } = true;

        /// <summary>
        /// 关联的业务表单ID
        /// </summary>
        [Required(ErrorMessage = "请选择关联表单")]
        public Guid FormId { get; set; }

        /// <summary>
        /// 发起人类型 0-全员 1-指定成员 2-均不可
        /// </summary>
        public int InitiatorType { get; set; } = 0;

        /// <summary>
        /// 发起人列表JSON
        /// </summary>
        public string FlowInitiators { get; set; }

        /// <summary>
        /// 节点配置JSON
        /// </summary>
        [Required(ErrorMessage = "节点配置不能为空")]
        public string NodeConfig { get; set; }

        /// <summary>
        /// 流程权限配置JSON
        /// </summary>
        [Required(ErrorMessage = "流程权限配置不能为空")]
        public string FlowPermission { get; set; }

        /// <summary>
        /// 是否创建新版本
        /// </summary>
        public bool CreateNewVersion { get; set; } = false;
    }

    /// <summary>
    /// 流程定义ID参数
    /// </summary>
    public class FlowDefinitionIdInput
    {
        /// <summary>
        /// 流程ID
        /// </summary>
        [Required(ErrorMessage = "流程ID不能为空")]
        public Guid Id { get; set; }
    }

    /// <summary>
    /// 更改流程状态参数
    /// </summary>
    public class ChangeFlowStatusInput
    {
        /// <summary>
        /// 流程ID
        /// </summary>
        [Required(ErrorMessage = "流程ID不能为空")]
        public Guid Id { get; set; }

        /// <summary>
        /// 状态 0-启用 1-停用
        /// </summary>
        public int Status { get; set; }
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
        /// 描述
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

    /// <summary>
    /// 流程配置JSON参数
    /// </summary>
    public class FlowConfigJsonInput
    {
        /// <summary>
        /// 流程定义ID
        /// </summary>
        public Guid FlowDefId { get; set; }




    }

    /// <summary>
    /// 流程配置JSON返回DTO
    /// </summary>
    public class FlowConfigJsonDto
    {
        /// <summary>
        /// 流程定义
        /// </summary>
        public FlowDefinitionDto WorkFlowDef { get; set; }

        /// <summary>
        /// 节点配置
        /// </summary>
        public object NodeConfig { get; set; }

        /// <summary>
        /// 流程权限配置
        /// </summary>
        public object FlowPermission { get; set; }
    }

}
