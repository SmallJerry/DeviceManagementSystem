namespace DeviceManagementSystem.Utils.Common
{
    /// <summary>
    /// 分页排序项
    /// </summary>
    public class OrderItem
    {

        /// <summary>
        /// 排序列
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// 是否升序（true: 升序, false: 降序）
        /// </summary>
        public bool Ascending { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public OrderItem() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="column">排序列</param>
        /// <param name="ascending">是否升序</param>
        public OrderItem(string column, bool ascending = true)
        {
            Column = column;
            Ascending = ascending;
        }

        /// <summary>
        /// 升序排序
        /// </summary>
        /// <param name="column">排序列</param>
        /// <returns>OrderItem</returns>
        public static OrderItem Asc(string column) => new OrderItem(column, true);

        /// <summary>
        /// 降序排序
        /// </summary>
        /// <param name="column">排序列</param>
        /// <returns>OrderItem</returns>
        public static OrderItem Desc(string column) => new OrderItem(column, false);

        /// <summary>
        /// 升序排序
        /// </summary>
        /// <param name="column">排序列</param>
        /// <returns>OrderItem</returns>
        public static OrderItem AscendingOrder(string column) => new OrderItem(column, true);

        /// <summary>
        /// 降序排序
        /// </summary>
        /// <param name="column">排序列</param>
        /// <returns>OrderItem</returns>
        public static OrderItem DescendingOrder(string column) => new OrderItem(column, false);
    }
}
