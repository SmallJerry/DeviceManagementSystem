using Abp.Net.Mail;
using Abp.Runtime.Session;
using DeviceManagementSystem.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Email
{
    /// <summary>
    /// 定制版邮件发送
    /// </summary>
    public class CustomEmailSender : IEmailSender
    {
        private readonly IConfigurationRoot _configuration;


        public CustomEmailSender(IAbpSession abpSession,IWebHostEnvironment env)
        {
            _configuration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, env.IsDevelopment());
        }




        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="env"></param>
        public CustomEmailSender(IWebHostEnvironment env)
        {
            _configuration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName, env.IsDevelopment());
        }

        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="to">收件人</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件正文内容</param>
        /// <param name="isBodyHtml">邮件正文是否为HTML格式</param>
        public void Send(string to, string subject, string body, bool isBodyHtml = true)
        {
            try
            {
                // 从配置获取 SMTP 服务器相关信息
                string smtpHost = _configuration["EmailSettings:Host"];
                int smtpPort = Convert.ToInt16(_configuration["EmailSettings:Port"]);
                string smtpUserName = _configuration["EmailSettings:UserName"];
                string smtpPassword = _configuration["EmailSettings:Password"];
                string smtpDisplayName = _configuration["EmailSettings:DisplayName"];
                
                // 创建 MailMessage 对象，用于构建邮件
                using MailMessage msg = new()
                {
                    From = new MailAddress(smtpUserName, smtpDisplayName),// 设置发件人地址                  
                    BodyEncoding = Encoding.UTF8,// 设置邮件正文的编码格式为UTF-8                  
                    Priority = MailPriority.Normal,// 设置邮件的优先级为正常                   
                    Subject = subject,// 设置邮件主题                    
                    Body = body,// 设置邮件正文内容                  
                    IsBodyHtml = isBodyHtml// 指示邮件正文是否为HTML格式
                };

                // 移除可能存在的额外空格，并分割收件人地址
                foreach (var item in to.Split(';').Select(address => address.Trim()))
                {
                    // 添加收件人地址
                    msg.To.Add(item);
                }

                using SmtpClient smtpClient = new(smtpHost);// 创建 SmtpClient 对象，用于发送邮件              
                smtpClient.Port = smtpPort;// 设置SMTP服务器端口             
                smtpClient.EnableSsl = false;// 不启用SSL加密                
                smtpClient.UseDefaultCredentials = false;// 不使用默认凭据

                // 使用CredentialCache设置凭据：login处 可以换成以下几类gssapi ntlm WDigest.试试
                // 使用 CredentialCache 实例有效地为 SMTP 客户端设置凭据。它包括主机、端口、身份验证类型（在这里是 "login"），以及使用提供的电子邮件和密码的 NetworkCredential。
                CredentialCache myCache = new()
                {
                    { smtpClient.Host, smtpClient.Port, "login", new NetworkCredential(smtpUserName, smtpPassword) }
                };
                smtpClient.Credentials = myCache;

                // 发送邮件
                smtpClient.Send(msg);
            }
            catch (FormatException ex)
            {
                // 处理邮件地址格式错误的异常
                throw new FormatException("无效的邮件地址格式。", ex);
            }
            catch (SmtpException ex)
            {
                // 处理SMTP发送错误的异常
                throw new SmtpException("通过SMTP发送邮件失败。", ex);
            }
            catch (Exception ex)
            {
                // 处理其他异常类型
                throw new Exception("发送邮件失败。", ex);
            }
        }

        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="from">发件人地址</param>
        /// <param name="to">收件人</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件正文内容</param>
        /// <param name="isBodyHtml">邮件正文是否为HTML格式</param>
        public void Send(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            try
            {
                // 从配置获取 SMTP 服务器相关信息
                string smtpHost = _configuration["EmailSettings:Host"];
                int smtpPort = Convert.ToInt16(_configuration["EmailSettings:Port"]);
                string smtpUserName = _configuration["EmailSettings:UserName"];
                string smtpPassword = _configuration["EmailSettings:Password"];
                string smtpDisplayName = _configuration["EmailSettings:DisplayName"];
              
                // 创建 MailMessage 对象，用于构建邮件
                using MailMessage msg = new()
                {
                    From = new MailAddress(from, smtpDisplayName),// 设置发件人地址                  
                    BodyEncoding = Encoding.UTF8,// 设置邮件正文的编码格式为UTF-8                  
                    Priority = MailPriority.Normal,// 设置邮件的优先级为正常                   
                    Subject = subject,// 设置邮件主题                    
                    Body = body,// 设置邮件正文内容                  
                    IsBodyHtml = isBodyHtml// 指示邮件正文是否为HTML格式
                };

                // 移除可能存在的额外空格，并分割收件人地址
                foreach (var item in to.Split(';').Select(address => address.Trim()))
                {
                    // 添加收件人地址
                    msg.To.Add(item);
                }

                using SmtpClient smtpClient = new(smtpHost);// 创建 SmtpClient 对象，用于发送邮件              
                smtpClient.Port = smtpPort;// 设置SMTP服务器端口             
                smtpClient.EnableSsl = false;// 不启用SSL加密                
                smtpClient.UseDefaultCredentials = false;// 不使用默认凭据

                // 使用CredentialCache设置凭据：login处 可以换成以下几类gssapi ntlm WDigest.试试
                // 使用 CredentialCache 实例有效地为 SMTP 客户端设置凭据。它包括主机、端口、身份验证类型（在这里是 "login"），以及使用提供的电子邮件和密码的 NetworkCredential。
                CredentialCache myCache = new()
                {
                    { smtpClient.Host, smtpClient.Port, "login", new NetworkCredential(smtpUserName, smtpPassword) }
                };
                smtpClient.Credentials = myCache;

                // 发送邮件
                smtpClient.Send(msg);
            }
            catch (FormatException ex)
            {
                // 处理邮件地址格式错误的异常
                throw new FormatException("无效的邮件地址格式。", ex);
            }
            catch (SmtpException ex)
            {
                // 处理SMTP发送错误的异常
                throw new SmtpException("通过SMTP发送邮件失败。", ex);
            }
            catch (Exception ex)
            {
                // 处理其他异常类型
                throw new Exception("发送邮件失败。", ex);
            }
        }

        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="mail">MailMessage</param>
        /// <param name="normalize">是否执行规范化或标准化</param>
        public void Send(MailMessage mail, bool normalize = true)
        {
            try
            {
                // 从配置获取 SMTP 服务器相关信息
                string smtpHost = _configuration["EmailSettings:Host"];
                int smtpPort = Convert.ToInt16(_configuration["EmailSettings:Port"]);
                string smtpUserName = _configuration["EmailSettings:UserName"];
                string smtpPassword = _configuration["EmailSettings:Password"];
                string smtpDisplayName = _configuration["EmailSettings:DisplayName"];
              
                using SmtpClient smtpClient = new(smtpHost);// 创建 SmtpClient 对象，用于发送邮件              
                smtpClient.Port = smtpPort;// 设置SMTP服务器端口             
                smtpClient.EnableSsl = false;// 不启用SSL加密                
                smtpClient.UseDefaultCredentials = false;// 不使用默认凭据

                // 使用CredentialCache设置凭据：login处 可以换成以下几类gssapi ntlm WDigest.试试
                // 使用 CredentialCache 实例有效地为 SMTP 客户端设置凭据。它包括主机、端口、身份验证类型（在这里是 "login"），以及使用提供的电子邮件和密码的 NetworkCredential。
                CredentialCache myCache = new()
                {
                    { smtpClient.Host, smtpClient.Port, "login", new NetworkCredential(smtpUserName, smtpPassword) }
                };
                smtpClient.Credentials = myCache;
                mail.From = new MailAddress(smtpUserName, smtpDisplayName);
                // 发送邮件
                smtpClient.Send(mail);
            }
            catch (FormatException ex)
            {
                // 处理邮件地址格式错误的异常
                throw new FormatException("无效的邮件地址格式。", ex);
            }
            catch (SmtpException ex)
            {
                // 处理SMTP发送错误的异常
                throw new SmtpException("通过SMTP发送邮件失败。", ex);
            }
            catch (Exception ex)
            {
                // 处理其他异常类型
                throw new Exception("发送邮件失败。", ex);
            }
        }

        /// <summary>
        /// 邮件发送 - 异步
        /// </summary>
        /// <param name="from">发件人地址</param>
        /// <param name="to">收件人</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件正文内容</param>
        /// <param name="isBodyHtml">邮件正文是否为HTML格式</param>
        /// <returns></returns>
        public async Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            try
            {
                // 从配置获取 SMTP 服务器相关信息
                string smtpHost = _configuration["EmailSettings:Host"];
                int smtpPort = Convert.ToInt16(_configuration["EmailSettings:Port"]);
                string smtpUserName = _configuration["EmailSettings:UserName"];
                string smtpPassword = _configuration["EmailSettings:Password"];
                string smtpDisplayName = _configuration["EmailSettings:DisplayName"];
              
                // 创建 MailMessage 对象，用于构建邮件
                using MailMessage msg = new()
                {
                    From = new MailAddress(from,smtpDisplayName),// 设置发件人地址                  
                    BodyEncoding = Encoding.UTF8,// 设置邮件正文的编码格式为UTF-8                  
                    Priority = MailPriority.Normal,// 设置邮件的优先级为正常                   
                    Subject = subject,// 设置邮件主题                    
                    Body = body,// 设置邮件正文内容                  
                    IsBodyHtml = isBodyHtml// 指示邮件正文是否为HTML格式
                };

                // 移除可能存在的额外空格，并分割收件人地址
                foreach (var item in to.Split(';').Select(address => address.Trim()))
                {
                    // 添加收件人地址
                    msg.To.Add(item);
                }

                using SmtpClient smtpClient = new(smtpHost);// 创建 SmtpClient 对象，用于发送邮件              
                smtpClient.Port = smtpPort;// 设置SMTP服务器端口             
                smtpClient.EnableSsl = false;// 不启用SSL加密                
                smtpClient.UseDefaultCredentials = false;// 不使用默认凭据

                // 使用CredentialCache设置凭据：login处 可以换成以下几类gssapi ntlm WDigest.试试
                // 使用 CredentialCache 实例有效地为 SMTP 客户端设置凭据。它包括主机、端口、身份验证类型（在这里是 "login"），以及使用提供的电子邮件和密码的 NetworkCredential。
                CredentialCache myCache = new()
                {
                    { smtpClient.Host, smtpClient.Port, "login", new NetworkCredential(smtpUserName, smtpPassword) }
                };
                smtpClient.Credentials = myCache;

                // 发送邮件
                await smtpClient.SendMailAsync(msg);
            }
            catch (FormatException ex)
            {
                // 处理邮件地址格式错误的异常
                throw new FormatException("无效的邮件地址格式。", ex);
            }
            catch (SmtpException ex)
            {
                // 处理SMTP发送错误的异常
                throw new SmtpException("通过SMTP发送邮件失败。", ex);
            }
            catch (Exception ex)
            {
                // 处理其他异常类型
                throw new Exception("发送邮件失败。", ex);
            }
        }

        /// <summary>
        /// 邮件发送 - 异步
        /// </summary>
        /// <param name="mail">MailMessage</param>
        /// <param name="normalize">是否执行规范化或标准化</param>
        /// <returns></returns>
        public async Task SendAsync(MailMessage mail, bool normalize = true)
        {
            try
            {
                // 从配置获取 SMTP 服务器相关信息
                string smtpHost = _configuration["EmailSettings:Host"];
                int smtpPort = Convert.ToInt16(_configuration["EmailSettings:Port"]);
                string smtpUserName = _configuration["EmailSettings:UserName"];
                string smtpPassword = _configuration["EmailSettings:Password"];
                string smtpDisplayName = _configuration["EmailSettings:DisplayName"];
               
                // 正常来说直接取mail就行，但是要在调用该API的时候把以下几个参数传递过来，为了调用的时候方便不用多赋值这几个参数，直接这里赋值
                mail.From ??= new MailAddress(smtpUserName,smtpDisplayName);// 如果发件人地址为空 则 取邮件配置的账号
                mail.IsBodyHtml = true;// 邮件正文是否为HTML格式
                mail.BodyEncoding = Encoding.UTF8;// 设置邮件正文的编码格式为UTF-8        
                mail.Priority = MailPriority.Normal;// 设置邮件的优先级为正常 

                using SmtpClient smtpClient = new(smtpHost);// 创建 SmtpClient 对象，用于发送邮件              
                smtpClient.Port = smtpPort;// 设置SMTP服务器端口             
                smtpClient.EnableSsl = false;// 不启用SSL加密                
                smtpClient.UseDefaultCredentials = false;// 不使用默认凭据

                // 使用CredentialCache设置凭据：login处 可以换成以下几类gssapi ntlm WDigest.试试
                // 使用 CredentialCache 实例有效地为 SMTP 客户端设置凭据。它包括主机、端口、身份验证类型（在这里是 "login"），以及使用提供的电子邮件和密码的 NetworkCredential。
                CredentialCache myCache = new()
                    {
                        { smtpClient.Host, smtpClient.Port, "login", new NetworkCredential(smtpUserName, smtpPassword) }
                    };
                smtpClient.Credentials = myCache;

                await smtpClient.SendMailAsync(mail);
            }
            catch (FormatException ex)
            {
                // 处理邮件地址格式错误的异常
                throw new FormatException("无效的邮件地址格式。", ex);
            }
            catch (SmtpException ex)
            {
                // 处理SMTP发送错误的异常
                throw new SmtpException("通过SMTP发送邮件失败。", ex);
            }
            catch (Exception ex)
            {
                // 处理其他异常类型
                throw new Exception("发送邮件失败。", ex);
            }
        }

        /// <summary>
        /// 邮件发送 - 异步
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isBodyHtml"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task SendAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            try
            {
                // 从配置获取 SMTP 服务器相关信息
                string smtpHost = _configuration["EmailSettings:Host"];
                int smtpPort = Convert.ToInt16(_configuration["EmailSettings:Port"]);
                string smtpUserName = _configuration["EmailSettings:UserName"];
                string smtpPassword = _configuration["EmailSettings:Password"];
                string smtpDisplayName = _configuration["EmailSettings:DisplayName"];
              
                // 创建 MailMessage 对象，用于构建邮件
                using MailMessage msg = new()
                {
                    From = new MailAddress(smtpUserName, smtpDisplayName),// 设置发件人地址                  
                    BodyEncoding = Encoding.UTF8,// 设置邮件正文的编码格式为UTF-8                  
                    Priority = MailPriority.Normal,// 设置邮件的优先级为正常                   
                    Subject = subject,// 设置邮件主题                    
                    Body = body,// 设置邮件正文内容                  
                    IsBodyHtml = isBodyHtml// 指示邮件正文是否为HTML格式
                };

                // 移除可能存在的额外空格，并分割收件人地址
                foreach (var item in to.Split(';').Select(address => address.Trim()))
                {
                    // 添加收件人地址
                    msg.To.Add(item);
                }

                using SmtpClient smtpClient = new(smtpHost);// 创建 SmtpClient 对象，用于发送邮件              
                smtpClient.Port = smtpPort;// 设置SMTP服务器端口             
                smtpClient.EnableSsl = false;// 不启用SSL加密                
                smtpClient.UseDefaultCredentials = false;// 不使用默认凭据

                // 使用CredentialCache设置凭据：login处 可以换成以下几类gssapi ntlm WDigest.试试
                // 使用 CredentialCache 实例有效地为 SMTP 客户端设置凭据。它包括主机、端口、身份验证类型（在这里是 "login"），以及使用提供的电子邮件和密码的 NetworkCredential。
                CredentialCache myCache = new()
                {
                    { smtpClient.Host, smtpClient.Port, "login", new NetworkCredential(smtpUserName, smtpPassword) }
                };
                smtpClient.Credentials = myCache;

                // 发送邮件
                await smtpClient.SendMailAsync(msg);
            }
            catch (FormatException ex)
            {
                // 处理邮件地址格式错误的异常
                throw new FormatException("无效的邮件地址格式。", ex);
            }
            catch (SmtpException ex)
            {
                // 处理SMTP发送错误的异常
                throw new SmtpException("通过SMTP发送邮件失败。", ex);
            }
            catch (Exception ex)
            {
                // 处理其他异常类型
                throw new Exception("发送邮件失败。", ex);
            }
        }
    }
}
