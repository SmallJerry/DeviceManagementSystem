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
    /// 类型
    /// </summary>
    [Table("Type")]
    public class Types : FullAuditedEntity<Guid>
    {


        /// <summary>
        /// 类型编码 ： ZJ治具、 SB设备、
        /// </summary>
        public string TypeCode { get; set; }    



        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }


        /// <summary>
        /// SVG图标ID - 对应附件表中的Id
        /// </summary>
        public Guid? Icon { get; set; }



        /// <summary>
        /// 排序码
        /// </summary>
        [StringLength(20)]
        public int? SortCode { get; set; }



        /// <summary>
        /// 状态    是否启用
        /// </summary>
        public bool Status { get; set; } = true;


        /// <summary>
        /// 类型描述
        /// </summary>
        public string Remark { get; set; }


        /// <summary>
        /// 创建者
        /// </summary>
        [StringLength(20)]
        public string Creator { get; set; }
        }
}
