using DeviceManagementSystem.Configuration.Dto;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
