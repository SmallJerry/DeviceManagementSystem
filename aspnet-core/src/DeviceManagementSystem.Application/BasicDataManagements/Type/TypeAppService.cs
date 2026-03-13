using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DeviceManagementSystem.BasicDataManagement;
using DeviceManagementSystem.BasicDataManagements.Type.Dto;
using DeviceManagementSystem.DeviceInfos;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeviceManagementSystem.BasicDataManagements.Type
{

    /// <summary>
    /// 类型管理服务
    /// </summary>
    [Authorize]
    public class TypeAppService : DeviceManagementSystemAppServiceBase
    {


        private readonly IRepository<Types, Guid> _typeRepository;
        private readonly IUserAppService _userAppService;
        private readonly IRepository<DeviceTypeRelations, Guid> _deviceTypeRelationRepository;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeRepository"></param>
        /// <param name="userAppService"></param>
        /// <param name="deviceTypeRelationRepository"></param>
        public TypeAppService(IRepository<Types, Guid> typeRepository,IUserAppService userAppService,IRepository<DeviceTypeRelations,Guid> deviceTypeRelationRepository)
        {
            _typeRepository = typeRepository;
            _userAppService = userAppService;
            _deviceTypeRelationRepository = deviceTypeRelationRepository;
        }




        /// <summary>
        /// 获取类型分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<CommonResult<Page<TypeDto>>> GetPageList(TypePageInput input)
        {
            try
            {
                // 验证分页参数
                if (input.Size > 100)
                {
                    input.Size = 100;
                }

                var query = _typeRepository.GetAll().AsNoTracking()
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey),
                        x => x.TypeName.Contains(input.SearchKey) || x.TypeCode.Contains(input.SearchKey))
                      .WhereIf(input.Status != null,
                        x => x.Status == input.Status)
                    .WhereIf(input.CreationTimeBegin.HasValue,
                        x => x.CreationTime >= input.CreationTimeBegin.Value)
                    .WhereIf(input.CreationTimeEnd.HasValue,
                        x => x.CreationTime <= input.CreationTimeEnd.Value.AddDays(1).AddSeconds(-1));

                // 排序处理
                if (!string.IsNullOrWhiteSpace(input.SortField))
                {
                    if (input.SortField.Equals("TypeCode", StringComparison.OrdinalIgnoreCase))
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.TypeCode)
                            : query.OrderByDescending(x => x.TypeCode);
                    }
                    else if (input.SortField.Equals("TypeName", StringComparison.OrdinalIgnoreCase))
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.TypeName)
                            : query.OrderByDescending(x => x.TypeName);
                    }
                    else if (input.SortField.Equals("SortCode", StringComparison.OrdinalIgnoreCase))
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.SortCode)
                            : query.OrderByDescending(x => x.SortCode);
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
                var items = await query
                    .Select(x => new TypeDto
                    {
                        Id = x.Id,
                        TypeCode = x.TypeCode,
                        TypeName = x.TypeName,
                        Icon = x.Icon,
                        SortCode = x.SortCode,
                        Status = x.Status,
                        Remark = x.Remark,
                        CreationTime = x.CreationTime,
                        Creator = x.Creator,
                    })
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var page = new Page<TypeDto>(input.Current, input.Size, total)
                {
                    Current = input.Current,
                    Records = items
                };

                return CommonResult<Page<TypeDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取类型分页列表失败", ex);
                return CommonResult<Page<TypeDto>>.Error("获取类型分页列表失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 获取类型列表
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<TypeDto>>> GetList(string searchKey = null)
        {
            try
            {
                var query = _typeRepository.GetAll().AsNoTracking()
                    .Where(x => x.Status) // 只查询启用状态
                    .WhereIf(!string.IsNullOrWhiteSpace(searchKey),
                        x => x.TypeName.Contains(searchKey) || x.TypeCode.Contains(searchKey));

                var items = await query
                    .OrderBy(x => x.SortCode)
                    .ThenBy(x => x.TypeName)
                    .Select(x => new TypeDto
                    {
                        Id = x.Id,
                        TypeCode = x.TypeCode,
                        TypeName = x.TypeName,
                        Icon = x.Icon,
                        SortCode = x.SortCode,
                        Status = x.Status,
                        Remark = x.Remark
                    })
                    .ToListAsync();

                return CommonResult<List<TypeDto>>.Success(items);
            }
            catch (Exception ex)
            {
                Logger.Error("获取类型列表失败", ex);
                return CommonResult<List<TypeDto>>.Error("获取类型列表失败:" + ex.Message);
            }
        }




        /// <summary>
        /// 获取类型详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<CommonResult<TypeDto>> GetById(Guid id)
        {
            try
            {
                var type = await _typeRepository.GetAll()
                    .Where(x => x.Id == id)
                    .Select(x => new TypeDto
                    {
                        Id = x.Id,
                        TypeCode = x.TypeCode,
                        TypeName = x.TypeName,
                        Icon = x.Icon,
                        SortCode = x.SortCode,
                        Status = x.Status,
                        Remark = x.Remark,
                        CreationTime = x.CreationTime,
                        Creator = x.Creator,
                    })
                    .FirstOrDefaultAsync();

                if (type == null)
                {
                    return CommonResult<TypeDto>.Error("类型不存在");
                }

                return CommonResult<TypeDto>.Success(type);
            }
            catch (Exception ex)
            {
                Logger.Error("获取类型详情失败", ex);
                return CommonResult<TypeDto>.Error("获取类型详情失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 创建类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CommonResult> Create(TypeAddInput input)
        {
            try
            {
                var userId = AbpSession.UserId;
                var creatorUser = await _userAppService.GetNameByUserId(userId.Value);
                // 检查类型编码是否重复
                var codeExists = await _typeRepository.GetAll()
                    .AnyAsync(x => x.TypeCode == input.TypeCode);
                if (codeExists)
                {
                    return CommonResult.Error($"类型编码 '{input.TypeCode}' 已存在");
                }

                // 检查类型名称是否重复
                var nameExists = await _typeRepository.GetAll()
                    .AnyAsync(x => x.TypeName == input.TypeName);
                if (nameExists)
                {
                    return CommonResult.Error($"类型名称 '{input.TypeName}' 已存在");
                }

                // 处理SVG图标ID
                Guid? iconId = null;
                if (!string.IsNullOrWhiteSpace(input.SvgIconUrl))
                {
                    // 这里假设从URL中提取附件ID，实际需要根据具体实现调整
                    // 例如：从文件服务器返回的URL中解析附件ID
                    // iconId = ExtractIconIdFromUrl(input.SvgIconUrl);
                }

                var type = new Types
                {
                    TypeCode = input.TypeCode,
                    TypeName = input.TypeName,
                    Icon = iconId,
                    SortCode = input.SortCode ?? 1,
                    Status = input.Status,
                    Remark = input.Remark,
                    Creator = creatorUser
                };

                await _typeRepository.InsertAsync(type);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("创建类型失败", ex);
                return CommonResult.Error("创建类型失败:" + ex.Message);
            }
        }




        /// <summary>
        /// 更新类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<CommonResult> UpdateById(TypeEditInput input)
        {
            try
            {
                var type = await _typeRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == input.Id);
                if (type == null)
                {
                    return CommonResult.Error("类型不存在");
                }

                // 检查类型名称是否重复（排除自身）
                var nameExists = await _typeRepository.GetAll()
                    .AnyAsync(x => x.TypeName == input.TypeName && x.Id != input.Id);
                if (nameExists)
                {
                    return CommonResult.Error($"类型名称 '{input.TypeName}' 已存在");
                }

                // 处理SVG图标ID
                if (!string.IsNullOrWhiteSpace(input.SvgIconUrl))
                {
                    // iconId = ExtractIconIdFromUrl(input.SvgIconUrl);
                }

                // 更新字段（类型编码不允许修改）
                type.TypeName = input.TypeName;
                type.SortCode = input.SortCode;
                type.Status = input.Status;
                type.Remark = input.Remark;

                await _typeRepository.UpdateAsync(type);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("更新类型失败", ex);
                return CommonResult.Error("更新类型失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 删除类型
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<CommonResult> DeleteByIds([FromBody] List<TypeIdInput> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的类型");
                }

                var typeIds = ids.Select(x => x.Id).ToList();
                var types = await _typeRepository.GetAll()
                    .Where(x => typeIds.Contains(x.Id))
                    .ToListAsync();

                if (!types.Any())
                {
                    return CommonResult.Error("未找到要删除的类型");
                }

                // 检查是否存在关联设备（这里需要根据实际情况实现）
                var hasRelatedDevices = await CheckHasRelatedDevices(typeIds);
                if (hasRelatedDevices)
                {
                    return CommonResult.Error("存在关联设备，不允许删除");
                }

                // 执行删除（软删除，由ABP自动处理）
                foreach (var type in types)
                {
                    await _typeRepository.DeleteAsync(type);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("删除类型失败", ex);
                return CommonResult.Error("删除类型失败:" + ex.Message);
            }
        }






        #region 辅助方法

        /// <summary>
        /// 验证是否为有效的SVG内容
        /// </summary>
        private bool IsValidSvg(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            // 简单的SVG验证：检查是否包含<svg>标签
            return Regex.IsMatch(content, @"<svg[\s\S]*?>[\s\S]*?</svg>", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 检查是否存在关联设备（示例方法，需要根据实际情况实现）
        /// </summary>
        private async Task<bool> CheckHasRelatedDevices(List<Guid> typeIds)
        {
            // 这里需要查询设备表，检查是否有设备使用了这些类型
            return await _deviceTypeRelationRepository.GetAll()
                .AnyAsync(x => typeIds.Contains(x.TypeId));
        }

        /// <summary>
        /// 从URL中提取图标ID（示例方法，需要根据实际情况实现）
        /// </summary>
        private Guid? ExtractIconIdFromUrl(string svgIconUrl)
        {
            // 这里需要根据实际的URL格式提取附件ID
            // 示例：从 "/api/file/download/{id}" 中提取ID
            // var match = Regex.Match(svgIconUrl, @"/download/([0-9a-fA-F-]{36})");
            // if (match.Success && Guid.TryParse(match.Groups[1].Value, out var id))
            // {
            //     return id;
            // }
            return null;
        }

        #endregion

    }
}
