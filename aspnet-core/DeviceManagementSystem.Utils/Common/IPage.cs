namespace DeviceManagementSystem.Utils.Common
{
    /// <summary>
    /// 分页接口
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public interface IPage<T>
    {
        /// <summary>
        /// 分页数据
        /// </summary>
        List<T> Records { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        long Total { get; set; }

        /// <summary>
        /// 每页显示条数
        /// </summary>
        long Size { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        long Current { get; set; }


        /// <summary>
        /// 获取总页数
        /// </summary>
        long Pages { get; }

    }
}
