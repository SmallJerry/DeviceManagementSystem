using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Users.Dto
{
    /// <summary>
    /// 用户ID参数
    /// </summary>
    public class SysUserIdParam
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long Id { get; set; }
    }

    /// <summary>
    /// 用户拥有资源结果
    /// </summary>
    public class SysUserOwnResourceResult
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 授权信息列表
        /// </summary>
        public List<SysUserOwnResource> GrantInfoList { get; set; } = new List<SysUserOwnResource>();

        /// <summary>
        /// 用户拥有资源信息
        /// </summary>
        public class SysUserOwnResource
        {
            /// <summary>
            /// 菜单ID
            /// </summary>
            public Guid MenuId { get; set; }

            /// <summary>
            /// 按钮信息列表
            /// </summary>
            public List<string> ButtonInfo { get; set; } = new List<string>();
        }
    }

    /// <summary>
    /// 用户授权资源参数
    /// </summary>
    public class SysUserGrantResourceParam
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 授权信息列表
        /// </summary>
        public List<SysUserGrantResource> GrantInfoList { get; set; } = new List<SysUserGrantResource>();

        /// <summary>
        /// 用户授权资源信息
        /// </summary>
        public class SysUserGrantResource
        {
            /// <summary>
            /// 菜单ID
            /// </summary>
            public Guid MenuId { get; set; }

            /// <summary>
            /// 按钮信息列表
            /// </summary>
            public List<string> ButtonInfo { get; set; } = new List<string>();
        }
    }




    /// <summary>
    /// 用户授权角色参数
    /// </summary>
    public class SysUserGrantRoleParam
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 角色Id列表
        /// </summary>
        public List<long> RoleIdList { get; set; } = new List<long>();

 
    }




    /// <summary>
    /// 菜单树节点
    /// </summary>
    public class Tree<T> where T : class
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public T Id { get; set; }

        /// <summary>
        /// 父节点ID
        /// </summary>
        public T ParentId { get; set; }


        /// <summary>
        /// 颜色
        /// </summary>
        public T Color { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public T Path { get; set; }

        /// <summary>
        /// 模块Id
        /// </summary>
        public T Module { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public T MenuType { get; set; }



        /// <summary>
        /// 节点标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 组件
        /// </summary>
        public string Component { get; set; }


        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public string Visible { get; set; }


        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }


        /// <summary>
        /// 布局
        /// </summary>
        public string DisplayLayout { get; set; }


        /// <summary>
        /// 缓存
        /// </summary>
        public string KeepLive { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateTime { get; set; }

        /// <summary>
        /// 额外数据
        /// </summary>
        public string ExtJson { get; set; }


        /// <summary>
        /// 权重（排序）
        /// </summary>
        public int? Weight { get; set; }

        /// <summary>
        /// 元数据
        /// </summary>
        public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 子节点
        /// </summary>
        public List<Tree<T>> Children { get; set; } = new List<Tree<T>>();
    }

    /// <summary>
    /// 菜单节点额外信息
    /// </summary>
    public class MenuNodeExtra
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 组件
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public string MenuType { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        public Guid? Module { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool? Visible { get; set; }

        /// <summary>
        /// 显示布局
        /// </summary>
        public string DisplayLayout { get; set; }

        /// <summary>
        /// 页面缓存
        /// </summary>
        public string KeepLive { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public string ExtJson { get; set; }
    }
}
