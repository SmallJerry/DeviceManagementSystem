using Abp.Modules;
using Abp.Reflection.Extensions;
using DeviceManagementSystem.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DeviceManagementSystem.Web.Host.Startup
{
    [DependsOn(
       typeof(DeviceManagementSystemWebCoreModule))]
    public class DeviceManagementSystemWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public DeviceManagementSystemWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DeviceManagementSystemWebHostModule).GetAssembly());
        }
    }
}
