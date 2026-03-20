using DeviceManagementSystem.Email.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Email
{
    public interface IEmailAppService
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="data">邮件数据</param>
        public Task SendEmailAsync(EmailRequest data);


        /// <summary>
        /// 发送脚本更新维护邮件通知,携带抄送人
        /// </summary>
        /// <param name="data">邮件数据</param>
        public Task SendEmailForNotificationAsync(EmailNotificationRequest data);

        /// <summary>
        /// 发送邮件携带附件
        /// </summary>
        /// <param name="data">邮件数据</param>
        public Task SendEmailByFileAsync(EmailByFileStreamRequest data);
    }
}
