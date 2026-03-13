using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Roles.Constants
{
    /// <summary>
    /// 角色相关常量
    /// </summary>
    public static class RoleConstant
    {
        /// <summary>
        /// 角色分类
        /// </summary>
        public static class Category
        {
            /// <summary>
            /// 全局
            /// </summary>
            public const string GLOBAL = "GLOBAL";

            /// <summary>
            /// 组织
            /// </summary>
            public const string ORG = "ORG";

            /// <summary>
            /// 验证角色分类
            /// </summary>
            public static void Validate(string value)
            {
                if (value != GLOBAL && value != ORG)
                {
                    throw new UserFriendlyException($"不支持的角色分类：{value}");
                }
            }
        }

        /// <summary>
        /// 数据范围分类
        /// </summary>
        public static class DataScopeCategory
        {
            /// <summary>
            /// 全部
            /// </summary>
            public const string SCOPE_ALL = "SCOPE_ALL";

            /// <summary>
            /// 仅自己
            /// </summary>
            public const string SCOPE_SELF = "SCOPE_SELF";

            /// <summary>
            /// 仅所属组织
            /// </summary>
            public const string SCOPE_ORG = "SCOPE_ORG";

            /// <summary>
            /// 所属组织及以下
            /// </summary>
            public const string SCOPE_ORG_CHILD = "SCOPE_ORG_CHILD";

            /// <summary>
            /// 自定义组织
            /// </summary>
            public const string SCOPE_ORG_DEFINE = "SCOPE_ORG_DEFINE";

            /// <summary>
            /// 验证数据范围分类
            /// </summary>
            public static void Validate(string value)
            {
                if (value != SCOPE_ALL && value != SCOPE_SELF && value != SCOPE_ORG &&
                    value != SCOPE_ORG_CHILD && value != SCOPE_ORG_DEFINE)
                {
                    throw new UserFriendlyException($"不支持的数据范围：{value}");
                }
            }
        }
    }




}
