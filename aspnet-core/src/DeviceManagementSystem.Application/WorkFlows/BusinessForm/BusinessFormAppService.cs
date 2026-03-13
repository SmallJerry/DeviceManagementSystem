using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DeviceManagementSystem.DeviceInfos;
using DeviceManagementSystem.FlowManagement;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using DeviceManagementSystem.WorkFlows.BusinessForm.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.WorkFlows.BusinessForm
{
    /// <summary>
    /// 业务表单管理服务
    /// </summary>
    [Authorize]
    public class BusinessFormAppService : DeviceManagementSystemAppServiceBase
    {
        private readonly IRepository<BusinessForms, Guid> _formRepository;
        private readonly IRepository<FlowDefinitions, Guid> _flowRepository;
        private readonly IUserAppService _userAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BusinessFormAppService(
            IRepository<BusinessForms, Guid> formRepository,
            IRepository<FlowDefinitions, Guid> flowRepository,
            IUserAppService userAppService,
            IDeviceAppService deviceAppService)
        {
            _formRepository = formRepository;
            _flowRepository = flowRepository;
            _userAppService = userAppService;
        }

        #region 基础CRUD

        /// <summary>
        /// 获取业务表单分页列表
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<Page<BusinessFormDto>>> GetPageList(BusinessFormPageInput input)
        {
            try
            {
                if (input.Size > 100)
                {
                    input.Size = 100;
                }

                var query = _formRepository.GetAll().AsNoTracking()
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey),
                        x => x.FormName.Contains(input.SearchKey) ||
                             x.FormCode.Contains(input.SearchKey) ||
                             x.TableName.Contains(input.SearchKey))
                    .WhereIf(input.Status.HasValue,
                        x => x.Status == input.Status.Value)
                    .WhereIf(input.IsBound.HasValue,
                        x => (input.IsBound.Value && x.FlowDefId != null) ||
                             (!input.IsBound.Value && x.FlowDefId == null))
                    .WhereIf(input.CreationTimeBegin.HasValue,
                        x => x.CreationTime >= input.CreationTimeBegin.Value)
                    .WhereIf(input.CreationTimeEnd.HasValue,
                        x => x.CreationTime <= input.CreationTimeEnd.Value.AddDays(1).AddSeconds(-1));

                // 排序处理
                if (!string.IsNullOrWhiteSpace(input.SortField))
                {
                    if (input.SortField.Equals("FormName", StringComparison.OrdinalIgnoreCase))
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.FormName)
                            : query.OrderByDescending(x => x.FormName);
                    }
                    else if (input.SortField.Equals("FormCode", StringComparison.OrdinalIgnoreCase))
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.FormCode)
                            : query.OrderByDescending(x => x.FormCode);
                    }
                    else if (input.SortField.Equals("CreationTime", StringComparison.OrdinalIgnoreCase))
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.CreationTime)
                            : query.OrderByDescending(x => x.CreationTime);
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.CreationTime);
                    }
                }
                else
                {
                    query = query.OrderByDescending(x => x.CreationTime);
                }

                var total = await query.CountAsync();

                var items = await query
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .Select(x => new BusinessFormDto
                    {
                        Id = x.Id,
                        FormName = x.FormName,
                        FormCode = x.FormCode,
                        TableName = x.TableName,
                        Status = x.Status,
                        FlowDefId = x.FlowDefId,
                        FlowName = x.FlowName,
                        Description = x.Description,
                        Creator = x.Creator,
                        CreationTime = x.CreationTime
                    })
                    .ToListAsync();

                var page = new Page<BusinessFormDto>(input.Current, input.Size, total)
                {
                    Current = input.Current,
                    Records = items
                };

                return CommonResult<Page<BusinessFormDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取业务表单分页列表失败", ex);
                return CommonResult<Page<BusinessFormDto>>.Error("获取业务表单分页列表失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取所有业务表单列表（不分页）
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<BusinessFormDto>>> GetAllList()
        {
            try
            {
                var items = await _formRepository.GetAll().AsNoTracking()
                    .OrderByDescending(x => x.CreationTime)
                    .Select(x => new BusinessFormDto
                    {
                        Id = x.Id,
                        FormName = x.FormName,
                        FormCode = x.FormCode,
                        TableName = x.TableName,
                        Status = x.Status,
                        FlowDefId = x.FlowDefId,
                        FlowName = x.FlowName,
                        Description = x.Description,
                        Creator = x.Creator,
                        CreationTime = x.CreationTime
                    })
                    .ToListAsync();

                return CommonResult<List<BusinessFormDto>>.Success(items);
            }
            catch (Exception ex)
            {
                Logger.Error("获取所有业务表单列表失败", ex);
                return CommonResult<List<BusinessFormDto>>.Error("获取业务表单列表失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取业务表单详情
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<BusinessFormDto>> GetById(Guid id)
        {
            try
            {
                var form = await _formRepository.GetAll()
                    .Where(x => x.Id == id)
                    .Select(x => new BusinessFormDto
                    {
                        Id = x.Id,
                        FormName = x.FormName,
                        FormCode = x.FormCode,
                        TableName = x.TableName,
                        Status = x.Status,
                        FlowDefId = x.FlowDefId,
                        FlowName = x.FlowName,
                        Description = x.Description,
                        Creator = x.Creator,
                        CreationTime = x.CreationTime
                    })
                    .FirstOrDefaultAsync();

                if (form == null)
                {
                    return CommonResult<BusinessFormDto>.Error("业务表单不存在");
                }

                return CommonResult<BusinessFormDto>.Success(form);
            }
            catch (Exception ex)
            {
                Logger.Error("获取业务表单详情失败", ex);
                return CommonResult<BusinessFormDto>.Error("获取业务表单详情失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 创建业务表单
        /// </summary>
        public async Task<CommonResult> Create(BusinessFormAddInput input)
        {
            try
            {
                var userId = AbpSession.UserId;
                var creatorUser = await _userAppService.GetNameByUserId(userId.Value);

                // 检查表单编码是否重复
                var codeExists = await _formRepository.GetAll()
                    .AnyAsync(x => x.FormCode == input.FormCode);
                if (codeExists)
                {
                    return CommonResult.Error($"表单编码 '{input.FormCode}' 已存在");
                }

                // 检查表单名称是否重复
                var nameExists = await _formRepository.GetAll()
                    .AnyAsync(x => x.FormName == input.FormName);
                if (nameExists)
                {
                    return CommonResult.Error($"表单名称 '{input.FormName}' 已存在");
                }

                // 检查表名是否重复
                var tableExists = await _formRepository.GetAll()
                    .AnyAsync(x => x.TableName == input.TableName);
                if (tableExists)
                {
                    return CommonResult.Error($"数据库表名 '{input.TableName}' 已存在");
                }

                var form = new BusinessForms
                {
                    FormName = input.FormName,
                    FormCode = input.FormCode,
                    TableName = input.TableName,
                    Status = input.Status,
                    Description = input.Description,
                    Creator = creatorUser
                };

                await _formRepository.InsertAsync(form);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("创建业务表单失败", ex);
                return CommonResult.Error("创建业务表单失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 更新业务表单
        /// </summary>
        public async Task<CommonResult> UpdateById(BusinessFormEditInput input)
        {
            try
            {
                var form = await _formRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == input.Id);

                if (form == null)
                {
                    return CommonResult.Error("业务表单不存在");
                }

                // 检查是否已绑定流程
                if (form.FlowDefId.HasValue)
                {
                    return CommonResult.Error("该表单已绑定流程，不允许修改");
                }

                // 检查表单编码是否重复（排除自身）
                var codeExists = await _formRepository.GetAll()
                    .AnyAsync(x => x.FormCode == input.FormCode && x.Id != input.Id);
                if (codeExists)
                {
                    return CommonResult.Error($"表单编码 '{input.FormCode}' 已存在");
                }

                // 检查表单名称是否重复（排除自身）
                var nameExists = await _formRepository.GetAll()
                    .AnyAsync(x => x.FormName == input.FormName && x.Id != input.Id);
                if (nameExists)
                {
                    return CommonResult.Error($"表单名称 '{input.FormName}' 已存在");
                }

                // 检查表名是否重复（排除自身）
                var tableExists = await _formRepository.GetAll()
                    .AnyAsync(x => x.TableName == input.TableName && x.Id != input.Id);
                if (tableExists)
                {
                    return CommonResult.Error($"数据库表名 '{input.TableName}' 已存在");
                }

                // 更新字段
                form.FormName = input.FormName;
                form.FormCode = input.FormCode;
                form.TableName = input.TableName;
                form.Status = input.Status;
                form.Description = input.Description;

                await _formRepository.UpdateAsync(form);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("更新业务表单失败", ex);
                return CommonResult.Error("更新业务表单失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 批量删除业务表单
        /// </summary>
        public async Task<CommonResult> DeleteByIds([FromBody]BusinessFormBatchDeleteInput input)
        {
            try
            {
                if (input.Ids == null || input.Ids.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的业务表单");
                }

                var forms = await _formRepository.GetAll()
                    .Where(x => input.Ids.Contains(x.Id))
                    .ToListAsync();

                if (!forms.Any())
                {
                    return CommonResult.Error("未找到要删除的业务表单");
                }

                // 检查是否有已绑定流程的表单
                var boundForms = forms.Where(x => x.FlowDefId.HasValue).ToList();
                if (boundForms.Any())
                {
                    var boundNames = string.Join("、", boundForms.Select(x => x.FormName));
                    return CommonResult.Error($"表单 '{boundNames}' 已绑定流程，不允许删除");
                }

                foreach (var form in forms)
                {
                    await _formRepository.DeleteAsync(form);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("批量删除业务表单失败", ex);
                return CommonResult.Error("批量删除业务表单失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 更改表单状态（启用/停用）
        /// </summary>
        public async Task<CommonResult> ChangeStatus(BusinessFormChangeStatusInput input)
        {
            try
            {
                var form = await _formRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == input.Id);

                if (form == null)
                {
                    return CommonResult.Error("业务表单不存在");
                }

                form.Status = input.Status;
                await _formRepository.UpdateAsync(form);
                await CurrentUnitOfWork.SaveChangesAsync();

                var statusText = input.Status ? "启用" : "停用";
                return CommonResult.Ok($"表单已{statusText}");
            }
            catch (Exception ex)
            {
                Logger.Error("更改表单状态失败", ex);
                return CommonResult.Error("更改表单状态失败:" + ex.Message);
            }
        }

        #endregion

        #region 选择器接口

        /// <summary>
        /// 获取业务表单选择器列表（供流程绑定使用）
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<BusinessFormSelectorDto>>> GetListForSelector(BusinessFormSelectorInput input)
        {
            try
            {
                var query = _formRepository.GetAll().AsNoTracking()
                    .Where(x => x.Status) // 只查询启用状态
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey),
                        x => x.FormName.Contains(input.SearchKey) ||
                             x.FormCode.Contains(input.SearchKey));

                // 只显示未绑定的表单（排除已绑定的）
                if (input.OnlyUnbound)
                {
                    if (input.ExcludeFlowId.HasValue && input.ExcludeFlowId.Value != Guid.Empty)
                    {
                        // 编辑时排除当前流程绑定的表单
                        query = query.Where(x => x.FlowDefId == null || x.FlowDefId != input.ExcludeFlowId.Value);
                    }
                    else
                    {
                        query = query.Where(x => x.FlowDefId == null);
                    }
                }

                var forms = await query
                    .OrderByDescending(x => x.CreationTime)
                    .Select(x => new BusinessFormSelectorDto
                    {
                        Id = x.Id,
                        FormName = x.FormName,
                        FormCode = x.FormCode,
                        TableName = x.TableName,
                        Description = x.Description,
                        IsBound = x.FlowDefId.HasValue,
                        FlowDefId = x.FlowDefId,
                        FlowName = x.FlowName,
                        Status = x.Status
                    })
                    .ToListAsync();

                return CommonResult<List<BusinessFormSelectorDto>>.Success(forms);
            }
            catch (Exception ex)
            {
                Logger.Error("获取业务表单选择器列表失败", ex);
                return CommonResult<List<BusinessFormSelectorDto>>.Error("获取业务表单选择器列表失败:" + ex.Message);
            }
        }

        #endregion
        }
    }
