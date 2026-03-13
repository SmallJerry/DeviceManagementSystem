using Abp.Authorization.Roles;
using DeviceManagementSystem.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;

namespace DeviceManagementSystem.Authorization.Roles
{
    public class Role : AbpRole<User>
    {
        public const int MaxDescriptionLength = 5000;


        /// <summary>
        /// 组织id
        /// </summary>
        public Guid? OrgId { get; set; }


        /// <summary>
        /// 分类
        /// </summary>
        [StringLength(64)]
        public string Category { get; set; }


        public Role()
        {
        }

        public Role(int? tenantId, string displayName)
            : base(tenantId, displayName)
        {
        }

        public Role(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {
        }

        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }
    }
}
