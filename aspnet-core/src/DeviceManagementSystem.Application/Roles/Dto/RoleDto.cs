using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using DeviceManagementSystem.Authorization.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeviceManagementSystem.Roles.Dto
{
    /// <summary>
    /// 角色Dto
    /// </summary>
    public class RoleDto : EntityDto<int>
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        [Required]
        [StringLength(AbpRoleBase.MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        /// <summary>
        /// 规范化名称
        /// </summary>
        public string NormalizedName { get; set; }

        /// <summary>
        /// 是否为默认角色
        /// </summary>
        public bool IsDefault { get; set; }


        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(Role.MaxDescriptionLength)]
        public string Description { get; set; }

        /// <summary>
        /// 授权的权限列表
        /// </summary>
        public List<string> GrantedPermissions { get; set; }
    }
}