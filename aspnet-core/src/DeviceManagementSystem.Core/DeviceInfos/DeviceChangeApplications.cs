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
    /// 设备变更申请
    /// </summary>
    [Table("DeviceChangeApplication")]
    public class DeviceChangeApplications : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 变更类型（新增、编辑、删除）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string ChangeType { get; set; }

        /// <summary>
        /// 变更快照（变更前的数据JSON，新增时为空）
        /// </summary>
        public string Snapshot { get; set; }

        /// <summary>
        /// 变更后的数据JSON
        /// </summary>
        public string NewData { get; set; }


        /// <summary>
        /// 申请单状态（草稿、待审核、审核中、已通过、已拒绝、已撤销）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string ApplicationStatus { get; set; } = "草稿";

        /// <summary>
        /// 申请原因（编辑/删除时必填）
        /// </summary>
        [StringLength(500)]
        public string ApplyReason { get; set; }

        /// <summary>
        /// 提交人ID
        /// </summary>
        public long? SubmitterId { get; set; }

        /// <summary>
        /// 提交人姓名
        /// </summary>
        [StringLength(50)]
        public string SubmitterName { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime? SubmitTime { get; set; }
    }
}