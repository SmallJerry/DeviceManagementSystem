using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using DeviceManagementSystem.Authorization;

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
