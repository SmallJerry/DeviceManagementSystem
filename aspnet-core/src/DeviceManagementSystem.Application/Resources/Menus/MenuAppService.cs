using Abp.Auditing;
using Abp.Collections.Extensions;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.UI;
using DeviceManagementSystem.Authorization.Resources;
using DeviceManagementSystem.Authorization.Users;
using DeviceManagementSystem.Resources.Menus.Constants;
using DeviceManagementSystem.Resources.Menus.Dto;
using DeviceManagementSystem.Resources.Modules.Constants;
using DeviceManagementSystem.Utils.Common;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Resources.Menus
{
    /// <summary>
    /// 菜单服务
    /// </summary>
    public class MenuAppService : DeviceManagementSystemAppServiceBase , IMenuAppService
    {

        private readonly IRepository<Resource, Guid> _resourceRepository;


        private readonly IRepository<SystemRelation, Guid> _systemRelationRepository;

        private readonly ICacheManager _cacheManager;

        private readonly IAbpSession _abpSession;

        /// <summary>
        /// 构造函数     
        /// </summary>
        /// <param name="resourceRepository"></param>
        /// <param name="systemRelationRepository"></param>
        /// <param name="cacheManager"></param>
        /// <param name="abpSession"></param>
        public MenuAppService(IRepository<Resource, Guid> resourceRepository, 
            IRepository<SystemRelation, Guid> systemRelationRepository,
            ICacheManager cacheManager, IAbpSession abpSession)
        {
            _resourceRepository = resourceRepository;
            _systemRelationRepository = systemRelationRepository;
            _cacheManager = cacheManager;
            _abpSession = abpSession;
        }





        /// <summary>
        /// 获取菜单分页
        /// </summary>
        /// <param name="input">分页参数</param>
        /// <returns>分页结果</returns>
        [DisableAuditing]
        public async Task<CommonResult<Page<Resource>>> GetPageList(MenuPageInput input)
        {
            try
            {
                var query = _resourceRepository.GetAll()
                    .Where(x => x.Category == ResourceCategoryConstant.MENU)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey), x => x.Title.Contains(input.SearchKey))
                    .WhereIf(input.Module != null, x => x.Module == input.Module);

                // 排序
                if (!string.IsNullOrWhiteSpace(input.SortField))
                {
                    var sortOrder = input.SortOrder?.ToUpper() == "DESCEND" ? "DESCEND" : "ASCEND";

                    query = sortOrder == "ASCEND"
                        ? query.OrderBy(x => x.SortCode)
                        : query.OrderByDescending(x => x.SortCode);
                }
                else
                {
                    query = query.OrderBy(x => x.SortCode);
                }

                var total = await query.CountAsync();
                var items = await query
                    .Select(x => new Resource
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        Title = x.Title,
                        Name = x.Name,
                        Category = x.Category,
                        Module = x.Module,
                        MenuType = x.MenuType,
                        Path = x.Path,
                        Component = x.Component,
                        Icon = x.Icon,
                        Color = x.Color,
                        Visible = x.Visible,
                        DisplayLayout = x.DisplayLayout,
                        SortCode = x.SortCode,
                        ExtJson = x.ExtJson,
                    })
                    .PageBy((input.Current - 1) * input.Size, input.Size)
                    .ToListAsync();

                var page = new Page<Resource>(input.Current, input.Size, total)
                {
                    Current = input.Current,
                    Records = items
                };

                return CommonResult<Page<Resource>>.Success(page);
            }
            catch (Exception ex)
            {
                return CommonResult<Page<Resource>>.Error("获取菜单分页失败:" + ex.Message);
            }
        }




        /// <summary>
        ///  获取菜单树
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CommonResult<List<object>>> GetTreeList(MenuTreeInput input)
        {
            try
            {
                var query = _resourceRepository.GetAll()
                    .Where(x => x.Category == ResourceCategoryConstant.MENU)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Module), x => string.Equals(x.Module.ToString(), input.Module))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey), x => x.Title.Contains(input.SearchKey))
                    .OrderBy(x => x.SortCode);

                var resourceList = await query.ToListAsync();

                // 构建树形结构
                var treeNodes = BuildMenuTreeNodes(resourceList);

                return CommonResult<List<object>>.Success(treeNodes);
            }
            catch (Exception ex)
            {
                Logger.Error("获取菜单树失败", ex);
                return CommonResult<List<object>>.Error("获取菜单树失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 根据ID获取菜单标题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetMenuTitleById(Guid id)
        {
            if(id == Guid.Empty)
            {
                return null;
            }
            var menu = await _resourceRepository.FirstOrDefaultAsync(id);
            return menu?.Title;
        }




        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="input">添加参数</param>
        /// <returns>操作结果</returns>
        public async Task<CommonResult> Create(MenuAddInput input)
        {
            try
            {
                // 参数验证
                var validationResult = ValidateMenuInput(input);
                if (!validationResult.IsValid)
                {
                    return CommonResult.Error(validationResult.ErrorMessage);
                }

                // 检查重复标题
                var exists = await _resourceRepository.GetAll()
                    .AnyAsync(x => x.ParentId == input.ParentId
                        && x.Module == input.Module
                        && x.Category == ResourceCategoryConstant.MENU
                        && x.Title == input.Title);

                if (exists)
                {
                    return CommonResult.Error($"同一模块中，相同父菜单下存在重复的子菜单，名称为：{input.Title}");
                }

                // 验证上级菜单
                if (input.ParentId != null && input.ParentId != Guid.Empty)
                {
                    var parentMenu = await GetResourceById(Guid.Parse(input.ParentId.ToString()));
                    if (parentMenu == null)
                    {
                        return CommonResult.Error($"上级菜单不存在，id值为：{input.ParentId}");
                    }

                    if (parentMenu.Module != input.Module)
                    {
                        return CommonResult.Error("module与上级菜单不一致");
                    }
                }

                var menu = new Resource
                {
                    ParentId = input.ParentId ?? Guid.Empty,
                    Title = input.Title,
                    Name = input.Name,
                    Category = ResourceCategoryConstant.MENU,
                    Module = input.Module,
                    MenuType = input.MenuType,
                    Path = input.Path,
                    Component = input.Component,
                    Icon = input.Icon,
                    Visible = string.Equals(input.Visible, "true") ? true : false,
                    DisplayLayout = input.DisplayLayout,
                    SortCode = input.SortCode,
                    ExtJson = input.ExtJson
                };

                await _resourceRepository.InsertAsync(menu);
                var cache = _cacheManager.GetCache("UserMenus");
                cache.Clear(); // 直接清空该缓存区域的所有内容
                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("添加菜单失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 编辑菜单
        /// </summary>
        /// <param name="input">编辑参数</param>
        /// <returns>操作结果</returns>
        public async Task<CommonResult> UpdateById(MenuEditInput input)
        {
            try
            {
                var menu = await GetMenuEntityById(input.Id);
                if (menu == null)
                {
                    return CommonResult.Error("菜单不存在");
                }

                // 参数验证
                var validationResult = ValidateMenuInput(input);
                if (!validationResult.IsValid)
                {
                    return CommonResult.Error(validationResult.ErrorMessage);
                }

                // 检查重复标题
                var exists = await _resourceRepository.GetAll()
                    .AnyAsync(x => x.ParentId == input.ParentId
                        && x.Module == input.Module
                        && x.Category == ResourceCategoryConstant.MENU
                        && x.Title == input.Title
                        && x.Id != input.Id);

                if (exists)
                {
                    return CommonResult.Error($"同一模块中，相同父菜单下存在重复的子菜单，名称为：{input.Title}");
                }

                // 检查层级关系（不能选择下级作为上级）
                var allMenus = await GetAllMenuResources();
                var childIds = GetChildResourceIds(allMenus, input.Id, true);
                if (childIds.Contains(input.ParentId.ToString()))
                {
                    var parentMenu = GetResourceById(allMenus, input.ParentId);
                    return CommonResult.Error($"不可选择上级菜单：{parentMenu?.Name}");
                }

                // 验证上级菜单
                if (input.ParentId != Guid.Empty)
                {
                    var parentMenu = GetResourceById(allMenus, input.ParentId);
                    if (parentMenu == null)
                    {
                        return CommonResult.Error($"上级菜单不存在，id值为：{input.ParentId}");
                    }

                    if (parentMenu.Module != input.Module)
                    {
                        return CommonResult.Error("module与上级菜单不一致");
                    }
                }

                // 更新菜单信息
                menu.ParentId = input.ParentId;
                menu.Title = input.Title;
                menu.Name = input.Name;
                menu.Module = input.Module;
                menu.MenuType = input.MenuType;
                menu.Path = input.Path;
                menu.Component = input.Component;
                menu.Icon = input.Icon;
                menu.Visible = string.Equals(input.Visible,"true") ? true : false;
                menu.DisplayLayout = input.DisplayLayout;
                menu.SortCode = input.SortCode;
                menu.ExtJson = input.ExtJson;

                await _resourceRepository.UpdateAsync(menu);
                var cache = _cacheManager.GetCache("UserMenus");
                cache.Clear(); // 直接清空该缓存区域的所有内容
                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("编辑菜单失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 更改菜单所属模块
        /// </summary>
        /// <param name="input">更改模块参数</param>
        /// <returns>操作结果</returns>
        public async Task<CommonResult> ChangeModule(MenuChangeModuleInput input)
        {
            try
            {
                var menu = await GetMenuEntityById(input.Id);
                if (menu == null)
                {
                    return CommonResult.Error("菜单不存在");
                }

                // 非顶级菜单不可修改所属模块
                if (menu.ParentId.HasValue && menu.ParentId.Value != Guid.Empty)
                {
                    return CommonResult.Error("非顶级菜单不可修改所属模块");
                }

                // 获取所有菜单资源
                var allMenus = await GetAllMenuResources();

                // 获取该菜单的所有子级（包括自身）
                var menuChildList = GetChildResources(allMenus, input.Id, true);

                // 更新所有子级的模块
                foreach (var childMenu in menuChildList)
                {
                    childMenu.Module = input.Module;

                    // 检查重复标题
                    var exists = await _resourceRepository.GetAll()
                        .AnyAsync(x => x.ParentId == childMenu.ParentId
                            && x.Module == childMenu.Module
                            && x.Category == ResourceCategoryConstant.MENU
                            && x.Title == childMenu.Title
                            && x.Id != childMenu.Id);

                    if (exists)
                    {
                        return CommonResult.Error($"同一模块中，相同父菜单下存在重复的子菜单，名称为：{childMenu.Title}");
                    }
                }

                // 批量更新
                await _resourceRepository.GetDbContext().BulkUpdateAsync(menuChildList);

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("更改菜单所属模块失败:" + ex.Message);
            }
        }





        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="menuIdInput">ID列表</param>
        /// <returns>操作结果</returns>
        public async Task<CommonResult> DeleteByIds([FromBody] List<MenuIdInput> menuIdInput)
        {
            try
            {
                if (menuIdInput == null || menuIdInput.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的菜单");
                }

                var idList = menuIdInput.Select(it => it.Id).ToList();

                // 获取所有菜单和按钮资源
                var allResources = await _resourceRepository.GetAll()
                    .Where(x => x.Category == ResourceCategoryConstant.MENU
                        || x.Category == ResourceCategoryConstant.BUTTON)
                    .ToListAsync();

                var toDeleteResourceIds = new List<string>();

                // 获取要删除的所有资源ID（包括子级）
                foreach (var id in idList)
                {
                    var childIds = GetChildResourceIds(allResources, id, true);
                    toDeleteResourceIds.AddRange(childIds);
                }

                toDeleteResourceIds = toDeleteResourceIds.Distinct().ToList();

                if (toDeleteResourceIds.Count > 0)
                {
                    // 清除对应的角色与资源关系
                    await _systemRelationRepository.DeleteAsync(x =>
                        toDeleteResourceIds.Contains(x.Target)
                        && x.Category == SysRelationCategoryConstant.SYS_ROLE_HAS_RESOURCE);

                    // 执行删除
                    foreach (var resourceId in toDeleteResourceIds)
                    {
                        await _resourceRepository.DeleteAsync(Guid.Parse(resourceId));
                    }
                }


                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("删除菜单失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 获取菜单详情     
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <returns>菜单详情</returns>
        public async Task<CommonResult<Resource>> GetById(string id)
        {
            try
            {
                var menu = await GetMenuEntityById(Guid.Parse(id));
                if (menu == null)
                {
                    return CommonResult<Resource>.Error("菜单不存在");
                }

                return CommonResult<Resource>.Success(menu);
            }
            catch (Exception ex)
            {
                return CommonResult<Resource>.Error("获取菜单详情失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 获取模块选择器
        /// </summary>  
        /// <param name="input">选择器参数</param>
        /// <returns>模块列表</returns>
        public async Task<CommonResult<List<Resource>>> GetModuleSelector(MenuSelectorModuleInput input)
        {
            try
            {
                var query = _resourceRepository.GetAll()
                    .Where(x => x.Category == ResourceCategoryConstant.MODULE)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey), x => x.Title.Contains(input.SearchKey))
                    .OrderBy(x => x.SortCode);

                var modules = await query.ToListAsync();

                return CommonResult<List<Resource>>.Success(modules);
            }
            catch (Exception ex)
            {
                return CommonResult<List<Resource>>.Error("获取模块选择器失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 获取菜单树选择器
        /// </summary>
        /// <param name="input">选择器参数</param>
        /// <returns>菜单树选择器结果</returns>
        public async Task<CommonResult<List<object>>> GetMenuTreeSelector(MenuSelectorMenuInput input)
        {
            try
            {
                var query = _resourceRepository.GetAll()
                    .Where(x => x.Category == ResourceCategoryConstant.MENU)
                    .Where(x => string.Equals(x.Module,input.Module))
                    .OrderBy(x => x.SortCode);

                var resourceList = await query.ToListAsync();

                // 构建树形结构
                var treeNodes = BuildMenuTreeSelectorNodes(resourceList);

                return CommonResult<List<object>>.Success(treeNodes);
            }
            catch (Exception ex)
            {
                Logger.Error("获取菜单树选择器失败", ex);
                return CommonResult<List<object>>.Error("获取菜单树选择器失败:" + ex.Message);
            }
        }


        #region 辅助方法


        /// <summary>
        /// 构建菜单树选择器节点
        /// </summary>
        private List<object> BuildMenuTreeSelectorNodes(List<Resource> resources)
        {
            // 构建树形结构，选择器只需要基本字段
            var tree = BuildTreeSelectorRecursive(resources, Guid.Empty.ToString());
            return tree;
        }

        /// <summary>
        /// 递归构建树选择器
        /// </summary>
        private List<object> BuildTreeSelectorRecursive(List<Resource> resources, string parentId)
        {
            // 1. 获取指定父节点的所有子节点
            var childResources = GetChildResources(resources, parentId);

            // 2. 如果没有子节点，返回空列表
            if (!childResources.Any())
            {
                return new List<object>();
            }

            // 3. 构建节点列表
            var nodes = new List<object>();

            foreach (var resource in childResources)
            {
                // 4. 创建选择器节点（简化字段）
                var node = CreateTreeSelectorNode(resource);

                // 5. 递归获取子节点
                var children = BuildTreeSelectorRecursive(resources, resource.Id.ToString());

                // 6. 如果有子节点，添加到当前节点
                if (children.Any())
                {
                    node["children"] = children;
                }

                nodes.Add(node);
            }

            return nodes;
        }

        /// <summary>
        /// 创建树选择器节点
        /// </summary>
        private Dictionary<string, object> CreateTreeSelectorNode(Resource resource)
        {
            var node = new Dictionary<string, object>
            {
                ["id"] = resource.Id.ToString(),
                ["parentId"] = resource.ParentId?.ToString() ?? Guid.Empty.ToString(),
                ["title"] = resource.Title ?? string.Empty,
                ["name"] = resource.Name ?? string.Empty,
                ["category"] = resource.Category ?? string.Empty,
                ["module"] = resource.Module.ToString() ?? Guid.Empty.ToString(),
                ["menuType"] = resource.MenuType ?? string.Empty,
                ["path"] = resource.Path ?? string.Empty,
                ["component"] = resource.Component ?? string.Empty,
                ["icon"] = resource.Icon ?? string.Empty,
                ["color"] = resource.Color ?? string.Empty,
                ["visible"] = resource.Visible,
                ["displayLayout"] = resource.DisplayLayout ?? string.Empty,
                ["sortCode"] = resource.SortCode,
                ["extJson"] = resource.ExtJson ?? string.Empty,
                ["creationTime"] = resource.CreationTime
            };

            return node;
        }

      

        /// <summary>
        /// 构建菜单树节点（包含所有字段）
        /// </summary>
        private List<object> BuildMenuTreeNodes(List<Resource> resources)
        {
            // 构建完整树结构
            var tree = BuildTreeRecursive(resources, Guid.Empty.ToString());
            return tree;
        }

        /// <summary>
        /// 递归构建树
        /// </summary>
        private List<object> BuildTreeRecursive(List<Resource> resources, string parentId)
        {
            // 1. 获取指定父节点的所有子节点
            var childResources = GetChildResources(resources, parentId);

            // 2. 如果没有子节点，返回空列表
            if (!childResources.Any())
            {
                return new List<object>();
            }

            // 3. 构建节点列表
            var nodes = new List<object>();

            foreach (var resource in childResources)
            {
                // 4. 创建节点
                var node = CreateTreeNode(resource);

                // 5. 递归获取子节点
                var children = BuildTreeRecursive(resources, resource.Id.ToString());

                // 6. 如果有子节点，添加到当前节点
                if (children.Any())
                {
                    node["children"] = children;
                }

                nodes.Add(node);
            }

            return nodes;
        }

        /// <summary>
        /// 获取指定父节点的所有子节点（已排序）
        /// </summary>
        private List<Resource> GetChildResources(List<Resource> resources, string parentId)
        {
            // 将parentId转换为Guid?类型进行比较
            Guid? parentGuid = ConvertToNullableGuid(parentId);

            // 根据parentGuid是否为Guid.Empty来筛选子节点
            if (parentGuid == Guid.Empty)
            {
                // 获取顶级节点：ParentId为null或Guid.Empty
                return resources
                    .Where(x => x.ParentId == null || x.ParentId == Guid.Empty)
                    .OrderBy(x => x.SortCode)
                    .ToList();
            }
            else
            {
                // 获取指定父节点的子节点
                return resources
                    .Where(x => x.ParentId == parentGuid)
                    .OrderBy(x => x.SortCode)
                    .ToList();
            }
        }

        /// <summary>
        /// 创建树节点
        /// </summary>
        private Dictionary<string, object> CreateTreeNode(Resource resource)
        {
            var node = new Dictionary<string, object>();

            // 添加字段
            AddBasicFields(node, resource);
            return node;
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        private void AddBasicFields(Dictionary<string, object> node, Resource resource)
        {
            node["id"] = resource.Id.ToString();
            node["parentId"] = resource.ParentId?.ToString() ?? Guid.Empty.ToString();
            node["title"] = resource.Title ?? string.Empty;
            node["name"] = resource.Name ?? string.Empty;
            node["category"] = resource.Category ?? string.Empty;
            node["module"] = resource.Module.ToString() ?? Guid.Empty.ToString();
            node["menuType"] = resource.MenuType ?? string.Empty;
            node["path"] = resource.Path ?? string.Empty;
            node["component"] = resource.Component ?? string.Empty;
            node["icon"] = resource.Icon ?? string.Empty;
            node["color"] = resource.Color ?? string.Empty;
            node["visible"] = resource.Visible;
            node["displayLayout"] = resource.DisplayLayout ?? string.Empty;
            node["sortCode"] = resource.SortCode;
            node["extJson"] = resource.ExtJson ?? string.Empty;
            node["weight"] = resource.SortCode;
            node["code"] = resource.Name ?? string.Empty;
            node["createTime"] = resource.CreationTime;
        }



        /// <summary>
        /// 将字符串转换为可空Guid
        /// </summary>
        private static Guid? ConvertToNullableGuid(string id)
        {
            if (string.IsNullOrEmpty(id) || id == Guid.Empty.ToString())
            {
                return Guid.Empty;
            }

            if (Guid.TryParse(id, out Guid result))
            {
                return result;
            }

            return Guid.Empty;
        }



        /// <summary>
        /// 根据ID获取菜单实体
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <returns>菜单实体</returns>
        private async Task<Resource> GetMenuEntityById(Guid id)
        {
            var menu = await _resourceRepository.FirstOrDefaultAsync(id);
            if (menu == null)
            {
                throw new UserFriendlyException($"菜单不存在，id值为：{id}");
            }

            // 验证是否为菜单类型
            if (menu.Category != ResourceCategoryConstant.MENU)
            {
                throw new UserFriendlyException("该资源不是菜单类型");
            }

            return menu;
        }

        /// <summary>
        /// 根据ID获取资源实体
        /// </summary>
        /// <param name="id">资源ID</param>
        /// <returns>资源实体</returns>
        private async Task<Resource> GetResourceById(Guid id)
        {
            return await _resourceRepository.FirstOrDefaultAsync(id);
        }

        /// <summary>
        /// 从列表中获取资源实体
        /// </summary>
        /// <param name="resources">资源列表</param>
        /// <param name="id">资源ID</param>
        /// <returns>资源实体</returns>
        private Resource GetResourceById(List<Resource> resources, Guid? id)
        {
            return resources.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 获取所有菜单资源
        /// </summary>
        /// <returns>菜单资源列表</returns>
        private async Task<List<Resource>> GetAllMenuResources()
        {
            return await _resourceRepository.GetAll()
                .Where(x => x.Category == ResourceCategoryConstant.MENU)
                .OrderBy(x => x.SortCode)
                .ToListAsync();
        }

        /// <summary>
        /// 验证菜单输入参数
        /// </summary>
        /// <typeparam name="T">输入参数类型</typeparam>
        /// <param name="input">输入参数</param>
        /// <returns>验证结果</returns>
        private (bool IsValid, string ErrorMessage) ValidateMenuInput<T>(T input)
        {
            string menuType = string.Empty;
            string title = string.Empty;

            if (input is MenuAddInput addInput)
            {
                menuType = addInput.MenuType;
                title = addInput.Title;
            }
            else if (input is MenuEditInput editInput)
            {
                menuType = editInput.MenuType;
                title = editInput.Title;
            }

            // 验证菜单类型
            if (!MenuConstant.IsValidMenuType(menuType))
            {
                return (false, $"不支持的菜单类型：{menuType}");
            }

            // 验证标题是否包含特殊字符
            if (MenuConstant.IsTitleContainsSpecialChars(title))
            {
                return (false, "title不可包含特殊字符【-】");
            }

            // 验证菜单类型相关字段
            if (menuType == MenuConstant.MENU_TYPE_MENU)
            {
                if (input is MenuAddInput add && string.IsNullOrEmpty(add.Name))
                {
                    return (false, "name不能为空");
                }
                if (input is MenuEditInput edit && string.IsNullOrEmpty(edit.Name))
                {
                    return (false, "name不能为空");
                }
                if (input is MenuAddInput addMenu && string.IsNullOrEmpty(addMenu.Component))
                {
                    return (false, "component不能为空");
                }
                if (input is MenuEditInput editMenu && string.IsNullOrEmpty(editMenu.Component))
                {
                    return (false, "component不能为空");
                }
            }
            else if (menuType == MenuConstant.MENU_TYPE_IFRAME || menuType == MenuConstant.MENU_TYPE_LINK)
            {
                if (input is MenuAddInput add && string.IsNullOrEmpty(add.Name))
                {
                    // 生成随机名称
                    var random = new Random();
                    add.Name = random.Next(1000000000).ToString("D10");
                }
                if (input is MenuEditInput edit && string.IsNullOrEmpty(edit.Name))
                {
                    var random = new Random();
                    edit.Name = random.Next(1000000000).ToString("D10");
                }

                // 设置组件为空
                if (input is MenuAddInput addLink)
                {
                    addLink.Component = null;
                }
                if (input is MenuEditInput editLink)
                {
                    editLink.Component = null;
                }
            }
            else // CATALOG
            {
                if (input is MenuAddInput addCatalog)
                {
                    addCatalog.Name = null;
                    addCatalog.Component = null;
                }
                if (input is MenuEditInput editCatalog)
                {
                    editCatalog.Name = null;
                    editCatalog.Component = null;
                }
            }

            return (true, string.Empty);
        }


        /// <summary>
        /// 获取资源的所有子级ID
        /// </summary>
        /// <param name="allResources">所有资源列表</param>
        /// <param name="parentId">父级ID</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>子级ID列表</returns>
        private List<string> GetChildResourceIds(List<Resource> allResources, Guid parentId, bool includeSelf)
        {
            var result = new List<string>();

            if (includeSelf)
            {
                result.Add(parentId.ToString());
            }

            GetChildResourceIdsRecursive(allResources, parentId, result);
            return result;
        }

        /// <summary>
        /// 递归获取子级资源ID
        /// </summary>
        private void GetChildResourceIdsRecursive(List<Resource> allResources, Guid parentId, List<string> result)
        {
            var children = allResources.Where(x => x.ParentId == parentId).ToList();
            foreach (var child in children)
            {
                result.Add(child.Id.ToString());
                GetChildResourceIdsRecursive(allResources, child.Id, result);
            }
        }

        /// <summary>
        /// 获取资源的子级列表
        /// </summary>
        /// <param name="allResources">所有资源列表</param>
        /// <param name="parentId">父级ID</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>子级资源列表</returns>
        private List<Resource> GetChildResources(List<Resource> allResources, Guid parentId, bool includeSelf)
        {
            var result = new List<Resource>();

            if (includeSelf)
            {
                var self = GetResourceById(allResources, parentId);
                if (self != null)
                {
                    result.Add(self);
                }
            }

            GetChildResourcesRecursive(allResources, parentId, result);
            return result;
        }

        /// <summary>
        /// 递归获取子级资源
        /// </summary>
        private void GetChildResourcesRecursive(List<Resource> allResources, Guid parentId, List<Resource> result)
        {
            var children = allResources.Where(x => x.ParentId == parentId).ToList();
            result.AddRange(children);

            foreach (var child in children)
            {
                GetChildResourcesRecursive(allResources, child.Id, result);
            }
        }


        /// <summary>
        /// 递归获取父级资源
        /// </summary>
        private void GetParentResourcesRecursive(List<Resource> allResources, Guid id, List<Resource> result)
        {
            var resource = GetResourceById(allResources, id);
            if (resource != null && resource.ParentId.HasValue && resource.ParentId.Value != Guid.Empty)
            {
                var parent = GetResourceById(allResources, resource.ParentId.Value);
                if (parent != null)
                {
                    result.Add(parent);
                    GetParentResourcesRecursive(allResources, resource.ParentId.Value, result);
                }
            }
        }

        #endregion
    }
}
