using Abp.Domain.Entities;
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
    /// 保养工单与项目关系表
    /// </summary>
    [Table("MaintenanceWorkOrderItemRelation")]
    public class MaintenanceWorkOrderItemRelation : Entity<Guid>
    {
        /// <summary>
        /// 保养工单Id
        /// </summary>
        public Guid WorkOrderId { get; set; }

        /// <summary>
        /// 保养项目Id
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// 项目名称（冗余）
        /// </summary>
        [MaxLength(200)]
        public string ItemName { get; set; }

        /// <summary>
        /// 保养方式
        /// </summary>
        [MaxLength(50)]
        public string MaintenanceMethod { get; set; }

        /// <summary>
        /// 保养内容及要求
        /// </summary>
        [MaxLength(1000)]
        public string Requirement { get; set; }

        /// <summary>
        /// 标准值/参考值
        /// </summary>
        [MaxLength(200)]
        public string StandardValue { get; set; }

        /// <summary>
        /// 上限值
        /// </summary>
        public decimal? UpperLimit { get; set; }

        /// <summary>
        /// 下限值
        /// </summary>
        public decimal? LowerLimit { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [MaxLength(20)]
        public string Unit { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 是否需要上传图片
        /// </summary>
        public bool NeedImage { get; set; }

        /// <summary>
        /// 是否需要填写数值
        /// </summary>
        public bool NeedValue { get; set; }

        /// <summary>
        /// 执行结果（OK/NG）
        /// </summary>
        [MaxLength(10)]
        public string ExecuteResult { get; set; }

        /// <summary>
        /// 执行数值（当NeedValue为true时填写）
        /// </summary>
        [MaxLength(50)]
        public string ExecuteValue { get; set; }

        /// <summary>
        /// 执行备注
        /// </summary>
        [MaxLength(500)]
        public string ExecuteRemark { get; set; }

        /// <summary>
        /// 执行图片（附件ID列表，JSON格式）
        /// </summary>
        public string ExecuteImages { get; set; }
    }
}
