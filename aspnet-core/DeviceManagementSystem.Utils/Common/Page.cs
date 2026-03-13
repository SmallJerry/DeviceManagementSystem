namespace DeviceManagementSystem.Utils.Common
{
    /// <summary>
    /// 分页类
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class Page<T> : IPage<T>
    {

        /// <summary>
        /// 分页数据
        /// </summary>
        public List<T> Records { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 每页显示条数
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public long Current { get; set; }


        /// <summary>
        /// 总页数
        /// </summary>
        public long Pages
        {
            get
            {
                if (Size == 0)
                {
                    return 0L;
                }

                long pages = Total / Size;
                if (Total % Size != 0)
                {
                    pages++;
                }

                return pages;
            }
        }



        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Page()
        {
            Records = new List<T>();
            Total = 0;
            Size = 10;
            Current = 1;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="current">当前页</param>
        /// <param name="size">每页大小</param>
        public Page(long current, long size) : this(current, size, 0) { }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="current">当前页</param>
        /// <param name="size">每页大小</param>
        /// <param name="total">总记录数</param>
        /// <param name="searchCount">是否进行 count 查询</param>
        public Page(long current, long size, long total)
        {
            Records = new List<T>();
            Total = 0;
            Size = 10;
            Current = 1;


            if (current > 1)
            {
                Current = current;
            }

            Size = size;
            Total = total;
        }


        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"Page{{current={Current}, size={Size}, total={Total}, pages={Pages}, records={Records?.Count ?? 0}}}";
        }
    }
}
