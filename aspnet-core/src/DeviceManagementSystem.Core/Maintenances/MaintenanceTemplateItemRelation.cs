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
    /// 保养模板与项目关系表
    /// </summary>
    [Table("MaintenanceTemplateItemRelation")]
    public class MaintenanceTemplateItemRelation : Entity<Guid>
    {
        /// <summary>
        /// 保养模板Id
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// 保养项目Id
        /// </summary>
        public Guid ItemId { get; set; }
    }
}
