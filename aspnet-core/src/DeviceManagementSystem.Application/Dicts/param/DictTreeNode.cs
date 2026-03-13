using DeviceManagementSystem.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Dicts.param
{
    /// <summary>
    /// 数据字典树节点
    /// </summary>
    public class DictTreeNode : Dict
    {

        /// <summary>
        /// 标题名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 子节点
        /// </summary>
        public List<DictTreeNode> Children { get; set; } = new List<DictTreeNode>();
    }
}
