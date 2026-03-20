using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Email.Dto
{
    public class EmailRequest
    {
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public IEnumerable<string> MailTo { get; set; }
        /// <summary>
        /// 邮件内容是否为html格式
        /// </summary>
        public bool IsBodyHtml { get; set; } = true;
    }


    /// <summary>
    /// 邮件携带附件请求
    /// </summary>
    public class EmailByFileStreamRequest : EmailRequest
    {
        public string FileName { get; set; }

        public Stream FileStream { get; set; }
    }



    public class EmailByMailCcAndFile : EmailNotificationRequest
    {
        public string FileName { get; set; }

        public Stream FileStream { get; set; }
    }



    public class EmailNotificationRequest
    {
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public IEnumerable<string> MailTo { get; set; }

        /// <summary>
        /// 抄送人
        /// </summary>
        public IEnumerable<string> MailCc { get; set; }

        /// <summary>
        /// 邮件内容是否为HTML格式
        /// </summary>
        public bool IsBodyHtml { get; set; } = true;
    }
}
