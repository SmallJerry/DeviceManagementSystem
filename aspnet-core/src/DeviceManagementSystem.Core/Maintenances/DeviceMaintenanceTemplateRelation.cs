using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances
{
    /// <summary>
    /// 设备与保养模板关系表
    /// </summary>
    [Table("DeviceMaintenanceTemplateRelation")]
    public class DeviceMaintenanceTemplateRelation : Entity<Guid>
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid? DeviceId { get; set; }


        /// <summary>
        /// 申请ID（设备未通过审核时使用）
        /// </summary>
        public Guid? ChangeApplyId { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 首次保养日期
        /// </summary>
        public DateTime FirstMaintenanceDate { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
