using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using DeviceManagementSystem.EntityFrameworkCore;
using DeviceManagementSystem.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace DeviceManagementSystem.Web.Tests
{
    [DependsOn(
        typeof(DeviceManagementSystemWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class DeviceManagementSystemWebTestModule : AbpModule
    {
        public DeviceManagementSystemWebTestModule(DeviceManagementSystemEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DeviceManagementSystemWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(DeviceManagementSystemWebMvcModule).Assembly);
        }
    }
}