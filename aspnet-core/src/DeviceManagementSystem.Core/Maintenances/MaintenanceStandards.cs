using Abp.Domain.Entities.Auditing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances
{
    /// <summary>
    /// 保养标准表
    /// </summary>
    [Table("MaintenanceStandard")]
    public class MaintenanceStandards : FullAuditedEntity<Guid>
    {

        /// <summary>
        /// 点检项目名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string PointName { get; set; }

        /// <summary>
        /// 点检部位
        /// </summary>
        [MaxLength(100)]
        public string PointType { get; set; }

        /// <summary>
        /// 点检内容
        /// </summary>
        [MaxLength(500)]
        public string InspectionContent { get; set; }

        /// <summary>
        /// 点检方法（JSON数组存储）
        /// </summary>
        [MaxLength(500)]
        public string InspectionMethodJson { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 获取点检方法列表
        /// </summary>
        [NotMapped]
        public List<string> InspectionMethod
        {
            get
            {
                if (string.IsNullOrEmpty(InspectionMethodJson))
                    return new List<string>();
                return JsonConvert.DeserializeObject<List<string>>(InspectionMethodJson);
            }
            set
            {
                InspectionMethodJson = JsonConvert.SerializeObject(value);
            }
        }

    }
}
