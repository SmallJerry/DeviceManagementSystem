using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DeviceManagementSystem.Authorization.Users;
using System;
using System.Collections.Generic;

namespace DeviceManagementSystem.Sessions.Dto
{
    /// <summary>
    /// 用户登录信息DTO
    /// </summary>
    [AutoMapFrom(typeof(User))]
    public class UserLoginInfoDto : EntityDto<long>
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string EmailAddress { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }



        /// <summary>
        /// 组织ID
        /// </summary>
        public Guid? OrgId { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 职位ID
        /// </summary>
        public Guid? PositionId { get; set; }

        /// <summary>
        /// 职位名称
        /// </summary>
        public string PositionName { get; set; }


        /// <summary>
        /// 主管ID
        /// </summary>
        public long? DirectorId { get; set; }


        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public string ExtJson { get; set; }



        /// <summary>
        /// 按钮权限码集合
        /// </summary>
        public List<string> ButtonCodeList { get; set; }

        /// <summary>
        /// 移动端按钮权限码集合
        /// </summary>
        public List<string> MobileButtonCodeList { get; set; }

        /// <summary>
        /// 权限码集合
        /// </summary>
        public List<string> PermissionCodeList { get; set; }

        /// <summary>
        /// 角色码集合
        /// </summary>
        public List<string> RoleCodeList { get; set; }

        /// <summary>
        /// 数据范围集合
        /// </summary>
        public List<DataScopeDto> DataScopeList { get; set; }


        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

    }



    /// <summary>
    /// 数据范围DTO
    /// </summary>
    public class DataScopeDto
    {
        /// <summary>
        /// API接口
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// 数据范围
        /// </summary>
        public List<string> DataScope { get; set; }
    }
}
