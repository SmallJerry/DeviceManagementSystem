using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using DeviceManagementSystem.Configuration;
using DeviceManagementSystem.EntityFrameworkCore;
using DeviceManagementSystem.Migrator.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace DeviceManagementSystem.Migrator
{
    [DependsOn(typeof(DeviceManagementSystemEntityFrameworkModule))]
    public class DeviceManagementSystemMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public DeviceManagementSystemMigratorModule(DeviceManagementSystemEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(DeviceManagementSystemMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                DeviceManagementSystemConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus),
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DeviceManagementSystemMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
