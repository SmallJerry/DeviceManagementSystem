using DeviceManagementSystem.DeviceInfos.Dto;
using DeviceManagementSystem.Maintenances.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

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
                var monthlyMaintenanceStr = jObject["MonthlyMaintenance"]?.ToString();
                var quarterlyMaintenanceStr = jObject["QuarterlyMaintenance"]?.ToString();
                var halfYearlyMaintenanceStr = jObject["HalfYearlyMaintenance"]?.ToString();
                var annualMaintenanceStr = jObject["AnnualMaintenance"]?.ToString();

                jObject.Remove("TechnicalParameters");
                jObject.Remove("CustomerRequirements");
                jObject.Remove("MonthlyMaintenance");
                jObject.Remove("QuarterlyMaintenance");
                jObject.Remove("HalfYearlyMaintenance");
                jObject.Remove("AnnualMaintenance");

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

                // 处理保养计划
                deviceEditInput.MonthlyMaintenance = DeserializeMaintenancePlanData(monthlyMaintenanceStr);
                deviceEditInput.QuarterlyMaintenance = DeserializeMaintenancePlanData(quarterlyMaintenanceStr);
                deviceEditInput.HalfYearlyMaintenance = DeserializeMaintenancePlanData(halfYearlyMaintenanceStr);
                deviceEditInput.AnnualMaintenance = DeserializeMaintenancePlanData(annualMaintenanceStr);

                return deviceEditInput;
            }
            catch (Exception ex)
            {
                // 可根据实际需求调整异常处理策略（日志/抛出自定义异常）
                throw new InvalidOperationException("反序列化DeviceEditInput失败", ex);
            }
        }

        /// <summary>
        /// 序列化DeviceEditInput对象为JSON字符串
        /// </summary>
        /// <param name="input">DeviceEditInput对象</param>
        /// <returns>JSON字符串</returns>
        public static string SerializeDeviceEditInput(DeviceEditInput input)
        {
            if (input == null)
            {
                return null;
            }

            try
            {
                var jObject = JObject.FromObject(input);

                // 将List类型的字段序列化为JSON字符串
                if (input.TechnicalParameters != null)
                {
                    jObject["TechnicalParameters"] = JsonConvert.SerializeObject(input.TechnicalParameters);
                }

                if (input.CustomerRequirements != null)
                {
                    jObject["CustomerRequirements"] = JsonConvert.SerializeObject(input.CustomerRequirements);
                }

                // 保养计划数据已经是对象格式，不需要特殊处理
                // 但需要确保它们被正确包含

                return jObject.ToString(Formatting.None);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("序列化DeviceEditInput失败", ex);
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

        /// <summary>
        /// 反序列化保养计划数据
        /// </summary>
        /// <param name="jsonStr">JSON字符串</param>
        /// <returns>保养计划数据对象</returns>
        private static MaintenancePlanDto DeserializeMaintenancePlanData(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr) || jsonStr.Trim() == "null")
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<MaintenancePlanDto>(jsonStr);
            }
            catch
            {
                return null;
            }
        }
    }
}