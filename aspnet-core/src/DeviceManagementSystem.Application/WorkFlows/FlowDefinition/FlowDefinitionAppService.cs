using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using DeviceManagementSystem.FlowManagement;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using DeviceManagementSystem.WorkFlows.FlowDefinition.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.WorkFlows.FlowDefinition
{
    /// <summary>
    /// 流程管理服务
    /// </summary>
    [Authorize]
    public class FlowDefinitionAppService : DeviceManagementSystemAppServiceBase
    {

        private readonly IRepository<FlowDefinitions, Guid> _flowRepository;
        private readonly IRepository<FlowDefinitionHistories, Guid> _flowHistoryRepository;
        private readonly IRepository<BusinessForms, Guid> _formRepository;
        private readonly IUserAppService _userAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FlowDefinitionAppService(
            IRepository<FlowDefinitions, Guid> flowRepository,
            IRepository<FlowDefinitionHistories, Guid> flowHistoryRepository,
            IRepository<BusinessForms, Guid> formRepository,
            IUserAppService userAppService)
        {
            _flowRepository = flowRepository;
            _flowHistoryRepository = flowHistoryRepository;
            _formRepository = formRepository;
            _userAppService = userAppService;
        }


        #region 流程定义管理

        /// <summary>
        /// 获取流程定义分页列表
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<Page<FlowDefinitionDto>>> GetPageList(FlowDefinitionPageInput input)
        {
            try
            {
                // 验证分页参数
                if (input.Size > 100)
                {
                    input.Size = 100;
                }

                var query = _flowRepository.GetAll().AsNoTracking()
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey),
                        x => x.Name.Contains(input.SearchKey))
                    .WhereIf(input.Status.HasValue,
                        x => x.Status == input.Status.Value)
                    .WhereIf(input.FormId.HasValue && input.FormId.Value != Guid.Empty,
                        x => x.FormId == input.FormId.Value)
                    .WhereIf(input.CreationTimeBegin.HasValue,
                        x => x.CreationTime >= input.CreationTimeBegin.Value)
                    .WhereIf(input.CreationTimeEnd.HasValue,
                        x => x.CreationTime <= input.CreationTimeEnd.Value.AddDays(1).AddSeconds(-1));

                // 排序处理
                if (!string.IsNullOrWhiteSpace(input.SortField))
                {
                    if (input.SortField.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.Name)
                            : query.OrderByDescending(x => x.Name);
                    }
                    else if (input.SortField.Equals("Version", StringComparison.OrdinalIgnoreCase))
                    {
                        query = input.SortOrder?.ToUpper() == "ASC"
                            ? query.OrderBy(x => x.Version)
                            : query.OrderByDescending(x => x.Version);
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
                    // 默认按创建时间倒序
                    query = query.OrderByDescending(x => x.CreationTime);
                }

                var total = await query.CountAsync();

                // 获取所有相关的表单信息
                var formIds = await query.Select(x => x.FormId).Distinct().ToListAsync();
                var forms = await _formRepository.GetAll()
                    .Where(x => formIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.FormName, x.FormCode })
                    .ToDictionaryAsync(x => x.Id);

                var items = await query
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .Select(x => new FlowDefinitionDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Version = x.Version,
                        Status = x.Status,
                        Remark = x.Remark,
                        Cancelable = x.Cancelable,
                        ShowInWorkbench = x.ShowInWorkbench,
                        FormId = (Guid)x.FormId,
                        InitiatorType = x.InitiatorType,
                        FlowInitiators = x.FlowInitiators,
                        NodeConfig = x.NodeConfig,
                        FlowPermission = x.FlowPermission,
                        Editable = true,
                        Creator = x.Creator,
                        CreationTime = x.CreationTime
                    })
                    .ToListAsync();

                // 填充表单信息
                foreach (var item in items)
                {
                    if (forms.ContainsKey(item.FormId))
                    {
                        var form = forms[item.FormId];
                        item.FormName = form.FormName;
                        item.FormCode = form.FormCode;
                    }
                }

                var page = new Page<FlowDefinitionDto>(input.Current, input.Size, total)
                {
                    Current = input.Current,
                    Records = items
                };

                return CommonResult<Page<FlowDefinitionDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取流程定义分页列表失败", ex);
                return CommonResult<Page<FlowDefinitionDto>>.Error("获取流程定义分页列表失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取所有流程定义列表（不分页）
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<FlowDefinitionDto>>> GetAllList()
        {
            try
            {
                var query = _flowRepository.GetAll().AsNoTracking()
                    .OrderByDescending(x => x.CreationTime);

                var flows = await query
                    .Select(x => new FlowDefinitionDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Version = x.Version,
                        Status = x.Status,
                        Remark = x.Remark,
                        Cancelable = x.Cancelable,
                        ShowInWorkbench = x.ShowInWorkbench,
                        FormId = (Guid)x.FormId,
                        InitiatorType = x.InitiatorType,
                        FlowInitiators = x.FlowInitiators,
                        Creator = x.Creator,
                        CreationTime = x.CreationTime
                    })
                    .ToListAsync();

                // 获取表单信息
                var formIds = flows.Select(x => x.FormId).Distinct().ToList();
                var forms = await _formRepository.GetAll()
                    .Where(x => formIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.FormName, x.FormCode })
                    .ToDictionaryAsync(x => x.Id);

                foreach (var item in flows)
                {
                    if (forms.ContainsKey(item.FormId))
                    {
                        var form = forms[item.FormId];
                        item.FormName = form.FormName;
                        item.FormCode = form.FormCode;
                    }
                }

                return CommonResult<List<FlowDefinitionDto>>.Success(flows);
            }
            catch (Exception ex)
            {
                Logger.Error("获取所有流程定义列表失败", ex);
                return CommonResult<List<FlowDefinitionDto>>.Error("获取所有流程定义列表失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取流程定义详情
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<FlowDefinitionDto>> GetById(Guid id)
        {
            try
            {
                var flow = await _flowRepository.GetAll()
                    .Where(x => x.Id == id)
                    .Select(x => new FlowDefinitionDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Version = x.Version,
                        Status = x.Status,
                        Remark = x.Remark,
                        Cancelable = x.Cancelable,
                        ShowInWorkbench = x.ShowInWorkbench,
                        FormId = (Guid)x.FormId,
                        InitiatorType = x.InitiatorType,
                        FlowInitiators = x.FlowInitiators,
                        NodeConfig = x.NodeConfig,
                        FlowPermission = x.FlowPermission,
                        Creator = x.Creator,
                        CreationTime = x.CreationTime
                    })
                    .FirstOrDefaultAsync();

                if (flow == null)
                {
                    return CommonResult<FlowDefinitionDto>.Error("流程定义不存在");
                }

                // 获取表单信息
                var form = await _formRepository.GetAll()
                    .Where(x => x.Id == flow.FormId)
                    .Select(x => new { x.FormName, x.FormCode })
                    .FirstOrDefaultAsync();

                if (form != null)
                {
                    flow.FormName = form.FormName;
                    flow.FormCode = form.FormCode;
                }

                return CommonResult<FlowDefinitionDto>.Success(flow);
            }
            catch (Exception ex)
            {
                Logger.Error("获取流程定义详情失败", ex);
                return CommonResult<FlowDefinitionDto>.Error("获取流程定义详情失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取流程配置JSON
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<FlowConfigJsonDto>> GetFlowConfig(FlowConfigJsonInput input)
        {
            try
            {
                var flow = await _flowRepository.FirstOrDefaultAsync(x => x.Id == input.FlowDefId);

                if (flow == null)
                {
                    return CommonResult<FlowConfigJsonDto>.Error("流程定义不存在");
                }

                var result = new FlowConfigJsonDto
                {
                    WorkFlowDef = new FlowDefinitionDto
                    {
                        Id = flow.Id,
                        Name = flow.Name,
                        Version = flow.Version,
                        Status = flow.Status,
                        Remark = flow.Remark,
                        Cancelable = flow.Cancelable,
                        ShowInWorkbench = flow.ShowInWorkbench,
                        FormId = (Guid)flow.FormId,
                        InitiatorType = flow.InitiatorType,
                        FlowInitiators = flow.FlowInitiators,
                        Creator = flow.Creator,
                        CreationTime = flow.CreationTime
                    },
                    NodeConfig = flow.NodeConfig,
                    FlowPermission = flow.FlowPermission
                };

                return CommonResult<FlowConfigJsonDto>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取流程配置JSON失败", ex);
                return CommonResult<FlowConfigJsonDto>.Error("获取流程配置JSON失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 获取流程配置JSON
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<FlowConfigJsonDto>> GetFlowConfigByFormCode(string formCode)
        {
            try
            {
                var  flowForm =   await   _formRepository.FirstOrDefaultAsync(it => string.Equals(it.FormCode, formCode));

                if(flowForm == null && flowForm.FlowDefId == null)
                {
                    return CommonResult<FlowConfigJsonDto>.Error("当前表单未绑定流程，请联系系统管理员处理！");
                }

                var flow = await _flowRepository.FirstOrDefaultAsync(x => x.Id == flowForm.FlowDefId);

                if (flow == null)
                {
                    return CommonResult<FlowConfigJsonDto>.Error("流程定义不存在");
                }

                var result = new FlowConfigJsonDto
                {
                    WorkFlowDef = new FlowDefinitionDto
                    {
                        Id = flow.Id,
                        Name = flow.Name,
                        Version = flow.Version,
                        Status = flow.Status,
                        Remark = flow.Remark,
                        Cancelable = flow.Cancelable,
                        ShowInWorkbench = flow.ShowInWorkbench,
                        FormId = (Guid)flow.FormId,
                        InitiatorType = flow.InitiatorType,
                        FlowInitiators = flow.FlowInitiators,
                        Creator = flow.Creator,
                        CreationTime = flow.CreationTime
                    },
                    NodeConfig = flow.NodeConfig,
                    FlowPermission = flow.FlowPermission
                };

                return CommonResult<FlowConfigJsonDto>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取流程配置JSON失败", ex);
                return CommonResult<FlowConfigJsonDto>.Error("获取流程配置JSON失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 创建流程定义
        /// </summary>
        public async Task<CommonResult> Create(FlowDefinitionAddInput input)
        {
            try
            {
                var userId = AbpSession.UserId;
                var creatorUser = await _userAppService.GetNameByUserId(userId.Value);

                // 检查流程名称是否重复
                var nameExists = await _flowRepository.GetAll()
                    .AnyAsync(x => x.Name == input.Name);
                if (nameExists)
                {
                    return CommonResult.Error($"流程名称 '{input.Name}' 已存在");
                }

                // 检查表单是否已绑定其他流程
                var formExists = await _flowRepository.GetAll()
                    .AnyAsync(x => x.FormId == input.FormId);
                if (formExists)
                {
                    return CommonResult.Error("该表单已绑定其他流程，一个表单只能绑定一个流程");
                }

                // 验证表单是否存在
                var form = await _formRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == input.FormId);
                if (form == null)
                {
                    return CommonResult.Error("选择的表单不存在");
                }

                // 更新表单的流程关联
                form.FlowDefId = null; // 先置空，下面会更新
                form.FlowName = null;

                var flow = new FlowDefinitions
                {
                    Name = input.Name,
                    Version = 1,
                    Status = 0,
                    Remark = input.Remark,
                    Cancelable = input.Cancelable,
                    ShowInWorkbench = input.ShowInWorkbench,
                    FormId = input.FormId,
                    InitiatorType = input.InitiatorType,
                    FlowInitiators = input.FlowInitiators,
                    NodeConfig = input.NodeConfig,
                    FlowPermission = input.FlowPermission,
                    Creator = creatorUser
                };

                await _flowRepository.InsertAsync(flow);

                // 更新表单的流程关联
                form.FlowDefId = flow.Id;
                form.FlowName = flow.Name;
                await _formRepository.UpdateAsync(form);

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("创建流程定义失败", ex);
                return CommonResult.Error("创建流程定义失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 更新流程定义
        /// </summary>
        public async Task<CommonResult> UpdateById(FlowDefinitionEditInput input)
        {
            try
            {
                var flow = await _flowRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == input.Id);
                if (flow == null)
                {
                    return CommonResult.Error("流程定义不存在");
                }

                // 检查流程名称是否重复（排除自身）
                var nameExists = await _flowRepository.GetAll()
                    .AnyAsync(x => x.Name == input.Name && x.Id != input.Id);
                if (nameExists)
                {
                    return CommonResult.Error($"流程名称 '{input.Name}' 已存在");
                }

                // 检查表单是否已绑定其他流程（排除自身）
                var formExists = await _flowRepository.GetAll()
                    .AnyAsync(x => x.FormId == input.FormId && x.Id != input.Id);
                if (formExists)
                {
                    return CommonResult.Error("该表单已绑定其他流程，一个表单只能绑定一个流程");
                }

                // 验证表单是否存在
                var form = await _formRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == input.FormId);
                if (form == null)
                {
                    return CommonResult.Error("选择的表单不存在");
                }

                // 检查是否需要创建新版本
                if (input.CreateNewVersion)
                {
                    // 保存历史版本
                    var history = new FlowDefinitionHistories
                    {
                        FlowDefId = flow.Id,
                        Name = flow.Name,
                        Version = flow.Version,
                        Remark = flow.Remark,
                        Cancelable = flow.Cancelable,
                        ShowInWorkbench = flow.ShowInWorkbench,
                        FormId = (Guid)flow.FormId,
                        InitiatorType = flow.InitiatorType,
                        FlowInitiators = flow.FlowInitiators,
                        NodeConfig = flow.NodeConfig,
                        FlowPermission = flow.FlowPermission,
                        Creator = flow.Creator
                    };
                    await _flowHistoryRepository.InsertAsync(history);

                    // 更新流程版本
                    flow.Version += 1;
                }

                // 如果表单ID变更，需要更新原表单和新表单的关联
                if (flow.FormId != input.FormId)
                {
                    // 解绑原表单
                    var oldForm = await _formRepository.GetAll()
                        .FirstOrDefaultAsync(x => x.Id == flow.FormId);
                    if (oldForm != null)
                    {
                        oldForm.FlowDefId = null;
                        oldForm.FlowName = null;
                        await _formRepository.UpdateAsync(oldForm);
                    }

                    // 绑定新表单
                    form.FlowDefId = flow.Id;
                    form.FlowName = input.Name;
                    await _formRepository.UpdateAsync(form);
                }

                // 更新字段
                flow.Name = input.Name;
                flow.Remark = input.Remark;
                flow.Cancelable = input.Cancelable;
                flow.ShowInWorkbench = input.ShowInWorkbench;
                flow.FormId = input.FormId;
                flow.InitiatorType = input.InitiatorType;
                flow.FlowInitiators = input.FlowInitiators;
                flow.NodeConfig = input.NodeConfig;
                flow.FlowPermission = input.FlowPermission;

                await _flowRepository.UpdateAsync(flow);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("更新流程定义失败", ex);
                return CommonResult.Error("更新流程定义失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 删除流程定义
        /// </summary>
        public async Task<CommonResult> DeleteByIds([FromBody] List<FlowDefinitionIdInput> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return CommonResult.Error("请选择要删除的流程定义");
                }

                var flowIds = ids.Select(x => x.Id).ToList();
                var flows = await _flowRepository.GetAll()
                    .Where(x => flowIds.Contains(x.Id))
                    .ToListAsync();

                if (!flows.Any())
                {
                    return CommonResult.Error("未找到要删除的流程定义");
                }

                // 检查是否存在关联的流程实例（这里需要根据实际情况实现）
                // foreach (var flow in flows)
                // {
                //     var hasRelatedInstances = await CheckHasRelatedInstances(flow.Id);
                //     if (hasRelatedInstances)
                //     {
                //         return CommonResult.Error($"流程 '{flow.Name}' 存在关联的流程实例，不允许删除");
                //     }
                // }

                // 解绑关联的表单
                foreach (var flow in flows)
                {
                    var form = await _formRepository.GetAll()
                        .FirstOrDefaultAsync(x => x.Id == flow.FormId);
                    if (form != null)
                    {
                        form.FlowDefId = null;
                        form.FlowName = null;
                        await _formRepository.UpdateAsync(form);
                    }
                }

                // 执行删除（软删除，由ABP自动处理）
                foreach (var flow in flows)
                {
                    await _flowRepository.DeleteAsync(flow);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("删除流程定义失败", ex);
                return CommonResult.Error("删除流程定义失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 更改流程状态（启用/停用）
        /// </summary>
        [HttpPut]
        public async Task<CommonResult> ChangeStatus(ChangeFlowStatusInput input)
        {
            try
            {
                var flow = await _flowRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == input.Id);
                if (flow == null)
                {
                    return CommonResult.Error("流程定义不存在");
                }

                flow.Status = input.Status;
                await _flowRepository.UpdateAsync(flow);
                await CurrentUnitOfWork.SaveChangesAsync();

                var statusText = input.Status == 0 ? "启用" : "停用";
                return CommonResult.Ok($"流程已{statusText}");
            }
            catch (Exception ex)
            {
                Logger.Error("更改流程状态失败", ex);
                return CommonResult.Error("更改流程状态失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 启用流程状态
        /// </summary>
        [HttpPut]
        public async Task<CommonResult> EnableById(FlowDefinitionIdInput input)
        {
            try
            {
                var flow = await _flowRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == input.Id);
                if (flow == null)
                {
                    return CommonResult.Error("流程定义不存在");
                }

                flow.Status = flow.Status == 0 ? 1 : 0;
                await _flowRepository.UpdateAsync(flow);
                await CurrentUnitOfWork.SaveChangesAsync();

                var statusText = flow.Status == 0 ? "启用" : "停用";
                return CommonResult.Ok($"流程已{statusText}");
            }
            catch (Exception ex)
            {
                Logger.Error("更改流程状态失败", ex);
                return CommonResult.Error("更改流程状态失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 禁用流程状态
        /// </summary>
        [HttpPut]
        public async Task<CommonResult> FreezeById(FlowDefinitionIdInput input)
        {
            try
            {
                var flow = await _flowRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == input.Id);
                if (flow == null)
                {
                    return CommonResult.Error("流程定义不存在");
                }

                flow.Status = flow.Status == 0 ? 1 : 0;
                await _flowRepository.UpdateAsync(flow);
                await CurrentUnitOfWork.SaveChangesAsync();

                var statusText = flow.Status == 0 ? "启用" : "停用";
                return CommonResult.Ok($"流程已{statusText}");
            }
            catch (Exception ex)
            {
                Logger.Error("更改流程状态失败", ex);
                return CommonResult.Error("更改流程状态失败:" + ex.Message);
            }
        }



        #endregion






        #region 辅助方法

        /// <summary>
        /// 检查是否存在关联的流程实例
        /// </summary>
        private async Task<bool> CheckHasRelatedInstances(Guid flowDefId)
        {
            // 这里需要查询流程实例表，检查是否有流程实例使用了这个流程定义
            // 示例代码：
            // return await _flowInstanceRepository.GetAll()
            //     .AnyAsync(x => x.FlowDefId == flowDefId);

            return false; // 暂时返回false，需要根据实际情况实现
        }

        #endregion

    }
}
