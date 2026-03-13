using Abp.Auditing;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using DeviceManagementSystem.Authorization.Organizations;
using DeviceManagementSystem.Authorization.Positions;
using DeviceManagementSystem.Authorization.Users;
using DeviceManagementSystem.Organizations;
using DeviceManagementSystem.Organizations.Dto;
using DeviceManagementSystem.Positions.Dto;
using DeviceManagementSystem.Users.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Positions
{
    /// <summary>
    /// 职位服务类接口
    /// </summary>
    [AbpAuthorize]
    public class PositionAppService : DeviceManagementSystemAppServiceBase , IPositionAppService
    {

        private readonly IRepository<Position, Guid> _positionRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Organization, Guid> _organizationRepository;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="positionRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="organizationRepository"></param>
        public PositionAppService(IRepository<Position, Guid> positionRepository,IRepository<User,long> userRepository, IRepository<Organization, Guid> organizationRepository)
        {
            _positionRepository = positionRepository;
            _userRepository = userRepository;
            _organizationRepository = organizationRepository;
        }



        /// <summary>
        /// 获取职位分页
        /// </summary>
        /// <param name="input">分页参数</param>
        /// <returns>分页结果</returns>
        [UnitOfWork]
        [DisableAuditing]
        public async Task<CommonResult<Page<Position>>> GetPageList(PositionPageInput input)
        {
            try
            {
                var query = _positionRepository.GetAll().AsNoTracking()
                    .WhereIf(input.OrgId.HasValue, x => x.OrgId == input.OrgId)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Category), x => x.Category == input.Category)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey), x => x.Name.Contains(input.SearchKey));

                // 排序
                if (!string.IsNullOrWhiteSpace(input.SortField))
                {
                    var sortOrder = input.SortOrder?.ToUpper() == "DESC" ? "DESC" : "ASC";

                    query = sortOrder == "ASC"
                        ? query.OrderBy(x =>  x.SortCode)
                        : query.OrderByDescending(x => x.SortCode);
                }
                else
                {
                    query = query.OrderBy(x => x.SortCode);
                }

                var total = await query.CountAsync();
                var items = await query
                    .Select(x => new Position
                    {
                        Id = x.Id,
                        OrgId = x.OrgId,
                        Name = x.Name,
                        Category = x.Category,
                        SortCode = x.SortCode
                    })
                    .PageBy((input.Current - 1) * input.Size, input.Size)
                    .ToListAsync();

                var page = new Page<Position>(input.Current, input.Size, total)
                {
                    Current = input.Current,
                    Records = items
                };

                return CommonResult<Page<Position>>.Success(page);
            }
            catch (Exception ex)
            {
                return CommonResult<Page<Position>>.Error("获取职位分页失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 添加职位
        /// </summary>
        /// <param name="input">添加参数</param>
        /// <returns>操作结果</returns>
        [UnitOfWork]
        public async Task<CommonResult> Create(PositionAddInput input)
        {
            try
            {
               
                // 检查同组织下是否有重复职位名称
                var exists = await _positionRepository.GetAll()
                    .AnyAsync(x => x.OrgId == input.OrgId && x.Name == input.Name);

                if (exists)
                {
                    return CommonResult.Error($"同组织下存在重复的职位，名称为：{input.Name}");
                }

                var position = new Position
                {
                    OrgId = input.OrgId,
                    Name = input.Name,
                    Category = input.Category,
                    SortCode = input.SortCode,
                    ExtJson = input.ExtJson
                };

                await _positionRepository.InsertAsync(position);

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("添加职位失败:" + ex.Message);
            }
        }





        /// <summary>
        /// 编辑职位
        /// </summary>
        /// <param name="input">编辑参数</param>
        /// <returns>操作结果</returns>
        [UnitOfWork]
        public async Task<CommonResult> UpdateById(PositionEditInput input)
        {
            try
            {
              
                var position = await GetEntityById(input.Id);
                if (position == null)
                {
                    return CommonResult.Error("职位不存在");
                }

                // 检查同组织下是否有重复职位名称
                var exists = await _positionRepository.GetAll()
                    .AnyAsync(x => x.OrgId == input.OrgId
                        && x.Name == input.Name
                        && x.Id != input.Id);

                if (exists)
                {
                    return CommonResult.Error($"同组织下存在重复的职位，名称为：{input.Name}");
                }

                // 更新职位信息
                position.OrgId = input.OrgId;
                position.Name = input.Name;
                position.Category = input.Category;
                position.SortCode = input.SortCode;
                position.ExtJson = input.ExtJson;

                await _positionRepository.UpdateAsync(position);

            

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("编辑职位失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 删除职位
        /// </summary>
        /// <param name="positionIdInput">ID列表</param>
        /// <returns>操作结果</returns>
        [UnitOfWork]
        public async Task<CommonResult> DeleteByIds([FromBody] List<PositionIdInput> positionIdInput)
        {
            try
            {
                if (positionIdInput == null || positionIdInput.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的职位");
                }

                var ids = positionIdInput.Select(x => x.Id).ToList();

                // 检查职位下是否有用户（直属职位）
                var hasUsers = await _userRepository.GetAll()
                    .AnyAsync(x => ids.Contains(x.PositionId ?? Guid.Empty));

                if (hasUsers)
                {
                    return CommonResult.Error("请先删除职位下的用户");
                }

                // 获取要删除的职位列表
                var positionsToDelete = await _positionRepository.GetAll()
                    .Where(x => ids.Contains(x.Id))
                    .ToListAsync();

                // 执行删除
                foreach (var position in positionsToDelete)
                {
                    await _positionRepository.DeleteAsync(position);
                }

             
                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("删除职位失败：" + ex.Message);
            }
        }



        /// <summary>
        /// 获取职位详情
        /// </summary>
        /// <param name="id">职位ID</param>
        /// <returns>职位详情</returns>
        [UnitOfWork]
        public async Task<CommonResult<Position>> GetById(Guid id)
        {
            try
            {
                var position = await GetEntityById(id);
                if (position == null)
                {
                    return CommonResult<Position>.Error("职位不存在");
                }

                return CommonResult<Position>.Success(position);
            }
            catch (Exception ex)
            {
                return CommonResult<Position>.Error("获取职位详情失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 根据ID列表获取职位集合
        /// </summary>
        /// <param name="ids">职位ID列表</param>
        /// <returns>职位集合</returns>
        [HttpPost]
        [UnitOfWork]
        public async Task<CommonResult<List<Position>>> GetPositionsByIds([FromBody] List<string> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return CommonResult<List<Position>>.Success(new List<Position>());
                }

                


                var positions = await _positionRepository.GetAll()
                    .Where(x => ids.Contains(x.Id.ToString()))
                    .Select(x => new Position
                    {
                        Id = x.Id,
                        OrgId = x.OrgId,
                        Name = x.Name,
                        Category = x.Category,
                        SortCode = x.SortCode,
                        ExtJson = x.ExtJson
                    })
                    .ToListAsync();

                return CommonResult<List<Position>>.Success(positions);
            }
            catch (Exception ex)
            {
                Logger.Error($"根据ID列表获取职位集合失败，IDs: {string.Join(",", ids)}", ex);
                return CommonResult<List<Position>>.Error("获取职位集合失败：" + ex.Message);
            }
        }


        /// <summary>
        /// 获取职位选择树
        /// </summary>
        /// <param name="param">查询对象</param>
        /// <returns>职位选择树</returns>
        [HttpGet]
        [UnitOfWork]
        public async Task<CommonResult<Page<Position>>> GetPositionSelector(PositionSelectorParam  param)
        {
            try
            {
                // 如果查询条件为空，则直接查询所有启用的用户
                if (param.OrgId == null && string.IsNullOrWhiteSpace(param.SearchKey))
                {
                    return await GetAllPositionSelectorList(param);
                }
                else
                {
                    return await GetFilteredPositionSelectorList(param);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"获取用户选择器失败，参数: {JsonConvert.SerializeObject(param)}", ex);
                return CommonResult<Page<Position>>.Error("获取职位选择器失败：" + ex.Message);
            }
        }




        #region 辅助方法


        // <summary>
        /// 获取所有用户选择器列表（简化查询）
        /// </summary>
        private async Task<CommonResult<Page<Position>>> GetAllPositionSelectorList(PositionSelectorParam param)
        {
            var query = _positionRepository.GetAll()
                .OrderBy(x => x.CreationTime);

            var totalCount = await query.CountAsync();

            var items = await query
                .Select(x => new Position
                {
                    Id = x.Id,
                    OrgId = x.OrgId,
                    Name = x.Name
                })
                .Skip((param.Current - 1) * param.Size)
                .Take(param.Size)
                .ToListAsync();


            var page = new Page<Position>
            {
                Current = param.Current,
                Size = param.Size,
                Total = totalCount,
                Records = items
            };

            return CommonResult<Page<Position>>.Success(page);
        }

        /// <summary>
        /// 获取筛选后的职位选择器列表
        /// </summary>
        private async Task<CommonResult<Page<Position>>> GetFilteredPositionSelectorList(PositionSelectorParam param)
        {
            var query = _positionRepository.GetAll();

            // 如果组织id不为空，则查询该组织及其子组织下的所有职位
            if (param.OrgId.HasValue)
            {
                // 获取该组织及其所有子组织的ID列表
                var childOrgIds = await GetChildOrgIds(param.OrgId.Value);
                if (childOrgIds.Any())
                {
                    query = query.Where(x => x.OrgId.HasValue && childOrgIds.Contains(x.OrgId.Value));
                }
                else
                {
                    // 如果没有子组织，返回空结果
                    return CommonResult<Page<Position>>.Success(new Page<Position>
                    {
                        Current = param.Current,
                        Size = param.Size,
                        Total = 0,
                        Records = new List<Position>()
                    });
                }
            }

            // 搜索关键词过滤
            if (!string.IsNullOrWhiteSpace(param.SearchKey))
            {
                query = query.Where(x => x.Name.Contains(param.SearchKey));
            }

            // 排序
            query = query.OrderBy(x => x.CreationTime);

            var totalCount = await query.CountAsync();

            var items = await query
                .Select(x => new Position
                {
                    Id = x.Id,
                    OrgId = x.OrgId,
                    Name = x.Name
                })
                .Skip((param.Current - 1) * param.Size)
                .Take(param.Size)
                .ToListAsync();

            var page = new Page<Position>
            {
                Current = param.Current,
                Size = param.Size,
                Total = totalCount,
                Records = items
            };

            return CommonResult<Page<Position>>.Success(page);
        }

        /// <summary>
        /// 获取组织及其所有子组织的ID列表
        /// </summary>
        private async Task<List<Guid>> GetChildOrgIds(Guid orgId)
        {
            // 获取所有组织
            var allOrgs = await _organizationRepository.GetAll().ToListAsync();

            // 递归查找子组织
            var result = new List<Guid> { orgId };
            FindChildOrgIds(orgId, allOrgs, result);

            return result;
        }


        /// <summary>
        /// 递归查找子组织ID
        /// </summary>
        private void FindChildOrgIds(Guid parentId, List<Organization> allOrgs, List<Guid> result)
        {
            var childOrgs = allOrgs.Where(x => x.ParentId == parentId).ToList();
            foreach (var child in childOrgs)
            {
                if (!result.Contains(child.Id))
                {
                    result.Add(child.Id);
                    FindChildOrgIds(child.Id, allOrgs, result);
                }
            }
        }


        /// <summary>
        /// 根据ID获取职位实体
        /// </summary>
        /// <param name="id">职位ID</param>
        /// <returns>职位实体</returns>
        private async Task<Position> GetEntityById(Guid id)
        {
            var position = await _positionRepository.FirstOrDefaultAsync(id);
            if (position == null)
            {
                throw new UserFriendlyException($"职位不存在，id值为：{id}");
            }
            return position;
        }
      

        #endregion

    }
}
