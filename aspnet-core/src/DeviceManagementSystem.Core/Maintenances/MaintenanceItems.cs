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
    /// 保养项目表
    /// </summary>
    [Table("MaintenanceItem")]
    public class MaintenanceItems : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        [MaxLength(200)]
        public string ItemName { get; set; }

        /// <summary>
        /// 保养方式（目测、清洁、气吹、更换）
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

        /// <
        /// <summary>
        /// 上限值
        /// </summary>
        public double? UpperLimit { get; set; }

        /// <summary>
        /// 下限值
        /// </summary>
        public double? LowerLimit { get; set; }

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
        public bool? NeedImage { get; set; }

        /// <summary>
        /// 是否需要填写数值
        /// </summary>
        public bool? NeedValue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; set; }
    }
}
