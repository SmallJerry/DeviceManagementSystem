using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.FileInfos
{
    /// <summary>
    /// 业务附件关系表
    /// </summary>
    [Table("BusinessAttachmentRelation")]
    public class BusinessAttachmentRelations : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 附件ID
        /// </summary>
        [Required]
        public Guid AttachmentId { get; set; }

        /// <summary>
        /// 业务ID
        /// </summary>
        [Required]
        public Guid BusinessId { get; set; }

        /// <summary>
        /// 业务类型(如:Device,Type等)
        /// </summary>
        [Required]
        [StringLength(100)]
        public string BusinessType { get; set; }


        /// <summary>
        /// 附件分类(如:TechnicalDoc, Manual, Drawing等)
        /// </summary>
        [StringLength(100)]
        public string Category { get; set; }

    }
}
