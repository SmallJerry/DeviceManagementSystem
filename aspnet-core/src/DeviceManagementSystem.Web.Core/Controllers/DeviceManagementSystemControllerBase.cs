using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace DeviceManagementSystem.Controllers
{
    public abstract class DeviceManagementSystemControllerBase : AbpController
    {
        protected DeviceManagementSystemControllerBase()
        {
            LocalizationSourceName = DeviceManagementSystemConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
