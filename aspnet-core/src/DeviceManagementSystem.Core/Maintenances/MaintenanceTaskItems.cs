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
    /// 保养工单项目执行记录表
    /// </summary>
    [Table("MaintenanceTaskItem")]
    public class MaintenanceTaskItems : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 项目ID（保养项目ID）
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [MaxLength(100)]
        public string ItemName { get; set; }

        /// <summary>
        /// 保养方式
        /// </summary>
        [MaxLength(50)]
        public string MaintenanceMethod { get; set; }

        /// <summary>
        /// 保养内容及要求
        /// </summary>
        [MaxLength(500)]
        public string Content { get; set; }

        /// <summary>
        /// 标准值/参考值
        /// </summary>
        [MaxLength(200)]
        public string StandardValue { get; set; }

        /// <summary>
        /// 执行结果（合格/不合格/未执行）
        /// </summary>
        [MaxLength(20)]
        public string Result { get; set; }

        /// <summary>
        /// 实际测量值
        /// </summary>
        [MaxLength(200)]
        public string ActualValue { get; set; }

        /// <summary>
        /// 执行备注
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortOrder { get; set; }



        /// <summary>
        /// 来源计划ID（用于区分项目来自哪个保养计划）
        /// </summary>
        public Guid? SourcePlanId { get; set; }

        /// <summary>
        /// 来源保养等级（月度/季度/半年度/年度）
        /// </summary>
        public string SourceMaintenanceLevel { get; set; }
    }
}
