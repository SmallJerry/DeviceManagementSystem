using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Roles.Dto
{
    /// <summary>
    /// 角色授权权限参数
    /// </summary>
    public class RoleGrantPermissionParam
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 授权权限信息
        /// </summary>
        [Required]
        public List<SysRoleGrantPermission> GrantInfoList { get; set; }

        /// <summary>
        /// 角色授权权限类
        /// </summary>
        public class SysRoleGrantPermission
        {
            /// <summary>
            /// 接口地址
            /// </summary>
            [Required]
            public string ApiUrl { get; set; }

            /// <summary>
            /// 数据范围分类
            /// </summary>
            [Required]
            public string ScopeCategory { get; set; }

            /// <summary>
            /// 自定义范围组织ID集合
            /// </summary>
            [Required]
            public List<Guid> ScopeDefineOrgIdList { get; set; }
        }
    }
}
