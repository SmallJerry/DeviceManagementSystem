using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Roles.Dto
{
    /// <summary>
    /// 角色选择器参数
    /// </summary>
    public class RoleSelectorRoleParam
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int Current { get; set; } = 1;

        /// <summary>
        /// 每页条数
        /// </summary>
        public int Size { get; set; } = 10;

        /// <summary>
        /// 组织ID
        /// </summary>
        public Guid? OrgId { get; set; }

        /// <summary>
        /// 角色分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 数据范围信息
        /// </summary>
        public List<Guid> DataScopeList { get; set; }

        /// <summary>
        /// 是否排除超管
        /// </summary>
        public bool ExcludeSuperAdmin { get; set; } = false;
    }
}
