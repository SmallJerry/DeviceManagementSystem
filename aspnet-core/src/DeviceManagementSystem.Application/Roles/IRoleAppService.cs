using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DeviceManagementSystem.Roles.Dto;
using DeviceManagementSystem.Utils.Common;
using System.Threading.Tasks;

namespace SoftwareReleaseManagement.Roles
{
    public interface IRoleAppService : IAsyncCrudAppService<RoleDto, int, PagedRoleResultRequestDto, CreateRoleDto, RoleDto>
    {
        // Task<ListResultDto<PermissionDto>> GetAllPermissions();

        //   Task<GetRoleForEditOutput> GetRoleForEdit(EntityDto input);

        // Task<ListResultDto<RoleListDto>> GetRolesAsync(GetRolesInput input);



        /// <summary>
        /// 查询默认角色列表
        /// </summary>
        /// <returns></returns>
        Task<string[]> GetDefaultRoleNamesAsync();


        /// <summary>
        /// 设置角色是否默认
        /// </summary>
        /// <returns></returns>
        Task<CommonResult> UpdateRoleIsDefaultAsync(EntityDto<int> roleDto);

        /// <summary>
        /// 查询默认角色列表
        /// </summary>
        /// <returns></returns>
        Task<long[]> GetDefaultRoleIdsAsync();


    }
}
