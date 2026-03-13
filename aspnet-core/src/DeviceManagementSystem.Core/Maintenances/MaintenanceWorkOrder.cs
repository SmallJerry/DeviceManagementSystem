using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances
{
    /// <summary>
    /// 保养工单表
    /// </summary>
    [Table("MaintenanceWorkOrders")]
    public class MaintenanceWorkOrder : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 工单编号
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string WorkOrderCode { get; set; }

        /// <summary>
        /// 工单名称
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string WorkOrderName { get; set; }

        /// <summary>
        /// 保养等级（月度/季度/半年度/年度）
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 保养模板ID（如果来自模板）
        /// </summary>
        public Guid? TemplateId { get; set; }


        /// <summary>
        /// 计划执行日期
        /// </summary>
        public DateTime PlanExecuteDate { get; set; }

        /// <summary>
        /// 实际执行日期
        /// </summary>
        public DateTime? ActualExecuteDate { get; set; }

        /// <summary>
        /// 工单状态（待执行/执行中/已完成/已取消）
        /// </summary>
        [MaxLength(20)]
        public string WorkOrderStatus { get; set; } = "待执行";

        /// <summary>
        /// 执行班组（从设备保养人员获取）
        /// </summary>
        [MaxLength(100)]
        public string ExecuteTeam { get; set; }

        /// <summary>
        /// 执行人（用户ID列表，JSON格式）
        /// </summary>
        public string ExecutorIds { get; set; }


        /// <summary>
        /// 工单描述
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 创建来源（Plan-计划生成/Manual-手动创建）
        /// </summary>
        [MaxLength(20)]
        public string SourceType { get; set; }

        /// <summary>
        /// 来源ID（如果是计划生成，关联的保养计划ID）
        /// </summary>
        public Guid? SourceId { get; set; }
    }
}
