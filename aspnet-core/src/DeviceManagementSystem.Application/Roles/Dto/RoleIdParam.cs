using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Roles.Dto
{
    /// <summary>
    /// 角色ID参数
    /// </summary>
    public class RoleIdParam
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        public int Id { get; set; }
    }
}
