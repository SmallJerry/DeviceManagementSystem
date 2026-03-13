namespace DeviceManagementSystem.Utils.Common
{
    public class PageRequest
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public long Current { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public long Size { get; set; } = 10;

        /// <summary>
        /// 排序字段
        /// </summary>
        public string? SortField { get; set; }

        /// <summary>
        /// 是否降序
        /// </summary>
        public bool Desc { get; set; }

        /// <summary>
        /// 关键字搜索
        /// </summary>
        public string? SearchKey { get; set; }

        /// <summary>
        /// 转换为分页对象
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>分页对象</returns>
        public Page<T> ToPage<T>()
        {
            var page = new Page<T>(Current, Size);
            return page;
        }
    }
}
