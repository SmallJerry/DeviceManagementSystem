using Abp.Auditing;
using Abp.Domain.Repositories;
using DeviceManagementSystem.BasicDataManagement;
using DeviceManagementSystem.BasicDataManagements.FactoryNode.Dto;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.BasicDataManagements.FactoryNode
{
    /// <summary>
    /// 工厂节点管理服务
    /// </summary>
    [Authorize]
    public class FactoryNodeAppService : DeviceManagementSystemAppServiceBase, IFactoryNodeAppService
    {
        private readonly IRepository<FactoryNodes, Guid> _factoryNodeRepository;
        private readonly IUserAppService _userAppService;

        /// <summary>
        /// 节点类型层级关系定义
        /// </summary>
        private readonly Dictionary<string, NodeTypeHierarchy> _nodeTypeHierarchy = new Dictionary<string, NodeTypeHierarchy>
        {
            ["Factory"] = new NodeTypeHierarchy { NodeType = "Factory", Name = "工厂", ParentType = null, ChildType = "Workshop", Level = 1 },
            ["Workshop"] = new NodeTypeHierarchy { NodeType = "Workshop", Name = "车间", ParentType = "Factory", ChildType = "ProductionLine", Level = 2 },
            ["ProductionLine"] = new NodeTypeHierarchy { NodeType = "ProductionLine", Name = "产线", ParentType = "Workshop", ChildType = "Workstation", Level = 3 },
            ["Workstation"] = new NodeTypeHierarchy { NodeType = "Workstation", Name = "工位", ParentType = "ProductionLine", ChildType = null, Level = 4 }
        };

        /// <summary>
        /// 构造函数
        /// </summary>
        public FactoryNodeAppService(
            IRepository<FactoryNodes, Guid> factoryNodeRepository,
            IUserAppService userAppService)
        {
            _factoryNodeRepository = factoryNodeRepository;
            _userAppService = userAppService;
        }



        /// <summary>
        /// 获取启用的节点（用于父级选择器）
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<EnabledNodeDto>>> GetEnabledNodesForSelector()
        {
            try
            {
                // 获取所有启用的节点
                var enabledNodes = await _factoryNodeRepository.GetAll()
                    .AsNoTracking()
                    .Where(x => x.Status == "Enabled")
                    .OrderBy(x => x.SortCode)
                    .ThenBy(x => x.Name)
                    .Select(x => new
                    {
                        x.Id,
                        x.ParentId,
                        x.NodeType,
                        x.Name
                    })
                    .ToListAsync();

                // 检查是否有子节点
                var nodeIds = enabledNodes.Select(x => x.Id).ToList();
                var childCounts = await _factoryNodeRepository.GetAll()
                    .AsNoTracking()
                    .Where(x => x.ParentId.HasValue && nodeIds.Contains(x.ParentId.Value))
                    .GroupBy(x => x.ParentId)
                    .Select(g => new { ParentId = g.Key.Value, Count = g.Count() })
                    .ToDictionaryAsync(x => x.ParentId, x => x.Count > 0);

                // 构建完整路径
                var allNodes = await _factoryNodeRepository.GetAll()
                    .AsNoTracking()
                    .Select(x => new { x.Id, x.ParentId, x.Name })
                    .ToListAsync();

                // 转换为DTO
                var nodes = enabledNodes.Select(x => new EnabledNodeDto
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    NodeType = x.NodeType,
                    Name = x.Name,
                    FullPath = BuildFullPath(x.Id, allNodes),
                    HasChildren = childCounts.ContainsKey(x.Id) && childCounts[x.Id]
                }).ToList();

                // 构建树形结构
                var treeData = BuildEnabledNodeTree(nodes);

                return CommonResult<List<EnabledNodeDto>>.Success(treeData);
            }
            catch (Exception ex)
            {
                Logger.Error("获取启用的节点失败", ex);
                return CommonResult<List<EnabledNodeDto>>.Error("获取启用的节点失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 根据父节点获取可创建的子节点类型
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<NodeTypeHierarchyDto>>> GetAllowedChildTypes(Guid parentId)
        {
            try
            {
                // 获取父节点
                var parentNode = await _factoryNodeRepository.GetAll()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == parentId);

                if (parentNode == null)
                {
                    return CommonResult<List<NodeTypeHierarchyDto>>.Error("父节点不存在");
                }

                // 根据父节点类型获取允许的子节点类型
                var allowedChildType = GetChildNodeType(parentNode.NodeType);

                var result = new List<NodeTypeHierarchyDto>();
                foreach (var hierarchy in _nodeTypeHierarchy.Values)
                {
                    result.Add(new NodeTypeHierarchyDto
                    {
                        NodeType = hierarchy.NodeType,
                        Name = hierarchy.Name,
                        ParentType = hierarchy.ParentType,
                        ChildType = hierarchy.ChildType,
                        Enabled = hierarchy.NodeType == allowedChildType
                    });
                }

                return CommonResult<List<NodeTypeHierarchyDto>>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取允许的子节点类型失败", ex);
                return CommonResult<List<NodeTypeHierarchyDto>>.Error("获取允许的子节点类型失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 获取工厂节点分页列表
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<Page<FactoryNodeDto>>> GetPageList(FactoryNodePageInput input)
        {
            try
            {
                if (input.Size > 100) input.Size = 100;

                var query = _factoryNodeRepository.GetAll().AsNoTracking();

                // 应用过滤条件
                if (!string.IsNullOrWhiteSpace(input.SearchKey))
                {
                    query = query.Where(x =>
                        x.Name.Contains(input.SearchKey) ||
                        x.Code.Contains(input.SearchKey) ||
                        x.Address.Contains(input.SearchKey));
                }

                if (!string.IsNullOrWhiteSpace(input.NodeType))
                {
                    query = query.Where(x => x.NodeType == input.NodeType);
                }

                if (!string.IsNullOrWhiteSpace(input.Status))
                {
                    query = query.Where(x => x.Status == input.Status);
                }

                if (input.ParentId.HasValue)
                {
                    query = query.Where(x => x.ParentId == input.ParentId);
                }

                // 获取总数
                var total = await query.CountAsync();

                // 应用排序
                var orderedQuery = ApplySorting(query, input);

                // 分页查询
                var pagedNodes = await orderedQuery
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .Select(x => new
                    {
                        x.Id,
                        x.ParentId,
                        x.NodeType,
                        x.Code,
                        x.Name,
                        x.Address,
                        x.Status,
                        x.SortCode,
                        x.ExtendInfo,
                        x.CreationTime,
                        x.Creator
                    })
                    .ToListAsync();

                // 获取父节点名称
                var parentIds = pagedNodes
                    .Where(x => x.ParentId.HasValue)
                    .Select(x => x.ParentId.Value)
                    .Distinct()
                    .ToList();

                var parentNodes = parentIds.Any()
                    ? await _factoryNodeRepository.GetAll()
                        .AsNoTracking()
                        .Where(x => parentIds.Contains(x.Id))
                        .Select(x => new { x.Id, x.Name })
                        .ToDictionaryAsync(x => x.Id, x => x.Name)
                    : new Dictionary<Guid, string>();

                // 检查是否有子节点
                var nodeIds = pagedNodes.Select(x => x.Id).ToList();
                var childCounts = nodeIds.Any()
                    ? await _factoryNodeRepository.GetAll()
                        .AsNoTracking()
                        .Where(x => x.ParentId.HasValue && nodeIds.Contains(x.ParentId.Value))
                        .GroupBy(x => x.ParentId)
                        .Select(g => new { ParentId = g.Key.Value, Count = g.Count() })
                        .ToDictionaryAsync(x => x.ParentId, x => x.Count > 0)
                    : new Dictionary<Guid, bool>();

                // 获取完整路径
                var allNodes = await _factoryNodeRepository.GetAll()
                    .AsNoTracking()
                    .Select(x => new { x.Id, x.ParentId, x.Name })
                    .ToListAsync();

                // 构建返回结果
                var items = new List<FactoryNodeDto>();
                foreach (var x in pagedNodes)
                {
                    var fullPath = BuildFullPath(x.Id, allNodes);
                    var nodeTypeHierarchy = _nodeTypeHierarchy.FirstOrDefault(h => h.Value.NodeType == x.NodeType).Value;

                    items.Add(new FactoryNodeDto
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        NodeType = x.NodeType,
                        NodeTypeName = nodeTypeHierarchy?.Name ?? x.NodeType,
                        Code = x.Code,
                        Name = x.Name,
                        Address = x.Address,
                        Status = x.Status,
                        StatusName = GetStatusName(x.Status),
                        SortCode = x.SortCode,
                        ExtendInfo = x.ExtendInfo,
                        CreationTime = x.CreationTime,
                        Creator = x.Creator,
                        HasChildren = childCounts.ContainsKey(x.Id) && childCounts[x.Id],
                        ParentName = x.ParentId.HasValue && parentNodes.ContainsKey(x.ParentId.Value)
                            ? parentNodes[x.ParentId.Value]
                            : null,
                        FullPath = fullPath,
                        Level = nodeTypeHierarchy?.Level ?? 0
                    });
                }

                var page = new Page<FactoryNodeDto>(input.Current, input.Size, total)
                {
                    Records = items
                };

                return CommonResult<Page<FactoryNodeDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取工厂节点分页列表失败", ex);
                return CommonResult<Page<FactoryNodeDto>>.Error("获取工厂节点分页列表失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取父级节点选择器数据
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<ParentNodeSelectorDto>> GetParentNodeSelector(string nodeType)
        {
            try
            {
                // 验证节点类型
                if (!_nodeTypeHierarchy.ContainsKey(nodeType))
                {
                    return CommonResult<ParentNodeSelectorDto>.Error("无效的节点类型");
                }

                var nodeHierarchy = _nodeTypeHierarchy[nodeType];
                string allowedParentType = nodeHierarchy.ParentType;

                // 如果没有父级类型（如Factory），返回空数据
                if (string.IsNullOrEmpty(allowedParentType))
                {
                    return CommonResult<ParentNodeSelectorDto>.Success(new ParentNodeSelectorDto
                    {
                        AllowedParentType = null,
                        TreeData = new List<FactoryNodeTreeDto>()
                    });
                }

                // 获取所有启用的、指定类型的节点
                var nodes = await _factoryNodeRepository.GetAll()
                    .AsNoTracking()
                    .Where(x => x.Status == "Enabled" && x.NodeType == allowedParentType)
                    .OrderBy(x => x.SortCode)
                    .ThenBy(x => x.Name)
                    .Select(x => new FactoryNodeTreeDto
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        NodeType = x.NodeType,
                        Code = x.Code,
                        Name = x.Name,
                        Status = x.Status,
                        SortCode = x.SortCode,
                        HasChildren = false,
                        Level = 0
                    })
                    .ToListAsync();

                // 检查是否有子节点
                var nodeIds = nodes.Select(x => x.Id).ToList();
                var childCounts = await _factoryNodeRepository.GetAll()
                    .AsNoTracking()
                    .Where(x => x.ParentId.HasValue && nodeIds.Contains(x.ParentId.Value))
                    .GroupBy(x => x.ParentId)
                    .Select(g => new { ParentId = g.Key.Value, Count = g.Count() })
                    .ToDictionaryAsync(x => x.ParentId, x => x.Count > 0);

                // 获取完整路径
                var allNodes = await _factoryNodeRepository.GetAll()
                    .AsNoTracking()
                    .Select(x => new { x.Id, x.ParentId, x.Name })
                    .ToListAsync();

                foreach (var node in nodes)
                {
                    node.HasChildren = childCounts.ContainsKey(node.Id) && childCounts[node.Id];
                    node.FullPath = BuildFullPath(node.Id, allNodes);
                }

                // 构建树形结构
                var treeData = BuildTreeStructure(nodes,null);

                var result = new ParentNodeSelectorDto
                {
                    AllowedParentType = allowedParentType,
                    TreeData = treeData
                };

                return CommonResult<ParentNodeSelectorDto>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取父级节点选择器失败", ex);
                return CommonResult<ParentNodeSelectorDto>.Error("获取父级节点选择器失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取工厂节点树形结构
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<FactoryNodeTreeDto>>> GetTreeList(FactoryNodeTreeInput input)
        {
            try
            {
                var query = _factoryNodeRepository.GetAll().AsNoTracking();

                if (!string.IsNullOrWhiteSpace(input.NodeType))
                {
                    query = query.Where(x => x.NodeType == input.NodeType);
                }

                if (!input.IncludeDisabled)
                {
                    query = query.Where(x => x.Status == "Enabled");
                }

                var allNodes = await query
                    .OrderBy(x => x.SortCode)
                    .ThenBy(x => x.Name)
                    .Select(x => new FactoryNodeTreeDto
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        NodeType = x.NodeType,
                        Code = x.Code,
                        Name = x.Name,
                        Status = x.Status,
                        SortCode = x.SortCode,
                        HasChildren = false,
                        CreationTime = x.CreationTime,
                        Creator = x.Creator,
                        Level = 0
                    })
                    .ToListAsync();

                // 检查子节点
                var nodeIds = allNodes.Select(x => x.Id).ToList();
                var childCounts = await _factoryNodeRepository.GetAll()
                    .AsNoTracking()
                    .Where(x => x.ParentId.HasValue && nodeIds.Contains(x.ParentId.Value))
                    .GroupBy(x => x.ParentId)
                    .Select(g => new { ParentId = g.Key.Value, Count = g.Count() })
                    .ToDictionaryAsync(x => x.ParentId, x => x.Count > 0);

                // 获取完整路径
                var allNodeData = await _factoryNodeRepository.GetAll()
                    .AsNoTracking()
                    .Select(x => new { x.Id, x.ParentId, x.Name })
                    .ToListAsync();

                foreach (var node in allNodes)
                {
                    node.HasChildren = childCounts.ContainsKey(node.Id) && childCounts[node.Id];
                    node.FullPath = BuildFullPath(node.Id, allNodeData);
                }

                // 构建树形结构
                List<FactoryNodeTreeDto> treeData;
                if (input.RootId.HasValue)
                {
                    treeData = BuildTreeFromRoot(allNodes, input.RootId.Value);
                }
                else
                {
                    treeData = BuildTreeStructure(allNodes, null);
                }

                // 设置层级
                foreach (var root in treeData)
                {
                    SetNodeLevel(root, 1);
                }

                return CommonResult<List<FactoryNodeTreeDto>>.Success(treeData);
            }
            catch (Exception ex)
            {
                Logger.Error("获取工厂节点树形结构失败", ex);
                return CommonResult<List<FactoryNodeTreeDto>>.Error("获取工厂节点树形结构失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取工厂节点详情
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<FactoryNodeDto>> GetById(Guid id)
        {
            try
            {
                var node = await _factoryNodeRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (node == null)
                {
                    return CommonResult<FactoryNodeDto>.Error("工厂节点不存在");
                }

                string parentName = null;
                if (node.ParentId.HasValue)
                {
                    parentName = await _factoryNodeRepository.GetAll()
                        .Where(x => x.Id == node.ParentId.Value)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync();
                }

                var hasChildren = await _factoryNodeRepository.GetAll()
                    .AnyAsync(x => x.ParentId == node.Id);

                // 获取完整路径
                var allNodes = await _factoryNodeRepository.GetAll()
                    .AsNoTracking()
                    .Select(x => new { x.Id, x.ParentId, x.Name })
                    .ToListAsync();
                var fullPath = BuildFullPath(node.Id, allNodes);

                var nodeHierarchy = _nodeTypeHierarchy.FirstOrDefault(h => h.Value.NodeType == node.NodeType).Value;

                var dto = new FactoryNodeDto
                {
                    Id = node.Id,
                    ParentId = node.ParentId,
                    NodeType = node.NodeType,
                    NodeTypeName = nodeHierarchy?.Name ?? node.NodeType,
                    Code = node.Code,
                    Name = node.Name,
                    Address = node.Address,
                    Status = node.Status,
                    StatusName = GetStatusName(node.Status),
                    SortCode = node.SortCode,
                    ExtendInfo = node.ExtendInfo,
                    CreationTime = node.CreationTime,
                    Creator = node.Creator,
                    HasChildren = hasChildren,
                    ParentName = parentName,
                    FullPath = fullPath,
                    Level = nodeHierarchy?.Level ?? 0
                };

                return CommonResult<FactoryNodeDto>.Success(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("获取工厂节点详情失败", ex);
                return CommonResult<FactoryNodeDto>.Error("获取工厂节点详情失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 创建工厂节点
        /// </summary>
        public async Task<CommonResult> Create(FactoryNodeAddInput input)
        {
            try
            {
                var userId = AbpSession.UserId;
                var creatorUser = await _userAppService.GetNameByUserId(userId.Value);

                // 验证节点类型
                if (!_nodeTypeHierarchy.ContainsKey(input.NodeType))
                {
                    return CommonResult.Error("无效的节点类型");
                }

                var nodeHierarchy = _nodeTypeHierarchy[input.NodeType];

                // 验证节点类型层级关系
                if (input.NodeType == "Factory")
                {
                    // 工厂节点不能有父级
                    if (input.ParentId.HasValue)
                    {
                        return CommonResult.Error("工厂节点不能有父级");
                    }
                }
                else
                {
                    // 非工厂节点必须有父级
                    if (!input.ParentId.HasValue || input.ParentId.Value == Guid.Empty)
                    {
                        return CommonResult.Error($"{nodeHierarchy.Name}必须选择父级节点");
                    }

                    // 验证父级节点存在性
                    var parentNode = await _factoryNodeRepository.GetAll()
                        .FirstOrDefaultAsync(x => x.Id == input.ParentId.Value);
                    if (parentNode == null)
                    {
                        return CommonResult.Error("父级节点不存在");
                    }

                    // 验证父级节点类型匹配
                    if (!_nodeTypeHierarchy.ContainsKey(parentNode.NodeType))
                    {
                        return CommonResult.Error("父级节点类型无效");
                    }

                    var parentHierarchy = _nodeTypeHierarchy[parentNode.NodeType];
                    if (parentHierarchy.ChildType != input.NodeType)
                    {
                        return CommonResult.Error(
                            $"{parentHierarchy.Name}只能创建{nodeHierarchy.Name}，无法创建{nodeHierarchy.Name}");
                    }
                }

                // 验证名称唯一性
                var nameExists = await _factoryNodeRepository.GetAll()
                    .AnyAsync(x => x.ParentId == input.ParentId && x.Name == input.Name);
                if (nameExists)
                {
                    return CommonResult.Error($"同一父级下已存在同名节点：{input.Name}");
                }

                // 验证编码唯一性
                if (!string.IsNullOrWhiteSpace(input.Code))
                {
                    var codeExists = await _factoryNodeRepository.GetAll()
                        .AnyAsync(x => x.Code == input.Code);
                    if (codeExists)
                    {
                        return CommonResult.Error($"节点编码已存在：{input.Code}");
                    }
                }

                // 工厂节点必须填写地址
                if (input.NodeType == "Factory" && string.IsNullOrWhiteSpace(input.Address))
                {
                    return CommonResult.Error("工厂节点必须填写地址");
                }

                var node = new FactoryNodes
                {
                    ParentId = input.ParentId,
                    NodeType = input.NodeType,
                    Code = input.Code,
                    Name = input.Name,
                    Address = input.Address,
                    Status = input.Status,
                    SortCode = input.SortCode,
                    ExtendInfo = input.ExtendInfo,
                    Creator = creatorUser
                };

                await _factoryNodeRepository.InsertAsync(node);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("创建工厂节点失败", ex);
                return CommonResult.Error("创建工厂节点失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 更新工厂节点
        /// </summary>
        public async Task<CommonResult> UpdateById(FactoryNodeEditInput input)
        {
            try
            {
                var node = await _factoryNodeRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == input.Id);
                if (node == null)
                {
                    return CommonResult.Error("工厂节点不存在");
                }

                // 验证节点类型
                if (!_nodeTypeHierarchy.ContainsKey(input.NodeType))
                {
                    return CommonResult.Error("无效的节点类型");
                }

                var nodeHierarchy = _nodeTypeHierarchy[input.NodeType];

                // 验证节点类型层级关系
                if (input.NodeType == "Factory")
                {
                    // 工厂节点不能有父级
                    if (input.ParentId.HasValue)
                    {
                        return CommonResult.Error("工厂节点不能有父级");
                    }
                }
                else
                {
                    // 非工厂节点必须有父级
                    if (!input.ParentId.HasValue || input.ParentId.Value == Guid.Empty)
                    {
                        return CommonResult.Error($"{nodeHierarchy.Name}必须选择父级节点");
                    }

                    // 验证父级节点存在性
                    var parentNode = await _factoryNodeRepository.GetAll()
                        .FirstOrDefaultAsync(x => x.Id == input.ParentId.Value);
                    if (parentNode == null)
                    {
                        return CommonResult.Error("父级节点不存在");
                    }

                    // 不能将自己设为自己的父级
                    if (input.ParentId.Value == input.Id)
                    {
                        return CommonResult.Error("不能将自己设为自己的父级");
                    }

                    // 验证父级节点类型匹配
                    if (!_nodeTypeHierarchy.ContainsKey(parentNode.NodeType))
                    {
                        return CommonResult.Error("父级节点类型无效");
                    }

                    var parentHierarchy = _nodeTypeHierarchy[parentNode.NodeType];
                    if (parentHierarchy.ChildType != input.NodeType)
                    {
                        return CommonResult.Error(
                            $"{parentHierarchy.Name}只能创建{nodeHierarchy.Name}，无法创建{nodeHierarchy.Name}");
                    }

                    // 检查是否会造成循环引用
                    if (await CheckCircularReference(input.Id, input.ParentId.Value))
                    {
                        return CommonResult.Error("修改父级会造成循环引用，操作被拒绝");
                    }
                }

                // 验证名称唯一性（排除自身）
                var nameExists = await _factoryNodeRepository.GetAll()
                    .AnyAsync(x => x.Id != input.Id && x.ParentId == input.ParentId && x.Name == input.Name);
                if (nameExists)
                {
                    return CommonResult.Error($"同一父级下已存在同名节点：{input.Name}");
                }

                // 验证编码唯一性（排除自身）
                if (!string.IsNullOrWhiteSpace(input.Code))
                {
                    var codeExists = await _factoryNodeRepository.GetAll()
                        .AnyAsync(x => x.Id != input.Id && x.Code == input.Code);
                    if (codeExists)
                    {
                        return CommonResult.Error($"节点编码已存在：{input.Code}");
                    }
                }

                // 工厂节点必须填写地址
                if (input.NodeType == "Factory" && string.IsNullOrWhiteSpace(input.Address))
                {
                    return CommonResult.Error("工厂节点必须填写地址");
                }

                // 更新字段
                node.ParentId = input.ParentId;
                node.NodeType = input.NodeType;
                node.Code = input.Code;
                node.Name = input.Name;
                node.Address = input.Address;
                node.Status = input.Status;
                node.SortCode = input.SortCode;
                node.ExtendInfo = input.ExtendInfo;

                await _factoryNodeRepository.UpdateAsync(node);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("更新工厂节点失败", ex);
                return CommonResult.Error("更新工厂节点失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 删除工厂节点
        /// </summary>
        public async Task<CommonResult> DeleteByIds([FromBody] List<FactoryNodeIdInput> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的工厂节点");
                }

                var nodeIds = ids.Select(x => x.Id).ToList();

                var nodesWithChildren = await (from node in _factoryNodeRepository.GetAll()
                                               let childCount = _factoryNodeRepository.GetAll()
                                                   .Count(child => child.ParentId == node.Id)
                                               where nodeIds.Contains(node.Id)
                                               select new
                                               {
                                                   Node = node,
                                                   HasChildren = childCount > 0
                                               }).ToListAsync();

                if (!nodesWithChildren.Any())
                {
                    return CommonResult.Error("未找到要删除的工厂节点");
                }

                var errors = new List<string>();
                var nodesToDelete = new List<FactoryNodes>();

                foreach (var item in nodesWithChildren)
                {
                    if (item.HasChildren)
                    {
                        errors.Add($"节点 '{item.Node.Name}' 存在子节点，不允许删除");
                        continue;
                    }

                    nodesToDelete.Add(item.Node);
                }

                if (nodesToDelete.Any())
                {
                    foreach (var node in nodesToDelete)
                    {
                        await _factoryNodeRepository.DeleteAsync(node);
                    }
                    await CurrentUnitOfWork.SaveChangesAsync();
                }

                if (errors.Any())
                {
                    return CommonResult.Error(string.Join("；", errors));
                }

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("删除工厂节点失败", ex);
                return CommonResult.Error("删除工厂节点失败:" + ex.Message);
            }
        }

        #region 辅助方法

        /// <summary>
        /// 应用排序
        /// </summary>
        private IOrderedQueryable<FactoryNodes> ApplySorting(IQueryable<FactoryNodes> query, FactoryNodePageInput input)
        {
            if (!string.IsNullOrWhiteSpace(input.SortField))
            {
                if (input.SortField.Equals("Code", StringComparison.OrdinalIgnoreCase))
                {
                    return input.SortOrder?.ToUpper() == "ASC"
                        ? query.OrderBy(x => x.Code)
                        : query.OrderByDescending(x => x.Code);
                }
                else if (input.SortField.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    return input.SortOrder?.ToUpper() == "ASC"
                        ? query.OrderBy(x => x.Name)
                        : query.OrderByDescending(x => x.Name);
                }
                else if (input.SortField.Equals("NodeType", StringComparison.OrdinalIgnoreCase))
                {
                    return input.SortOrder?.ToUpper() == "ASC"
                        ? query.OrderBy(x => x.NodeType)
                        : query.OrderByDescending(x => x.NodeType);
                }
                else if (input.SortField.Equals("CreationTime", StringComparison.OrdinalIgnoreCase))
                {
                    return input.SortOrder?.ToUpper() == "ASC"
                        ? query.OrderBy(x => x.CreationTime)
                        : query.OrderByDescending(x => x.CreationTime);
                }
            }

            // 默认排序
            return input.SortOrder?.ToUpper() == "DESC"
                ? query.OrderByDescending(x => x.SortCode).ThenByDescending(x => x.CreationTime)
                : query.OrderBy(x => x.SortCode).ThenBy(x => x.CreationTime);
        }

        /// <summary>
        /// 构建树形结构
        /// </summary>
        private List<FactoryNodeTreeDto> BuildTreeStructure(List<FactoryNodeTreeDto> nodes, string nodeTypeFilter)
        {
            var nodeDict = nodes.ToDictionary(x => x.Id);
            var tree = new List<FactoryNodeTreeDto>();

            foreach (var node in nodes)
            {
                if (!node.ParentId.HasValue || node.ParentId == Guid.Empty)
                {
                    tree.Add(node);
                }
                else if (nodeDict.ContainsKey(node.ParentId.Value))
                {
                    var parent = nodeDict[node.ParentId.Value];
                    parent.Children.Add(node);
                    parent.HasChildren = true;
                }
            }

            return tree;
        }

        /// <summary>
        /// 从根节点构建树
        /// </summary>
        private List<FactoryNodeTreeDto> BuildTreeFromRoot(List<FactoryNodeTreeDto> nodes, Guid rootId)
        {
            var rootNode = nodes.FirstOrDefault(x => x.Id == rootId);
            if (rootNode == null)
            {
                return new List<FactoryNodeTreeDto>();
            }

            var nodeDict = nodes.ToDictionary(x => x.Id);
            var result = new List<FactoryNodeTreeDto> { rootNode };

            // 构建子树
            BuildSubTree(nodeDict, rootNode);

            return result;
        }

        /// <summary>
        /// 构建子树
        /// </summary>
        private void BuildSubTree(Dictionary<Guid, FactoryNodeTreeDto> nodeDict, FactoryNodeTreeDto parent)
        {
            var children = nodeDict.Values.Where(x => x.ParentId == parent.Id).ToList();
            if (!children.Any()) return;

            parent.Children = children;
            parent.HasChildren = true;

            foreach (var child in children)
            {
                BuildSubTree(nodeDict, child);
            }
        }

        /// <summary>
        /// 设置节点层级
        /// </summary>
        private void SetNodeLevel(FactoryNodeTreeDto node, int level)
        {
            node.Level = level;
            foreach (var child in node.Children)
            {
                SetNodeLevel(child, level + 1);
            }
        }

        /// <summary>
        /// 检查循环引用
        /// </summary>
        private async Task<bool> CheckCircularReference(Guid nodeId, Guid parentId)
        {
            var visitedIds = new HashSet<Guid> { nodeId };
            var currentParentId = parentId;

            while (currentParentId != Guid.Empty)
            {
                if (visitedIds.Contains(currentParentId))
                {
                    return true;
                }

                visitedIds.Add(currentParentId);

                var parentNode = await _factoryNodeRepository.GetAll()
                    .Where(x => x.Id == currentParentId)
                    .Select(x => x.ParentId)
                    .FirstOrDefaultAsync();

                if (!parentNode.HasValue)
                {
                    break;
                }

                currentParentId = parentNode.Value;
            }

            return false;
        }


        /// <summary>
        /// 构建启用的节点树
        /// </summary>
        private List<EnabledNodeDto> BuildEnabledNodeTree(List<EnabledNodeDto> nodes)
        {
            var nodeDict = nodes.ToDictionary(x => x.Id);
            var tree = new List<EnabledNodeDto>();

            foreach (var node in nodes)
            {
                if (!node.ParentId.HasValue || node.ParentId == Guid.Empty)
                {
                    tree.Add(node);
                }
                else if (nodeDict.ContainsKey(node.ParentId.Value))
                {
                    var parent = nodeDict[node.ParentId.Value];
                    parent.Children.Add(node);
                }
            }

            return tree;
        }

        /// <summary>
        /// 根据父节点类型获取子节点类型
        /// </summary>
        private string GetChildNodeType(string parentType)
        {
            return _nodeTypeHierarchy.ContainsKey(parentType)
                ? _nodeTypeHierarchy[parentType].ChildType
                : null;
        }

        /// <summary>
        /// 构建完整路径
        /// </summary>
        private string BuildFullPath(Guid nodeId, IEnumerable<dynamic> allNodes)
        {
            var path = new List<string>();
            var currentNodeId = nodeId;

            while (currentNodeId != Guid.Empty)
            {
                var node = allNodes.FirstOrDefault(x => x.Id == currentNodeId);
                if (node == null) break;

                path.Insert(0, node.Name);
                currentNodeId = node.ParentId ?? Guid.Empty;
            }

            return string.Join(" / ", path);
        }

        /// <summary>
        /// 获取状态名称
        /// </summary>
        private string GetStatusName(string status)
        {
            return status switch
            {
                "Enabled" => "启用",
                "Disabled" => "停用",
                _ => status
            };
        }

        #endregion
    }
}