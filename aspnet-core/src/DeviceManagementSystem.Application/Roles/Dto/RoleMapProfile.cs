using Abp.Authorization;
using Abp.Authorization.Roles;
using AutoMapper;
using DeviceManagementSystem.Authorization.Roles;
using System.Linq;

namespace DeviceManagementSystem.Roles.Dto
{
    /// <summary>
    /// 角色映射配置
    /// </summary>
    public class RoleMapProfile : Profile
    {

        /// <summary>
        /// 角色映射配置构造函数
        /// </summary>
        public RoleMapProfile()
        {
            // Role and permission
            CreateMap<Permission, string>().ConvertUsing(r => r.Name);
            CreateMap<RolePermissionSetting, string>().ConvertUsing(r => r.Name);

            CreateMap<CreateRoleDto, Role>();

            CreateMap<RoleDto, Role>();

            CreateMap<Role, RoleDto>().ForMember(x => x.GrantedPermissions,
                opt => opt.MapFrom(x => x.Permissions.Where(p => p.IsGranted)));

            CreateMap<Role, RoleListDto>();
            CreateMap<Role, RoleEditDto>();
            CreateMap<Permission, FlatPermissionDto>();
        }
    }
}
