using Abp.Auditing;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using DeviceManagementSystem.Authorization.Organizations;
using DeviceManagementSystem.Authorization.Users;
using DeviceManagementSystem.Organizations.Constants;
using DeviceManagementSystem.Organizations.Dto;
using DeviceManagementSystem.Positions;
using DeviceManagementSystem.Positions.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Organizations
{
    /// <summary>
    /// 组织服务类
    /// </summary>
    [AbpAuthorize]
    public class OrganizationAppService : DeviceManagementSystemAppServiceBase, IOrganizationAppService
    {

        private readonly IRepository<Organization, Guid> _organizationRepository;


        private readonly IRepository<User, long> _userRepository;


        private readonly IPositionAppService _positionAppService;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="organizationRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="positionAppService"></param>
        public OrganizationAppService(IRepository<Organization, Guid> organizationRepository, IRepository<User, long> userRepository, IPositionAppService positionAppService)
        {
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _positionAppService = positionAppService;
        }



        /// <summary>
        /// 获取组织分页
        /// </summary>
        /// <param name="input">分页参数</param>
        /// <returns>分页结果</returns>
        [DisableAuditing]
        public async Task<CommonResult<Page<Organization>>> GetPageList(OrganizationPageInput input)
        {
            try
            {
                var query = _organizationRepository.GetAll().AsNoTracking()
                    .WhereIf(input.ParentId.HasValue, x => x.ParentId == input.ParentId)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey), x => x.Name.Contains(input.SearchKey));

                // 排序
                if (!string.IsNullOrWhiteSpace(input.SortField) && !string.IsNullOrWhiteSpace(input.SortOrder))
                {
                    if (input.SortOrder.ToUpper() == "ASC")
                    {
                        query = query.OrderBy(x => x.SortCode);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.SortCode);
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.SortCode);
                }

                var total = query.Count();
                var items = await query
                    .Select(x => new Organization
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        Name = x.Name,
                        Category = x.Category,
                        SortCode = x.SortCode
                    })
                     .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var page = new Page<Organization>(input.Current, input.Size, total)
                {
                    Current = input.Current,
                    Records = items
                };

                return CommonResult<Page<Organization>>.Success(page);
            }
            catch (Exception ex)
            {
                return CommonResult<Page<Organization>>.Error("获取组织分页失败:" + ex.Message);
            }
        }




        /// <summary>
        /// 获取组织树
        /// </summary>
        /// <returns>树形结构</returns>
        public async Task<CommonResult<List<object>>> GetTreeList()
        {
            try
            {
                var allOrgs = await GetAllOrganizationList();

                // 使用稳定的排序方式，首先按排序码排序，然后按机构ID排序作为次级条件
                var sortedOrgs = allOrgs
                    .OrderBy(x => x.SortCode)
                    .ThenBy(x => x.Id)
                    .ToList();

                var rootNodes = BuildOrgTreeNodes(sortedOrgs);
                return CommonResult<List<object>>.Success(rootNodes);
            }
            catch (Exception ex)
            {
                return CommonResult<List<object>>.Error("获取组织树失败:" + ex.Message);
            }
        }







        /// <summary>
        /// 添加组织
        /// </summary>
        /// <param name="input">添加参数</param>
        /// <returns>操作结果</returns>        
        public async Task<CommonResult> Create(OrganizationAddInput input)
        {
            try
            {

                // 检查重复名称
                var exists = await _organizationRepository.GetAll()
                    .AnyAsync(x => x.ParentId == input.ParentId && x.Name == input.Name);

                if (exists)
                {
                    return CommonResult.Error($"存在重复的同级组织，名称为：{input.Name}");
                }

                var organization = new Organization
                {
                    ParentId = input.ParentId,
                    Name = input.Name,
                    Category = input.Category,
                    SortCode = input.SortCode,
                    ExtJson = input.ExtJson,
                };

                await _organizationRepository.InsertAsync(organization);

                //// 创建默认职位
                var positionInput = new PositionAddInput
                {
                    OrgId = organization.Id,
                    Name = "普通员工",
                    Category = "LOW",
                    SortCode = 99
                };
                await _positionAppService.Create(positionInput);

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("添加组织失败:" + ex.Message);
            }
        }






        /// <summary>
        /// 编辑组织
        /// </summary>
        /// <param name="input">编辑参数</param>
        /// <returns>操作结果</returns>        
        public async Task<CommonResult> UpdateById(OrganizationEditInput input)
        {
            try
            {

                var organization = await GetEntityById(input.Id);
                if (organization == null)
                {
                    return CommonResult.Error("组织不存在");
                }

                // 检查重复名称
                var exists = await _organizationRepository.GetAll()
                    .AnyAsync(x => x.ParentId == input.ParentId
                        && x.Name == input.Name
                        && x.Id != input.Id);

                if (exists)
                {
                    return CommonResult.Error($"存在重复的同级组织，名称为：{input.Name}");
                }

                // 检查层级关系（不能选择下级作为上级）
                var allOrgs = await GetAllOrganizationList();
                var childIds = GetChildIds(allOrgs, input.Id, true);
                if (childIds.Contains(input.ParentId))
                {
                    var parentOrg = GetById(allOrgs, input.ParentId);
                    return CommonResult.Error($"不可选择上级组织：{parentOrg?.Name}");
                }

                // 更新组织
                organization.ParentId = input.ParentId;
                organization.Name = input.Name;
                organization.Category = input.Category;
                organization.SortCode = input.SortCode;
                organization.ExtJson = input.ExtJson;

                await _organizationRepository.UpdateAsync(organization);

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("编辑组织失败:" + ex.Message);
            }
        }




        /// <summary>
        /// 删除组织
        /// </summary>
        /// <param name="organizationIdInput">ID列表</param>
        /// <returns>操作结果</returns>
        public async Task<CommonResult> DeleteByIds([FromBody] List<OrganizationIdInput> organizationIdInput)
        {
            try
            {
                if (organizationIdInput == null || organizationIdInput.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的组织");
                }
                var ids = organizationIdInput.Select(x => x.Id).ToList();
                var allOrgs = await GetAllOrganizationList();
                var toDeleteIds = new List<Guid>();

                // 获取所有要删除的组织ID（包括子组织）
                foreach (var id in ids)
                {
                    var childIds = GetChildIds(allOrgs, id, true);
                    toDeleteIds.AddRange(childIds);
                }

                toDeleteIds = toDeleteIds.Distinct().ToList();

                // 检查组织下是否有用户（直属组织）
                var hasUsers = await _userRepository.GetAll()
                    .AnyAsync(x => toDeleteIds.Contains(x.OrgId ?? Guid.Empty));

                if (hasUsers)
                {
                    return CommonResult.Error("请先删除组织下的用户");
                }

                // 执行删除
                foreach (var id in toDeleteIds)
                {
                    await _organizationRepository.DeleteAsync(id);
                }

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                return CommonResult.Error("删除组织失败：" + ex.Message);
            }
        }



        /// <summary>
        /// 获取组织详情
        /// </summary>
        /// <param name="id">组织ID</param>
        /// <returns>组织详情</returns>
        public async Task<CommonResult<Organization>> GetById(Guid id)
        {
            try
            {
                var organization = await GetEntityById(id);
                if (organization == null)
                {
                    return CommonResult<Organization>.Error("组织不存在");
                }

                return CommonResult<Organization>.Success(organization);
            }
            catch (Exception ex)
            {
                return CommonResult<Organization>.Error("获取组织详情失败:" + ex.Message);
            }
        }




        /// <summary>
        /// 获取组织树选择器
        /// </summary>
        /// <returns></returns>
        public async Task<CommonResult<List<TreeNode>>> GetOrgTreeSelector()
        {
            try
            {
                // 获取所有组织数据
                var orgList = await _organizationRepository.GetAll()
                    .Select(x => new OrganizationDto
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        Name = x.Name,
                        SortCode = (int)x.SortCode,
                        Category = x.Category,
                        ExtJson = x.ExtJson
                    })
                    .OrderBy(x => x.SortCode)
                    .ThenBy(x => x.Name)
                    .ToListAsync();

                // 转换为TreeNode列表
                var nodeList = orgList.Select(org => new TreeNode
                {
                    Id = org.Id,
                    ParentId = org.ParentId,
                    Name = org.Name,
                    SortCode = org.SortCode

                }).ToList();

                // 构建树结构
                var tree = BuildTree(nodeList);

                return CommonResult<List<TreeNode>>.Success(tree);
            }
            catch (Exception ex)
            {
                Logger.Error("获取组织树选择器失败", ex);
                return CommonResult<List<TreeNode>>.Error($"获取组织树失败: {ex.Message}");
            }
        }



        


        #region 辅助方法


        /// <summary>
        /// 构建树结构
        /// </summary>
        /// <param name="nodes">所有节点列表</param>
        /// <returns>树结构列表</returns>
        private List<TreeNode> BuildTree(List<TreeNode> nodes)
        {
            // 创建一个字典用于快速查找节点
            var nodeDict = nodes.ToDictionary(n => n.Id);

            // 存储根节点（ParentId为null或Guid.Empty）
            var rootNodes = new List<TreeNode>();

            foreach (var node in nodes)
            {
                // 如果ParentId为null或Guid.Empty，则为根节点
                if (!node.ParentId.HasValue || node.ParentId.Value == Guid.Empty)
                {
                    rootNodes.Add(node);
                }
                else
                {
                    // 查找父节点
                    if (nodeDict.TryGetValue(node.ParentId.Value, out var parentNode))
                    {
                        // 确保子节点列表已初始化
                        parentNode.Children ??= new List<TreeNode>();

                        // 添加子节点到父节点
                        parentNode.Children.Add(node);
                    }
                    else
                    {
                        // 如果找不到父节点，也作为根节点处理
                        rootNodes.Add(node);
                    }
                }
            }

            // 对每个节点及其子节点按SortCode排序
            SortTreeNodes(rootNodes);

            return rootNodes;
        }

        /// <summary>
        /// 递归排序树节点
        /// </summary>
        /// <param name="nodes">节点列表</param>
        private void SortTreeNodes(List<TreeNode> nodes)
        {
            if (nodes == null || !nodes.Any()) return;

            // 按SortCode排序，然后按Name排序
            nodes = nodes.OrderBy(n => n.SortCode)
                        .ThenBy(n => n.Name)
                        .ToList();

            // 递归排序子节点
            foreach (var node in nodes)
            {
                if (node.Children != null && node.Children.Any())
                {
                    SortTreeNodes(node.Children);
                }
            }
        }

        /// <summary>
        /// 根据ID获取组织实体
        /// </summary>
        /// <param name="id">组织ID</param>
        /// <returns>组织实体</returns>
        private async Task<Organization> GetEntityById(Guid id)
        {
            var organization = await _organizationRepository.FirstOrDefaultAsync(id);
            if (organization == null)
            {
                throw new UserFriendlyException($"组织不存在，id值为：{id}");
            }
            return organization;
        }

        /// <summary>
        /// 获取所有组织列表
        /// </summary>
        /// <returns>组织列表</returns>
        private async Task<List<Organization>> GetAllOrganizationList()
        {
            return await _organizationRepository.GetAll()
                .OrderBy(x => x.SortCode)
                .ToListAsync();
        }




        /// <summary>
        /// 构建树形结构
        /// </summary>
        /// <param name="organizations">组织列表</param>
        /// <param name="parentId">父级ID</param>
        /// <returns>树节点列表</returns>
        private List<TreeNode> BuildTree(List<Organization> organizations, Guid? parentId)
        {
            var nodes = organizations
                .Where(x => x.ParentId == parentId)
                .OrderBy(x => x.SortCode)
                .ThenBy(x => x.Id)
                .Select(x => new TreeNode
                {
                    Id = x.Id,
                    ParentId = x.ParentId ?? Guid.Empty,
                    Name = x.Name,
                    SortCode = (int)x.SortCode,
                    Extra = new Dictionary<string, object>
                    {
                        { "id", x.Id },
                        { "parentId", x.ParentId },
                        { "name", x.Name },
                        { "category", x.Category },
                        { "sortCode", x.SortCode }
                    }
                })
                .ToList();

            foreach (var node in nodes)
            {
                node.Children = BuildTree(organizations, node.Id);
            }

            return nodes;
        }

        /// <summary>
        /// 获取子组织ID列表
        /// </summary>
        /// <param name="organizations">所有组织</param>
        /// <param name="parentId">父级ID</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>子组织ID列表</returns>
        private List<Guid> GetChildIds(List<Organization> organizations, Guid parentId, bool includeSelf)
        {
            var result = new List<Guid>();

            if (includeSelf)
            {
                result.Add(parentId);
            }

            GetChildIdsRecursive(organizations, parentId, result);
            return result;
        }

        private void GetChildIdsRecursive(List<Organization> organizations, Guid parentId, List<Guid> result)
        {
            var children = organizations.Where(x => x.ParentId == parentId).ToList();
            foreach (var child in children)
            {
                result.Add(child.Id);
                GetChildIdsRecursive(organizations, child.Id, result);
            }
        }

        /// <summary>
        /// 根据ID从列表中获取组织
        /// </summary>
        /// <param name="organizations">组织列表</param>
        /// <param name="id">组织ID</param>
        /// <returns>组织实体</returns>
        private Organization GetById(List<Organization> organizations, Guid id)
        {
            return organizations.FirstOrDefault(x => x.Id == id);
        }


        private void GetParentIdsRecursive(List<Organization> organizations, Guid orgId, List<Guid> result)
        {
            var org = organizations.FirstOrDefault(x => x.Id == orgId);
            if (org != null && org.ParentId.HasValue)
            {
                result.Add(org.ParentId.Value);
                GetParentIdsRecursive(organizations, org.ParentId.Value, result);
            }
        }


        /// <summary>
        /// 构建菜单树节点（包含所有字段）
        /// </summary>
        private List<object> BuildOrgTreeNodes(List<Organization> organizations)
        {
            // 构建完整树结构
            var tree = BuildTreeRecursive(organizations, Guid.Empty.ToString());
            return tree;
        }

        /// <summary>
        /// 递归构建树
        /// </summary>
        private List<object> BuildTreeRecursive(List<Organization> organizations, string parentId)
        {
            // 1. 获取指定父节点的所有子节点
            var childResources = GetChildResources(organizations, parentId);

            // 2. 如果没有子节点，返回空列表
            if (!childResources.Any())
            {
                return new List<object>();
            }

            // 3. 构建节点列表
            var nodes = new List<object>();

            foreach (var organization in childResources)
            {
                // 4. 创建节点
                var node = CreateTreeNode(organization);

                // 5. 递归获取子节点
                var children = BuildTreeRecursive(organizations, organization.Id.ToString());

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
        private List<Organization> GetChildResources(List<Organization> organizations, string parentId)
        {
            // 将parentId转换为Guid?类型进行比较
            Guid? parentGuid = ConvertToNullableGuid(parentId);

            // 根据parentGuid是否为Guid.Empty来筛选子节点
            if (parentGuid == Guid.Empty)
            {
                // 获取顶级节点：ParentId为null或Guid.Empty
                return organizations
                    .Where(x => x.ParentId == null || x.ParentId == Guid.Empty)
                    .OrderBy(x => x.SortCode)
                    .ToList();
            }
            else
            {
                // 获取指定父节点的子节点
                return organizations
                    .Where(x => x.ParentId == parentGuid)
                    .OrderBy(x => x.SortCode)
                    .ToList();
            }
        }

        /// <summary>
        /// 创建树节点
        /// </summary>
        private Dictionary<string, object> CreateTreeNode(Organization organization)
        {
            var node = new Dictionary<string, object>();

            // 添加字段
            AddBasicFields(node, organization);
            return node;
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        private void AddBasicFields(Dictionary<string, object> node, Organization organization)
        {
            node["id"] = organization.Id.ToString();
            node["parentId"] = organization.ParentId?.ToString() ?? Guid.Empty.ToString();
            node["sortCode"] = organization.SortCode;
            node["weight"] = organization.SortCode;
            node["name"] = organization.Name;
            node["createTime"] = organization.CreationTime;
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



        #endregion

    }
}
