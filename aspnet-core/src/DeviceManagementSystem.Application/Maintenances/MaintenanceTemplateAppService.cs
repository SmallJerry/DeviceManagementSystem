using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using DeviceManagementSystem.BasicDataManagement;
using DeviceManagementSystem.Maintenances.Dto;
using DeviceManagementSystem.Maintenances.Interface;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances
{
    /// <summary>
    /// 保养模板服务实现
    /// </summary>
    public class MaintenanceTemplateAppService : DeviceManagementSystemAppServiceBase, IMaintenanceTemplateAppService
    {
        private readonly IRepository<MaintenanceTemplates, Guid> _templateRepository;
        private readonly IRepository<MaintenanceItems, Guid> _itemRepository;
        private readonly IRepository<MaintenanceItemGroups, Guid> _itemGroupRepository;
        private readonly IRepository<MaintenanceStandards, Guid> _standardRepository; // 新增保养标准仓储
        private readonly IRepository<Types, Guid> _typeRepository;

        public MaintenanceTemplateAppService(
            IRepository<MaintenanceTemplates, Guid> templateRepository,
            IRepository<MaintenanceItems, Guid> itemRepository,
            IRepository<MaintenanceItemGroups, Guid> itemGroupRepository,
            IRepository<MaintenanceStandards, Guid> standardRepository, // 注入保养标准仓储
            IRepository<Types, Guid> typeRepository)
        {
            _templateRepository = templateRepository;
            _itemRepository = itemRepository;
            _itemGroupRepository = itemGroupRepository;
            _standardRepository = standardRepository;
            _typeRepository = typeRepository;
        }

        /// <summary>
        /// 获取保养模板分页列表
        /// </summary>
        public async Task<CommonResult<Page<MaintenanceTemplateDto>>> GetPageList([FromQuery] MaintenanceTemplatePageInput input)
        {
            try
            {
                if (input.Size > 100) input.Size = 100;

                var query = from t in _templateRepository.GetAll().AsNoTracking()
                            join type in _typeRepository.GetAll().AsNoTracking() on t.DeviceTypeId equals type.Id into typeJoin
                            from type in typeJoin.DefaultIfEmpty()
                            select new { Template = t, TypeName = type != null ? type.TypeName : null };

                // 应用过滤条件
                if (!string.IsNullOrWhiteSpace(input.SearchKey))
                {
                    query = query.Where(x => x.Template.TemplateName.Contains(input.SearchKey) ||
                                            x.Template.Description.Contains(input.SearchKey));
                }

                if (input.DeviceTypeId.HasValue)
                {
                    query = query.Where(x => x.Template.DeviceTypeId == input.DeviceTypeId.Value);
                }

                if (!string.IsNullOrWhiteSpace(input.MaintenanceLevel))
                {
                    query = query.Where(x => x.Template.MaintenanceLevel == input.MaintenanceLevel);
                }

                if (input.IsActive.HasValue)
                {
                    query = query.Where(x => x.Template.IsActive == input.IsActive.Value);
                }

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Template.CreationTime)
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var result = new List<MaintenanceTemplateDto>();

                foreach (var item in items)
                {
                    var dto = new MaintenanceTemplateDto
                    {
                        Id = item.Template.Id,
                        TemplateName = item.Template.TemplateName,
                        DeviceTypeId = item.Template.DeviceTypeId,
                        DeviceTypeName = item.TypeName,
                        MaintenanceLevel = item.Template.MaintenanceLevel,
                        MaintenanceLevelText = GetMaintenanceLevelText(item.Template.MaintenanceLevel),
                        Description = item.Template.Description,
                        IsActive = item.Template.IsActive,
                        CreationTime = item.Template.CreationTime,
                        ItemCount = await _itemRepository.CountAsync(x => x.TemplateId == item.Template.Id)
                    };

                    result.Add(dto);
                }

                var page = new Page<MaintenanceTemplateDto>(input.Current, input.Size, total)
                {
                    Records = result
                };

                return CommonResult<Page<MaintenanceTemplateDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取保养模板分页列表失败", ex);
                return CommonResult<Page<MaintenanceTemplateDto>>.Error($"获取保养模板分页列表失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取所有保养模板（用于选择器）
        /// </summary>
        public async Task<CommonResult<List<MaintenanceTemplateDto>>> GetListForSelector(Guid? deviceTypeId = null, string level = null)
        {
            try
            {
                var query = from t in _templateRepository.GetAll().AsNoTracking()
                            where t.IsActive == true
                            join type in _typeRepository.GetAll().AsNoTracking() on t.DeviceTypeId equals type.Id into typeJoin
                            from type in typeJoin.DefaultIfEmpty()
                            select new { Template = t, TypeName = type != null ? type.TypeName : null };

                if (deviceTypeId.HasValue)
                {
                    query = query.Where(x => x.Template.DeviceTypeId == deviceTypeId.Value);
                }

                if (!string.IsNullOrWhiteSpace(level))
                {
                    query = query.Where(x => x.Template.MaintenanceLevel == level);
                }

                var items = await query
                    .OrderBy(x => x.Template.MaintenanceLevel)
                    .ThenBy(x => x.Template.TemplateName)
                    .ToListAsync();

                var result = items.Select(x => new MaintenanceTemplateDto
                {
                    Id = x.Template.Id,
                    TemplateName = x.Template.TemplateName,
                    DeviceTypeId = x.Template.DeviceTypeId,
                    DeviceTypeName = x.TypeName,
                    MaintenanceLevel = x.Template.MaintenanceLevel,
                    MaintenanceLevelText = GetMaintenanceLevelText(x.Template.MaintenanceLevel),
                    Description = x.Template.Description,
                    IsActive = x.Template.IsActive,
                    ItemCount = _itemRepository.Count(i => i.TemplateId == x.Template.Id)
                }).ToList();

                return CommonResult<List<MaintenanceTemplateDto>>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取保养模板列表失败", ex);
                return CommonResult<List<MaintenanceTemplateDto>>.Error($"获取保养模板列表失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取保养模板详情（带分组和排序）
        /// </summary>
        public async Task<CommonResult<MaintenanceTemplateDetailDto>> GetById(Guid id)
        {
            try
            {
                var template = await _templateRepository.FirstOrDefaultAsync(id);
                if (template == null)
                {
                    return CommonResult<MaintenanceTemplateDetailDto>.Error("保养模板不存在");
                }

                var type = await _typeRepository.FirstOrDefaultAsync(template.DeviceTypeId);

                // 获取分组
                var groups = await _itemGroupRepository.GetAll()
                    .Where(x => x.TemplateId == id)
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new MaintenanceItemGroupDetailDto
                    {
                        GroupId = x.Id,
                        GroupName = x.GroupName,
                        PointType = x.PointType,
                        SortOrder = (int)x.SortOrder
                    })
                    .ToListAsync();

                // 获取项目（按分组和排序）
                var items = await _itemRepository.GetAll()
                    .Where(x => x.TemplateId == id)
                    .OrderBy(x => x.GroupSortOrder)
                    .ThenBy(x => x.ItemSortOrder)
                    .ToListAsync();

                // 按分组组织数据
                var groupItems = groups.Select(g => new MaintenanceItemGroupDetailDto
                {
                    GroupId = g.GroupId,
                    GroupName = g.GroupName,
                    PointType = g.PointType,
                    SortOrder = g.SortOrder,
                    Items = items.Where(i => i.GroupId == g.GroupId)
                                 .OrderBy(i => i.ItemSortOrder)
                                 .Select(i => new MaintenanceItemDetailDto
                                 {
                                     Id = i.Id,
                                     PointNo = i.PointNo,
                                     PointName = i.PointName,
                                     InspectionContent = i.InspectionContent,
                                     InspectionMethod = string.IsNullOrEmpty(i.InspectionMethod)
                                         ? new List<string>()
                                         : JsonConvert.DeserializeObject<List<string>>(i.InspectionMethod),
                                     GroupSortOrder = (int)i.GroupSortOrder,
                                     ItemSortOrder = (int)i.ItemSortOrder
                                 }).ToList()
                }).ToList();

                var dto = new MaintenanceTemplateDetailDto
                {
                    Id = template.Id,
                    TemplateName = template.TemplateName,
                    DeviceTypeId = template.DeviceTypeId,
                    DeviceTypeName = type?.TypeName,
                    MaintenanceLevel = template.MaintenanceLevel,
                    MaintenanceLevelText = GetMaintenanceLevelText(template.MaintenanceLevel),
                    Description = template.Description,
                    IsActive = template.IsActive,
                    CreationTime = template.CreationTime,
                    Groups = groupItems
                };

                return CommonResult<MaintenanceTemplateDetailDto>.Success(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("获取保养模板详情失败", ex);
                return CommonResult<MaintenanceTemplateDetailDto>.Error($"获取保养模板详情失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建保养模板（支持分组和排序）
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult<Guid>> Create([FromBody] MaintenanceTemplateFullInput input)
        {
            try
            {
                // 验证设备类型是否存在
                var type = await _typeRepository.FirstOrDefaultAsync(input.DeviceTypeId);
                if (type == null)
                {
                    return CommonResult<Guid>.Error("设备类型不存在");
                }

                // 创建模板
                var template = new MaintenanceTemplates
                {
                    TemplateName = input.TemplateName,
                    DeviceTypeId = input.DeviceTypeId,
                    MaintenanceLevel = input.MaintenanceLevel,
                    Description = input.Description,
                    IsActive = input.IsActive
                };

                var templateId = await _templateRepository.InsertAndGetIdAsync(template);
                await CurrentUnitOfWork.SaveChangesAsync();

                // 创建分组和项目
                if (input.Groups != null && input.Groups.Any())
                {
                    int groupSortOrder = 1;
                    foreach (var groupDto in input.Groups)
                    {
                        // 创建分组
                        var group = new MaintenanceItemGroups
                        {
                            TemplateId = templateId,
                            GroupName = groupDto.GroupName,
                            PointType = groupDto.PointType,
                            SortOrder = groupSortOrder++
                        };
                        var groupId = await _itemGroupRepository.InsertAndGetIdAsync(group);
                        await CurrentUnitOfWork.SaveChangesAsync();

                        // 创建项目
                        if (groupDto.Items != null && groupDto.Items.Any())
                        {
                            int itemSortOrder = 1;
                            foreach (var itemDto in groupDto.Items)
                            {
                                // 将 List<string> 序列化为 JSON 字符串
                                string inspectionMethodJson = itemDto.InspectionMethod != null
                                    ? JsonConvert.SerializeObject(itemDto.InspectionMethod)
                                    : null;

                                var item = new MaintenanceItems
                                {
                                    TemplateId = templateId,
                                    GroupId = groupId,
                                    PointNo = itemDto.PointNo,
                                    PointName = itemDto.PointName,
                                    InspectionContent = itemDto.InspectionContent,
                                    InspectionMethod = inspectionMethodJson,
                                    GroupSortOrder = groupSortOrder - 1,
                                    ItemSortOrder = itemSortOrder++
                                };
                                await _itemRepository.InsertAsync(item);

                                // 同步到保养标准库
                                await SyncToStandardLibrary(itemDto, groupDto.PointType);
                            }
                        }
                    }
                    await CurrentUnitOfWork.SaveChangesAsync();
                }

                return CommonResult<Guid>.Success(templateId);
            }
            catch (Exception ex)
            {
                Logger.Error("创建保养模板失败", ex);
                return CommonResult<Guid>.Error($"创建保养模板失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新保养模板（支持分组和排序）
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Update([FromBody] MaintenanceTemplateFullInput input)
        {
            try
            {
                if (!input.Id.HasValue)
                {
                    return CommonResult.Error("模板ID不能为空");
                }

                var template = await _templateRepository.FirstOrDefaultAsync(input.Id.Value);
                if (template == null)
                {
                    return CommonResult.Error("保养模板不存在");
                }

                // 更新模板
                template.TemplateName = input.TemplateName;
                template.DeviceTypeId = input.DeviceTypeId;
                template.MaintenanceLevel = input.MaintenanceLevel;
                template.Description = input.Description;
                template.IsActive = input.IsActive;

                await _templateRepository.UpdateAsync(template);
                await CurrentUnitOfWork.SaveChangesAsync();

                // 获取现有分组
                var existingGroups = await _itemGroupRepository.GetAll()
                    .Where(x => x.TemplateId == input.Id.Value)
                    .ToListAsync();

                var existingGroupIds = existingGroups.Select(x => x.Id).ToHashSet();
                var inputGroupIds = input.Groups?.Where(x => x.GroupId.HasValue).Select(x => x.GroupId.Value).ToHashSet() ?? new HashSet<Guid>();

                // 删除的分组（级联删除项目）
                var groupsToDelete = existingGroups.Where(x => !inputGroupIds.Contains(x.Id)).ToList();
                foreach (var group in groupsToDelete)
                {
                    await _itemRepository.DeleteAsync(x => x.GroupId == group.Id);
                    await _itemGroupRepository.DeleteAsync(group);
                }

                // 新增或更新分组
                if (input.Groups != null)
                {
                    int groupSortOrder = 1;
                    foreach (var groupDto in input.Groups.OrderBy(g => g.SortOrder))
                    {
                        if (groupDto.GroupId.HasValue && existingGroupIds.Contains(groupDto.GroupId.Value))
                        {
                            // 更新分组
                            var group = await _itemGroupRepository.GetAsync(groupDto.GroupId.Value);
                            group.GroupName = groupDto.GroupName;
                            group.PointType = groupDto.PointType;
                            group.SortOrder = groupSortOrder++;
                            await _itemGroupRepository.UpdateAsync(group);
                        }
                        else
                        {
                            // 新增分组
                            var group = new MaintenanceItemGroups
                            {
                                TemplateId = input.Id.Value,
                                GroupName = groupDto.GroupName,
                                PointType = groupDto.PointType,
                                SortOrder = groupSortOrder++
                            };
                            var groupId = await _itemGroupRepository.InsertAndGetIdAsync(group);
                            groupDto.GroupId = groupId;
                        }
                        await CurrentUnitOfWork.SaveChangesAsync();
                    }
                }

                // 获取所有项目
                var existingItems = await _itemRepository.GetAll()
                    .Where(x => x.TemplateId == input.Id.Value)
                    .ToListAsync();

                var existingItemIds = existingItems.Select(x => x.Id).ToHashSet();

                // 收集所有输入项目的ID
                var inputItemIds = new HashSet<Guid>();
                if (input.Groups != null)
                {
                    foreach (var groupDto in input.Groups)
                    {
                        if (groupDto.Items != null)
                        {
                            inputItemIds.UnionWith(groupDto.Items.Where(x => x.Id.HasValue).Select(x => x.Id.Value));
                        }
                    }
                }

                // 删除的项目
                var itemsToDelete = existingItems.Where(x => !inputItemIds.Contains(x.Id)).ToList();
                foreach (var item in itemsToDelete)
                {
                    await _itemRepository.DeleteAsync(item);
                }

                // 新增或更新项目
                if (input.Groups != null)
                {
                    int groupSortOrder = 1;
                    foreach (var groupDto in input.Groups.OrderBy(g => g.SortOrder))
                    {
                        if (groupDto.Items != null)
                        {
                            int itemSortOrder = 1;
                            foreach (var itemDto in groupDto.Items.OrderBy(i => i.SortOrder))
                            {
                                // 将 List<string> 序列化为 JSON 字符串
                                string inspectionMethodJson = itemDto.InspectionMethod != null
                                    ? JsonConvert.SerializeObject(itemDto.InspectionMethod)
                                    : null;

                                if (itemDto.Id.HasValue && existingItemIds.Contains(itemDto.Id.Value))
                                {
                                    // 更新项目
                                    var item = await _itemRepository.GetAsync(itemDto.Id.Value);
                                    item.PointNo = itemDto.PointNo;
                                    item.PointName = itemDto.PointName;
                                    item.InspectionContent = itemDto.InspectionContent;
                                    item.InspectionMethod = inspectionMethodJson;
                                    item.GroupId = groupDto.GroupId.Value;
                                    item.GroupSortOrder = groupSortOrder;
                                    item.ItemSortOrder = itemSortOrder++;
                                    await _itemRepository.UpdateAsync(item);
                                }
                                else
                                {
                                    // 新增项目
                                    var newItem = new MaintenanceItems
                                    {
                                        TemplateId = input.Id.Value,
                                        GroupId = groupDto.GroupId.Value,
                                        PointNo = itemDto.PointNo,
                                        PointName = itemDto.PointName,
                                        InspectionContent = itemDto.InspectionContent,
                                        InspectionMethod = inspectionMethodJson,
                                        GroupSortOrder = groupSortOrder,
                                        ItemSortOrder = itemSortOrder++
                                    };
                                    await _itemRepository.InsertAsync(newItem);

                                    // 同步到保养标准库
                                    await SyncToStandardLibrary(itemDto, groupDto.PointType);
                                }
                            }
                        }
                        groupSortOrder++;
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("更新成功");
            }
            catch (Exception ex)
            {
                Logger.Error("更新保养模板失败", ex);
                return CommonResult.Error($"更新保养模板失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 同步项目到保养标准库
        /// </summary>
        private async Task SyncToStandardLibrary(MaintenanceItemInput itemDto, string pointType)
        {
            try
            {
                // 检查是否已存在相同的保养标准
                var exists = await _standardRepository.GetAll()
                    .AnyAsync(x => x.PointName == itemDto.PointName
                                 && x.PointType == pointType
                                 && x.InspectionContent == itemDto.InspectionContent);

                if (!exists)
                {
                    var standard = new MaintenanceStandards
                    {
                        PointName = itemDto.PointName,
                        PointType = pointType,
                        InspectionContent = itemDto.InspectionContent,
                        Remark = $"从保养模板同步"
                    };

                    // 设置点检方法
                    standard.InspectionMethod = itemDto.InspectionMethod ?? new List<string>();

                    await _standardRepository.InsertAsync(standard);
                    Logger.Info($"新增保养标准: {itemDto.PointName} - {pointType}");
                }
            }
            catch (Exception ex)
            {
                // 记录错误但不要影响主流程
                Logger.Error($"同步保养标准失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 删除保养模板
        /// </summary>
        public async Task<CommonResult> Delete(Guid id)
        {
            try
            {
                // 检查是否有关联的计划
                var planRepository = IocManager.Instance.Resolve<IRepository<MaintenancePlans, Guid>>();
                var hasPlans = await planRepository.GetAll().AnyAsync(x => x.TemplateId == id);
                if (hasPlans)
                {
                    return CommonResult.Error("该模板已被保养计划引用，不能删除");
                }

                // 删除项目
                await _itemRepository.DeleteAsync(x => x.TemplateId == id);

                // 删除分组
                await _itemGroupRepository.DeleteAsync(x => x.TemplateId == id);

                // 删除模板
                await _templateRepository.DeleteAsync(id);

                return CommonResult.Ok("删除成功");
            }
            catch (Exception ex)
            {
                Logger.Error("删除保养模板失败", ex);
                return CommonResult.Error($"删除保养模板失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 批量删除保养模板
        /// </summary>
        public async Task<CommonResult> BatchDelete([FromBody] List<Guid> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的模板");
                }

                foreach (var id in ids)
                {
                    await Delete(id);
                }

                return CommonResult.Ok("批量删除成功");
            }
            catch (Exception ex)
            {
                Logger.Error("批量删除保养模板失败", ex);
                return CommonResult.Error($"批量删除保养模板失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 调整分组顺序
        /// </summary>
        public async Task<CommonResult> ReorderGroups(Guid templateId, List<Guid> groupIds)
        {
            try
            {
                var groups = await _itemGroupRepository.GetAll()
                    .Where(x => x.TemplateId == templateId)
                    .ToListAsync();

                var groupDict = groups.ToDictionary(x => x.Id);

                for (int i = 0; i < groupIds.Count; i++)
                {
                    if (groupDict.TryGetValue(groupIds[i], out var group))
                    {
                        group.SortOrder = i + 1;
                        await _itemGroupRepository.UpdateAsync(group);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();
                return CommonResult.Ok("顺序调整成功");
            }
            catch (Exception ex)
            {
                Logger.Error("调整分组顺序失败", ex);
                return CommonResult.Error($"调整分组顺序失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 调整项目顺序
        /// </summary>
        public async Task<CommonResult> ReorderItems(Guid groupId, List<Guid> itemIds)
        {
            try
            {
                var items = await _itemRepository.GetAll()
                    .Where(x => x.GroupId == groupId)
                    .ToListAsync();

                var itemDict = items.ToDictionary(x => x.Id);

                for (int i = 0; i < itemIds.Count; i++)
                {
                    if (itemDict.TryGetValue(itemIds[i], out var item))
                    {
                        item.ItemSortOrder = i + 1;
                        await _itemRepository.UpdateAsync(item);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();
                return CommonResult.Ok("顺序调整成功");
            }
            catch (Exception ex)
            {
                Logger.Error("调整项目顺序失败", ex);
                return CommonResult.Error($"调整项目顺序失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取保养等级显示文本
        /// </summary>
        private string GetMaintenanceLevelText(string level)
        {
            return level switch
            {
                "月度" => "月度保养",
                "季度" => "季度保养",
                "半年度" => "半年度保养",
                "年度" => "年度保养",
                _ => level
            };
        }
    }
}