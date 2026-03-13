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
    /// 保养工单执行历史
    /// </summary>
    [Table("MaintenanceWorkOrderHistories")]
    public class MaintenanceWorkOrderHistory : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 工单Id
        /// </summary>
        public Guid WorkOrderId { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [MaxLength(200)]
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        [MaxLength(50)]
        public string DeviceCode { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        [MaxLength(20)]
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 执行日期
        /// </summary>
        public DateTime ExecuteDate { get; set; }

        /// <summary>
        /// 执行人
        /// </summary>
        [MaxLength(50)]
        public string Executor { get; set; }

        /// <summary>
        /// 执行人ID
        /// </summary>
        public long? ExecutorId { get; set; }

        /// <summary>
        /// 执行结果（全部OK/部分NG）
        /// </summary>
        [MaxLength(20)]
        public string ExecuteResult { get; set; }

        /// <summary>
        /// 执行备注
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 项目执行详情（JSON格式，记录每个项目的执行情况）
        /// </summary>
        public string ItemDetails { get; set; }
    }
}
