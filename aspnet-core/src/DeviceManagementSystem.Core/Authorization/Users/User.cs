using Abp.Authorization.Users;
using Abp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeviceManagementSystem.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(64)]
        public string NickName { get; set; }

        /// <summary>
        /// 机构id
        /// </summary>
        public Guid? OrgId { get; set; }


        [StringLength(64)]
        public override string Surname { get; set; } = " ";




        /// <summary>
        /// 职位id
        /// </summary>
        public Guid? PositionId { get; set; }


        /// <summary>
        /// 主管id
        /// </summary>
        public long? DirectorId { get; set; }


        /// <summary>
        /// 拓展信息
        /// </summary>
        public string ExtJson { get; set; }



        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            return user;
        }
    }
}
