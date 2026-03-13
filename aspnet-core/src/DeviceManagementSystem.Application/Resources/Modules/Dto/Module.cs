using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Resources.Modules.Dto
{
    /// <summary>
    /// 模块实体
    /// </summary>
    public class Module : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }


        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }


        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }
    
        
        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; }


        /// <summary>
        /// 拓展信息
        /// </summary>
        public string ExtJson { get; set; }
    }
}
