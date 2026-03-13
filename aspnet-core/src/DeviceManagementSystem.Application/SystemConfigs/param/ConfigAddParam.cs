using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeviceManagementSystem.SystemConfigs.param
{
    /// <summary>
    /// 配置添加参数
    /// </summary>
    public class ConfigAddParam
    {

        /// <summary>
        /// 配置键
        /// </summary>
        [Required(ErrorMessage = "configKey不能为空")]
        [JsonPropertyName("configKey")]
        public string ConfigKey { get; set; }

        /// <summary>
        /// 配置值
        /// </summary>
        //[Required(ErrorMessage = "configValue不能为空")]
        [JsonPropertyName("configValue")]
        public string ConfigValue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [JsonPropertyName("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        [Required(ErrorMessage = "sortCode不能为空")]
        [JsonPropertyName("sortCode")]
        public int SortCode { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        [JsonPropertyName("extJson")]
        public JsonDocument ExtJson { get; set; }

    }
}
