using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using DeviceManagementSystem.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;

namespace DeviceManagementSystem.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserDto : EntityDto<long>
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
        public string NickName { get; set; }


        /// <summary>
        /// 职位id
        /// </summary>
        public Guid? PositionId { get; set; }


        /// <summary>
        /// 职位名称
        /// </summary>
        public string PositionName { get; set; }


        /// <summary>
        /// 机构Id
        /// </summary>
        public Guid? OrgId { get; set; }


        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }




        /// <summary>
        /// 机构名称
        /// </summary>
        public string OrgName { get; set; }




        /// <summary>
        /// 主管id
        /// </summary>
        public long? DirectorId { get; set; }


        /// <summary>
        /// 主管名称
        /// </summary>
        public string DirectorName { get; set; }


        /// <summary>
        /// 拓展信息
        /// </summary>
        public string ExtJson { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }


        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 角色名列表
        /// </summary>
        public string[] RoleNames { get; set; }
    }

    /// <summary>
    /// 用户基础信息
    /// </summary>

    public class BaseUserInfo
    {

        /// <summary>
        /// 名字
        /// </summary>
        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

    }




  
}
