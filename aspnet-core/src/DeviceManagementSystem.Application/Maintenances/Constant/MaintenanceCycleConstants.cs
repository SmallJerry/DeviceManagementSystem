using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances.Constant
{
    #region 周期常量

    /// <summary>
    /// 保养周期常量
    /// </summary>
    public static class MaintenanceCycleConstants
    {
        /// <summary>
        /// 月度：30天
        /// </summary>
        public const int MONTHLY_DAYS = 30;

        /// <summary>
        /// 季度：90天
        /// </summary>
        public const int QUARTERLY_DAYS = 90;

        /// <summary>
        /// 半年度：180天
        /// </summary>
        public const int HALF_YEARLY_DAYS = 180;

        /// <summary>
        /// 年度：365天
        /// </summary>
        public const int ANNUAL_DAYS = 365;

        /// <summary>
        /// 获取周期天数
        /// </summary>
        public static int GetCycleDays(string level)
        {
            return level switch
            {
                "月度" => MONTHLY_DAYS,
                "季度" => QUARTERLY_DAYS,
                "半年度" => HALF_YEARLY_DAYS,
                "年度" => ANNUAL_DAYS,
                _ => MONTHLY_DAYS
            };
        }

        /// <summary>
        /// 获取周期类型
        /// </summary>
        public static string GetCycleType(string level)
        {
            return level switch
            {
                "月度" => "月",
                "季度" => "季度",
                "半年度" => "半年",
                "年度" => "年",
                _ => "月"
            };
        }

        /// <summary>
        /// 所有保养等级
        /// </summary>
        public static readonly string[] AllLevels = { "月度", "季度", "半年度", "年度" };
    }

    #endregion
}
