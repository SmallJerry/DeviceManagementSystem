using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;

namespace DeviceManagementSystem.Roles.Dto
{
    /// <summary>
    /// 角色列表Dto
    /// </summary>
    public class RoleListDto : EntityDto, IHasCreationTime
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// 是否为默认角色
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
