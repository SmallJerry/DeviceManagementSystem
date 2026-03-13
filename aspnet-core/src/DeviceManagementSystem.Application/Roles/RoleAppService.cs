using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using Abp.UI;
using DeviceManagementSystem.Authorization.Resources;
using DeviceManagementSystem.Authorization.Roles;
using DeviceManagementSystem.Authorization.Users;
using DeviceManagementSystem.Resources.Menus;
using DeviceManagementSystem.Resources.Modules.Constants;
using DeviceManagementSystem.Roles;
using DeviceManagementSystem.Roles.Constants;
using DeviceManagementSystem.Roles.Dto;
using DeviceManagementSystem.Roles.Result;
using DeviceManagementSystem.SystemRelations;
using DeviceManagementSystem.SystemRelations.Constants;
using DeviceManagementSystem.Users.Dto;
using DeviceManagementSystem.Utils.Common;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftwareReleaseManagement.Roles
{
    /// <summary>
    /// 角色服务接口实现类
    /// </summary>
    [AbpAuthorize]
    [DisableAuditing]
    public class RoleAppService : AsyncCrudAppService<Role, RoleDto, int, PagedRoleResultRequestDto, CreateRoleDto, RoleDto>, IRoleAppService
    {
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<SystemRelation, Guid> _systemRelationRepository;
        private readonly ISystemRelationAppService _systemRelationAppService;
        private readonly IRepository<Resource,Guid> _resourceRepository;
        private readonly IMenuAppService _menuAppService;
        private readonly ICacheManager _cacheManager;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="roleManager"></param>
        /// <param name="userManager"></param>
        /// <param name="systemRelationRepository"></param>
        /// <param name="systemRelationAppService"></param>
        /// <param name="resourceRepository"></param>
        /// <param name="menuAppService"></param>
        /// <param name="cacheManager"></param>
        public RoleAppService(IRepository<Role> repository, RoleManager roleManager, UserManager userManager,IRepository<SystemRelation, Guid> systemRelationRepository,
            ISystemRelationAppService systemRelationAppService,
            IRepository<Resource, Guid> resourceRepository,
            IMenuAppService menuAppService, ICacheManager cacheManager)
            : base(repository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _roleRepository = repository;
            _systemRelationRepository = systemRelationRepository;
            _systemRelationAppService = systemRelationAppService;
            _resourceRepository = resourceRepository;
            _menuAppService = menuAppService;
            _cacheManager = cacheManager;
        }


        /// <summary>
        ///删除角色及其相关的用户关联信息。首先将拥有该角色的用户从角色中移除，然后删除角色实体对象。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task DeleteAsync(EntityDto<int> input)
        {
            //CheckDeletePermission();
            var role = await _roleManager.FindByIdAsync(input.Id.ToString());
            var users = await _userManager.GetUsersInRoleAsync(role.Name);
            foreach (var user in users)
            {
                CheckErrors(await _userManager.RemoveFromRoleAsync(user, role.Name));
            }

            CheckErrors(await _roleManager.DeleteAsync(role));
        }





        /// <summary>
        /// 重写基于过滤条件的角色查询。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<Role> CreateFilteredQuery(PagedRoleResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword)
                || x.DisplayName.Contains(input.Keyword)
                || x.Description.Contains(input.Keyword));
        }

        /// <summary>
        /// 根据id获取角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override async Task<Role> GetEntityByIdAsync(int id)
        {
            return await Repository.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// 列表排序
        /// </summary>
        /// <param name="query"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<Role> ApplySorting(IQueryable<Role> query, PagedRoleResultRequestDto input)
        {
            return query.OrderBy(r => r.DisplayName);
        }

        /// <summary>
        /// 用于检查身份验证结果是否包含错误，并对错误进行相应的处理。
        /// </summary>
        /// <param name="identityResult"></param>
        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }



        /// <summary>
        /// 设置角色是否默认
        /// </summary>
        /// <returns></returns>
        public async Task<CommonResult> UpdateRoleIsDefaultAsync(EntityDto<int> roleDto)
        {
            var role = await _roleManager.GetRoleByIdAsync(roleDto.Id);
            if (role == null)
            {
                return CommonResult.Error();
            }
            role.IsDefault = !role.IsDefault;
            await CurrentUnitOfWork.SaveChangesAsync();
            return CommonResult.Ok();
        }


        /// <summary>
        /// 查询默认角色列表
        /// </summary>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [DisableAuditing]
        public async Task<string[]> GetDefaultRoleNamesAsync()
        {
            string[] defaultRoleName = null;
            var roleNames = await _roleRepository.GetAll().AsNoTracking().Where(it => it.IsDefault).Select(it => it.Name).ToListAsync();
            if (roleNames.Any())
            {
                defaultRoleName = new string[roleNames.Count];
                for (int i = 0; i < roleNames.Count; i++)
                {
                    defaultRoleName[i] = roleNames[i];
                }
            }
            return defaultRoleName;
        }





        /// <summary>
        /// 查询默认角色Id列表
        /// </summary>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [DisableAuditing]
        public async Task<long[]> GetDefaultRoleIdsAsync()
        {
            long[] defaultRoleIdList = null;
            var roleIds = await _roleRepository.GetAll().AsNoTracking().Where(it => it.IsDefault).Select(it => it.Id).ToListAsync();
            if (roleIds.Any())
            {
                defaultRoleIdList = new long[roleIds.Count];
                for (int i = 0; i < roleIds.Count; i++)
                {
                    defaultRoleIdList[i] = roleIds[i];
                }
            }
            return defaultRoleIdList;
        }




        /// <summary>
        /// 获取角色分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<CommonResult<Page<RoleDto>>> GetPageList(RolePageParam param)
        {
            try
            {
                var query = _roleRepository.GetAll()
                    .WhereIf(param.OrgId.HasValue, x => x.OrgId == param.OrgId)
                    .WhereIf(!string.IsNullOrEmpty(param.Category), x => x.Category == param.Category)
                    .WhereIf(!string.IsNullOrEmpty(param.SearchKey), x =>
                        x.Name.Contains(param.SearchKey) || x.DisplayName.Contains(param.SearchKey));

                // 排序
                if (!string.IsNullOrEmpty(param.SortField) && !string.IsNullOrEmpty(param.SortOrder))
                {
                    if (param.SortOrder == "ASCEND")
                    {
                        query = query.OrderBy(x => x.CreationTime);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.CreationTime);
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.CreationTime);
                }

                var totalCount = await query.CountAsync();
                var items = await query
                    .Skip((param.Current - 1) * param.Size)
                    .Take(param.Size)
                    .ToListAsync();

                var page = new Page<RoleDto>
                {
                    Current = param.Current,
                    Size = param.Size,
                    Total = totalCount,
                    Records = ObjectMapper.Map<List<RoleDto>>(items)
                };

                return CommonResult<Page<RoleDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取角色分页失败", ex);
                return CommonResult<Page<RoleDto>>.Error("获取角色分页失败");
            }
        }





        /// <summary>
        /// 添加角色
        /// </summary>
        public async Task<CommonResult> CreateRole(RoleAddParam param)
        {
            try
            {
                // 验证参数
                ValidateAddParam(param);

                var role = new Role
                {
                    Name = param.Name,
                    DisplayName = param.DisplayName,
                    NormalizedName = param.Name.ToUpperInvariant(),
                    IsDefault = param.IsDefault,
                    Category = param.Category,
                    OrgId = param.OrgId,
                    Description = param.ExtJson
                };

                await _roleManager.CreateAsync(role);

                return CommonResult.Ok();
            }
            catch (UserFriendlyException ex)
            {
                return CommonResult.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("添加角色失败", ex);
                return CommonResult.Error("添加角色失败");
            }
        }

        private void ValidateAddParam(RoleAddParam param)
        {
            // 验证分类
            RoleConstant.Category.Validate(param.Category);

            if (param.Category == RoleConstant.Category.ORG)
            {
                if (!param.OrgId.HasValue)
                {
                    throw new UserFriendlyException("组织ID不能为空");
                }
            }
            else
            {
                param.OrgId = null;
            }

            // 检查重复名称
            var existByName = _roleRepository.GetAll()
                .Where(x => x.OrgId == param.OrgId && x.Name == param.Name)
                .Any();

            if (existByName)
            {
                if (!param.OrgId.HasValue)
                {
                    throw new UserFriendlyException($"存在重复的全局角色，名称为：{param.Name}");
                }
                else
                {
                    throw new UserFriendlyException($"同组织下存在重复的角色，名称为：{param.Name}");
                }
            }

            // 检查重复编码
            var existByCode = _roleRepository.GetAll()
                .Where(x => x.OrgId == param.OrgId && x.DisplayName == param.DisplayName)
                .Any();

            if (existByCode)
            {
                if (!param.OrgId.HasValue)
                {
                    throw new UserFriendlyException($"存在重复的全局角色，显示名为：{param.DisplayName}");
                }
                else
                {
                    throw new UserFriendlyException($"同组织下存在重复的角色，显示名为：{param.DisplayName}");
                }
            }
        }



        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<CommonResult> UpdateByIdAsync(RoleEditParam param)
        {
            try
            {
                var role = await GetEntityAsync(param.Id);

                // 验证是否为超管角色
                if (role.NormalizedName == SysBuildInEnum.BUILD_IN_ROLE_CODE)
                {
                    throw new UserFriendlyException("不可编辑超管角色");
                }

                ValidateEditParam(param);

                role.Name = param.Name;
                role.DisplayName = param.Name;
                role.NormalizedName = param.Name.ToUpperInvariant();
                role.Category = param.Category;
                role.IsDefault = param.IsDefault;
                role.OrgId = param.OrgId;
                role.Description = param.ExtJson;

                await _roleManager.UpdateAsync(role);

                return CommonResult.Ok();
            }
            catch (UserFriendlyException ex)
            {
                return CommonResult.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("编辑角色失败", ex);
                return CommonResult.Error("编辑角色失败");
            }
        }

        private void ValidateEditParam(RoleEditParam param)
        {
            // 验证分类
            RoleConstant.Category.Validate(param.Category);

            if (param.Category == RoleConstant.Category.ORG)
            {
                if (!param.OrgId.HasValue)
                {
                    throw new UserFriendlyException("组织ID不能为空");
                }
            }
            else
            {
                param.OrgId = null;
            }

            // 检查重复名称（排除自己）
            var existByName = _roleRepository.GetAll()
                .Where(x => x.OrgId == param.OrgId && x.Name == param.Name && x.Id != param.Id)
                .Any();

            if (existByName)
            {
                if (!param.OrgId.HasValue)
                {
                    throw new UserFriendlyException($"存在重复的全局角色，名称为：{param.Name}");
                }
                else
                {
                    throw new UserFriendlyException($"同组织下存在重复的角色，名称为：{param.Name}");
                }
            }

            // 检查重复编码（排除自己）
            var existByCode = _roleRepository.GetAll()
                .Where(x => x.OrgId == param.OrgId && x.DisplayName == param.DisplayName && x.Id != param.Id)
                .Any();

            if (existByCode)
            {
                if (!param.OrgId.HasValue)
                {
                    throw new UserFriendlyException($"存在重复的全局角色，显示名为：{param.DisplayName}");
                }
                else
                {
                    throw new UserFriendlyException($"同组织下存在重复的角色，显示名为：{param.DisplayName}");
                }
            }
        }



        /// <summary>
        /// 根据id列表获取角色Dto列表
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost]
        [DisableAuditing]
        public async Task<CommonResult<List<RoleDto>>> GetRoleListByIdList([FromBody] List<long> idList)
        {
            if (idList == null || idList.Count == 0)
            {
                return CommonResult<List<RoleDto>>.Success();
            }
            try
            {
                var roles = await _roleRepository.GetAll()
                    .Where(u => idList.Contains(u.Id))
                    .ToListAsync();
                var roleDtos = roles.Select(u => ObjectMapper.Map<RoleDto>(u)).ToList();
                return CommonResult<List<RoleDto>>.Success(roleDtos);
            }
            catch (Exception ex)
            {
                return CommonResult<List<RoleDto>>.Error("获取失败");
            }
        }



        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public async Task<CommonResult> DeleteByIdsAsync([FromBody]List<RoleIdParam> paramList)
        {
            try
            {
                var roleIds = paramList.Select(x => x.Id.ToString()).ToList();

                if (roleIds.Count == 0)
                {
                    return CommonResult.Ok();
                }

                // 检查是否包含超管角色
                var roles = await _roleRepository.GetAll()
                    .Where(x => roleIds.Contains(x.Id.ToString()))
                    .ToListAsync();

                var superAdminRole = roles.FirstOrDefault(x => x.NormalizedName == SysBuildInEnum.BUILD_IN_ROLE_CODE);
                if (superAdminRole != null)
                {
                    throw new UserFriendlyException("不可删除系统内置超管角色");
                }

                // 检查并删除角色与用户关系
                var userRelations = await _systemRelationRepository.GetAll().AsNoTracking().Where(x =>
                    roleIds.Contains(x.Target) &&
                    string.Equals(x.Category, SystemRelationCategoryConstant.SYS_USER_HAS_ROLE)).ToListAsync();

                if (userRelations != null && userRelations.Count != 0)
                {
                    await _systemRelationRepository.GetDbContext().BulkDeleteAsync(userRelations);
                }

                // 检查并删除角色与资源关系
                var resourceRelations = await _systemRelationRepository.GetAll().AsNoTracking().Where(x =>
                    roleIds.Contains(x.ObjectId) &&
                    string.Equals(x.Category, SystemRelationCategoryConstant.SYS_ROLE_HAS_RESOURCE)).ToListAsync();

                if (resourceRelations != null && resourceRelations.Count != 0)
                {
                    await _systemRelationRepository.GetDbContext().BulkDeleteAsync(resourceRelations);
                }

                // 检查并删除角色与权限关系
                var permissionRelations = await _systemRelationRepository.GetAll().AsNoTracking().Where(x =>
                    roleIds.Contains(x.ObjectId) &&
                    string.Equals(x.Category, SystemRelationCategoryConstant.SYS_ROLE_HAS_PERMISSION)).ToListAsync();

                if (permissionRelations != null && permissionRelations.Count != 0)
                {
                    await _systemRelationRepository.GetDbContext().BulkDeleteAsync(permissionRelations);
                }

                // 删除角色
                await _roleRepository.DeleteAsync(x => roleIds.Contains(x.Id.ToString()));

                return CommonResult.Ok();
            }
            catch (UserFriendlyException ex)
            {
                return CommonResult.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("删除角色失败", ex);
                return CommonResult.Error("删除角色失败");
            }
        }


        /// <summary>
        /// 获取角色详情
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<CommonResult<RoleDto>> GetByIdAsync(RoleIdParam param)
        {
            try
            {
                var role = await GetEntityAsync(param.Id);
                var result = ObjectMapper.Map<RoleDto>(role);
                return CommonResult<RoleDto>.Success(result);
            }
            catch (UserFriendlyException ex)
            {
                return CommonResult<RoleDto>.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("获取角色详情失败", ex);
                return CommonResult<RoleDto>.Error("获取角色详情失败");
            }
        }


        /// <summary>
        /// 获取角色拥有资源
        /// </summary>
        public async Task<CommonResult<RoleOwnResourceResult>> GetOwnResourceAsync(RoleIdParam param)
        {
            try
            {
                var relations = await _systemRelationAppService.GetRelationListByObjectIdAndCategoryAsync(
                    param.Id.ToString(),
                    SystemRelationCategoryConstant.SYS_ROLE_HAS_RESOURCE);

                var result = new RoleOwnResourceResult
                {
                    Id = param.Id,
                    GrantInfoList = relations.Select(x =>
                        JsonConvert.DeserializeObject<RoleOwnResourceResult.SysRoleOwnResource>(x.ExtJson))
                        .ToList()
                };

                return CommonResult<RoleOwnResourceResult>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取角色拥有资源失败", ex);
                return CommonResult<RoleOwnResourceResult>.Error("获取角色拥有资源失败");
            }
        }


        /// <summary>
        /// 给角色授权资源
        /// </summary>
        public async Task<CommonResult> GrantResourceAsync(RoleGrantResourceParam param)
        {
            try
            {
                var role = await GetEntityAsync(param.Id);

                var menuIdList = param.GrantInfoList.Select(x => x.MenuId.ToString()).ToList();

                // 非超管角色不能授权系统模块资源
                if (role.NormalizedName == SysBuildInEnum.BUILD_IN_ROLE_CODE && menuIdList.Count > 0)
                {
                    var menuIds = param.GrantInfoList.Select(x => x.MenuId).ToList();
                    var menus = await _resourceRepository.GetAll()
                        .Where(x => menuIds.Contains(x.Id))
                        .ToListAsync();

                    var moduleIds = menus.Select(x => x.Module).Distinct().ToList();
                    var modules = await _resourceRepository.GetAll()
                        .Where(x => moduleIds.Contains(x.Id) && x.Category == ResourceCategoryConstant.MODULE)
                        .ToListAsync();

                    var systemModule = modules.FirstOrDefault(x =>
                        x.Code == SysBuildInEnum.BUILD_IN_MODULE_CODE);

                    if (systemModule != null)
                    {
                        throw new UserFriendlyException("非超管角色不可被授权系统模块菜单资源");
                    }
                }

                var extJsonList = param.GrantInfoList.Select(x => JsonConvert.SerializeObject(x)).ToList();

                await _systemRelationAppService.SaveRelationBatchWithClearAsync(
                    param.Id.ToString(),
                    menuIdList,
                    SystemRelationCategoryConstant.SYS_ROLE_HAS_RESOURCE,
                    extJsonList);

                return CommonResult.Ok();
            }
            catch (UserFriendlyException ex)
            {
                return CommonResult.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error("给角色授权资源失败", ex);
                return CommonResult.Error("给角色授权资源失败");
            }
        }


        /// <summary>
        /// 获取角色拥有权限
        /// </summary>
        public async Task<CommonResult<SysRoleOwnPermissionResult>> GetOwnPermissionAsync(RoleIdParam param)
        {
            try
            {
                var relations = await _systemRelationAppService.GetRelationListByObjectIdAndCategoryAsync(
                    param.Id.ToString(),
                    SystemRelationCategoryConstant.SYS_ROLE_HAS_PERMISSION);

                var result = new SysRoleOwnPermissionResult
                {
                    Id = param.Id,
                    GrantInfoList = relations.Select(x =>
                        JsonConvert.DeserializeObject<SysRoleOwnPermissionResult.SysRoleOwnPermission>(x.ExtJson))
                        .ToList()
                };

                return CommonResult<SysRoleOwnPermissionResult>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取角色拥有权限失败", ex);
                return CommonResult<SysRoleOwnPermissionResult>.Error("获取角色拥有权限失败");
            }
        }


        /// <summary>
        /// 给角色授权权限
        /// </summary>
        public async Task<CommonResult> GrantPermissionAsync(RoleGrantPermissionParam param)
        {
            try
            {
                var apiUrlList = param.GrantInfoList.Select(x => x.ApiUrl).ToList();
                var extJsonList = param.GrantInfoList.Select(x => JsonConvert.SerializeObject(x)).ToList();

                await _systemRelationAppService.SaveRelationBatchWithClearAsync(
                    param.Id.ToString(),
                    apiUrlList,
                    SystemRelationCategoryConstant.SYS_ROLE_HAS_PERMISSION,
                    extJsonList);

                // 获取所有拥有该角色的用户
                var userRelations = await _systemRelationRepository.GetAll()
                    .Where(x => x.Target == param.Id.ToString() &&
                                x.Category == SystemRelationCategoryConstant.SYS_USER_HAS_ROLE)
                    .Select(x => x.ObjectId)
                    .ToListAsync();

                // 清除这些用户的菜单缓存
                var userCache = _cacheManager.GetCache("UserMenus");
                foreach (var userId in userRelations)
                {
                    var cacheKey = $"UserMenu_{userId}";
                    await userCache.RemoveAsync(cacheKey);
                }

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("给角色授权权限失败", ex);
                return CommonResult.Error("给角色授权权限失败");
            }
        }


        /// <summary>
        /// 获取角色下的用户
        /// </summary>
        public async Task<CommonResult<List<long>>> GetOwnUserAsync(RoleIdParam param)
        {
            try
            {
                var userIds = await _systemRelationAppService.GetRelationObjectIdListByTargetAndCategoryAsync(
                    param.Id.ToString(),
                    SystemRelationCategoryConstant.SYS_USER_HAS_ROLE);

                var result = userIds.Select(x => long.Parse(x)).ToList();
                return CommonResult<List<long>>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取角色下的用户失败", ex);
                return CommonResult<List<long>>.Error("获取角色下的用户失败");
            }
        }


        /// <summary>
        /// 给角色授权用户
        /// </summary>
        public async Task<CommonResult> GrantUserAsync(RoleGrantUserParam param)
        {
            try
            {
                var roleIdStr = param.Id.ToString();
                var category = SystemRelationCategoryConstant.SYS_USER_HAS_ROLE;

                // 获取新授权的用户ID字符串列表
                var newUserIds = param.GrantInfoList.Select(x => x.ToString()).ToList();

                // 获取该角色现有的所有用户授权关系
                var existingRelations = await _systemRelationRepository.GetAll()
                    .Where(x => x.Target == roleIdStr && x.Category == category)
                    .ToListAsync();

                var existingUserIds = existingRelations.Select(x => x.ObjectId).ToList();

                // 找出需要删除的关系（存在于现有关系中但不在新授权列表中的）
                var userIdsToDelete = existingUserIds.Except(newUserIds).ToList();

                // 找出需要添加的关系（存在于新授权列表中但不在现有关系中的）
                var userIdsToAdd = newUserIds.Except(existingUserIds).ToList();

                // 批量删除需要删除的关系
                if (userIdsToDelete.Any())
                {
                    var relationsToDelete = existingRelations
                        .Where(x => userIdsToDelete.Contains(x.ObjectId))
                        .ToList();

                    foreach (var relation in relationsToDelete)
                    {
                        await _systemRelationRepository.DeleteAsync(relation.Id);
                    }
                }

                // 批量添加需要添加的关系
                if (userIdsToAdd.Any())
                {
                    var relationsToAdd = userIdsToAdd.Select(userId => new SystemRelation
                    {
                        ObjectId = userId,
                        Target = roleIdStr,
                        Category = category,
                    }).ToList();

                    await _systemRelationRepository.InsertRangeAsync(relationsToAdd);
                }

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("给角色授权用户失败", ex);
                return CommonResult.Error("给角色授权用户失败");
            }
        }


        /// <summary>
        /// 获取资源授权树
        /// </summary>
        public async Task<CommonResult<List<SysRoleGrantResourceTreeResult>>> GetResourceTreeSelectorAsync()
        {
            try
            {
                var query = _resourceRepository.GetAll()
                    .Where(x => x.Category == ResourceCategoryConstant.MODULE ||
                               x.Category == ResourceCategoryConstant.MENU ||
                               x.Category == ResourceCategoryConstant.BUTTON);

     

                var originData = await query.OrderBy(it => it.SortCode).ToListAsync();
                var result = await BuildResourceTree(originData);

                return CommonResult<List<SysRoleGrantResourceTreeResult>>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取资源授权树失败", ex);
                return CommonResult<List<SysRoleGrantResourceTreeResult>>.Error("获取资源授权树失败");
            }
        }


        /// <summary>
        /// 获取角色选择器
        /// </summary>
        public async Task<CommonResult<Page<RoleDto>>> GetRoleSelectorAsync(RoleSelectorRoleParam param)
        {
            try
            {
                var query = _roleRepository.GetAll()
                    .WhereIf(param.OrgId.HasValue, x => x.OrgId == param.OrgId)
                    .WhereIf(!string.IsNullOrEmpty(param.Category), x => x.Category == param.Category)
                    .WhereIf(!string.IsNullOrEmpty(param.SearchKey), x =>
                        x.Name.Contains(param.SearchKey) || x.DisplayName.Contains(param.SearchKey))
                    .WhereIf(param.DataScopeList != null && param.DataScopeList.Count > 0,
                        x => param.DataScopeList.Contains(x.OrgId ?? Guid.Empty))
                    .WhereIf(param.ExcludeSuperAdmin, x => x.NormalizedName != SysBuildInEnum.BUILD_IN_ROLE_CODE)
                    .OrderBy(x => x.CreationTime);

                var totalCount = await query.CountAsync();
                var items = await query
                    .Skip((param.Current - 1) * param.Size)
                    .Take(param.Size)
                    .ToListAsync();

                var page = new Page<RoleDto>
                {
                    Current = param.Current,
                    Size = param.Size,
                    Total = totalCount,
                    Records = ObjectMapper.Map<List<RoleDto>>(items)
                };

                return CommonResult<Page<RoleDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取角色选择器失败", ex);
                return CommonResult<Page<RoleDto>>.Error("获取角色选择器失败");
            }
        }


        private async Task<List<SysRoleGrantResourceTreeResult>> BuildResourceTree(List<Resource> originData)
        {
            var moduleList = originData.Where(x => x.Category == ResourceCategoryConstant.MODULE).ToList();
            var menuList = originData.Where(x => x.Category == ResourceCategoryConstant.MENU).ToList();
            var buttonList = originData.Where(x => x.Category == ResourceCategoryConstant.BUTTON).ToList();

            // 构建树形结构
            var treeNodes = menuList.Select(menu => new
            {
                Id = menu.Id,
                ParentId = menu.ParentId ?? Guid.Empty,
                Title = menu.Title,
                SortCode = menu.SortCode ?? 0,
                Menu = menu
            }).OrderBy(x => x.SortCode).ToList();

            // 按模块分组
            var menuGroups = menuList.Where(menu => menu.Module.HasValue)
                .GroupBy(menu => menu.Module.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<SysRoleGrantResourceTreeResult>();

            foreach (var module in moduleList)
            {
                if (!menuGroups.ContainsKey(module.Id))
                {
                    continue;
                }

                var moduleMenus = menuGroups[module.Id];
                var treeResult = new SysRoleGrantResourceTreeResult
                {
                    Id = module.Id,
                    Title = module.Title,
                    Icon = module.Icon,
                    Menu = new List<SysRoleGrantResourceTreeResult.SysRoleGrantResourceMenuResult>()
                };

                foreach (var menu in moduleMenus)
                {
                    // 排除目录类型的菜单
                    if (menu.MenuType == MenuTypeConstant.CATALOG)
                    {
                        continue;
                    }

                    var menuResult = new SysRoleGrantResourceTreeResult.SysRoleGrantResourceMenuResult
                    {
                        Id = menu.Id,
                        ParentId = menu.ParentId ?? Guid.Empty,
                        ParentName = await _menuAppService.GetMenuTitleById((Guid)menu.ParentId) ?? menu.Title, // 这里需要获取父级名称，简化处理
                        Title = menu.Title,
                        Module = menu.Module,
                        Button = buttonList.Where(b => b.ParentId == menu.Id)
                            .Select(b => new SysRoleGrantResourceTreeResult.SysRoleGrantResourceMenuResult
                                .SysRoleGrantResourceButtonResult
                            {
                                Id = b.Id,
                                Title = b.Title
                            }).ToList()
                    };

                    treeResult.Menu.Add(menuResult);
                }

                result.Add(treeResult);
            }

            return result;
        }


        private async Task<Role> GetEntityAsync(int id)
        {
            var role = await _roleRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (role == null)
            {
                throw new UserFriendlyException($"角色不存在");
            }
            return role;
        }


    }
}

