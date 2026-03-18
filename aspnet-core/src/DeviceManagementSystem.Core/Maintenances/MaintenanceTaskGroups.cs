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
    /// 保养任务整合组表
    /// </summary>
    [Table("MaintenanceTaskGroup")]
    public class MaintenanceTaskGroups : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 组编号
        /// </summary>
        [MaxLength(50)]
        public string GroupNo { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        [MaxLength(200)]
        public string GroupName { get; set; }

        /// <summary>
        /// 提醒日期
        /// </summary>
        public DateTime RemindDate { get; set; }

        /// <summary>
        /// 计划开始日期
        /// </summary>
        public DateTime PlanStartDate { get; set; }

        /// <summary>
        /// 计划完成日期
        /// </summary>
        public DateTime PlanEndDate { get; set; }

        /// <summary>
        /// 执行人ID（整合后统一执行人）
        /// </summary>
        [MaxLength(500)]
        public string ExecutorIds { get; set; }

        /// <summary>
        /// 执行人姓名
        /// </summary>
        [MaxLength(1000)]
        public string ExecutorNames { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [MaxLength(20)]
        public string Status { get; set; }
    }
}
