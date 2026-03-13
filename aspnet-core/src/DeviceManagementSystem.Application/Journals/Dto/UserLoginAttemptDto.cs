using Abp.Authorization.Users;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Journals.Dto
{
    /// <summary>
    /// 用户登录日志对象
    /// </summary>
    [AutoMapFrom(typeof(UserLoginAttempt))]
    public class UserLoginAttemptDto : UserLoginAttempt
    {
        /// <summary>
        /// 登录结果消息
        /// </summary>
        public string ResultMsg { get; set; }
    }
}
