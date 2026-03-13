using Abp.Authorization;
using DeviceManagementSystem.Authorization.Roles;
using DeviceManagementSystem.Authorization.Users;

namespace DeviceManagementSystem.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
