namespace DeviceManagementSystem.SystemConfigs.param
{
    /// <summary>
    /// 配置查询参数
    /// </summary>
    public class ConfigPageParam
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
        /// 配置键搜索关键字
        /// </summary>
        public string SearchKey { get; set; }

    }
}
