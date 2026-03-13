using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Roles.Result
{
    /// <summary>
    /// 角色拥有的资源结果
    /// </summary>
    public class RoleOwnResourceResult
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 已授权资源信息
        /// </summary>
        public List<SysRoleOwnResource> GrantInfoList { get; set; }

        /// <summary>
        /// 角色拥有资源类
        /// </summary>
        public class SysRoleOwnResource
        {
            /// <summary>
            /// 菜单ID
            /// </summary>
            public Guid MenuId { get; set; }

            /// <summary>
            /// 按钮ID集合
            /// </summary>
            public List<Guid> ButtonInfo { get; set; }
        }
    }
}
