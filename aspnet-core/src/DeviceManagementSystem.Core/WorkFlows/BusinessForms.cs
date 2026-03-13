using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.WorkFlows
{
    /// <summary>
    /// 业务表单定义
    /// </summary>
    [Table("BusinessForm")]
    public class BusinessForms : FullAuditedEntity<Guid>
    {
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
        /// 状态 是否启用
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// 关联的流程定义ID
        /// </summary>
        public Guid? FlowDefId { get; set; }

        /// <summary>
        /// 关联的流程名称
        /// </summary>
        public string FlowName { get; set; }

        /// <summary>
        /// 表单配置JSON
        /// </summary>
        public string FormConfig { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }
    }
}
