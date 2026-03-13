using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Roles.Dto
{
    /// <summary>
    /// 角色授权资源参数
    /// </summary>
    public class RoleGrantResourceParam
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 授权资源信息
        /// </summary>
        [Required]
        public List<SysRoleGrantResource> GrantInfoList { get; set; }

        /// <summary>
        /// 角色授权资源类
        /// </summary>
        public class SysRoleGrantResource
        {
            /// <summary>
            /// 菜单ID
            /// </summary>
            [Required]
            public Guid MenuId { get; set; }

            /// <summary>
            /// 按钮ID集合
            /// </summary>
            [Required]
            public List<Guid> ButtonInfo { get; set; }
        }
    }
}
