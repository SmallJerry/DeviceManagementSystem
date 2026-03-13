using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Roles.Result
{
    /// <summary>
    /// 角色拥有的权限结果
    /// </summary>
    public class SysRoleOwnPermissionResult
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 已授权权限信息
        /// </summary>
        public List<SysRoleOwnPermission> GrantInfoList { get; set; }

        /// <summary>
        /// 角色拥有权限类
        /// </summary>
        public class SysRoleOwnPermission
        {
            /// <summary>
            /// 接口地址
            /// </summary>
            public string ApiUrl { get; set; }

            /// <summary>
            /// 数据范围分类
            /// </summary>
            public string ScopeCategory { get; set; }

            /// <summary>
            /// 自定义范围组织ID集合
            /// </summary>
            public List<Guid> ScopeDefineOrgIdList { get; set; }
        }
    }

}
