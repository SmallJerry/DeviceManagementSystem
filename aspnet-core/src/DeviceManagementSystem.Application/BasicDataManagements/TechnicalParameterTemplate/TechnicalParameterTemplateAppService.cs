using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DeviceManagementSystem.BasicDataManagement;
using DeviceManagementSystem.BasicDataManagements.TechnicalParameterTemplate.Dto;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.BasicDataManagements.TechnicalParameterTemplate
{
    /// <summary>
    /// 技术参数模板管理服务
    /// </summary>
    [Authorize]
    public class TechnicalParameterTemplateAppService : DeviceManagementSystemAppServiceBase, ITechnicalParameterTemplateAppService
    {
        private readonly IRepository<TechnicalParameterTemplates, Guid> _templateRepository;
        private readonly IRepository<Types, Guid> _typeRepository;
        private readonly IUserAppService _userAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TechnicalParameterTemplateAppService(
            IRepository<TechnicalParameterTemplates, Guid> templateRepository,
            IRepository<Types, Guid> typeRepository,
            IUserAppService userAppService)
        {
            _templateRepository = templateRepository;
            _typeRepository = typeRepository;
            _userAppService = userAppService;
        }

        /// <summary>
        /// 获取技术参数模板分页列表
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<Page<TechnicalParameterTemplateDto>>> GetPageList(TechnicalParameterTemplatePageInput input)
        {
            try
            {
                // 验证分页参数
                if (input.Size > 100)
                {
                    input.Size = 100;
                }

                var query = _templateRepository.GetAll().AsNoTracking()
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey),
                        x => x.TemplateName.Contains(input.SearchKey))
                    .WhereIf(input.TypeId != Guid.Empty,
                        x => x.TypeId == input.TypeId)
                    .WhereIf(input.Status != null,
                        x => x.Status == input.Status)
                    .WhereIf(input.CreationTimeBegin.HasValue,
                        x => x.CreationTime >= input.CreationTimeBegin.Value)
                    .WhereIf(input.CreationTimeEnd.HasValue,
                        x => x.CreationTime <= input.CreationTimeEnd.Value.AddDays(1).AddSeconds(-1));

                // 排序处理
                if (!string.IsNullOrWhiteSpace(input.SortField))
                {
                    if (input.SortField.Equals("TemplateName", StringComparison.OrdinalIgnoreCase))
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.TemplateName)
                            : query.OrderByDescending(x => x.TemplateName);
                    }
                    else if (input.SortField.Equals("CreationTime", StringComparison.OrdinalIgnoreCase))
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.CreationTime)
                            : query.OrderByDescending(x => x.CreationTime);
                    }
                    else
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.CreationTime)
                            : query.OrderByDescending(x => x.CreationTime);
                    }
                }
                else
                {
                    // 默认按创建时间倒序
                    query = query.OrderByDescending(x => x.CreationTime);
                }

                var total = await query.CountAsync();

                // 获取所有相关的类型信息
                var typeIds = await query.Select(x => x.TypeId).Distinct().ToListAsync();
                var types = await _typeRepository.GetAll()
                    .Where(x => typeIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.TypeName, x.TypeCode })
                    .ToDictionaryAsync(x => x.Id);

                var items = await query
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .Select(x => new TechnicalParameterTemplateDto
                    {
                        Id = x.Id,
                        TemplateName = x.TemplateName,
                        TypeId = x.TypeId,
                        Status = x.Status,
                        ParameterJson = x.ParameterJson,
                        Creator = x.Creator,
                        CreationTime = x.CreationTime
                    })
                    .ToListAsync();

                // 填充类型信息并解析参数
                foreach (var item in items)
                {
                    if (types.ContainsKey(item.TypeId))
                    {
                        var type = types[item.TypeId];
                        item.TypeName = type.TypeName;
                        item.TypeCode = type.TypeCode;
                    }

                }

                var page = new Page<TechnicalParameterTemplateDto>(input.Current, input.Size, total)
                {
                    Current = input.Current,
                    Records = items
                };

                return CommonResult<Page<TechnicalParameterTemplateDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取技术参数模板分页列表失败", ex);
                return CommonResult<Page<TechnicalParameterTemplateDto>>.Error("获取技术参数模板分页列表失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取技术参数模板选择器列表
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<TechnicalParameterTemplateSelectorDto>>> GetListForSelector(TechnicalParameterTemplateSelectorInput input)
        {
            try
            {
                var query = _templateRepository.GetAll().AsNoTracking()
                    .Where(x => x.Status) // 只查询启用状态
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey),
                        x => x.TemplateName.Contains(input.SearchKey))
                    .WhereIf(input.TypeId != Guid.Empty,
                        x => x.TypeId == input.TypeId);

                // 获取类型信息
                var templates = await query
                    .OrderByDescending(x => x.CreationTime)
                    .Select(x => new
                    {
                        x.Id,
                        x.TemplateName,
                        x.TypeId,
                        x.ParameterJson,
                        x.CreationTime
                    })
                    .ToListAsync();

                var typeIds = templates.Select(x => x.TypeId).Distinct().ToList();
                var types = await _typeRepository.GetAll()
                    .Where(x => typeIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.TypeName })
                    .ToDictionaryAsync(x => x.Id);

                var result = templates.Select(x =>
                {
                    var dto = new TechnicalParameterTemplateSelectorDto
                    {
                        Id = x.Id,
                        TemplateName = x.TemplateName,
                        TypeId = x.TypeId,
                        CreationTime = x.CreationTime
                    };

                    if (types.ContainsKey(x.TypeId))
                    {
                        dto.TypeName = types[x.TypeId].TypeName;
                    }

                    // 解析参数并计算数量
                    var parameters = ParseParameterJson(x.ParameterJson);
                    dto.ParameterCount = parameters?.Count ?? 0;
                    dto.Parameters = parameters; // 将解析后的参数列表返回给前端

                    return dto;
                }).ToList();

                return CommonResult<List<TechnicalParameterTemplateSelectorDto>>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取技术参数模板选择器列表失败", ex);
                return CommonResult<List<TechnicalParameterTemplateSelectorDto>>.Error("获取技术参数模板选择器列表失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取技术参数模板详情
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<TechnicalParameterTemplateDto>> GetById(Guid id)
        {
            try
            {
                var template = await _templateRepository.GetAll()
                    .Where(x => x.Id == id)
                    .Select(x => new TechnicalParameterTemplateDto
                    {
                        Id = x.Id,
                        TemplateName = x.TemplateName,
                        TypeId = x.TypeId,
                        Status = x.Status,
                        ParameterJson = x.ParameterJson,
                        Creator = x.Creator,
                        CreationTime = x.CreationTime
                    })
                    .FirstOrDefaultAsync();

                if (template == null)
                {
                    return CommonResult<TechnicalParameterTemplateDto>.Error("技术参数模板不存在");
                }

                // 获取类型信息
                var type = await _typeRepository.GetAll()
                    .Where(x => x.Id == template.TypeId)
                    .Select(x => new { x.TypeName, x.TypeCode })
                    .FirstOrDefaultAsync();

                if (type != null)
                {
                    template.TypeName = type.TypeName;
                    template.TypeCode = type.TypeCode;
                }


                return CommonResult<TechnicalParameterTemplateDto>.Success(template);
            }
            catch (Exception ex)
            {
                Logger.Error("获取技术参数模板详情失败", ex);
                return CommonResult<TechnicalParameterTemplateDto>.Error("获取技术参数模板详情失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 创建技术参数模板
        /// </summary>
        public async Task<CommonResult> Create(TechnicalParameterTemplateAddInput input)
        {
            try
            {
                var userId = AbpSession.UserId;
                var creatorUser = await _userAppService.GetNameByUserId(userId.Value);

                // 验证参数JSON格式
                if (!input.IsValidParameterJson())
                {
                    return CommonResult.Error("技术参数JSON格式无效");
                }

                // 检查模板名称是否重复
                var nameExists = await _templateRepository.GetAll()
                    .AnyAsync(x => x.TemplateName == input.TemplateName);
                if (nameExists)
                {
                    return CommonResult.Error($"模板名称 '{input.TemplateName}' 已存在");
                }

                // 验证设备类型是否存在（如果TypeId不为Empty）
                if (input.TypeId != Guid.Empty)
                {
                    var typeExists = await _typeRepository.GetAll()
                        .AnyAsync(x => x.Id == input.TypeId);
                    if (!typeExists)
                    {
                        return CommonResult.Error("指定的设备类型不存在");
                    }
                }

                var template = new TechnicalParameterTemplates
                {
                    TemplateName = input.TemplateName,
                    TypeId = input.TypeId,
                    Status = input.Status,
                    ParameterJson = input.ParameterJson,
                    Creator = creatorUser
                };

                await _templateRepository.InsertAsync(template);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("创建技术参数模板失败", ex);
                return CommonResult.Error("创建技术参数模板失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 更新技术参数模板
        /// </summary>
        public async Task<CommonResult> UpdateById(TechnicalParameterTemplateEditInput input)
        {
            try
            {
                var template = await _templateRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == input.Id);
                if (template == null)
                {
                    return CommonResult.Error("技术参数模板不存在");
                }

                // 检查模板名称是否重复（排除自身）
                var nameExists = await _templateRepository.GetAll()
                    .AnyAsync(x => x.TemplateName == input.TemplateName && x.Id != input.Id);
                if (nameExists)
                {
                    return CommonResult.Error($"模板名称 '{input.TemplateName}' 已存在");
                }

                // 验证设备类型是否存在（如果TypeId不为Empty）
                if (input.TypeId != Guid.Empty)
                {
                    var typeExists = await _typeRepository.GetAll()
                        .AnyAsync(x => x.Id == input.TypeId);
                    if (!typeExists)
                    {
                        return CommonResult.Error("指定的设备类型不存在");
                    }
                }

                // 验证参数JSON格式
                try
                {
                    var parameters = JsonConvert.DeserializeObject<List<TechnicalParameter>>(input.ParameterJson);
                    if (parameters == null || !parameters.Any())
                    {
                        return CommonResult.Error("技术参数JSON格式无效");
                    }
                }
                catch
                {
                    return CommonResult.Error("技术参数JSON格式无效");
                }

                // 更新字段
                template.TemplateName = input.TemplateName;
                template.TypeId = input.TypeId;
                template.Status = input.Status;
                template.ParameterJson = input.ParameterJson;

                await _templateRepository.UpdateAsync(template);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("更新技术参数模板失败", ex);
                return CommonResult.Error("更新技术参数模板失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 删除技术参数模板
        /// </summary>
        public async Task<CommonResult> DeleteByIds([FromBody] List<TechnicalParameterTemplateIdInput> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的技术参数模板");
                }

                var templateIds = ids.Select(x => x.Id).ToList();
                var templates = await _templateRepository.GetAll()
                    .Where(x => templateIds.Contains(x.Id))
                    .ToListAsync();

                if (!templates.Any())
                {
                    return CommonResult.Error("未找到要删除的技术参数模板");
                }

                // 执行删除（软删除，由ABP自动处理）
                foreach (var template in templates)
                {
                    await _templateRepository.DeleteAsync(template);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("删除技术参数模板失败", ex);
                return CommonResult.Error("删除技术参数模板失败:" + ex.Message);
            }
        }

        #region 辅助方法

        /// <summary>
        /// 解析参数JSON
        /// </summary>
        private List<TechnicalParameter> ParseParameterJson(string parameterJson)
        {
            if (string.IsNullOrWhiteSpace(parameterJson))
                return new List<TechnicalParameter>();

            try
            {
                return JsonConvert.DeserializeObject<List<TechnicalParameter>>(parameterJson) ?? new List<TechnicalParameter>();
            }
            catch
            {
                return new List<TechnicalParameter>();
            }
        }



        /// <summary>
        /// 验证参数JSON格式
        /// </summary>
        private bool ValidateParameterJson(string parameterJson)
        {
            if (string.IsNullOrWhiteSpace(parameterJson))
                return false;

            try
            {
                var jsonArray = JArray.Parse(parameterJson);
                if (!jsonArray.Any())
                    return false;

                foreach (var item in jsonArray)
                {
                    if (item["ParameterName"] == null || item["ParameterValue"] == null)
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}