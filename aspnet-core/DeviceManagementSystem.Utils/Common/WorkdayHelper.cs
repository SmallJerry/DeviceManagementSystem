using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Utils.Common
{
    /// <summary>
    /// 工作日判断工具类
    /// </summary>
    public static class WorkdayHelper
    {
       private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// 判断指定日期是否为工作日（同步方法）
        /// </summary>
        /// <param name="date">要判断的日期</param>
        /// <returns>true表示工作日，false表示非工作日（周末或节假日）</returns>
        public static bool IsWorkday(DateTime date)
        {
            try
            {
                string dateStr = date.ToString("yyyy-MM-dd");
                string url = $"http://timor.tech/api/holiday/info/{dateStr}";

                // 使用同步方式发送请求
                string result = _httpClient.GetStringAsync(url).GetAwaiter().GetResult();

                JObject obj = JObject.Parse(result);
                int? code = obj["code"]?.Value<int>();

                if (code.HasValue && code.Value == 0)
                {
                    // 获取type对象中的type字段
                    int dayType = obj["type"]?["type"]?.Value<int>() ?? 1; // 默认1（非工作日）以防字段缺失

                    // 返回true表示工作日（type=0），false表示非工作日（type=1或2）
                    return dayType == 0;
                }
            }
            catch (Exception ex)
            {
                return date.DayOfWeek switch
                {
                    DayOfWeek.Saturday => false,  // 周六为非工作日
                    DayOfWeek.Sunday => false,    // 周日为非工作日
                    _ => true                     // 周一至周五为工作日
                };
            }

            return true;
        }

        /// <summary>
        /// 判断指定日期是否为工作日（异步方法）
        /// </summary>
        /// <param name="date">要判断的日期</param>
        /// <returns>true表示工作日，false表示非工作日（周末或节假日）</returns>
        public static async Task<bool> IsWorkdayAsync(DateTime date)
        {
            try
            {
                string dateStr = date.ToString("yyyy-MM-dd");
                string url = $"http://timor.tech/api/holiday/info/{dateStr}";

                string result = await _httpClient.GetStringAsync(url);

                JObject obj = JObject.Parse(result);
                int? code = obj["code"]?.Value<int>();

                if (code.HasValue && code.Value == 0)
                {
                    int dayType = obj["type"]?["type"]?.Value<int>() ?? 1;
                    return dayType == 0;
                }
            }
            catch (Exception ex)
            {
                return date.DayOfWeek switch
                {
                    DayOfWeek.Saturday => false,  // 周六为非工作日
                    DayOfWeek.Sunday => false,    // 周日为非工作日
                    _ => true                     // 周一至周五为工作日
                };
            }

            return true;
        }
    }
}
