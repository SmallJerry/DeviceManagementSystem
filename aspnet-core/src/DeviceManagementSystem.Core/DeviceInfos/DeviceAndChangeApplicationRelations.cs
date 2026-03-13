using Abp.Domain.Entities;
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
    /// 设备与变更申请关系表
    /// </summary>
    [Table("DeviceAndChangeApplicationRelation")]
    public class DeviceAndChangeApplicationRelations : Entity<Guid>
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 设备变更申请Id
        /// </summary>
        public Guid DeviceChangeApplicationId { get; set; }

        /// <summary>
        /// 变更类型（新增、编辑、删除）
        /// </summary>
        [StringLength(20)]
        public string ChangeType { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime SubmitTime { get; set; }

        /// <summary>
        /// 提交人ID
        /// </summary>
        public long SubmitterId { get; set; }

        /// <summary>
        /// 提交人姓名
        /// </summary>
        [StringLength(50)]
        public string SubmitterName { get; set; }


        /// <summary>
        /// 流程实例ID
        /// </summary>
        public Guid? FlowInstanceId { get; set; }

        /// <summary>
        /// 申请原因
        /// </summary>
        [StringLength(500)]
        public string ApplyReason { get; set; }
    }
}