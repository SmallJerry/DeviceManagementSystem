using DeviceManagementSystem.DeviceInfos.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.DeviceInfos.Utils
{
    /// <summary>
    /// 设备相关JSON反序列化工具类
    /// </summary>
    public static class DeviceJsonHelper
    {
        /// <summary>
        /// 反序列化包含嵌套JSON字符串的DeviceEditInput对象
        /// 解决TechnicalParameters/CustomerRequirements字段是JSON字符串的问题
        /// </summary>
        /// <param name="jsonStr">原始JSON字符串</param>
        /// <returns>解析后的DeviceEditInput对象</returns>
        public static DeviceEditInput DeserializeDeviceEditInput(string jsonStr)
        {
            // 空值校验
            if (string.IsNullOrEmpty(jsonStr))
            {
                return null;
            }

            try
            {
                // 1. 先解析为JObject
                var jObject = JObject.Parse(jsonStr);

                // 2. 提取并移除需要特殊处理的字段
                var technicalParamsStr = jObject["TechnicalParameters"]?.ToString();
                var customerReqsStr = jObject["CustomerRequirements"]?.ToString();
                jObject.Remove("TechnicalParameters");
                jObject.Remove("CustomerRequirements");

                // 3. 转换为基础的DeviceEditInput对象
                var deviceEditInput = jObject.ToObject<DeviceEditInput>();
                if (deviceEditInput == null)
                {
                    return null;
                }

                // 4. 手动反序列化并赋值嵌套字段
                // 处理技术参数
                deviceEditInput.TechnicalParameters = DeserializeList<TechnicalParameterItem>(technicalParamsStr);
                // 处理客户要求
                deviceEditInput.CustomerRequirements = DeserializeList<CustomerRequirementItem>(customerReqsStr);

                return deviceEditInput;
            }
            catch (Exception ex)
            {
                // 可根据实际需求调整异常处理策略（日志/抛出自定义异常）
                throw new InvalidOperationException("反序列化DeviceEditInput失败", ex);
            }
        }

        /// <summary>
        /// 通用的List反序列化方法（处理空值/空数组）
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <param name="jsonStr">JSON字符串</param>
        /// <returns>解析后的List集合（空列表兜底）</returns>
        private static List<T> DeserializeList<T>(string jsonStr)
        {
            // 空值/空数组处理
            if (string.IsNullOrEmpty(jsonStr) || jsonStr.Trim() == "[]")
            {
                return new List<T>();
            }

            try
            {
                return JsonConvert.DeserializeObject<List<T>>(jsonStr);
            }
            catch
            {
                // 解析失败时返回空列表，避免业务中断
                return new List<T>();
            }
        }
    }
}
