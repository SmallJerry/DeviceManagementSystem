using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using DeviceManagementSystem.Authorization.Resources;
using DeviceManagementSystem.Resources.Buttons.Constants;
using DeviceManagementSystem.Resources.Buttons.Dto;
using DeviceManagementSystem.Resources.Menus;
using DeviceManagementSystem.Resources.Menus.Constants;
using DeviceManagementSystem.Resources.Modules.Constants;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Resources.Buttons
{
    /// <summary>
    /// 按钮服务类
    /// </summary>
    public class ButtonAppService : DeviceManagementSystemAppServiceBase
    {


        private readonly IRepository<Resource, Guid> _resourceRepository;
        private readonly IRepository<SystemRelation, Guid> _systemRelationRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ButtonAppService(
            IRepository<Resource, Guid> resourceRepository,
            IRepository<SystemRelation, Guid> systemRelationRepository)
        {
            _resourceRepository = resourceRepository;
            _systemRelationRepository = systemRelationRepository;
        }



        /// <summary>
        /// 获取按钮分页
        /// </summary>
        /// <param name="input">分页参数</param>
        /// <returns>分页结果</returns>
        [UnitOfWork]
        [DisableAuditing]
        public async Task<CommonResult<Page<Resource>>> GetPageList(ButtonPageInput input)
        {
            try
            {
                var query = _resourceRepository.GetAll()
                    .Where(x => x.Category == ResourceCategoryConstant.BUTTON)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.ParentId), x => x.ParentId == Guid.Parse(input.ParentId))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey), x => x.Title.Contains(input.SearchKey));

                // 排序处理
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
                        Category = x.Category,
                        SortCode = x.SortCode,
                        ExtJson = x.ExtJson,
                        Code = x.Code
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
                return CommonResult<Page<Resource>>.Error("获取按钮分页失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="input">添加参数</param>
        /// <returns>操作结果</returns>
        [UnitOfWork]
        public async Task<CommonResult> Create(ButtonAddInput input)
        {
            try
            {
                // 检查重复编码
                var exists = await _resourceRepository.GetAll()
                    .AnyAsync(x => x.Category == ResourceCategoryConstant.BUTTON
                        && x.Code == input.Code);

                if (exists)
                {
                    return CommonResult.Error($"存在重复的按钮，编码为：{input.Code}");
                }

                var button = new Resource
                {
                    ParentId = Guid.Parse(input.ParentId),
                    Title = input.Title,
                    Code = input.Code,
                    Category = ResourceCategoryConstant.BUTTON,
                    SortCode = input.SortCode,
                    ExtJson = input.ExtJson
                };

                await _resourceRepository.InsertAsync(button);
                await CurrentUnitOfWork.SaveChangesAsync();

           
                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("添加按钮失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 编辑按钮
        /// </summary>
        /// <param name="input">编辑参数</param>
        /// <returns>操作结果</returns>
        [UnitOfWork]
        public async Task<CommonResult> UpdateById(ButtonEditInput input)
        {
            try
            {
                var button = await GetButtonEntityById(Guid.Parse(input.Id));
                if (button == null)
                {
                    return CommonResult.Error("按钮不存在");
                }

                // 检查重复编码
                var exists = await _resourceRepository.GetAll()
                    .AnyAsync(x => x.Category == ResourceCategoryConstant.BUTTON
                        && x.Code == input.Code
                        && x.Id != Guid.Parse(input.Id));

                if (exists)
                {
                    return CommonResult.Error($"存在重复的按钮，编码为：{input.Code}");
                }

                // 更新按钮信息
                button.ParentId = Guid.Parse(input.ParentId);
                button.Title = input.Title;
                button.Code = input.Code;
                button.SortCode = input.SortCode;
                button.ExtJson = input.ExtJson;

                await _resourceRepository.UpdateAsync(button);


                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("编辑按钮失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="ids">ID列表</param>
        /// <returns>操作结果</returns>
        [UnitOfWork]
        public async Task<CommonResult> DeleteByIds([FromBody] List<string> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的按钮");
                }

                var buttonIds = ids.Select(Guid.Parse).ToList();

                // 获取按钮的父菜单ID集合
                var parentMenuIds = await _resourceRepository.GetAll()
                    .Where(x => buttonIds.Contains(x.Id) && x.Category == ResourceCategoryConstant.BUTTON)
                    .Select(x => x.ParentId)
                    .Where(pid => pid.HasValue)
                    .Select(pid => pid.Value)
                    .Distinct()
                    .ToListAsync();

                if (parentMenuIds.Count > 0)
                {
                    // 获取相关的关系记录
                    var relations = await _systemRelationRepository.GetAll()
                        .Where(x => parentMenuIds.Contains(Guid.Parse(x.Target))
                            && x.Category == SysRelationCategoryConstant.SYS_ROLE_HAS_RESOURCE
                            && !string.IsNullOrEmpty(x.ExtJson))
                        .ToListAsync();

                    foreach (var relation in relations)
                    {
                        try
                        {
                            // 解析扩展JSON
                            var extJson = JsonDocument.Parse(relation.ExtJson);
                            if (extJson.RootElement.TryGetProperty(ButtonConstant.RELATION_EXT_JSON_BUTTON_INFO, out var buttonInfoElement))
                            {
                                var buttonInfoList = buttonInfoElement.EnumerateArray()
                                    .Select(x => x.GetString())
                                    .Where(x => !string.IsNullOrEmpty(x))
                                    .ToList();

                                if (buttonInfoList.Count > 0)
                                {
                                    // 找出交集（要删除的按钮ID）
                                    var intersection = buttonInfoList
                                        .Where(x => ids.Contains(x))
                                        .ToList();

                                    if (intersection.Count > 0)
                                    {
                                        // 计算差集（保留的按钮ID）
                                        var remainingButtons = buttonInfoList
                                            .Except(intersection)
                                            .ToList();

                                        // 更新扩展JSON
                                        var newExtJson = JsonDocument.Parse(relation.ExtJson);
                                        var newRoot = newExtJson.RootElement.Clone();

                                        var options = new JsonSerializerOptions
                                        {
                                            WriteIndented = true,
                                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                        };

                                        var updatedExtJson = JsonSerializer.Serialize(new
                                        {
                                            ButtonInfo = remainingButtons
                                        }, options);

                                        relation.ExtJson = updatedExtJson;
                                        await _systemRelationRepository.UpdateAsync(relation);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn($"处理关系扩展JSON失败，关系ID：{relation.Id}", ex);
                            // 继续处理其他关系
                        }
                    }

                    // 执行删除
                    foreach (var buttonId in buttonIds)
                    {
                        await _resourceRepository.DeleteAsync(buttonId);
                    }


                    return CommonResult.Ok();
                }
                else
                {
                    // 没有父菜单，直接删除
                    foreach (var buttonId in buttonIds)
                    {
                        await _resourceRepository.DeleteAsync(buttonId);
                    }

                    return CommonResult.Ok();
                }
            }
            catch (Exception ex)
            {
                return CommonResult.Error("删除按钮失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 获取按钮详情     
        /// </summary>
        /// <param name="id">按钮ID</param>
        /// <returns>按钮详情</returns>
        [UnitOfWork]
        [DisableAuditing]
        public async Task<CommonResult<Resource>> GetById(string id)
        {
            try
            {
                var button = await GetButtonEntityById(Guid.Parse(id));
                if (button == null)
                {
                    return CommonResult<Resource>.Error("按钮不存在");
                }

                return CommonResult<Resource>.Success(button);
            }
            catch (Exception ex)
            {
                return CommonResult<Resource>.Error("获取按钮详情失败:" + ex.Message);
            }
        }




        #region 辅助方法

        /// <summary>
        /// 根据ID获取按钮实体
        /// </summary>
        /// <param name="id">按钮ID</param>
        /// <returns>按钮实体</returns>
        private async Task<Resource> GetButtonEntityById(Guid id)
        {
            var button = await _resourceRepository.FirstOrDefaultAsync(id);
            if (button == null)
            {
                throw new UserFriendlyException($"按钮不存在，id值为：{id}");
            }

            // 验证是否为按钮类型
            if (button.Category != ResourceCategoryConstant.BUTTON)
            {
                throw new UserFriendlyException("该资源不是按钮类型");
            }

            return button;
        }




        /// <summary>
        /// 获取按钮实体（从列表中）
        /// </summary>
        /// <param name="resources">资源列表</param>
        /// <param name="id">资源ID</param>
        /// <returns>资源实体</returns>
        public Resource GetResourceById(List<Resource> resources, Guid id)
        {
            return resources.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 验证按钮编码格式
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns>是否有效</returns>
        public bool ValidateButtonCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            // 编码只能包含字母、数字和下划线
            var regex = new Regex(@"^[a-zA-Z0-9_]+$");
            return regex.IsMatch(code);
        }

        /// <summary>
        /// 根据编码获取按钮
        /// </summary>
        /// <param name="code">按钮编码</param>
        /// <returns>按钮实体</returns>
        [UnitOfWork]
        public async Task<CommonResult<Resource>> GetButtonByCode(string code)
        {
            try
            {
                var button = await _resourceRepository.GetAll()
                    .Where(x => x.Category == ResourceCategoryConstant.BUTTON && x.Code == code)
                    .FirstOrDefaultAsync();

                if (button == null)
                {
                    return CommonResult<Resource>.Error($"按钮不存在，编码：{code}");
                }

                return CommonResult<Resource>.Success(button);
            }
            catch (Exception ex)
            {
                return CommonResult<Resource>.Error("根据编码获取按钮失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 根据父菜单ID获取按钮列表
        /// </summary>
        /// <param name="parentMenuId">父菜单ID</param>
        /// <returns>按钮列表</returns>
        [UnitOfWork]
        public async Task<CommonResult<List<Resource>>> GetButtonsByParentMenuId(string parentMenuId)
        {
            try
            {
                var buttons = await _resourceRepository.GetAll()
                    .Where(x => x.Category == ResourceCategoryConstant.BUTTON
                        && x.ParentId == Guid.Parse(parentMenuId))
                    .OrderBy(x => x.SortCode)
                    .ThenBy(x => x.Title)
                    .ToListAsync();

                return CommonResult<List<Resource>>.Success(buttons);
            }
            catch (Exception ex)
            {
                return CommonResult<List<Resource>>.Error("根据父菜单ID获取按钮列表失败:" + ex.Message);
            }
        }

        #endregion
    }
}
