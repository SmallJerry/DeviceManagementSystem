using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Users.Dto
{
    /// <summary>
    /// 树节点辅助类
    /// </summary>
    public class TreeNode<T>
    {
        /// <summary>
        /// 主键
        /// </summary>
        public T Id { get; set; }
        
        /// <summary>
        /// 父Id
        /// </summary>
        public T ParentId { get; set; }


        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public int Weight { get; set; }


        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }





        /// <summary>
        /// 额外数据
        /// </summary>
        public Dictionary<string, object> Extra { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentId"></param>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="weight"></param>
        public TreeNode(T id, T parentId, string name,string title, int weight)
        {
            Id = id;
            ParentId = parentId;
            Name = name;
            Title = title;
            Weight = weight;
        }
    }
}
