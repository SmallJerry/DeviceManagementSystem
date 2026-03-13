namespace DeviceManagementSystem.Utils.Common
{
    /// <summary>
    /// 提供静态工厂方法的帮助类
    /// </summary>
    public static class CommonResultHelper
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
        /// 成功响应（无数据）
        /// </summary>
        /// <returns>响应结果</returns>
        public static CommonResult Ok()
        {
            return new CommonResult(CODE_SUCCESS, "操作成功", null);
        }

        /// <summary>
        /// 成功响应（自定义消息）
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <returns>响应结果</returns>
        public static CommonResult Ok(string message)
        {
            return new CommonResult(CODE_SUCCESS, message, null);
        }

        /// <summary>
        /// 返回指定状态码
        /// </summary>
        /// <param name="code">状态码</param>
        /// <returns>响应结果</returns>
        public static CommonResult Code(int code)
        {
            return new CommonResult(code, null, null);
        }

        /// <summary>
        /// 成功响应（有数据）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <returns>响应结果</returns>
        public static CommonResult<T> Data<T>(T data)
        {
            return new CommonResult<T>(CODE_SUCCESS, "操作成功", data);
        }

        /// <summary>
        /// 成功响应（自定义消息和数据）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="message">提示信息</param>
        /// <param name="data">数据</param>
        /// <returns>响应结果</returns>
        public static CommonResult<T> Ok<T>(string message, T data)
        {
            return new CommonResult<T>(CODE_SUCCESS, message, data);
        }

        /// <summary>
        /// 错误响应（默认错误）
        /// </summary>
        /// <returns>响应结果</returns>
        public static CommonResult Error()
        {
            return new CommonResult(CODE_ERROR, "服务器异常", null);
        }

        /// <summary>
        /// 错误响应（自定义消息）
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <returns>响应结果</returns>
        public static CommonResult Error(string message)
        {
            return new CommonResult(CODE_ERROR, message, null);
        }

        /// <summary>
        /// 错误响应（自定义状态码和消息）
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">错误信息</param>
        /// <returns>响应结果</returns>
        public static CommonResult Error(int code, string message)
        {
            return new CommonResult(code, message, null);
        }

        /// <summary>
        /// 创建自定义响应（泛型）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="code">状态码</param>
        /// <param name="message">提示信息</param>
        /// <param name="data">数据</param>
        /// <returns>响应结果</returns>
        public static CommonResult<T> Create<T>(int code, string message, T data)
        {
            return new CommonResult<T>(code, message, data);
        }

        /// <summary>
        /// 创建自定义响应（非泛型）
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">提示信息</param>
        /// <param name="data">数据</param>
        /// <returns>响应结果</returns>
        public static CommonResult Create(int code, string message, object? data)
        {
            return new CommonResult(code, message, data);
        }


        /// <summary>
        /// 创建自定义响应（非泛型）
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">提示信息</param>
        /// <returns>响应结果</returns>
        public static CommonResult Create(int code, string message)
        {
            return new CommonResult(code, message);
        }
    }
}
