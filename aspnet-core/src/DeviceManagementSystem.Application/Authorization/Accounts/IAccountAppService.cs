using Abp.Application.Services;
using DeviceManagementSystem.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
