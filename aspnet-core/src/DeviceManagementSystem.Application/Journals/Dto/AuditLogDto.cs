using Abp.Auditing;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Journals.Dto
{

    /// <summary>
    /// 日志对象
    /// </summary>
    [AutoMapFrom(typeof(AuditLog))]
    public class AuditLogDto : AuditLog
    {

        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }


        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }



    }
}
