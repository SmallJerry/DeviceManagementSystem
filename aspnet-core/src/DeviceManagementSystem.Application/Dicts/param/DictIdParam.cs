using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Dicts.param
{
    /// <summary>
    /// 字典Id参数
    /// </summary>
    public class DictIdParam
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }
    }



    /// <summary>
    /// 字典列表参数
    /// </summary>
    public class DictListParam
    {
        /// <summary>
        /// 父ID
        /// </summary>
        public Guid? ParentId { get; set; }


        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }
    }


    /// <summary>
    /// 字典树参数
    /// </summary>
    public class DictTreeParam
    {
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }
    }
}
