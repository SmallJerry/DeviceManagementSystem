using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Roles.Dto
{
    /// <summary>
    /// 角色分页查询参数
    /// </summary>
    public class RolePageParam
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
        /// 排序字段
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public string SortOrder { get; set; }

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
    }
}
