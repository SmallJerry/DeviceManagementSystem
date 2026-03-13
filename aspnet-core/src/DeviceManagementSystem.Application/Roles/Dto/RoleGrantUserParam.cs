using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Roles.Dto
{
    /// <summary>
    /// 角色授权用户参数
    /// </summary>
    public class RoleGrantUserParam
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 授权用户信息
        /// </summary>
        [Required]
        public List<long> GrantInfoList { get; set; }

        /// <summary>
        /// 是否先清空授权信息
        /// </summary>
        public bool RemoveFirst { get; set; } = false;
    }
}
