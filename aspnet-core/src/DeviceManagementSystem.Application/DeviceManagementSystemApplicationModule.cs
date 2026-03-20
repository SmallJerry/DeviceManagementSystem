using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.Reflection.Extensions;
using DeviceManagementSystem.Authorization;
using DeviceManagementSystem.Email;

namespace DeviceManagementSystem
{
    [DependsOn(
        typeof(DeviceManagementSystemCoreModule),
        typeof(AbpAutoMapperModule))]
    public class DeviceManagementSystemApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<DeviceManagementSystemAuthorizationProvider>();
            Configuration.ReplaceService<IEmailSender, CustomEmailSender>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(DeviceManagementSystemApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
