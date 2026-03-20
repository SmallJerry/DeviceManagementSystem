using Abp.Authorization;
using Abp.UI;
using DeviceManagementSystem;
using DeviceManagementSystem.Email;
using DeviceManagementSystem.Email.Dto;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
namespace DeviceManagementSystem.Email
{
    /// <summary>
    /// 邮件服务配置
    /// </summary>
    [AbpAllowAnonymous]
    public class EmailAppService:DeviceManagementSystemAppServiceBase, IEmailAppService
    {
        private readonly CustomEmailSender _iEmailSender;

        public EmailAppService(CustomEmailSender iEmailSender)
        {
            _iEmailSender = iEmailSender;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="data">邮件数据</param>
        public async Task SendEmailAsync(EmailRequest data)
        {
                MailMessage mailMessage = new MailMessage();
                mailMessage.Subject = data.Subject; //主题
                mailMessage.Body = data.Body; //内容
                mailMessage.BodyEncoding = Encoding.UTF8; //正文编码
                mailMessage.IsBodyHtml = data.IsBodyHtml; //设置为HTML格式
                mailMessage.Priority = MailPriority.Normal; //优先级
               
                // 每次发送的收件人数量
                int batchSize = 30;
                // 将收件人分批发送
                foreach (var address in data.MailTo)
                {
                    mailMessage.To.Add(new MailAddress(address));
                    // 达到批次数量时发送邮件并清空收件人列表
                    if (mailMessage.To.Count >= batchSize)
                    {
                        try
                        {
                            await Task.Run(() => _iEmailSender.SendAsync(mailMessage)); // 异步发送邮件
                        }
                        catch (SmtpException ex)
                        {
                            throw new UserFriendlyException("发送失败", ex.StackTrace);
                        }

                        // 清空收件人列表，准备下一批发送
                        mailMessage.To.Clear();
                    }
                }
                // 发送剩余的邮件
                if (mailMessage.To.Count > 0)
                {
                    try
                    {
                        await Task.Run(() => _iEmailSender.SendAsync(mailMessage)); // 异步发送邮件
                    }
                    catch (SmtpException ex)
                    {
                        throw new UserFriendlyException("发送失败", ex.StackTrace);
                    }
                }
            }



        /// <summary>
        /// 发送邮件携带附件
        /// </summary>
        /// <param name="data">邮件数据</param>
        public async Task SendEmailByFileAsync(EmailByFileStreamRequest data)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = data.Subject; //主题
            mailMessage.Body = data.Body; //内容
            mailMessage.BodyEncoding = Encoding.UTF8; //正文编码
            mailMessage.IsBodyHtml = data.IsBodyHtml; //设置为HTML格式
            mailMessage.Priority = MailPriority.Normal; //优先级
                                                        // 每次发送的收件人数量
            //携带附件
            if (data.FileStream != null)
            {
                var attachment = new System.Net.Mail.Attachment(data.FileStream, data.FileName);
                mailMessage.Attachments.Add(attachment);
            }

            int batchSize = 30;
            // 将收件人分批发送
            foreach (var address in data.MailTo)
            {
                mailMessage.To.Add(new MailAddress(address));
                // 达到批次数量时发送邮件并清空收件人列表
                if (mailMessage.To.Count >= batchSize)
                {
                    try
                    {
                        await Task.Run(() => _iEmailSender.SendAsync(mailMessage)); // 异步发送邮件
                    }
                    catch (SmtpException ex)
                    {
                        throw new UserFriendlyException("发送失败", ex.Message);
                    }

                    // 清空收件人列表，准备下一批发送
                    mailMessage.To.Clear();
                }
            }
            // 发送剩余的邮件
            if (mailMessage.To.Count > 0)
            {
                try
                {
                    await Task.Run(() => _iEmailSender.SendAsync(mailMessage)); // 异步发送邮件
                }
                catch (SmtpException ex)
                {
                    throw new UserFriendlyException("发送失败", ex.Message);
                }
            }
        }



        /// <summary>
        /// 发送邮件携带附件和抄送人
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendEmailByFileAndMailCc(EmailByMailCcAndFile data)
        {
            // 发送邮件设置
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = data.Subject; //主题
            mailMessage.Body = data.Body; //内容
            mailMessage.BodyEncoding = Encoding.UTF8; //正文编码
            mailMessage.IsBodyHtml = data.IsBodyHtml; //设置为HTML格式
            mailMessage.Priority = MailPriority.Normal; //优先级
            //携带附件
            if (data.FileStream != null)
            {
                var attachment = new System.Net.Mail.Attachment(data.FileStream, data.FileName);
                mailMessage.Attachments.Add(attachment);
            }
            //添加抄送人
            int batchSize = 30;
            mailMessage.To.Clear();
            mailMessage.CC.Clear(); // 清空抄送人列表
           if(data.MailCc != null)
            {
                foreach (var address in data.MailCc)
                {
                    mailMessage.CC.Add(address);
                }
            }
            foreach (var address in data.MailTo)
            {
                mailMessage.To.Add(new MailAddress(address));
                // 达到批次数量时发送邮件并清空抄送人列表
                if (mailMessage.To.Count >= batchSize)
                {
                    try
                    {
                        await Task.Run(() => _iEmailSender.SendAsync(mailMessage)); // 异步发送邮件
                    }
                    catch (SmtpException ex)
                    {
                        throw new UserFriendlyException("发送失败", ex.Message);
                    }

                    // 清空抄送人列表，准备下一批发送
                    mailMessage.To.Clear();
                }
            }
            // 发送剩余的邮件
            if (mailMessage.To.Count > 0)
            {
                try
                {
                    await Task.Run(() => _iEmailSender.SendAsync(mailMessage)); // 异步发送邮件
                }
                catch (SmtpException ex)
                {
                    throw new UserFriendlyException("发送失败", ex.Message);
                }
            }
        }





        /// <summary>
        /// 发送脚本更新维护邮件通知,携带抄送人
        /// </summary>
        /// <param name="data">邮件数据</param>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task SendEmailForNotificationAsync(EmailNotificationRequest data)
        {
            // 发送邮件设置
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = data.Subject; //主题
            mailMessage.Body = data.Body; //内容
            mailMessage.BodyEncoding = Encoding.UTF8; //正文编码
            mailMessage.IsBodyHtml = data.IsBodyHtml; //设置为HTML格式
            mailMessage.Priority = MailPriority.Normal; //优先级
            //添加收件人
            //添加抄送人
            int batchSize = 30;
            mailMessage.To.Clear();
            mailMessage.CC.Clear(); // 清空抄送人列表
            foreach (var address in data.MailCc)
            {
                mailMessage.CC.Add(address);
            }
            foreach (var address in data.MailTo)
            {
                mailMessage.To.Add(new MailAddress(address));
                // 达到批次数量时发送邮件并清空抄送人列表
                if (mailMessage.To.Count >= batchSize)
                {
                    try
                    {
                        await Task.Run(() => _iEmailSender.SendAsync(mailMessage)); // 异步发送邮件
                    }
                    catch (SmtpException ex)
                    {
                        throw new UserFriendlyException("发送失败", ex.Message);
                    }

                    // 清空抄送人列表，准备下一批发送
                    mailMessage.To.Clear();
                }
            }
            // 发送剩余的邮件
            if (mailMessage.To.Count > 0)
            {
                try
                {
                    await Task.Run(() => _iEmailSender.SendAsync(mailMessage)); // 异步发送邮件
                }
                catch (SmtpException ex)
                {
                    throw new UserFriendlyException("发送失败", ex.Message);
                }
            }
        }

    }
}
