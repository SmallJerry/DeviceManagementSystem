using Hangfire.Dashboard;
using System;
using System.Linq;

namespace DeviceManagementSystem.Web.Host.Startup
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // 检查 Basic 认证
            if (httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = httpContext.Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Basic ".Length).Trim();
                    var credentialBytes = Convert.FromBase64String(token);
                    var credentials = System.Text.Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                    var username = credentials[0];
                    var password = credentials[1];

                    // 静态用户名密码
                    if (username == "admin" && password == "test")
                    {
                        return true;
                    }
                }
            }

            // 未认证，发送 401 响应
            httpContext.Response.Headers["WWW-Authenticate"] = "Basic";
            httpContext.Response.StatusCode = 401;
            return false;
        }
    }
}
