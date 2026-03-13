using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Roles.Result
{
    /// <summary>
    /// 角色授权资源树结果
    /// </summary>
    public class SysRoleGrantResourceTreeResult
    {
        /// <summary>
        /// 模块主键
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 模块图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 模块下菜单集合
        /// </summary>
        public List<SysRoleGrantResourceMenuResult> Menu { get; set; }

        /// <summary>
        /// 授权菜单类
        /// </summary>
        public class SysRoleGrantResourceMenuResult
        {
            /// <summary>
            /// 菜单主键
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// 父ID
            /// </summary>
            public Guid? ParentId { get; set; }

            /// <summary>
            /// 父名称
            /// </summary>
            public string ParentName { get; set; }

            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// 模块
            /// </summary>
            public Guid? Module { get; set; }

            /// <summary>
            /// 菜单下按钮集合
            /// </summary>
            public List<SysRoleGrantResourceButtonResult> Button { get; set; }

            /// <summary>
            /// 授权按钮类
            /// </summary>
            public class SysRoleGrantResourceButtonResult
            {
                /// <summary>
                /// 按钮主键
                /// </summary>
                public Guid Id { get; set; }

                /// <summary>
                /// 按钮标题
                /// </summary>
                public string Title { get; set; }
            }
        }
    }
}
