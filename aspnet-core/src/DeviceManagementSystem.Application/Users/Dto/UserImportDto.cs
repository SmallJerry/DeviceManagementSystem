using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Users.Dto
{
    /// <summary>
    /// 用户导入Dto
    /// </summary>
    public class UserImportDto
    {
        /// <summary>
        /// 导入数据列表
        /// </summary>
        [Required]
        public List<UserImportItemDto> DataList { get; set; }
    }


    /// <summary>
    /// 用户导入项Dto
    /// </summary>
    public class UserImportItemDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 职位名称
        /// </summary>
        public string PositionName { get; set; }

    }



    /// <summary>
    /// 导入结果Dto
    /// </summary>
    public class UserImportResultDto
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 成功数量
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 失败数量
        /// </summary>
        public int FailCount { get; set; }

        /// <summary>
        /// 失败记录详情
        /// </summary>
        public List<UserImportFailItemDto> FailItems { get; set; } = new List<UserImportFailItemDto>();
    }


    /// <summary>
    /// 导入失败项Dto
    /// </summary>
    public class UserImportFailItemDto
    {

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
