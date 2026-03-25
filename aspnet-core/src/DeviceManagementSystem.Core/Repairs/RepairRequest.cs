using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Repairs
{
    /// <summary>
    /// 维修申报表
    /// </summary>
    [Table("RepairRequest")]
    public class RepairRequests : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 申报编号（格式：RR+年月日+流水号）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string RequestNo { get; set; }

        /// <summary>
        /// 维修类型：0-维修，1-升级
        /// </summary>
        public int? RepairType { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 报修人ID
        /// </summary>
        [MaxLength(20)]
        public long RequesterId { get; set; }

        /// <summary>
        /// 报修人姓名
        /// </summary>
        [StringLength(50)]
        public string RequesterName { get; set; }

        /// <summary>
        /// 故障发现时间
        /// </summary>
        public DateTime FaultFoundTime { get; set; }

        /// <summary>
        /// 故障处理等级：0-一般，1-紧急，2-特急
        /// </summary>
        [MaxLength(20)]
        public int FaultLevel { get; set; }

        /// <summary>
        /// 期望完成时间
        /// </summary>
        public DateTime ExpectedCompleteTime { get; set; }

        /// <summary>
        /// 故障现象描述
        /// </summary>
        [StringLength(1000)]
        public string FaultDescription { get; set; }

        /// <summary>
        /// 申报状态：0-待派单，1-已派单，2-维修中，3-已完成，4-已取消
        /// </summary>
        [MaxLength(10)]
        public int? RequestStatus { get; set; } = 0;

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

    }
}
