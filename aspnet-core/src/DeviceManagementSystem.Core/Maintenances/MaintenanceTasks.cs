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
    [Table("MaintenanceTask")]
    public class MaintenanceTasks : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 工单编号
        /// </summary>
        [MaxLength(50)]
        public string TaskNo { get; set; }

        /// <summary>
        /// 工单名称
        /// </summary>
        [MaxLength(200)]
        public string TaskName { get; set; }

        /// <summary>
        /// 整合任务ID（相同任务组的标识）
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 保养计划ID
        /// </summary>
        public Guid PlanId { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        [MaxLength(20)]
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 工单状态（计划、待执行、执行中、已完成、已取消、已委派）
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "计划";

        /// <summary>
        /// 计划开始日期
        /// </summary>
        public DateTime PlanStartDate { get; set; }

        /// <summary>
        /// 计划完成日期
        /// </summary>
        public DateTime PlanEndDate { get; set; }

        /// <summary>
        /// 提醒日期（提前提醒日期）
        /// </summary>
        public DateTime? RemindDate { get; set; }

        /// <summary>
        /// 实际开始时间
        /// </summary>
        public DateTime? ActualStartTime { get; set; }

        /// <summary>
        /// 实际完成时间
        /// </summary>
        public DateTime? ActualEndTime { get; set; }

        /// <summary>
        /// 执行人ID（多个用逗号分隔）
        /// </summary>
        [MaxLength(500)]
        public string ExecutorIds { get; set; }

        /// <summary>
        /// 执行人姓名（多个用逗号分隔）
        /// </summary>
        [MaxLength(1000)]
        public string ExecutorNames { get; set; }

        /// <summary>
        /// 原执行人ID（委派前）
        /// </summary>
        [MaxLength(500)]
        public string OriginalExecutorIds { get; set; }

        /// <summary>
        /// 委派人ID
        /// </summary>
        public long? DelegatorId { get; set; }

        /// <summary>
        /// 委派人姓名
        /// </summary>
        [MaxLength(50)]
        public string DelegatorName { get; set; }

        /// <summary>
        /// 委派原因
        /// </summary>
        [MaxLength(500)]
        public string DelegateReason { get; set; }

        /// <summary>
        /// 执行小结
        /// </summary>
        [MaxLength(1000)]
        public string Summary { get; set; }

        /// <summary>
        /// 完成备注
        /// </summary>
        [MaxLength(500)]
        public string CompletionRemark { get; set; }

        /// <summary>
        /// 创建方式（自动/手动）
        /// </summary>
        [MaxLength(20)]
        public string CreateType { get; set; } = "自动";

        /// <summary>
        /// 是否已提醒
        /// </summary>
        public bool IsReminded { get; set; }



        /// <summary>
        /// 是否为合并工单
        /// </summary>
        public bool IsMergedTask { get; set; } = false;

        /// <summary>
        /// 合并的计划ID列表（逗号分隔）
        /// </summary>
        public string MergedPlanIds { get; set; }
    }

}
