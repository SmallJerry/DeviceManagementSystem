using Abp.Authorization;
using Abp.Runtime.Session;
using DeviceManagementSystem.Configuration.Dto;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : DeviceManagementSystemAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
