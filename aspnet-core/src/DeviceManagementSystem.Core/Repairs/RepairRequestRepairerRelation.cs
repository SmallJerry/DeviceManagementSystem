using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Repairs
{
    /// 维修申报与维修人员关系表
    /// </summary>
    [Table("RepairRequestRepairerRelation")]
    public class RepairRequestRepairerRelation : Entity<Guid>
    {
        /// <summary>
        /// 维修申报ID
        /// </summary>
        public Guid RepairRequestId { get; set; }

        /// <summary>
        /// 维修人员ID
        /// </summary>
        [MaxLength(20)]
        public long RepairerId { get; set; }

        /// <summary>
        /// 维修人员姓名
        /// </summary>
        [MaxLength(50)]
        public string RepairerName { get; set; }
    }
}
