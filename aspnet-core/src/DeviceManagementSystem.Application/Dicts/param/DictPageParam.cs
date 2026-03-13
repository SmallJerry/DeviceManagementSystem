using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Dicts.param
{
    /// <summary>
    /// 字典查询参数
    /// </summary>
    public class DictPageParam
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
        /// 父id
        /// </summary>
        public Guid? ParentId { get; set; }


        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }


        /// <summary>
        /// 配置键搜索关键字
        /// </summary>
        public string SearchKey { get; set; }


    }
}
