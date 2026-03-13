using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.DeviceInfos
{
    /// <summary>
    /// 设备主表
    /// </summary>
    [Table("Device")]
    public class Devices : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 设备编码（管理编号）
        /// </summary>
        [Required]
        [StringLength(100)]
        public string DeviceCode { get; set; }

        /// <summary>
        /// 设备二维码
        /// </summary>
        [StringLength(500)]
        public string QrCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [Required]
        [StringLength(200)]
        public string DeviceName { get; set; }



        /// <summary>
        /// 设备等级 (如: A级, B级, C级)
        /// </summary>
        [StringLength(50)]
        public string DeviceLevel { get; set; }


        /// <summary>
        /// 是否为重点设备，默认值为false（非重点设备）。重点设备在系统中可能会有特殊的管理流程和权限控制。
        /// </summary>
        public bool IsKeyDevice { get; set; } = false;



        /// <summary>
        /// 规格型号
        /// </summary>
        [StringLength(200)]
        public string Specification { get; set; }

        /// <summary>
        /// 具体规格参数JSON列表（从技术参数模块中选择，可二次编辑）
        /// </summary>
        public string TechnicalParameters { get; set; }

        /// <summary>
        /// 客户要求指标JSON
        /// </summary>
        public string CustomerRequirements { get; set; }
        /// <summary>
        /// 物流单号
        /// </summary>
        [StringLength(100)]
        public string LogisticsNo { get; set; }

        /// <summary>
        /// 出厂编号
        /// </summary>
        [StringLength(100)]
        public string FactoryNo { get; set; }

        /// <summary>
        /// 生产厂商
        /// </summary>
        [StringLength(200)]
        public string Manufacturer { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ManufactureDate { get; set; }

        /// <summary>
        /// 申购单号
        /// </summary>
        [StringLength(100)]
        public string PurchaseNo { get; set; }

        /// <summary>
        /// 来源方式（采购/自制）
        /// </summary>
        [StringLength(50)]
        public string SourceType { get; set; }

        /// <summary>
        /// 存放地点
        /// </summary>
        [StringLength(500)]
        public string Location { get; set; }

        /// <summary>
        /// 设备状态（未验收/使用中/故障/闲置/报废等）
        /// </summary>
        [StringLength(50)]
        public string DeviceStatus { get; set; }

        /// <summary>
        /// 启用日期（用于初次保养发起节点判断依据）
        /// </summary>
        public DateTime? EnableDate { get; set; }

        /// <summary>
        /// 业务状态（草稿、提交、审核中、已确认）
        /// </summary>
        [StringLength(50)]
        public string BusinessStatus { get; set; } = "草稿";

        /// <summary>
        /// 创建者
        /// </summary>
        [StringLength(50)]
        public string Creator { get; set; }
    }
    
}
