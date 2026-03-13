using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Utils.Common
{
    /// <summary>
    /// 树节点
    /// </summary>
    public class TreeNode
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 父节点ID
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int SortCode { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();

        /// <summary>
        /// 额外数据
        /// </summary>
        public Dictionary<string, object> Extra { get; set; } = new Dictionary<string, object>();
    }
}
