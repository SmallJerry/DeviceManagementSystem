using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Journals.Dto
{
    /// <summary>
    /// 日志分页查询参数
    /// </summary>
    public class PagedLogResultRequestDto
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
        /// 日志名称关键词
        /// </summary>
        public string SearchKey { get; set; }


        /// <summary>
        /// 日志范围
        /// </summary>
        public DateTime?[] Time { get; set; }


        /// <summary>
        /// 日志结果
        /// </summary>
        public bool? Result { get; set; }

    }
}
