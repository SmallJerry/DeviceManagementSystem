using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using DeviceManagementSystem.Authorization.Roles;
using System.ComponentModel.DataAnnotations;

namespace DeviceManagementSystem.Roles.Dto
{
    /// <summary>
    /// 角色编辑Dto
    /// </summary>
    public class RoleEditDto : EntityDto<int>
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
        /// 描述
        /// </summary>
        [StringLength(Role.MaxDescriptionLength)]
        public string Description { get; set; }

        /// <summary>
        /// 是否为默认角色
        /// </summary>
        public bool IsDefault { get; set; }
    }
}