using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceManagementSystem.FlowManagement
{
    /// <summary>
    /// 流程定义
    /// </summary>
    [Table("FlowDefinition")]
    public class FlowDefinitions : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// 状态：0-启用，1-停用
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// 说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否可取消：0-否，1-是
        /// </summary>
        public int Cancelable { get; set; } = 1;

        /// <summary>
        /// 关联表单ID
        /// </summary>
        public Guid? FormId { get; set; }

        /// <summary>
        /// 表单版本
        /// </summary>
        public int? FormVersion { get; set; }

        /// <summary>
        /// 发起人类型：0-全员，1-指定成员，2-均不可
        /// </summary>
        public int InitiatorType { get; set; } = 0;

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
        /// 表单控件配置JSON（作为预留字段）
        /// </summary>
        public string FlowWidgets { get; set; }

        /// <summary>
        /// 展现在工作台权限
        /// </summary>
        public bool? ShowInWorkbench { get; set; } = true;


        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }
    }
}