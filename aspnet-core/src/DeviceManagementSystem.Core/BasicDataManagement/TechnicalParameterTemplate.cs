using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceManagementSystem.BasicDataManagement
{
    /// <summary>
    /// 技术参数模板
    /// </summary>
    [Table("TechnicalParameterTemplate")]
    public class TechnicalParameterTemplates : FullAuditedEntity<Guid>
    {

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }



        /// <summary>
        /// 对应的设备类型Id
        /// </summary>
        public Guid TypeId { get; set; } = Guid.Empty;


        /// <summary>
        /// 状态    是否启用
        /// </summary>
        public bool Status { get; set; } = true;


        /// <summary>
        /// 技术参数列表的Json字符串，格式为：[{"ParameterName":"参数1","ParameterValue":"参数描述1"},{"ParameterName":"参数2","ParameterValue":"参数描述2"}]
        /// </summary>
        public string ParameterJson { get; set; }


        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }


    }
}
