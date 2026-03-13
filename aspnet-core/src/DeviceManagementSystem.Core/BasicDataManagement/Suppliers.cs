using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.BasicDataManagement
{
    /// <summary>
    /// 供应商
    /// </summary>
    [Table("Supplier")]
    public class Suppliers : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 供应商名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string SupplierName { get; set; }


        /// <summary>
        /// 供应商编码
        /// </summary>
        [StringLength(100)]
        public string SupplierCode { get; set; }


        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        [StringLength(100)]
        public string UnifiedSocialCreditCode { get; set; }


        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(100)]
        public string Address { get; set; }


        /// <summary>
        /// 联系人
        /// </summary>
        [StringLength(100)]
        public string ContactPerson { get; set; }


        /// <summary>
        /// 服务热线
        /// </summary>
        [StringLength(100)]
        public string ServiceHotline { get; set; }


        /// <summary>
        /// 供应商等级
        /// </summary>
        [StringLength(10)]
        public string SupplierLevel { get; set; }

        /// <summary>
        /// 状态 默认启用
        /// </summary>
        public bool Status { get; set; } = true;
    
    
        /// <summary>
        /// 拓展信息
        /// </summary>
        public string ExtendInfo { get; set; }


        /// <summary>
        /// 创建者
        /// </summary>
        [StringLength(20)]
        public string Creator { get; set; }

    }
}
