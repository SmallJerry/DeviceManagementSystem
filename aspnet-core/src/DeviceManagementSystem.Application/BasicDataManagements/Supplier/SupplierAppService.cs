using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DeviceManagementSystem.BasicDataManagement;
using DeviceManagementSystem.BasicDataManagements.Supplier.Dto;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.BasicDataManagements.Supplier
{
    /// <summary>
    /// 供应商管理服务
    /// </summary>
    [Authorize]
    public class SupplierAppService : DeviceManagementSystemAppServiceBase, ISupplierAppService
    {
        private readonly IRepository<Suppliers, Guid> _supplierRepository;
        private readonly IUserAppService _userAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="supplierRepository"></param>
        /// <param name="userAppService"></param>
        public SupplierAppService(IRepository<Suppliers, Guid> supplierRepository, IUserAppService userAppService)
        {
            _supplierRepository = supplierRepository;
            _userAppService = userAppService;
        }

        /// <summary>
        /// 获取供应商分页列表
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<Page<SupplierDto>>> GetPageList(SupplierPageInput input)
        {
            try
            {
                // 验证分页参数
                if (input.Size > 100)
                {
                    input.Size = 100;
                }

                var query = _supplierRepository.GetAll().AsNoTracking()
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey),
                        x => x.SupplierName.Contains(input.SearchKey) ||
                             x.SupplierCode.Contains(input.SearchKey) ||
                             x.UnifiedSocialCreditCode.Contains(input.SearchKey) ||
                             x.ContactPerson.Contains(input.SearchKey))
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SupplierLevel),
                        x => x.SupplierLevel == input.SupplierLevel)
                    .WhereIf(input.Status != null,
                        x => x.Status == input.Status)
                    .WhereIf(input.CreationTimeBegin.HasValue,
                        x => x.CreationTime >= input.CreationTimeBegin.Value)
                    .WhereIf(input.CreationTimeEnd.HasValue,
                        x => x.CreationTime <= input.CreationTimeEnd.Value.AddDays(1).AddSeconds(-1));

                // 排序处理
                if (!string.IsNullOrWhiteSpace(input.SortField))
                {
                    if (input.SortField.Equals("SupplierCode", StringComparison.OrdinalIgnoreCase))
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.SupplierCode)
                            : query.OrderByDescending(x => x.SupplierCode);
                    }
                    else if (input.SortField.Equals("SupplierName", StringComparison.OrdinalIgnoreCase))
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.SupplierName)
                            : query.OrderByDescending(x => x.SupplierName);
                    }
                    else if (input.SortField.Equals("SupplierLevel", StringComparison.OrdinalIgnoreCase))
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.SupplierLevel)
                            : query.OrderByDescending(x => x.SupplierLevel);
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
                    .Select(x => new SupplierDto
                    {
                        Id = x.Id,
                        SupplierCode = x.SupplierCode,
                        SupplierName = x.SupplierName,
                        UnifiedSocialCreditCode = x.UnifiedSocialCreditCode,
                        Address = x.Address,
                        ContactPerson = x.ContactPerson,
                        ServiceHotline = x.ServiceHotline,
                        SupplierLevel = x.SupplierLevel,
                        Status = x.Status,
                        ExtendInfo = x.ExtendInfo,
                        CreationTime = x.CreationTime,
                        Creator = x.Creator,
                    })
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var page = new Page<SupplierDto>(input.Current, input.Size, total)
                {
                    Current = input.Current,
                    Records = items
                };

                return CommonResult<Page<SupplierDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取供应商分页列表失败", ex);
                return CommonResult<Page<SupplierDto>>.Error("获取供应商分页列表失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取供应商列表
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<SupplierDto>>> GetList(string searchKey = null)
        {
            try
            {
                var query = _supplierRepository.GetAll().AsNoTracking()
                    .Where(x => x.Status) // 只查询启用状态
                    .WhereIf(!string.IsNullOrWhiteSpace(searchKey),
                        x => x.SupplierName.Contains(searchKey) ||
                             x.SupplierCode.Contains(searchKey) ||
                             x.UnifiedSocialCreditCode.Contains(searchKey));

                var items = await query
                    .OrderBy(x => x.SupplierLevel)
                    .ThenBy(x => x.SupplierName)
                    .Select(x => new SupplierDto
                    {
                        Id = x.Id,
                        SupplierCode = x.SupplierCode,
                        SupplierName = x.SupplierName,
                        UnifiedSocialCreditCode = x.UnifiedSocialCreditCode,
                        Address = x.Address,
                        ContactPerson = x.ContactPerson,
                        ServiceHotline = x.ServiceHotline,
                        SupplierLevel = x.SupplierLevel,
                        Status = x.Status,
                        ExtendInfo = x.ExtendInfo
                    })
                    .ToListAsync();

                return CommonResult<List<SupplierDto>>.Success(items);
            }
            catch (Exception ex)
            {
                Logger.Error("获取供应商列表失败", ex);
                return CommonResult<List<SupplierDto>>.Error("获取供应商列表失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取供应商详情
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<SupplierDto>> GetById(Guid id)
        {
            try
            {
                var supplier = await _supplierRepository.GetAll()
                    .Where(x => x.Id == id)
                    .Select(x => new SupplierDto
                    {
                        Id = x.Id,
                        SupplierCode = x.SupplierCode,
                        SupplierName = x.SupplierName,
                        UnifiedSocialCreditCode = x.UnifiedSocialCreditCode,
                        Address = x.Address,
                        ContactPerson = x.ContactPerson,
                        ServiceHotline = x.ServiceHotline,
                        SupplierLevel = x.SupplierLevel,
                        Status = x.Status,
                        ExtendInfo = x.ExtendInfo,
                        CreationTime = x.CreationTime
                    })
                    .FirstOrDefaultAsync();

                if (supplier == null)
                {
                    return CommonResult<SupplierDto>.Error("供应商不存在");
                }

                return CommonResult<SupplierDto>.Success(supplier);
            }
            catch (Exception ex)
            {
                Logger.Error("获取供应商详情失败", ex);
                return CommonResult<SupplierDto>.Error("获取供应商详情失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 创建供应商
        /// </summary>
        public async Task<CommonResult> Create(SupplierAddInput input)
        {
            try
            {
                var userId = AbpSession.UserId;
                var creatorUser = await _userAppService.GetNameByUserId(userId.Value);
                // 检查统一社会信用代码是否重复
                var creditCodeExists = await _supplierRepository.GetAll()
                    .AnyAsync(x => x.UnifiedSocialCreditCode == input.UnifiedSocialCreditCode);
                if (creditCodeExists)
                {
                    return CommonResult.Error($"统一社会信用代码 '{input.UnifiedSocialCreditCode}' 已存在");
                }

                // 检查供应商名称是否重复
                var nameExists = await _supplierRepository.GetAll()
                    .AnyAsync(x => x.SupplierName == input.SupplierName);
                if (nameExists)
                {
                    return CommonResult.Error($"供应商名称 '{input.SupplierName}' 已存在");
                }

                // 如果提供了供应商编码，检查是否重复
                if (!string.IsNullOrWhiteSpace(input.SupplierCode))
                {
                    var codeExists = await _supplierRepository.GetAll()
                        .AnyAsync(x => x.SupplierCode == input.SupplierCode);
                    if (codeExists)
                    {
                        return CommonResult.Error($"供应商编码 '{input.SupplierCode}' 已存在");
                    }
                }

                var supplier = new Suppliers
                {
                    SupplierName = input.SupplierName,
                    SupplierCode = input.SupplierCode,
                    UnifiedSocialCreditCode = input.UnifiedSocialCreditCode,
                    Address = input.Address,
                    ContactPerson = input.ContactPerson,
                    ServiceHotline = input.ServiceHotline,
                    SupplierLevel = input.SupplierLevel,
                    Status = input.Status,
                    ExtendInfo = input.ExtendInfo,
                    Creator = creatorUser
                };

                await _supplierRepository.InsertAsync(supplier);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("创建供应商失败", ex);
                return CommonResult.Error("创建供应商失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 更新供应商
        /// </summary>
        public async Task<CommonResult> UpdateById(SupplierEditInput input)
        {
            try
            {
                var supplier = await _supplierRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == input.Id);
                if (supplier == null)
                {
                    return CommonResult.Error("供应商不存在");
                }

                // 检查统一社会信用代码是否重复（排除自身）
                var creditCodeExists = await _supplierRepository.GetAll()
                    .AnyAsync(x => x.UnifiedSocialCreditCode == input.UnifiedSocialCreditCode && x.Id != input.Id);
                if (creditCodeExists)
                {
                    return CommonResult.Error($"统一社会信用代码 '{input.UnifiedSocialCreditCode}' 已存在");
                }

                // 检查供应商名称是否重复（排除自身）
                var nameExists = await _supplierRepository.GetAll()
                    .AnyAsync(x => x.SupplierName == input.SupplierName && x.Id != input.Id);
                if (nameExists)
                {
                    return CommonResult.Error($"供应商名称 '{input.SupplierName}' 已存在");
                }

                // 更新字段
                supplier.SupplierName = input.SupplierName;
                supplier.UnifiedSocialCreditCode = input.UnifiedSocialCreditCode;
                supplier.Address = input.Address;
                supplier.ContactPerson = input.ContactPerson;
                supplier.ServiceHotline = input.ServiceHotline;
                supplier.SupplierLevel = input.SupplierLevel;
                supplier.Status = input.Status;
                supplier.ExtendInfo = input.ExtendInfo;

                await _supplierRepository.UpdateAsync(supplier);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("更新供应商失败", ex);
                return CommonResult.Error("更新供应商失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 删除供应商
        /// </summary>
        public async Task<CommonResult> DeleteByIds([FromBody] List<SupplierIdInput> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的供应商");
                }

                var typeIds = ids.Select(x => x.Id).ToList();
                var types = await _supplierRepository.GetAll()
                    .Where(x => typeIds.Contains(x.Id))
                    .ToListAsync();

                if (!types.Any())
                {
                    return CommonResult.Error("未找到要删除的供应商");
                }

                // 检查是否存在关联设备（这里需要根据实际情况实现）
                // var hasRelatedDevices = await CheckHasRelatedDevices(typeIds);
                // if (hasRelatedDevices)
                // {
                //     return CommonResult.Error("存在关联设备，不允许删除");
                // }

                // 执行删除（软删除，由ABP自动处理）
                foreach (var type in types)
                {
                    await _supplierRepository.DeleteAsync(type);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("删除供应商失败", ex);
                return CommonResult.Error("删除供应商失败:" + ex.Message);
            }
        }

        #region 辅助方法

        /// <summary>
        /// 检查是否存在关联设备（示例方法，需要根据实际情况实现）
        /// </summary>
        private async Task<bool> CheckHasRelatedDevices(Guid supplierId)
        {
            // 这里需要查询设备表，检查是否有设备使用了该供应商
            // 示例代码：
            // return await _deviceRepository.GetAll()
            //     .AnyAsync(x => x.SupplierId == supplierId);

            return false; // 暂时返回false，需要根据实际情况实现
        }

        /// <summary>
        /// 检查是否存在采购业务（示例方法，需要根据实际情况实现）
        /// </summary>
        private async Task<bool> CheckHasPurchaseBusiness(Guid supplierId)
        {
            // 这里需要查询采购订单表，检查是否有采购业务使用了该供应商
            // 示例代码：
            // return await _purchaseOrderRepository.GetAll()
            //     .AnyAsync(x => x.SupplierId == supplierId);

            return false; // 暂时返回false，需要根据实际情况实现
        }

        #endregion




    }
}
