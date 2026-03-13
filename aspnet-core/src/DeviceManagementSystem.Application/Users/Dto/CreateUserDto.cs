using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using DeviceManagementSystem.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;

namespace DeviceManagementSystem.Users.Dto
{
    /// <summary>
    /// 用户创建对象
    /// </summary>
    [AutoMapTo(typeof(User))]
    public class CreateUserDto : IShouldNormalize
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(AbpUserBase.MaxNameLength)]

        public string NickName { get; set; }


        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }



        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 是否被激活
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 角色列表
        /// </summary>
        public string[] RoleNames { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }


        /// <summary>
        /// 职位Id
        /// </summary>
        public Guid? PositionId { get; set; }


        /// <summary>
        /// 主管Id
        /// </summary>
        public long? DirectorId { get; set; }


        /// <summary>
        /// 组织Id
        /// </summary>
        public Guid? OrgId { get; set; }





        /// <summary>
        /// 初始化
        /// </summary>
        public void Normalize()
        {
            if (RoleNames == null)
            {
                RoleNames = new string[0];
            }
        }
    }
}
