using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Resources.Menus.Dto
{
    /// <summary>
    /// 树节点DTO
    /// </summary>
    public class TreeNodeDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        ///标题 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 键值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 父级主键
        /// </summary>
        public string ParentKey { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int SortCode { get; set; }

        /// <summary>
        /// 子对象
        /// </summary>
        public List<TreeNodeDto> Children { get; set; } = new List<TreeNodeDto>();

        /// <summary>
        /// 附加数据
        /// </summary>
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    }


    /// <summary>
    /// 模块选择器项
    /// </summary>
    public class ModuleSelectorItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }
}
