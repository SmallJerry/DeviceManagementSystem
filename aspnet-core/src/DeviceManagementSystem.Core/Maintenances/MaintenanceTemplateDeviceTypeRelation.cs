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
    /// 保养模板与设备类型关系表
    /// </summary>
    [Table("MaintenanceTemplateDeviceTypeRelation")]
    public class MaintenanceTemplateDeviceTypeRelation : Entity<Guid>
    {
        /// <summary>
        /// 保养模板Id
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// 设备类型Id
        /// </summary>
        public Guid DeviceTypeId { get; set; }
    }
}
