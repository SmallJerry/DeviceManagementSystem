using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeviceManagementSystem.SystemConfigs.param
{
    /// <summary>
    /// 配置批量更新参数
    /// </summary>
    public class ConfigBatchParam
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
        [JsonPropertyName("configValue")]
        [JsonConverter(typeof(StringOrObjectConverter))]
        public string ConfigValue { get; set; }

    }


    /// <summary>
    /// 自定义 JSON 转换器
    /// </summary>
    public class StringOrObjectConverter : JsonConverter<string>
    {
        /// <summary>
        /// 读取数据操作
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.Number:
                    return reader.GetDecimal().ToString();
                case JsonTokenType.True:
                    return "true";
                case JsonTokenType.False:
                    return "false";
                case JsonTokenType.Null:
                    return null;
                default:
                    // 如果是对象或数组，序列化为字符串
                    using (var document = JsonDocument.ParseValue(ref reader))
                    {
                        return document.RootElement.GetRawText();
                    }
            }
        }

        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
