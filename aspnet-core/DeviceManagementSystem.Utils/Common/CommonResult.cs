using System.Text.Json.Serialization;

namespace DeviceManagementSystem.Utils.Common
{
    public class CommonResult<T>
    {
        /// <summary>
        /// 成功状态码
        /// </summary>
        public const int CODE_SUCCESS = 200;

        /// <summary>
        /// 错误状态码
        /// </summary>
        public const int CODE_ERROR = 500;

        /// <summary>
        /// 状态码
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        [JsonPropertyName("msg")]
        public string? Message { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonIgnore]
        public bool IsSuccess => Code == CODE_SUCCESS;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CommonResult()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">提示信息</param>
        public CommonResult(int code, string? message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">提示信息</param>
        /// <param name="data">数据</param>
        public CommonResult(int code, string? message, T? data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// 创建成功响应（无数据）
        /// </summary>
        /// <returns>响应结果</returns>
        public static CommonResult<T> Success()
        {
            return new CommonResult<T>(CODE_SUCCESS, "操作成功", default);
        }

        /// <summary>
        /// 创建成功响应（自定义消息）
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <returns>响应结果</returns>
        public static CommonResult<T> Success(string message)
        {
            return new CommonResult<T>(CODE_SUCCESS, message, default);
        }

        /// <summary>
        /// 创建成功响应（有数据）
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>响应结果</returns>
        public static CommonResult<T> Success(T data)
        {
            return new CommonResult<T>(CODE_SUCCESS, "操作成功", data);
        }

        /// <summary>
        /// 创建成功响应（自定义消息和数据）
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="data">数据</param>
        /// <returns>响应结果</returns>
        public static CommonResult<T> Success(string message, T data)
        {
            return new CommonResult<T>(CODE_SUCCESS, message, data);
        }

        /// <summary>
        /// 创建错误响应（默认错误）
        /// </summary>
        /// <returns>响应结果</returns>
        public static CommonResult<T> Error()
        {
            return new CommonResult<T>(CODE_ERROR, "服务器异常", default);
        }

        /// <summary>
        /// 创建错误响应（自定义消息）
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <returns>响应结果</returns>
        public static CommonResult<T> Error(string message)
        {
            return new CommonResult<T>(CODE_ERROR, message, default);
        }

        /// <summary>
        /// 创建错误响应（自定义状态码和消息）
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">错误信息</param>
        /// <returns>响应结果</returns>
        public static CommonResult<T> Error(int code, string message)
        {
            return new CommonResult<T>(code, message, default);
        }

        /// <summary>
        /// 创建响应（完全自定义）
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">提示信息</param>
        /// <param name="data">数据</param>
        /// <returns>响应结果</returns>
        public static CommonResult<T> Create(int code, string message, T data)
        {
            return new CommonResult<T>(code, message, data);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>JSON格式字符串</returns>
        public override string ToString()
        {
            return $"{{ \"code\": {Code}, \"msg\": \"{Message}\", \"data\": {GetDataString()} }}";
        }

        private string GetDataString()
        {
            if (Data == null) return "null";

            // 如果是字符串类型，需要加引号
            if (Data is string)
            {
                return $"\"{Data}\"";
            }

            // 简单处理，复杂对象建议使用 System.Text.Json.JsonSerializer.Serialize(Data)
            return Data.ToString() ?? "null";
        }
    }

    /// <summary>
    /// 非泛型版本的CommonResult，方便使用
    /// </summary>
    public class CommonResult : CommonResult<object?>
    {
        public CommonResult()
        {
        }

        public CommonResult(int code, string? message) : base(code, message)
        {
        }

        public CommonResult(int code, string? message, object? data) : base(code, message, data)
        {
        }

        /// <summary>
        /// 创建成功响应（无数据）
        /// </summary>
        /// <returns>响应结果</returns>
        public static new CommonResult Ok()
        {
            return new CommonResult(CODE_SUCCESS, "操作成功", null);
        }

        /// <summary>
        /// 创建成功响应（自定义消息）
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <returns>响应结果</returns>
        public static new CommonResult Ok(string message)
        {
            return new CommonResult(CODE_SUCCESS, message, null);
        }

        /// <summary>
        /// 创建错误响应（默认错误）
        /// </summary>
        /// <returns>响应结果</returns>
        public static new CommonResult Error()
        {
            return new CommonResult(CODE_ERROR, "服务器异常", null);
        }

        /// <summary>
        /// 创建错误响应（自定义消息）
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <returns>响应结果</returns>
        public static new CommonResult Error(string message)
        {
            return new CommonResult(CODE_ERROR, message, null);
        }

        /// <summary>
        /// 创建错误响应（自定义状态码和消息）
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">错误信息</param>
        /// <returns>响应结果</returns>
        public static new CommonResult Error(int code, string message)
        {
            return new CommonResult(code, message, null);
        }

        /// <summary>
        /// 创建响应（完全自定义）
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">提示信息</param>
        /// <param name="data">数据</param>
        /// <returns>响应结果</returns>
        public static new CommonResult Create(int code, string message, object? data)
        {
            return new CommonResult(code, message, data);
        }

        /// <summary>
        /// 通用成功方法，支持泛型
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <returns>响应结果</returns>
        public static CommonResult<TData> Ok<TData>(TData data)
        {
            return CommonResult<TData>.Success(data);
        }

        /// <summary>
        /// 通用成功方法，支持泛型和自定义消息
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="message">提示信息</param>
        /// <param name="data">数据</param>
        /// <returns>响应结果</returns>
        public static CommonResult<TData> Ok<TData>(string message, TData data)
        {
            return CommonResult<TData>.Success(message, data);
        }

        /// <summary>
        /// 通用错误方法，支持泛型
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="message">错误信息</param>
        /// <returns>响应结果</returns>
        public static CommonResult<TData> Error<TData>(string message)
        {
            return CommonResult<TData>.Error(message);
        }

        /// <summary>
        /// 通用错误方法，支持泛型和自定义状态码
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="code">状态码</param>
        /// <param name="message">错误信息</param>
        /// <returns>响应结果</returns>
        public static CommonResult<TData> Error<TData>(int code, string message)
        {
            return CommonResult<TData>.Error(code, message);
        }
    }


}