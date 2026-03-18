using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances
{
    /// <summary>
    /// 保养工单与执行人关系表
    /// </summary>
    [Table("MaintenanceTaskExecutorRelations")]
    public class MaintenanceTaskExecutorRelation : Entity<Guid>
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 执行人ID
        /// </summary>
        public long ExecutorId { get; set; }

        /// <summary>
        /// 执行人姓名
        /// </summary>
        public string ExecutorName { get; set; }

        /// <summary>
        /// 是否为主执行人
        /// </summary>
        public bool? IsPrimary { get; set; }

        /// <summary>
        /// 执行状态
        /// </summary>
        public string Status { get; set; }
    }
}
