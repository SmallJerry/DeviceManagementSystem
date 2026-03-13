using Abp.MultiTenancy;
using DeviceManagementSystem.Authorization.Users;

namespace DeviceManagementSystem.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
