using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Roles.Dto
{
    /// <summary>
    /// 角色添加参数
    /// </summary>
    public class RoleAddParam
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        [Required]
        public string DisplayName { get; set; }


        /// <summary>
        /// 是否为默认角色
        /// </summary>
        public bool IsDefault { get; set; } = false;


        /// <summary>
        /// 分类
        /// </summary>
        [Required]
        public string Category { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public Guid? OrgId { get; set; }

        /// <summary>
        /// 扩展JSON
        /// </summary>
        public string ExtJson { get; set; }
    }
}
