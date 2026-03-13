using Abp.Auditing;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using DeviceManagementSystem.Authorization.Resources;
using DeviceManagementSystem.Resources.Modules.Constants;
using DeviceManagementSystem.Resources.Modules.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Resources.Modules
{
    /// <summary>
    /// 模块服务类接口
    /// </summary>
    public class ModuleAppService : DeviceManagementSystemAppServiceBase
    {

        private readonly IRepository<Resource, Guid> _resourceRepository;


        private readonly IRepository<SystemRelation, Guid> _systemRelationRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="resourceRepository"></param>
        /// <param name="systemRelationRepository"></param>
        public ModuleAppService(IRepository<Resource,Guid> resourceRepository, IRepository<SystemRelation, Guid> systemRelationRepository)
        {
            _resourceRepository = resourceRepository;
            _systemRelationRepository = systemRelationRepository;
        }




        /// <summary>
        /// 获取模块分页
        /// </summary>
        /// <param name="input">分页参数</param>
        /// <returns>分页结果</returns>
        [DisableAuditing]
        public async Task<CommonResult<Page<Resource>>> GetPageList(ModulePageInput input)
        {
            try
            {
                var query = _resourceRepository.GetAll()
                    .Where(x => x.Category == ResourceCategoryConstant.MODULE)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey), x => x.Title.Contains(input.SearchKey));

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
                        Title = x.Title,
                        Category = x.Category,
                        Icon = x.Icon,
                        Color = x.Color,
                        SortCode = x.SortCode,
                        ExtJson = x.ExtJson
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
                return CommonResult<Page<Resource>>.Error("获取模块分页失败：" + ex.Message);
            }
        }




        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="input">添加参数</param>
        /// <returns>操作结果</returns>
        public async Task<CommonResult> Create(ModuleAddInput input)
        {
            try
            {
                // 检查重复标题
                var exists = await _resourceRepository.GetAll()
                    .AnyAsync(x => x.Category == ResourceCategoryConstant.MODULE
                        && x.Title == input.Title);

                if (exists)
                {
                    return CommonResult.Error($"存在重复的模块，名称为：{input.Title}");
                }

                var module = new Resource
                {
                    Title = input.Title,
                    Icon = input.Icon,
                    Color = input.Color,
                    SortCode = input.SortCode,
                    ExtJson = input.ExtJson,
                    Category = ResourceCategoryConstant.MODULE
                };

                await _resourceRepository.InsertAsync(module);
                await CurrentUnitOfWork.SaveChangesAsync();
                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("添加模块失败:" + ex.Message);
            }
        }





        /// <summary>
        /// 编辑模块
        /// </summary>
        /// <param name="input">编辑参数</param>
        /// <returns>操作结果</returns>
        public async Task<CommonResult> UpdateById(ModuleEditInput input)
        {
            try
            {
                var module = await GetModuleEntityById(input.Id);
                if (module == null)
                {
                    return CommonResult.Error("模块不存在");
                }

                // 检查重复标题
                var exists = await _resourceRepository.GetAll()
                    .AnyAsync(x => x.Category == ResourceCategoryConstant.MODULE
                        && x.Title == input.Title
                        && x.Id != input.Id);

                if (exists)
                {
                    return CommonResult.Error($"存在重复的模块，名称为：{input.Title}");
                }

                // 更新模块信息
                module.Title = input.Title;
                module.Icon = input.Icon;
                module.Color = input.Color;
                module.SortCode = input.SortCode;
                module.ExtJson = input.ExtJson;

                await _resourceRepository.UpdateAsync(module);


                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("编辑模块失败：" + ex.Message);
            }
        }


        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="ids">ID列表</param>
        /// <returns>操作结果</returns>
        public async Task<CommonResult> DeleteByIds([FromBody] List<Guid> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的模块");
                }

                // 获取要删除的模块列表
                var modulesToDelete = await _resourceRepository.GetAll()
                    .Where(x => ids.Contains(x.Id) && x.Category == ResourceCategoryConstant.MODULE)
                    .ToListAsync();


                // 获取所有资源列表（用于查找相关菜单和按钮）
                var allResources = await _resourceRepository.GetAll()
                    .Where(x => x.Category == ResourceCategoryConstant.MENU.ToString()
                        || x.Category == ResourceCategoryConstant.BUTTON.ToString())
                    .ToListAsync();

                var toDeleteResourceIds = new List<Guid>(ids);

                // 查找模块下的菜单和按钮
                var moduleResources = allResources
                    .Where(x => x.Module != null  && ids.Contains(Guid.Parse(x.Module.ToString())))
                    .ToList();

                if (moduleResources.Count > 0)
                {
                    foreach (var resource in moduleResources)
                    {
                        // 获取资源的所有子级（包括自身）
                        var childResourceIds = GetChildResourceIds(allResources, resource.Id, true);
                        toDeleteResourceIds.AddRange(childResourceIds);
                    }
                }

                toDeleteResourceIds = toDeleteResourceIds.Distinct().ToList();

                if (toDeleteResourceIds.Count > 0)
                {
                    // 清除对应的角色与资源关系
                    await _systemRelationRepository.DeleteAsync(x =>
                        toDeleteResourceIds.Contains(x.Id));

                    // 执行删除
                    foreach (var resourceId in toDeleteResourceIds)
                    {
                        await _resourceRepository.DeleteAsync(resourceId);
                    }
                }

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("删除模块失败：" + ex.Message);
            }
        }


        /// <summary>
        /// 获取模块详情     
        /// </summary>
        /// <param name="id">模块ID</param>
        /// <returns>模块详情</returns>
        public async Task<CommonResult<Resource>> GetById(string id)
        {
            try
            {
                var module = await GetModuleEntityById(Guid.Parse(id));
                if (module == null)
                {
                    return CommonResult<Resource>.Error("模块不存在");
                }

                return CommonResult<Resource>.Success(module);
            }
            catch (Exception ex)
            {
                return CommonResult<Resource>.Error("获取模块详情失败:" + ex.Message);
            }
        }


        #region 辅助方法

        /// <summary>
        /// 根据ID获取模块实体
        /// </summary>
        /// <param name="id">模块ID</param>
        /// <returns>模块实体</returns>
        private async Task<Resource> GetModuleEntityById(Guid id)
        {
            var module = await _resourceRepository.FirstOrDefaultAsync(id);
            if (module == null)
            {
                throw new UserFriendlyException($"模块不存在，id值为：{id}");
            }

            // 验证是否为模块类型
            if (module.Category != ResourceCategoryConstant.MODULE.ToString())
            {
                throw new UserFriendlyException("该资源不是模块类型");
            }

            return module;
        }


        /// <summary>
        /// 获取资源的所有子级ID
        /// </summary>
        /// <param name="allResources">所有资源列表</param>
        /// <param name="parentId">父级ID</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>子级ID列表</returns>
        private List<Guid> GetChildResourceIds(List<Resource> allResources, Guid parentId, bool includeSelf)
        {
            var result = new List<Guid>();

            if (includeSelf)
            {
                result.Add(parentId);
            }

            GetChildResourceIdsRecursive(allResources, parentId, result);
            return result;
        }

        /// <summary>
        /// 递归获取子级资源ID
        /// </summary>
        private void GetChildResourceIdsRecursive(List<Resource> allResources, Guid parentId, List<Guid> result)
        {
            var children = allResources.Where(x => x.ParentId == parentId).ToList();
            foreach (var child in children)
            {
                result.Add(child.Id);
                GetChildResourceIdsRecursive(allResources, child.Id, result);
            }
        }


        /// <summary>
        /// 从列表中获取资源实体
        /// </summary>
        /// <param name="resources">资源列表</param>
        /// <param name="id">资源ID</param>
        /// <returns>资源实体</returns>
        public Resource GetResourceById(List<Resource> resources, Guid id)
        {
            return resources.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 获取资源的子级列表
        /// </summary>
        /// <param name="resources">资源列表</param>
        /// <param name="parentId">父级ID</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>子级资源列表</returns>
        public List<Resource> GetChildResources(List<Resource> resources, Guid parentId, bool includeSelf)
        {
            var result = new List<Resource>();

            if (includeSelf)
            {
                var self = GetResourceById(resources, parentId);
                if (self != null)
                {
                    result.Add(self);
                }
            }

            GetChildResourcesRecursive(resources, parentId, result);
            return result;
        }

        /// <summary>
        /// 递归获取子级资源
        /// </summary>
        private void GetChildResourcesRecursive(List<Resource> resources, Guid parentId, List<Resource> result)
        {
            var children = resources.Where(x => x.ParentId == parentId).ToList();
            result.AddRange(children);

            foreach (var child in children)
            {
                GetChildResourcesRecursive(resources, child.Id, result);
            }
        }

        #endregion
    }
}
