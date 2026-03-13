using Abp.Application.Services;
using DeviceManagementSystem.MultiTenancy.Dto;

namespace DeviceManagementSystem.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

