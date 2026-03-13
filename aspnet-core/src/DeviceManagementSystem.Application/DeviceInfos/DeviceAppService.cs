using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using DeviceManagementSystem.Attachment;
using DeviceManagementSystem.Attachment.Dto;
using DeviceManagementSystem.BasicDataManagement;
using DeviceManagementSystem.DeviceInfos.Dto;
using DeviceManagementSystem.DeviceInfos.Utils;
using DeviceManagementSystem.FileInfos;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using DeviceManagementSystem.WorkFlows;
using DeviceManagementSystem.WorkFlows.FlowInstance;
using DeviceManagementSystem.WorkFlows.FlowInstance.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.DeviceInfos
{
    /// <summary>
    /// 设备管理服务
    /// </summary>
    [Authorize]
    [Audited]
    public class DeviceAppService : DeviceManagementSystemAppServiceBase, IDeviceAppService
    {
        private readonly IRepository<Devices, Guid> _deviceRepository;
        private readonly IRepository<DeviceChangeApplications, Guid> _changeApplyRepository;
        private readonly IRepository<DeviceAndChangeApplicationRelations, Guid> _deviceAndChangeApplicationRelationsRepository;
        private readonly IRepository<DeviceSupplierRelations, Guid> _supplierRelationRepository;
        private readonly IRepository<DeviceFactoryNodeRelations, Guid> _factoryNodeRelationRepository;
        private readonly IRepository<DeviceUserRelations, Guid> _userRelationRepository;
        private readonly IRepository<DeviceTypeRelations, Guid> _deviceTypeRepository;
        private readonly IRepository<Types, Guid> _typeRepository;
        private readonly IRepository<Suppliers, Guid> _supplierRepository;
        private readonly IRepository<FactoryNodes, Guid> _factoryNodeRepository;
        private readonly IRepository<BusinessForms, Guid> _formRepository;
        private readonly IAttachmentAppService _attachmentAppService;
        private readonly IUserAppService _userAppService;
        private readonly IFlowInstanceAppService _flowInstanceAppService;
        private readonly IRepository<FlowInstanceHistories, Guid> _historyRepository;
        private readonly IRepository<FlowNodeTasks, Guid> _taskRepository;

        // 字段标签映射（用于变更对比显示）
        private readonly Dictionary<string, string> _fieldLabels = new Dictionary<string, string>
        {
            { "deviceCode", "设备编码" },
            { "deviceName", "设备名称" },
            { "specification", "规格型号" },
            { "deviceLevel", "设备等级" },
            { "isKeyDevice", "是否为重点设备" },
            { "typeId", "设备类型" },
            { "supplierId", "供应商" },
            { "manufacturer", "生产厂商" },
            { "manufactureDate", "生产日期" },
            { "factoryNo", "出厂编号" },
            { "logisticsNo", "物流单号" },
            { "purchaseNo", "申购单号" },
            { "sourceType", "来源方式" },
            { "location", "存放地点" },
            { "deviceStatus", "设备状态" },
            { "enableDate", "启用日期" },
            { "factoryNodeId", "所属节点" },
            { "maintainUserIds", "维修人员" },
            { "maintenanceUserIds", "保养人员" },
            { "technicalParameters", "技术参数" },
            { "customerRequirements", "客户要求" }
        };


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="deviceRepository"></param>
        /// <param name="changeApplyRepository"></param>
        /// <param name="deviceAndChangeApplicationRelationsRepository"></param>
        /// <param name="supplierRelationRepository"></param>
        /// <param name="factoryNodeRelationRepository"></param>
        /// <param name="userRelationRepository"></param>
        /// <param name="deviceTypeRepository"></param>
        /// <param name="typeRepository"></param>
        /// <param name="supplierRepository"></param>
        /// <param name="factoryNodeRepository"></param>
        /// <param name="formRepository"></param>
        /// <param name="attachmentAppService"></param>
        /// <param name="userAppService"></param>
        /// <param name="flowInstanceAppService"></param>
        /// <param name="historyRepository"></param>
        /// <param name="taskRepository"></param>
        public DeviceAppService(
            IRepository<Devices, Guid> deviceRepository,
            IRepository<DeviceChangeApplications, Guid> changeApplyRepository,
            IRepository<DeviceAndChangeApplicationRelations, Guid> deviceAndChangeApplicationRelationsRepository,
            IRepository<DeviceSupplierRelations, Guid> supplierRelationRepository,
            IRepository<DeviceFactoryNodeRelations, Guid> factoryNodeRelationRepository,
            IRepository<DeviceUserRelations, Guid> userRelationRepository,
            IRepository<DeviceTypeRelations, Guid> deviceTypeRepository,
            IRepository<Types, Guid> typeRepository,
            IRepository<Suppliers, Guid> supplierRepository,
            IRepository<FactoryNodes, Guid> factoryNodeRepository,
            IRepository<BusinessForms, Guid> formRepository,
            IAttachmentAppService attachmentAppService,
            IUserAppService userAppService,
            IFlowInstanceAppService flowInstanceAppService,
            IRepository<FlowInstanceHistories, Guid> historyRepository,
            IRepository<FlowNodeTasks, Guid> taskRepository)
        {
            _deviceRepository = deviceRepository;
            _changeApplyRepository = changeApplyRepository;
            _deviceAndChangeApplicationRelationsRepository = deviceAndChangeApplicationRelationsRepository;
            _supplierRelationRepository = supplierRelationRepository;
            _factoryNodeRelationRepository = factoryNodeRelationRepository;
            _userRelationRepository = userRelationRepository;
            _deviceTypeRepository = deviceTypeRepository;
            _typeRepository = typeRepository;
            _supplierRepository = supplierRepository;
            _factoryNodeRepository = factoryNodeRepository;
            _formRepository = formRepository;
            _attachmentAppService = attachmentAppService;
            _userAppService = userAppService;
            _flowInstanceAppService = flowInstanceAppService;
            _historyRepository = historyRepository;
            _taskRepository = taskRepository;
        }

        #region 设备列表查询

        /// <summary>
        /// 获取设备分页列表
        /// </summary>
        [UnitOfWork]
        [DisableAuditing]
        public async Task<CommonResult<Page<DeviceDto>>> GetPageList([FromQuery] DevicePageInput input)
        {
            try
            {
                if (input.Size > 100)
                    input.Size = 100;

                var query = from d in _deviceRepository.GetAll().AsNoTracking()
                            join dt in _deviceTypeRepository.GetAll().AsNoTracking() on d.Id equals dt.DeviceId into deviceTypeJoin
                            from dt in deviceTypeJoin.DefaultIfEmpty()
                            join t in _typeRepository.GetAll().AsNoTracking() on dt.TypeId equals t.Id into typeJoin
                            from t in typeJoin.DefaultIfEmpty()
                            join ds in _supplierRelationRepository.GetAll().AsNoTracking() on d.Id equals ds.DeviceId into deviceSupplierJoin
                            from ds in deviceSupplierJoin.DefaultIfEmpty()
                            join s in _supplierRepository.GetAll().AsNoTracking() on ds.SupplierId equals s.Id into supplierJoin
                            from s in supplierJoin.DefaultIfEmpty()
                            select new DeviceQueryResult
                            {
                                Device = d,
                                TypeId = dt != null ? dt.TypeId : (Guid?)null,
                                TypeName = t != null ? t.TypeName : null,
                                SupplierId = ds != null ? ds.SupplierId : (Guid?)null,
                                SupplierName = s != null ? s.SupplierName : null
                            };

                query = ApplyFilters(query, input);
                query = ApplySorting(query, input);

                var total = await query.CountAsync();
                var items = await query
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var result = new List<DeviceDto>();
                foreach (var item in items)
                {
                    var dto = await MapToDeviceDto(item.Device, item.TypeId, item.TypeName, item.SupplierId, item.SupplierName);
                    result.Add(dto);
                }

                var page = new Page<DeviceDto>(input.Current, input.Size, total)
                {
                    Records = result
                };

                return CommonResult<Page<DeviceDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取设备分页列表失败", ex);
                return CommonResult<Page<DeviceDto>>.Error("获取设备分页列表失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取设备详情
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<DeviceDto>> GetById(Guid id)
        {
            try
            {
                var device = await _deviceRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (device == null)
                {
                    return CommonResult<DeviceDto>.Error("设备不存在");
                }

                // 获取设备类型关系
                var deviceTypeRelation = await _deviceTypeRepository
                    .FirstOrDefaultAsync(x => x.DeviceId == id);

                Guid? typeId = deviceTypeRelation?.TypeId;
                string typeName = null;

                if (typeId.HasValue)
                {
                    var type = await _typeRepository.FirstOrDefaultAsync(typeId.Value);
                    typeName = type?.TypeName;
                }

                // 获取供应商关系
                var supplierRelation = await _supplierRelationRepository
                    .FirstOrDefaultAsync(x => x.DeviceId == id);

                Guid? supplierId = supplierRelation?.SupplierId;
                string supplierName = null;

                if (supplierId.HasValue)
                {
                    var supplier = await _supplierRepository.FirstOrDefaultAsync(supplierId.Value);
                    supplierName = supplier?.SupplierName;
                }

                // 映射基础信息
                var dto = await MapToDeviceDto(device, typeId, typeName, supplierId, supplierName);

                // 获取变更历史
                dto.ChangeHistory = await GetDeviceChangeHistoryAsync(id);

                return CommonResult<DeviceDto>.Success(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("获取设备详情失败", ex);
                return CommonResult<DeviceDto>.Error("获取设备详情失败:" + ex.Message);
            }
        }

        #endregion

        #region 变更申请管理


        /// <summary>
        /// 检查设备编码是否唯一
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [DisableAuditing]
        public async Task<CommonResult<bool>> CheckDeviceCodeUnique([FromQuery] CheckDeviceCodeUniqueInput input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input.DeviceCode))
                {
                    return CommonResult<bool>.Success(true);
                }

                // 1. 检查设备主表
                var deviceQuery = await _deviceRepository.GetAll().AsNoTracking()
                    .Where(d => d.DeviceCode == input.DeviceCode).ToListAsync();

                if (input.ExcludeId.HasValue && input.ExcludeId.Value != Guid.Empty)
                {
                    deviceQuery = deviceQuery.Where(d => d.Id != input.ExcludeId.Value).ToList();
                }

                var existDeviceQueryCount = deviceQuery.Count;
                if (existDeviceQueryCount > 0)
                {

                    return CommonResult<bool>.Success(false);
                }

                // 2. 检查正在审核的申请（状态为：待审核、审核中）
                // 获取所有非草稿状态的申请
                var applyingApplies = await _changeApplyRepository.GetAll()
                    .Where(a => a.ApplicationStatus == "审核中")
                    .ToListAsync();

                foreach (var apply in applyingApplies)
                {
                    try
                    {
                        // 解析 NewData 中的设备编码
                        var newData = DeviceJsonHelper.DeserializeDeviceEditInput(apply.NewData);
                        if (newData != null && newData.DeviceCode == input.DeviceCode)
                        {
                            // 如果是编辑自身，且是同一个申请ID，则忽略
                            if (input.ExcludeApplyId.HasValue && apply.Id == input.ExcludeApplyId.Value)
                            {
                                continue;
                            }

                            // 如果申请的变更类型是删除，且申请状态是审核中，也需要检查
                            // 因为删除操作也会占用设备编码
                            return CommonResult<bool>.Success(false);
                        }
                    }
                    catch
                    {
                        // 解析失败忽略
                        continue;
                    }
                }

                return CommonResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                Logger.Error("检查设备编码唯一性失败", ex);
                return CommonResult<bool>.Error("检查设备编码唯一性失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 保存草稿
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<Guid>> SaveDraft(DeviceEditInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                var creatorUser = await _userAppService.GetNameByUserId(userId);

                // 验证设备编码是否已存在
                var checkResult = await CheckDeviceCodeUniqueInternal(
                    input.DeviceCode,
                    input.Id,  // 如果是编辑已有草稿，传入草稿ID
                    null       // 新增草稿没有申请ID
                );

                if (!checkResult)
                {
                    return CommonResult<Guid>.Error($"设备编码 '{input.DeviceCode}' 已存在");
                }

                DeviceChangeApplications apply;

                if (input.Id.HasValue)
                {
                    // 编辑草稿：查找对应的变更申请
                    apply = await _changeApplyRepository.FirstOrDefaultAsync(x => x.Id == input.Id.Value);

                    if (apply == null)
                    {
                        // 如果没有找到申请单，说明是编辑已有设备，创建新的变更申请
                        var device = await _deviceRepository.FirstOrDefaultAsync(input.Id.Value);
                        if (device == null)
                            return CommonResult<Guid>.Error("设备不存在");

                        apply = new DeviceChangeApplications
                        {
                            ChangeType = "编辑",
                            Snapshot = apply.NewData,
                            NewData = JsonConvert.SerializeObject(input),
                            ApplicationStatus = "草稿"
                        };

                        await _changeApplyRepository.InsertAsync(apply);
                    }
                    else
                    {
                        // 更新草稿
                        apply.NewData = JsonConvert.SerializeObject(input);
                        await _changeApplyRepository.UpdateAsync(apply);
                    }
                }
                else
                {
                    // 新增草稿
                    apply = new DeviceChangeApplications
                    {
                        ChangeType = "新增",
                        Snapshot = null,
                        NewData = JsonConvert.SerializeObject(input),
                        ApplicationStatus = "草稿"
                    };

                    await _changeApplyRepository.InsertAsync(apply);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult<Guid>.Success(apply.Id);
            }
            catch (Exception ex)
            {
                Logger.Error("保存设备草稿失败", ex);
                return CommonResult<Guid>.Error("保存设备草稿失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 更新草稿
        /// </summary>
        [UnitOfWork]
        [DisableAuditing]
        public async Task<CommonResult> UpdateDraft(UpdateDraftInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                var userName = await _userAppService.GetNameByUserId(userId);

                // 查找申请
                var apply = await _changeApplyRepository.FirstOrDefaultAsync(input.ChangeApplyId);
                if (apply == null)
                    return CommonResult.Error("申请不存在");

                // 检查是否可编辑
                if (!CanEditApply(apply))
                    return CommonResult.Error("当前状态不可编辑");

                // 验证设备编码是否已存在（排除自身）
                var currentData = DeviceJsonHelper.DeserializeDeviceEditInput(apply.NewData);
                if (currentData?.DeviceCode != input.FormData.DeviceCode)
                {
                    var checkResult = await CheckDeviceCodeUniqueInternal(
                        input.FormData.DeviceCode,
                        input.FormData.Id,        // 设备ID（如果是编辑已有设备）
                        input.ChangeApplyId        // 当前申请ID（排除自身）
                    );

                    if (!checkResult)
                    {
                        return CommonResult.Error($"设备编码 '{input.FormData.DeviceCode}' 已存在");
                    }
                }

                // 更新申请数据
                apply.NewData = JsonConvert.SerializeObject(input.FormData);

                // 如果是从已拒绝状态编辑，可以选择重置为草稿或保持原状态
                if (apply.ApplicationStatus == "已拒绝" && input.ResetToDraft.HasValue)
                {
                    apply.ApplicationStatus = "草稿";
                }

                await _changeApplyRepository.UpdateAsync(apply);

                // 更新附件关系
                if (input.FormData?.TechnicalAttachmentWithCategories?.Any() == true)
                {
                    await _attachmentAppService.SetBusinessAttachments(new SetBusinessAttachmentInput
                    {
                        BusinessId = apply.Id,
                        BusinessType = "DeviceChangeApplication",
                        AttachmentWithCategories = input.FormData.TechnicalAttachmentWithCategories
                    });
                }
                else
                {
                    await _attachmentAppService.SetBusinessAttachments(new SetBusinessAttachmentInput
                    {
                        BusinessId = apply.Id,
                        BusinessType = "DeviceChangeApplication",
                        AttachmentWithCategories = new List<AttachmentWithCategory>()
                    });
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("更新成功");
            }
            catch (Exception ex)
            {
                Logger.Error("更新草稿失败", ex);
                return CommonResult.Error("更新草稿失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 提交审核
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> SubmitApply(DeviceSubmitInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                var userName = await _userAppService.GetNameByUserId(userId);


                // 验证设备编码是否已存在
                var checkResult = await CheckDeviceCodeUniqueInternal(
                    input.FormData.DeviceCode,
                    input.DeviceId,  // 如果是编辑，传入设备ID
                    input.ChangeApplyId  // 如果有草稿ID，传入排除
                );

                if (!checkResult)
                {
                    return CommonResult.Error($"设备编码 '{input.FormData.DeviceCode}' 已存在");
                }

                // 查找或创建变更申请
                DeviceChangeApplications apply;

                if (input.ChangeApplyId.HasValue)
                {
                    // 提交已有的草稿 - 使用 Get 而不是 FirstOrDefault，确保实体被跟踪
                    apply = await _changeApplyRepository.GetAsync(input.ChangeApplyId.Value);
                }
                else
                {
                    // 创建新的变更申请
                    if (input.ChangeType == "编辑" && input.DeviceId.HasValue)
                    {
                        // 编辑已有设备 - 使用 Get 确保实体被跟踪
                        var device = await _deviceRepository.GetAsync(input.DeviceId.Value);
                        var latestApprovedApply = await (from deviceAndChangeApplicationRelation in _deviceAndChangeApplicationRelationsRepository.GetAll()
                                                         join changeApply in _changeApplyRepository.GetAll()
                                                           on deviceAndChangeApplicationRelation.DeviceChangeApplicationId equals changeApply.Id
                                                         where deviceAndChangeApplicationRelation.DeviceId == device.Id
                                                           && changeApply.ApplicationStatus == "已通过"
                                                         orderby deviceAndChangeApplicationRelation.SubmitTime descending
                                                         select changeApply)
                                                 .FirstOrDefaultAsync();
                        apply = new DeviceChangeApplications
                        {
                            ChangeType = input.ChangeType,
                            Snapshot = latestApprovedApply.NewData,
                            NewData = JsonConvert.SerializeObject(input.FormData),
                            ApplicationStatus = "待审核",
                            ApplyReason = input.ApplyReason,
                            SubmitterId = userId,
                            SubmitterName = userName,
                            SubmitTime = DateTime.Now
                        };
                    }
                    else if (input.ChangeType == "新增")
                    {
                        apply = new DeviceChangeApplications
                        {
                            ChangeType = input.ChangeType,
                            Snapshot = null,
                            NewData = JsonConvert.SerializeObject(input.FormData),
                            ApplicationStatus = "待审核",
                            ApplyReason = input.ApplyReason,
                            SubmitterId = userId,
                            SubmitterName = userName,
                            SubmitTime = DateTime.Now
                        };
                    }
                    else
                    {
                        return CommonResult.Error("无效的变更类型");
                    }

                    // 插入新实体
                    await _changeApplyRepository.InsertAsync(apply);
                    // 立即保存以获取 ID
                    await CurrentUnitOfWork.SaveChangesAsync();
                }

                // 验证：编辑/删除必须填写申请原因
                if ((apply.ChangeType == "编辑" || apply.ChangeType == "删除") && string.IsNullOrEmpty(apply.ApplyReason))
                {
                    return CommonResult.Error("编辑操作必须填写申请原因");
                }

                // 获取流程定义
                var flowDefId = await GetFlowDefinitionByBusinessForm(apply.ChangeType);
                if (!flowDefId.HasValue)
                    return CommonResult.Error($"未找到{apply.ChangeType}设备关联的流程定义，请先配置业务流程");

                // 构建流程表单数据
                var formData = new
                {
                    ApplyId = apply.Id,
                    ChangeType = apply.ChangeType,
                    DeviceName = input.FormData?.DeviceName,
                    ApplyReason = apply.ApplyReason,
                    SubmitterName = userName
                };

                // 启动流程
                var flowInstanceId = await _flowInstanceAppService.StartProcessAsync(
                    flowDefId.Value,
                    apply.Id,
                    "DeviceChangeApplication",
                    userId,
                    JsonConvert.SerializeObject(formData)
                );

                // 一次性更新所有需要修改的字段
                apply.ApplicationStatus = "审核中";
                apply.SubmitterId = userId;
                apply.SubmitterName = userName;
                apply.SubmitTime = DateTime.Now;
                if (!string.IsNullOrEmpty(input.ApplyReason))
                    apply.ApplyReason = input.ApplyReason;

                await _changeApplyRepository.UpdateAsync(apply);

                // 创建关系记录
                var relation = new DeviceAndChangeApplicationRelations
                {
                    DeviceId = input.DeviceId ?? Guid.Empty,
                    DeviceChangeApplicationId = apply.Id,
                    ChangeType = apply.ChangeType,
                    SubmitTime = DateTime.Now,
                    SubmitterId = userId,
                    SubmitterName = userName,
                    ApplyReason = apply.ApplyReason,
                    FlowInstanceId = flowInstanceId
                };

                await _deviceAndChangeApplicationRelationsRepository.InsertAsync(relation);

                // 保存附件关系（如果有）
                if (input.FormData?.TechnicalAttachmentWithCategories?.Any() == true)
                {
                    await _attachmentAppService.SetBusinessAttachments(new SetBusinessAttachmentInput
                    {
                        BusinessId = apply.Id,
                        BusinessType = "DeviceChangeApplication",
                        AttachmentWithCategories = input.FormData.TechnicalAttachmentWithCategories
                    });
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("提交设备申请失败", ex);
                return CommonResult.Error("提交设备申请失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 撤销申请
        /// </summary>
        public async Task<CommonResult> CancelApply(CancelApplyInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                var userName = await _userAppService.GetNameByUserId(userId);

                var apply = await _changeApplyRepository.FirstOrDefaultAsync(input.ChangeApplyId);
                if (apply == null)
                    return CommonResult.Error("变更申请不存在");

                if (apply.ApplicationStatus != "待审核" && apply.ApplicationStatus != "审核中")
                    return CommonResult.Error("当前状态不可撤销");

                // 查找关联的流程实例
                var relation = await _deviceAndChangeApplicationRelationsRepository
                    .FirstOrDefaultAsync(x => x.DeviceChangeApplicationId == apply.Id);

                if (relation?.FlowInstanceId != null)
                {
                    // 撤销流程
                    await _flowInstanceAppService.CancelProcessAsync(
                        relation.FlowInstanceId.Value,
                        userId,
                        input.Reason ?? "用户撤销"
                    );
                }

                // 更新申请单状态
                apply.ApplicationStatus = "已撤销";
                await _changeApplyRepository.UpdateAsync(apply);

                return CommonResult.Ok("撤销成功");
            }
            catch (Exception ex)
            {
                Logger.Error("撤销申请失败", ex);
                return CommonResult.Error("撤销申请失败:" + ex.Message);
            }
        }




        /// <summary>
        /// 重新提交申请（针对已拒绝/已撤销的申请）- 优化版：在原申请上修改，不创建新申请
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Reapply(ReapplyInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                var userName = await _userAppService.GetNameByUserId(userId);

                // 获取原申请
                var apply = await _changeApplyRepository.FirstOrDefaultAsync(input.ChangeApplyId);
                if (apply == null)
                    return CommonResult.Error("申请不存在");

                // 验证状态 - 只有已拒绝/已撤销可以重新申请
                if (!CanReapply(apply))
                    return CommonResult.Error("当前状态不可重新申请，只有已拒绝或已撤销的申请可以重新提交");

                // 获取原关联关系
                var relation = await _deviceAndChangeApplicationRelationsRepository
                    .FirstOrDefaultAsync(x => x.DeviceChangeApplicationId == apply.Id);

                // 解析数据 - 如果提供了新的表单数据则使用，否则使用原数据
                var newData = input.FormData != null
                    ? input.FormData
                    : DeviceJsonHelper.DeserializeDeviceEditInput(apply.NewData);

                // 验证设备编码唯一性（排除自身）
                var checkResult = await CheckDeviceCodeUniqueInternal(
                    newData.DeviceCode,
                    relation?.DeviceId,  // 设备ID（如果是编辑已有设备）
                    apply.Id              // 当前申请ID（排除自身）
                );

                if (!checkResult)
                {
                    return CommonResult.Error($"设备编码 '{newData.DeviceCode}' 已存在");
                }

                // 删除原流程实例的关联（如果有）
                if (relation?.FlowInstanceId != null)
                {
                    // 可选：取消原流程（如果还在运行）
                    try
                    {
                        await _flowInstanceAppService.CancelProcessAsync(
                            relation.FlowInstanceId.Value,
                            userId,
                            "重新提交申请，原流程已作废"
                        );
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"取消原流程失败: {ex.Message}");
                        // 继续执行，不影响重新提交
                    }
                }

                // 更新申请单数据（在原申请上修改）
                apply.NewData = JsonConvert.SerializeObject(newData);
                apply.ApplyReason = input.ApplyReason ?? apply.ApplyReason;
                apply.ApplicationStatus = "待审核"; // 重置状态为待审核
                apply.SubmitterId = userId;
                apply.SubmitterName = userName;
                apply.SubmitTime = DateTime.Now;

                await _changeApplyRepository.UpdateAsync(apply);

                // 获取流程定义
                var flowDefId = await GetFlowDefinitionByBusinessForm(apply.ChangeType);
                if (!flowDefId.HasValue)
                    return CommonResult.Error($"未找到{apply.ChangeType}设备关联的流程定义，请先配置业务流程");

                // 构建流程表单数据
                var formData = new
                {
                    ApplyId = apply.Id,
                    ChangeType = apply.ChangeType,
                    DeviceName = newData.DeviceName,
                    ApplyReason = apply.ApplyReason,
                    SubmitterName = userName,
                    IsReapply = true // 标记为重新申请
                };

                // 启动新流程
                var flowInstanceId = await _flowInstanceAppService.StartProcessAsync(
                    flowDefId.Value,
                    apply.Id,
                    "DeviceChangeApplication",
                    userId,
                    JsonConvert.SerializeObject(formData)
                );

                // 更新申请单状态为审核中
                apply.ApplicationStatus = "审核中";
                await _changeApplyRepository.UpdateAsync(apply);

                // 更新或创建关系记录
                if (relation != null)
                {
                    // 更新现有关系
                    relation.FlowInstanceId = flowInstanceId;
                    relation.SubmitTime = DateTime.Now;
                    relation.SubmitterId = userId;
                    relation.SubmitterName = userName;
                    relation.ApplyReason = apply.ApplyReason;
                    await _deviceAndChangeApplicationRelationsRepository.UpdateAsync(relation);
                }
                else
                {
                    // 创建新关系（理论上不会发生，但为安全起见）
                    relation = new DeviceAndChangeApplicationRelations
                    {
                        DeviceId = Guid.Empty, // 新增类型没有设备ID
                        DeviceChangeApplicationId = apply.Id,
                        ChangeType = apply.ChangeType,
                        SubmitTime = DateTime.Now,
                        SubmitterId = userId,
                        SubmitterName = userName,
                        ApplyReason = apply.ApplyReason,
                        FlowInstanceId = flowInstanceId
                    };
                    await _deviceAndChangeApplicationRelationsRepository.InsertAsync(relation);
                }

                // 处理附件：如果提供了新的附件，则更新；否则保留原附件
                if (input.FormData?.TechnicalAttachmentWithCategories != null)
                {
                    await _attachmentAppService.SetBusinessAttachments(new SetBusinessAttachmentInput
                    {
                        BusinessId = apply.Id,
                        BusinessType = "DeviceChangeApplication",
                        AttachmentWithCategories = input.FormData.TechnicalAttachmentWithCategories
                    });
                }
                // 如果没有提供新附件，但原附件存在且需要保留，则不做任何操作（保留原附件）

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("重新提交成功");
            }
            catch (Exception ex)
            {
                Logger.Error("重新申请失败", ex);
                return CommonResult.Error("重新申请失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 获取设备二维码
        /// </summary>
        [HttpGet]
        [DisableAuditing]
        public async Task<CommonResult<string>> GetQrCode(Guid id)
        {
            try
            {
                var device = await _deviceRepository.FirstOrDefaultAsync(id);
                if (device == null)
                    return CommonResult<string>.Error("设备不存在");

                var baseUrl = $"1";
                var qrCodeUrl = $"{baseUrl}/device/info/{id}";

                if (string.IsNullOrEmpty(device.QrCode))
                {
                    device.QrCode = qrCodeUrl;
                    await _deviceRepository.UpdateAsync(device);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }

                return CommonResult<string>.Success("", qrCodeUrl);
            }
            catch (Exception ex)
            {
                Logger.Error("获取设备二维码失败", ex);
                return CommonResult<string>.Error("获取设备二维码失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取草稿详情（增强版，包含完整关联数据）
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<ChangeApplyDetailDto>> GetDraftById(Guid applyId)
        {
            try
            {
                var apply = await _changeApplyRepository.FirstOrDefaultAsync(applyId);
                if (apply == null)
                    return CommonResult<ChangeApplyDetailDto>.Error("草稿不存在");

                if ((apply.ApplicationStatus != "草稿" || apply.ApplicationStatus != "已撤销") && apply.ChangeType != "新增")
                    return CommonResult<ChangeApplyDetailDto>.Error("当前申请单不是草稿状态");

                // 获取关联关系
                var relation = await _deviceAndChangeApplicationRelationsRepository
                    .FirstOrDefaultAsync(x => x.DeviceChangeApplicationId == applyId);

                // 获取设备信息（如果是编辑已有设备）
                Devices device = null;
                if (relation?.DeviceId != null && relation.DeviceId != Guid.Empty)
                {
                    device = await _deviceRepository.FirstOrDefaultAsync(relation.DeviceId);
                }

                // 解析草稿数据
                var draftData = DeviceJsonHelper.DeserializeDeviceEditInput(apply.NewData);

                // 获取类型信息
                string typeName = null;
                Guid? typeId = null;
                if (draftData?.TypeId.HasValue == true)
                {
                    typeId = draftData.TypeId.Value;
                    var type = await _typeRepository.FirstOrDefaultAsync(typeId.Value);
                    typeName = type?.TypeName;
                }
                else if (device != null)
                {
                    var deviceTypeRelation = await _deviceTypeRepository
                        .FirstOrDefaultAsync(x => x.DeviceId == device.Id);
                    if (deviceTypeRelation != null)
                    {
                        typeId = deviceTypeRelation.TypeId;
                        var type = await _typeRepository.FirstOrDefaultAsync(deviceTypeRelation.TypeId);
                        typeName = type?.TypeName;
                    }
                }

                // 获取供应商信息
                string supplierName = null;
                Guid? supplierId = null;
                if (draftData?.SupplierId.HasValue == true)
                {
                    supplierId = draftData.SupplierId.Value;
                    var supplier = await _supplierRepository.FirstOrDefaultAsync(supplierId.Value);
                    supplierName = supplier?.SupplierName;
                }
                else if (device != null)
                {
                    var deviceSupplierRelation = await _supplierRelationRepository
                        .FirstOrDefaultAsync(x => x.DeviceId == device.Id);
                    if (deviceSupplierRelation != null)
                    {
                        supplierId = deviceSupplierRelation.SupplierId;
                        var supplier = await _supplierRepository.FirstOrDefaultAsync(deviceSupplierRelation.SupplierId);
                        supplierName = supplier?.SupplierName;
                    }
                }

                // 获取工厂节点信息
                string factoryNodeFullPath = null;
                Guid? factoryNodeId = null;
                if (draftData?.FactoryNodeId.HasValue == true)
                {
                    factoryNodeId = draftData.FactoryNodeId.Value;
                    var factoryNodeInfo = await GetFactoryNodeInfo(factoryNodeId.Value);
                    factoryNodeFullPath = factoryNodeInfo?.FullPath;
                }
                else if (device != null)
                {
                    var deviceFactoryRelation = await _factoryNodeRelationRepository
                        .FirstOrDefaultAsync(x => x.DeviceId == device.Id);
                    if (deviceFactoryRelation != null)
                    {
                        factoryNodeId = deviceFactoryRelation.FactoryNodeId;
                        var factoryNodeInfo = await GetFactoryNodeInfo(deviceFactoryRelation.FactoryNodeId);
                        factoryNodeFullPath = factoryNodeInfo?.FullPath;
                    }
                }

                // 获取人员信息
                var maintainUsers = await GetUsersFromIds(draftData?.MaintainUserIds);
                var maintenanceUsers = await GetUsersFromIds(draftData?.MaintenanceUserIds);

                // 获取附件
                List<AttachmentWithCategory> attachments = new List<AttachmentWithCategory>();
                var attachmentsResult = await _attachmentAppService.GetBusinessAttachmentsWithCategory(
                    new GetBusinessAttachmentsInput
                    {
                        BusinessId = applyId,
                        BusinessType = "DeviceChangeApplication"
                    });

                if (attachmentsResult.IsSuccess && attachmentsResult.Data != null)
                {
                    attachments = attachmentsResult.Data.Select(x => new AttachmentWithCategory
                    {
                        AttachmentId = x.Id,
                        Category = x.Category,
                        FileName = x.FileName,
                        FileSize = x.FileSize
                    }).ToList();
                }

                var result = new ChangeApplyDetailDto
                {
                    ApplyId = apply.Id,
                    ChangeType = apply.ChangeType,
                    ApplicationStatus = apply.ApplicationStatus,
                    ApplyReason = apply.ApplyReason,
                    SubmitterId = apply.SubmitterId,
                    SubmitterName = apply.SubmitterName,
                    SubmitTime = apply.SubmitTime,
                    CreationTime = apply.CreationTime,
                    DeviceId = relation?.DeviceId,

                    // 设备基本信息
                    DeviceCode = draftData?.DeviceCode ?? device?.DeviceCode ?? "",
                    DeviceName = draftData?.DeviceName ?? device?.DeviceName ?? "",
                    Specification = draftData?.Specification ?? device?.Specification ?? "",
                    DeviceLevel = draftData?.DeviceLevel ?? device?.DeviceLevel,
                    IsKeyDevice = draftData?.IsKeyDevice ?? device?.IsKeyDevice ?? false,
                    Manufacturer = draftData?.Manufacturer ?? device?.Manufacturer ?? "",
                    ManufactureDate = draftData?.ManufactureDate ?? device?.ManufactureDate,
                    FactoryNo = draftData?.FactoryNo ?? device?.FactoryNo ?? "",
                    LogisticsNo = draftData?.LogisticsNo ?? device?.LogisticsNo ?? "",
                    PurchaseNo = draftData?.PurchaseNo ?? device?.PurchaseNo ?? "",
                    SourceType = draftData?.SourceType ?? device?.SourceType ?? "采购",
                    Location = draftData?.Location ?? device?.Location ?? "",
                    DeviceStatus = draftData?.DeviceStatus ?? device?.DeviceStatus ?? "未验收",
                    EnableDate = draftData?.EnableDate ?? device?.EnableDate,

                    // 关联数据ID
                    TypeId = typeId,
                    SupplierId = supplierId,
                    FactoryNodeId = factoryNodeId,

                    // 关联数据名称
                    TypeName = typeName,
                    SupplierName = supplierName,
                    FactoryNodeFullPath = factoryNodeFullPath,

                    // 人员配置
                    MaintainUsers = maintainUsers,
                    MaintenanceUsers = maintenanceUsers,

                    // 技术参数
                    TechnicalParameters = draftData?.TechnicalParameters ?? new List<TechnicalParameterItem>(),

                    // 客户要求
                    CustomerRequirements = draftData?.CustomerRequirements ?? new List<CustomerRequirementItem>(),

                    // 草稿数据
                    NewData = draftData,

                    // 附件
                    TechnicalAttachmentWithCategories = attachments
                };

                return CommonResult<ChangeApplyDetailDto>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取草稿详情失败", ex);
                return CommonResult<ChangeApplyDetailDto>.Error("获取草稿详情失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 删除草稿
        /// </summary>
        public async Task<CommonResult> DeleteDraft([FromBody] List<Guid> applyIds)
        {
            try
            {
                if (applyIds == null || applyIds.Count == 0)
                    return CommonResult.Error("请选择要删除的草稿");

                var applies = await _changeApplyRepository.GetAll()
                    .Where(x => applyIds.Contains(x.Id) && x.ApplicationStatus == "草稿")
                    .ToListAsync();

                if (!applies.Any())
                    return CommonResult.Error("未找到要删除的草稿");

                foreach (var apply in applies)
                {
                    // 删除附件关系
                    await _attachmentAppService.SetBusinessAttachments(new SetBusinessAttachmentInput
                    {
                        BusinessId = apply.Id,
                        BusinessType = "DeviceChangeApplication",
                        AttachmentWithCategories = new List<AttachmentWithCategory>()
                    });

                    await _changeApplyRepository.DeleteAsync(apply);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("删除草稿失败", ex);
                return CommonResult.Error("删除草稿失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 获取当前登录用户草稿列表（增强版，包含完整关联数据）
        /// </summary>
        [UnitOfWork]
        [DisableAuditing]
        public async Task<CommonResult<Page<DeviceDto>>> GetMyDraftList([FromQuery] DevicePageInput input)
        {
            try
            {
                var CurrentUserId = AbpSession.UserId;

                if (input.Size > 100)
                    input.Size = 100;

                // 从变更申请表中查询草稿
                var query = from a in _changeApplyRepository.GetAll().Where(it => it.CreatorUserId == CurrentUserId).AsNoTracking()
                            where a.ApplicationStatus == "草稿"
                            join r in _deviceAndChangeApplicationRelationsRepository.GetAll().AsNoTracking()
                                on a.Id equals r.DeviceChangeApplicationId into relationJoin
                            from r in relationJoin.DefaultIfEmpty()
                            select new
                            {
                                Apply = a,
                                Relation = r,
                                DeviceId = r != null ? r.DeviceId : (Guid?)null
                            };

                // 应用搜索条件
                if (!string.IsNullOrWhiteSpace(input.SearchKey))
                {
                    query = query.Where(x =>
                        x.Apply.NewData.Contains(input.SearchKey) ||
                        (x.DeviceId != null && _deviceRepository.GetAll().Any(d =>
                            d.Id == x.DeviceId &&
                            (d.DeviceName.Contains(input.SearchKey) || d.DeviceCode.Contains(input.SearchKey))
                        ))
                    );
                }



                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Apply.CreationTime)
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var result = new List<DeviceDto>();

                foreach (var item in items)
                {
                    try
                    {
                        // 反序列化草稿数据
                        var draftData = DeviceJsonHelper.DeserializeDeviceEditInput(item.Apply.NewData);

                        // 获取类型信息
                        string typeName = null;
                        Guid? typeId = null;
                        if (draftData?.TypeId.HasValue == true)
                        {
                            typeId = draftData.TypeId.Value;
                            var type = await _typeRepository.FirstOrDefaultAsync(typeId.Value);
                            typeName = type?.TypeName;
                        }

                        // 获取供应商信息
                        string supplierName = null;
                        Guid? supplierId = null;
                        if (draftData?.SupplierId.HasValue == true)
                        {
                            supplierId = draftData.SupplierId.Value;
                            var supplier = await _supplierRepository.FirstOrDefaultAsync(supplierId.Value);
                            supplierName = supplier?.SupplierName;
                        }

                        // 获取工厂节点信息
                        string factoryNodeFullPath = null;
                        Guid? factoryNodeId = null;
                        if (draftData?.FactoryNodeId.HasValue == true)
                        {
                            factoryNodeId = draftData.FactoryNodeId.Value;
                            var factoryNodeInfo = await GetFactoryNodeInfo(factoryNodeId.Value);
                            factoryNodeFullPath = factoryNodeInfo?.FullPath;
                        }

                        // 获取人员信息
                        var maintainUsers = await GetUsersFromIds(draftData?.MaintainUserIds);
                        var maintenanceUsers = await GetUsersFromIds(draftData?.MaintenanceUserIds);



                        // 转换为 DeviceDto
                        var dto = new DeviceDto
                        {
                            Id = item.DeviceId ?? Guid.Empty,
                            DeviceCode = draftData?.DeviceCode ?? "",
                            DeviceName = draftData?.DeviceName ?? "",
                            Specification = draftData?.Specification ?? "",
                            DeviceLevel = draftData?.DeviceLevel,
                            IsKeyDevice = draftData?.IsKeyDevice ?? false,

                            // 类型信息
                            TypeId = typeId,
                            TypeName = typeName,

                            // 供应商信息
                            SupplierId = supplierId,
                            SupplierName = supplierName,

                            // 工厂节点信息
                            FactoryNodeId = factoryNodeId,
                            FactoryNodeFullPath = factoryNodeFullPath,

                            // 其他基本信息
                            Manufacturer = draftData?.Manufacturer,
                            ManufactureDate = draftData?.ManufactureDate,
                            FactoryNo = draftData?.FactoryNo,
                            LogisticsNo = draftData?.LogisticsNo,
                            PurchaseNo = draftData?.PurchaseNo,
                            SourceType = draftData?.SourceType ?? "采购",
                            Location = draftData?.Location,
                            DeviceStatus = draftData?.DeviceStatus ?? "未验收",
                            EnableDate = draftData?.EnableDate,

                            // 人员配置
                            MaintainUsers = maintainUsers,
                            MaintenanceUsers = maintenanceUsers,

                            // 技术参数
                            TechnicalParameters = draftData?.TechnicalParameters ?? new List<TechnicalParameterItem>(),

                            // 客户要求
                            CustomerRequirements = draftData?.CustomerRequirements ?? new List<CustomerRequirementItem>(),

                            // 申请相关信息
                            BusinessStatus = "草稿",
                            ChangeApplyId = item.Apply.Id,
                            CreationTime = item.Apply.CreationTime,

                        };

                        var attachmentsResult = await _attachmentAppService.GetBusinessAttachmentsWithCategory(
                           new GetBusinessAttachmentsInput
                           {
                               BusinessId = item.Apply.Id,
                               BusinessType = "DeviceChangeApplication"
                           });

                        if (attachmentsResult.IsSuccess && attachmentsResult.Data != null)
                        {
                            dto.TechnicalAttachments = attachmentsResult.Data.GroupBy(x => x.Category)
                                        .ToDictionary(
                                            g => g.Key,
                                            g => g.Select(x => new AttachmentInfo
                                            {
                                                Id = x.Id,
                                                FileName = x.FileName,
                                                FileSize = x.FileSize,
                                                FileSizeFormat = x.FileSizeFormat,
                                                FileUrl = x.FileUrl,
                                                Category = x.Category,
                                                CreationTime = x.CreationTime
                                            }).ToList()
                                        );
                        }

                        result.Add(dto);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"解析草稿数据失败: {ex.Message}");
                        continue;
                    }
                }

                var page = new Page<DeviceDto>(input.Current, input.Size, total)
                {
                    Records = result
                };

                return CommonResult<Page<DeviceDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取当前登录用户草稿列表失败", ex);
                return CommonResult<Page<DeviceDto>>.Error("获取当前登录用户草稿列表失败:" + ex.Message);
            }
        }




        /// <summary>
        /// 获取当前登录用户已提交列表（增强版，包含完整关联数据）
        /// </summary>
        [UnitOfWork]
        [DisableAuditing]
        public async Task<CommonResult<Page<DeviceDto>>> GetMySubmittedList([FromQuery] DevicePageInput input)
        {
            try
            {
                var CurrentUserId = AbpSession.UserId;

                if (input.Size > 100)
                    input.Size = 100;

                // 从变更申请表中查询已提交
                var query = from a in _changeApplyRepository.GetAll().Where(it => it.CreatorUserId == CurrentUserId).AsNoTracking()
                            where a.ApplicationStatus != "草稿"
                            join r in _deviceAndChangeApplicationRelationsRepository.GetAll().AsNoTracking()
                                on a.Id equals r.DeviceChangeApplicationId into relationJoin
                            from r in relationJoin.DefaultIfEmpty()
                            select new
                            {
                                Apply = a,
                                Relation = r,
                                DeviceId = r != null ? r.DeviceId : (Guid?)null
                            };

                // 应用搜索条件
                query = query
                    .WhereIf(!string.IsNullOrWhiteSpace(input.SearchKey), x =>
                        x.Apply.NewData.Contains(input.SearchKey) ||
                        (x.DeviceId != null && _deviceRepository.GetAll().Any(d =>
                            d.Id == x.DeviceId &&
                            (d.DeviceName.Contains(input.SearchKey) || d.DeviceCode.Contains(input.SearchKey))
                        ))

                    )
                    .WhereIf(!string.IsNullOrWhiteSpace(input.BusinessStatus), x => x.Apply.ApplicationStatus == input.BusinessStatus)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.ChangeType), x => x.Apply.ChangeType == input.ChangeType);

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Apply.SubmitTime ?? x.Apply.CreationTime)
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var result = new List<DeviceDto>();

                foreach (var item in items)
                {
                    try
                    {
                        // 反序列化申请数据
                        var applyData = DeviceJsonHelper.DeserializeDeviceEditInput(item.Apply.NewData);

                        // 获取类型、供应商等关联信息
                        string typeName = null;
                        if (applyData?.TypeId.HasValue == true)
                        {
                            var type = await _typeRepository.FirstOrDefaultAsync(applyData.TypeId.Value);
                            typeName = type?.TypeName;
                        }

                        string supplierName = null;
                        if (applyData?.SupplierId.HasValue == true)
                        {
                            var supplier = await _supplierRepository.FirstOrDefaultAsync(applyData.SupplierId.Value);
                            supplierName = supplier?.SupplierName;
                        }

                        string factoryNodeFullPath = null;
                        if (applyData?.FactoryNodeId.HasValue == true)
                        {
                            var factoryNodeInfo = await GetFactoryNodeInfo(applyData.FactoryNodeId.Value);
                            factoryNodeFullPath = factoryNodeInfo?.FullPath;
                        }

                        // 获取人员信息
                        var maintainUsers = await GetUsersFromIds(applyData?.MaintainUserIds);
                        var maintenanceUsers = await GetUsersFromIds(applyData?.MaintenanceUserIds);

                        // 转换为 DeviceDto
                        var dto = new DeviceDto
                        {
                            Id = item.DeviceId ?? Guid.Empty,
                            DeviceCode = applyData?.DeviceCode ?? "",
                            DeviceName = applyData?.DeviceName ?? "",
                            Specification = applyData?.Specification ?? "",
                            DeviceLevel = applyData?.DeviceLevel,
                            IsKeyDevice = applyData?.IsKeyDevice ?? false,
                            TypeId = applyData?.TypeId,
                            TypeName = typeName,
                            SupplierId = applyData?.SupplierId,
                            SupplierName = supplierName,
                            Manufacturer = applyData?.Manufacturer,
                            ManufactureDate = applyData?.ManufactureDate,
                            FactoryNo = applyData?.FactoryNo,
                            LogisticsNo = applyData?.LogisticsNo,
                            PurchaseNo = applyData?.PurchaseNo,
                            SourceType = applyData?.SourceType ?? "采购",
                            Location = applyData?.Location,
                            DeviceStatus = applyData?.DeviceStatus ?? "未验收",
                            EnableDate = applyData?.EnableDate,
                            FactoryNodeId = applyData?.FactoryNodeId,
                            FactoryNodeFullPath = factoryNodeFullPath,
                            BusinessStatus = item.Apply.ApplicationStatus,
                            ChangeType = item.Apply.ChangeType,
                            ChangeApplyId = item.Apply.Id,
                            FlowInstanceId = item.Relation?.FlowInstanceId,
                            ApplyReason = item.Apply.ApplyReason,

                            // 判断是否可撤销：审核中状态
                            Cancelable = item.Apply.ApplicationStatus == "审核中",
                            CreationTime = item.Apply.CreationTime,
                            MaintainUsers = maintainUsers,
                            MaintenanceUsers = maintenanceUsers,
                            TechnicalParameters = applyData?.TechnicalParameters ?? new List<TechnicalParameterItem>(),
                            CustomerRequirements = applyData?.CustomerRequirements ?? new List<CustomerRequirementItem>()
                        };

                        result.Add(dto);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"解析申请数据失败: {ex.Message}");
                        continue;
                    }
                }

                var page = new Page<DeviceDto>(input.Current, input.Size, total)
                {
                    Records = result
                };

                return CommonResult<Page<DeviceDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取当前登录用户已提交列表失败", ex);
                return CommonResult<Page<DeviceDto>>.Error("获取当前登录用户已提交列表失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 获取变更申请详情（增强版，包含完整关联数据）
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<ChangeApplyDetailDto>> GetChangeApplyDetail(Guid applyId)
        {
            try
            {
                var apply = await _changeApplyRepository.FirstOrDefaultAsync(applyId);
                if (apply == null)
                    return CommonResult<ChangeApplyDetailDto>.Error("变更申请不存在");

                // 获取关联关系
                var relation = await _deviceAndChangeApplicationRelationsRepository
                    .FirstOrDefaultAsync(x => x.DeviceChangeApplicationId == applyId);

                // 获取设备信息（如果是编辑或删除）
                Devices device = null;
                if (relation?.DeviceId != null && relation.DeviceId != Guid.Empty)
                {
                    device = await _deviceRepository.FirstOrDefaultAsync(relation.DeviceId);
                }

                // 解析快照和新增数据
                var snapshot = !string.IsNullOrEmpty(apply.Snapshot)
                    ? DeviceJsonHelper.DeserializeDeviceEditInput(apply.Snapshot)
                    : null;

                var newData = !string.IsNullOrEmpty(apply.NewData)
                    ? DeviceJsonHelper.DeserializeDeviceEditInput(apply.NewData)
                    : null;

                // 获取关联数据的名称（类型、供应商、节点等）
                string typeName = null;
                Guid? typeId = null;

                // 优先从newData获取类型ID
                if (newData?.TypeId.HasValue == true)
                {
                    typeId = newData.TypeId.Value;
                    var type = await _typeRepository.FirstOrDefaultAsync(typeId.Value);
                    typeName = type?.TypeName;
                }
                // 其次从设备的关系表获取
                else if (device != null)
                {
                    var deviceTypeRelation = await _deviceTypeRepository
                        .FirstOrDefaultAsync(x => x.DeviceId == device.Id);
                    if (deviceTypeRelation != null)
                    {
                        typeId = deviceTypeRelation.TypeId;
                        var type = await _typeRepository.FirstOrDefaultAsync(deviceTypeRelation.TypeId);
                        typeName = type?.TypeName;
                    }
                }

                // 获取供应商信息
                string supplierName = null;
                Guid? supplierId = null;

                // 优先从newData获取供应商ID
                if (newData?.SupplierId.HasValue == true)
                {
                    supplierId = newData.SupplierId.Value;
                    var supplier = await _supplierRepository.FirstOrDefaultAsync(supplierId.Value);
                    supplierName = supplier?.SupplierName;
                }
                // 其次从设备的关系表获取
                else if (device != null)
                {
                    var deviceSupplierRelation = await _supplierRelationRepository
                        .FirstOrDefaultAsync(x => x.DeviceId == device.Id);
                    if (deviceSupplierRelation != null)
                    {
                        supplierId = deviceSupplierRelation.SupplierId;
                        var supplier = await _supplierRepository.FirstOrDefaultAsync(deviceSupplierRelation.SupplierId);
                        supplierName = supplier?.SupplierName;
                    }
                }

                // 获取工厂节点信息
                string factoryNodeFullPath = null;
                Guid? factoryNodeId = null;

                // 优先从newData获取节点ID
                if (newData?.FactoryNodeId.HasValue == true)
                {
                    factoryNodeId = newData.FactoryNodeId.Value;
                    var factoryNodeInfo = await GetFactoryNodeInfo(factoryNodeId.Value);
                    factoryNodeFullPath = factoryNodeInfo?.FullPath;
                }
                // 其次从设备的关系表获取
                else if (device != null)
                {
                    var deviceFactoryRelation = await _factoryNodeRelationRepository
                        .FirstOrDefaultAsync(x => x.DeviceId == device.Id);
                    if (deviceFactoryRelation != null)
                    {
                        factoryNodeId = deviceFactoryRelation.FactoryNodeId;
                        var factoryNodeInfo = await GetFactoryNodeInfo(deviceFactoryRelation.FactoryNodeId);
                        factoryNodeFullPath = factoryNodeInfo?.FullPath;
                    }
                }

                // 获取人员信息
                List<UserInfo> maintainUsers = new List<UserInfo>();
                List<UserInfo> maintenanceUsers = new List<UserInfo>();

                if (newData != null)
                {
                    // 从newData获取人员
                    maintainUsers = await GetUsersFromIds(newData.MaintainUserIds);
                    maintenanceUsers = await GetUsersFromIds(newData.MaintenanceUserIds);
                }
                else if (snapshot != null)
                {
                    // 从snapshot获取人员
                    maintainUsers = await GetUsersFromIds(snapshot.MaintainUserIds);
                    maintenanceUsers = await GetUsersFromIds(snapshot.MaintenanceUserIds);
                }
                else if (device != null)
                {
                    // 从设备的关系表获取人员
                    maintainUsers = await GetUsersByTypeAsync(device.Id, "维修人员");
                    maintenanceUsers = await GetUsersByTypeAsync(device.Id, "保养人员");
                }

                // 获取附件
                List<AttachmentWithCategory> attachments = new List<AttachmentWithCategory>();
                var attachmentsResult = await _attachmentAppService.GetBusinessAttachmentsWithCategory(
                    new GetBusinessAttachmentsInput
                    {
                        BusinessId = applyId,
                        BusinessType = "DeviceChangeApplication"
                    });

                if (attachmentsResult.IsSuccess && attachmentsResult.Data != null)
                {
                    attachments = attachmentsResult.Data.Select(x => new AttachmentWithCategory
                    {
                        AttachmentId = x.Id,
                        Category = x.Category,
                        FileName = x.FileName,
                        FileSize = x.FileSize
                    }).ToList();
                }

                // 生成字段级变更对比
                var fieldChanges = GenerateChangeDetails(apply.Snapshot, apply.NewData);

                var result = new ChangeApplyDetailDto
                {
                    ApplyId = apply.Id,
                    ChangeType = apply.ChangeType,
                    ApplicationStatus = apply.ApplicationStatus,
                    ApplyReason = apply.ApplyReason,
                    SubmitterId = apply.SubmitterId,
                    SubmitterName = apply.SubmitterName,
                    SubmitTime = apply.SubmitTime,
                    CreationTime = apply.CreationTime,
                    DeviceId = relation?.DeviceId,

                    // 设备基本信息
                    DeviceCode = newData?.DeviceCode ?? device?.DeviceCode ?? "",
                    DeviceName = newData?.DeviceName ?? device?.DeviceName ?? "",
                    Specification = newData?.Specification ?? device?.Specification ?? "",
                    DeviceLevel = newData?.DeviceLevel ?? device?.DeviceLevel,
                    IsKeyDevice = newData?.IsKeyDevice ?? device?.IsKeyDevice ?? false,
                    Manufacturer = newData?.Manufacturer ?? device?.Manufacturer ?? "",
                    ManufactureDate = newData?.ManufactureDate ?? device?.ManufactureDate,
                    FactoryNo = newData?.FactoryNo ?? device?.FactoryNo ?? "",
                    LogisticsNo = newData?.LogisticsNo ?? device?.LogisticsNo ?? "",
                    PurchaseNo = newData?.PurchaseNo ?? device?.PurchaseNo ?? "",
                    SourceType = newData?.SourceType ?? device?.SourceType ?? "采购",
                    Location = newData?.Location ?? device?.Location ?? "",
                    DeviceStatus = newData?.DeviceStatus ?? device?.DeviceStatus ?? "未验收",
                    EnableDate = newData?.EnableDate ?? device?.EnableDate,

                    // 关联数据ID
                    TypeId = typeId,
                    SupplierId = supplierId,
                    FactoryNodeId = factoryNodeId,

                    // 关联数据名称
                    TypeName = typeName,
                    SupplierName = supplierName,
                    FactoryNodeFullPath = factoryNodeFullPath,

                    // 人员配置
                    MaintainUsers = maintainUsers,
                    MaintenanceUsers = maintenanceUsers,

                    // 技术参数
                    TechnicalParameters = newData?.TechnicalParameters ?? snapshot?.TechnicalParameters ?? new List<TechnicalParameterItem>(),

                    // 客户要求
                    CustomerRequirements = newData?.CustomerRequirements ?? snapshot?.CustomerRequirements ?? new List<CustomerRequirementItem>(),

                    // 快照和新数据
                    Snapshot = snapshot,
                    NewData = newData,

                    // 变更对比和流程信息
                    FieldChanges = fieldChanges,
                    FlowInstanceId = relation?.FlowInstanceId,

                    // 附件
                    TechnicalAttachmentWithCategories = attachments
                };

                return CommonResult<ChangeApplyDetailDto>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取变更申请详情失败", ex);
                return CommonResult<ChangeApplyDetailDto>.Error("获取变更申请详情失败:" + ex.Message);
            }
        }


        /// <summary>
        /// 提交删除申请
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> SubmitDeleteApply(DeleteApplyInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                var userName = await _userAppService.GetNameByUserId(userId);

                // 获取设备信息
                var device = await _deviceRepository.FirstOrDefaultAsync(input.DeviceId);
                if (device == null)
                    return CommonResult.Error("设备不存在");

                // 获取设备最新已通过的变更申请
                var latestApprovedApply = await (from deviceAndChangeApplicationRelation in _deviceAndChangeApplicationRelationsRepository.GetAll()
                                                 join changeApply in _changeApplyRepository.GetAll()
                                                   on deviceAndChangeApplicationRelation.DeviceChangeApplicationId equals changeApply.Id
                                                 where deviceAndChangeApplicationRelation.DeviceId == device.Id
                                                   && changeApply.ApplicationStatus == "已通过"
                                                 orderby deviceAndChangeApplicationRelation.SubmitTime descending
                                                 select changeApply)
                                                 .FirstOrDefaultAsync();

                // 创建删除申请
                var apply = new DeviceChangeApplications
                {
                    ChangeType = "删除",
                    Snapshot = null,
                    NewData = JsonConvert.SerializeObject(latestApprovedApply.NewData), // 删除操作NewData可以保存设备信息
                    ApplicationStatus = "待审核",
                    ApplyReason = input.ApplyReason,
                    SubmitterId = userId,
                    SubmitterName = userName,
                    SubmitTime = DateTime.Now
                };

                await _changeApplyRepository.InsertAsync(apply);
                await CurrentUnitOfWork.SaveChangesAsync();

                // 获取流程定义
                var flowDefId = await GetFlowDefinitionByBusinessForm("删除");
                if (!flowDefId.HasValue)
                    return CommonResult.Error("未找到删除设备关联的流程定义，请先配置业务流程");

                // 构建流程表单数据
                var formData = new
                {
                    ApplyId = apply.Id,
                    ChangeType = "删除",
                    DeviceName = device.DeviceName,
                    ApplyReason = input.ApplyReason,
                    SubmitterName = userName
                };

                // 启动流程
                var flowInstanceId = await _flowInstanceAppService.StartProcessAsync(
                    flowDefId.Value,
                    apply.Id,
                    "DeviceChangeApplication",
                    userId,
                    JsonConvert.SerializeObject(formData)
                );

                // 更新申请单状态
                apply.ApplicationStatus = "审核中";
                await _changeApplyRepository.UpdateAsync(apply);

                // 创建关系记录
                var relation = new DeviceAndChangeApplicationRelations
                {
                    DeviceId = input.DeviceId,
                    DeviceChangeApplicationId = apply.Id,
                    ChangeType = "删除",
                    SubmitTime = DateTime.Now,
                    SubmitterId = userId,
                    SubmitterName = userName,
                    ApplyReason = input.ApplyReason,
                    FlowInstanceId = flowInstanceId
                };

                await _deviceAndChangeApplicationRelationsRepository.InsertAsync(relation);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("删除申请提交成功");
            }
            catch (Exception ex)
            {
                Logger.Error("提交删除申请失败", ex);
                return CommonResult.Error("提交删除申请失败:" + ex.Message);
            }
        }




        /// <summary>
        /// 获取申请详情（增强版，供前端使用）
        /// </summary>
        [HttpGet]
        [DisableAuditing]
        public async Task<CommonResult<ChangeApplyDetailDto>> GetApplyDetailWithActions(Guid applyId)
        {
            try
            {
                var result = await GetChangeApplyDetail(applyId);
                if (!result.IsSuccess) return result;

                // 添加可执行操作列表
                var apply = await _changeApplyRepository.FirstOrDefaultAsync(applyId);
                if (apply != null)
                {
                    result.Data.AvailableActions = GetAvailableActions(apply);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("获取申请详情失败", ex);
                return CommonResult<ChangeApplyDetailDto>.Error("获取申请详情失败:" + ex.Message);
            }
        }





        /// <summary>
        /// 删除申请记录（草稿/已撤销/已拒绝）
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> DeleteApply([FromBody] List<Guid> applyIds)
        {
            try
            {
                if (applyIds == null || applyIds.Count == 0)
                    return CommonResult.Error("请选择要删除的申请记录");

                var applies = await _changeApplyRepository.GetAll()
                    .Where(x => applyIds.Contains(x.Id) &&
                               (x.ApplicationStatus == "草稿" ||
                                x.ApplicationStatus == "已撤销" ||
                                x.ApplicationStatus == "已拒绝"))
                    .ToListAsync();

                if (!applies.Any())
                    return CommonResult.Error("未找到可删除的记录");

                foreach (var apply in applies)
                {
                    // 删除附件关系
                    await _attachmentAppService.SetBusinessAttachments(new SetBusinessAttachmentInput
                    {
                        BusinessId = apply.Id,
                        BusinessType = "DeviceChangeApplication",
                        AttachmentWithCategories = new List<AttachmentWithCategory>()
                    });

                    // 删除关联关系
                    var relations = await _deviceAndChangeApplicationRelationsRepository
                        .GetAll()
                        .Where(x => x.DeviceChangeApplicationId == apply.Id)
                        .ToListAsync();

                    foreach (var relation in relations)
                    {
                        await _deviceAndChangeApplicationRelationsRepository.DeleteAsync(relation);
                    }

                    // 删除申请记录
                    await _changeApplyRepository.DeleteAsync(apply);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("删除成功");
            }
            catch (Exception ex)
            {
                Logger.Error("删除失败", ex);
                return CommonResult.Error("删除失败:" + ex.Message);
            }
        }





        #endregion

        #region 流程回调接口

        /// <summary>
        /// 流程完成后处理（公开接口，供流程引擎回调）
        /// </summary>
        [HttpPost]
        public async Task<CommonResult> OnProcessCompleted(ProcessCompletedInput input)
        {
            try
            {
                return await OnProcessCompleted(input.FlowInstanceId, input.ProcessStatus);
            }
            catch (Exception ex)
            {
                Logger.Error("流程完成处理失败", ex);
                return CommonResult.Error("流程完成处理失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 流程完成后处理（由流程引擎回调）
        /// </summary>
        private async Task<CommonResult> OnProcessCompleted(Guid flowInstanceId, int processStatus)
        {
            try
            {
                // 查找关联的申请单
                var relation = await _deviceAndChangeApplicationRelationsRepository
                    .FirstOrDefaultAsync(x => x.FlowInstanceId == flowInstanceId);

                if (relation == null)
                    return CommonResult.Error("未找到关联的申请单");

                var apply = await _changeApplyRepository.FirstOrDefaultAsync(relation.DeviceChangeApplicationId);
                if (apply == null)
                    return CommonResult.Error("申请单不存在");

                // 更新申请单状态
                if (processStatus == 1) // 已通过
                {
                    apply.ApplicationStatus = "已通过";
                    await _changeApplyRepository.UpdateAsync(apply);

                    // 执行变更操作
                    await ApplyChangeToDevice(apply, relation);
                }
                else if (processStatus == 2) // 已拒绝
                {
                    apply.ApplicationStatus = "已拒绝";
                    await _changeApplyRepository.UpdateAsync(apply);
                }
                else if (processStatus == 3) // 已撤销
                {
                    apply.ApplicationStatus = "已撤销";
                    await _changeApplyRepository.UpdateAsync(apply);
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok();
            }
            catch (Exception ex)
            {
                Logger.Error("流程完成处理失败", ex);
                return CommonResult.Error("流程完成处理失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 将变更应用到设备主表
        /// </summary>
        private async Task ApplyChangeToDevice(DeviceChangeApplications apply, DeviceAndChangeApplicationRelations relation)
        {
            var newData = DeviceJsonHelper.DeserializeDeviceEditInput(apply.NewData);

            if (apply.ChangeType == "新增")
            {
                // 创建新设备
                var device = new Devices();
                UpdateDeviceEntity(device, newData);
                device.Creator = apply.SubmitterName;

                var deviceId = await _deviceRepository.InsertAndGetIdAsync(device);

                // 更新关系中的设备ID
                relation.DeviceId = deviceId;
                await _deviceAndChangeApplicationRelationsRepository.UpdateAsync(relation);

                // 保存关联关系
                await SaveDeviceRelations(deviceId, newData);

                // 将附件从申请单转移到设备
                await MoveAttachments(apply.Id, deviceId, "Device");
            }
            else if (apply.ChangeType == "编辑")
            {

                var device = await _deviceRepository.FirstOrDefaultAsync(relation.DeviceId);
                if (device == null)
                    throw new Exception("设备不存在");

                UpdateDeviceEntity(device, newData);
                await _deviceRepository.UpdateAsync(device);

                // 更新关联关系
                await SaveDeviceRelations(device.Id, newData);

                // 将附件从申请单转移到设备
                await MoveAttachments(apply.Id, device.Id, "Device");
            }
            else if (apply.ChangeType == "删除")
            {
                // 软删除设备
                await _deviceRepository.DeleteAsync(relation.DeviceId);
            }
        }

        /// <summary>
        /// 移动附件
        /// </summary>
        private async Task MoveAttachments(Guid fromBusinessId, Guid toBusinessId, string businessType)
        {
            // 获取申请单的附件
            var attachments = await _attachmentAppService.GetBusinessAttachmentsWithCategory(
                new GetBusinessAttachmentsInput
                {
                    BusinessId = fromBusinessId,
                    BusinessType = "DeviceChangeApplication"
                });

            if (attachments.IsSuccess && attachments.Data != null && attachments.Data.Any())
            {
                // 重新绑定到设备
                await _attachmentAppService.SetBusinessAttachments(new SetBusinessAttachmentInput
                {
                    BusinessId = toBusinessId,
                    BusinessType = businessType,
                    AttachmentWithCategories = attachments.Data.Select(x => new AttachmentWithCategory
                    {
                        AttachmentId = x.Id,
                        Category = x.Category
                    }).ToList()
                });

                // 解除申请单的绑定
                await _attachmentAppService.SetBusinessAttachments(new SetBusinessAttachmentInput
                {
                    BusinessId = fromBusinessId,
                    BusinessType = "DeviceChangeApplication",
                    AttachmentWithCategories = new List<AttachmentWithCategory>()
                });
            }
        }

        #endregion

        #region 辅助方法

        private class DeviceQueryResult
        {
            public Devices Device { get; set; }
            public Guid? TypeId { get; set; }
            public string TypeName { get; set; }
            public Guid? SupplierId { get; set; }
            public string SupplierName { get; set; }
        }

        private IQueryable<DeviceQueryResult> ApplyFilters(IQueryable<DeviceQueryResult> query, DevicePageInput input)
        {
            if (!string.IsNullOrWhiteSpace(input.SearchKey))
            {
                query = query.Where(x => x.Device.DeviceName.Contains(input.SearchKey)
                    || x.Device.DeviceCode.Contains(input.SearchKey)
                    || x.Device.Specification.Contains(input.SearchKey));
            }

            if (input.TypeId.HasValue)
                query = query.Where(x => x.TypeId == input.TypeId.Value);

            if (!string.IsNullOrWhiteSpace(input.DeviceStatus))
                query = query.Where(x => x.Device.DeviceStatus == input.DeviceStatus);

            if (!string.IsNullOrWhiteSpace(input.DeviceLevel))
                query = query.Where(x => x.Device.DeviceLevel == input.DeviceLevel);

            if (input.IsKeyDevice.HasValue)
                query = query.Where(x => x.Device.IsKeyDevice == input.IsKeyDevice.Value);

            if (input.CreationTimeBegin.HasValue)
                query = query.Where(x => x.Device.CreationTime >= input.CreationTimeBegin.Value);

            if (input.CreationTimeEnd.HasValue)
            {
                var endDate = input.CreationTimeEnd.Value.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.Device.CreationTime <= endDate);
            }

            return query;
        }

        private IQueryable<DeviceQueryResult> ApplySorting(IQueryable<DeviceQueryResult> query, DevicePageInput input)
        {
            if (!string.IsNullOrWhiteSpace(input.SortField))
            {
                if (input.SortField.Equals("DeviceCode", StringComparison.OrdinalIgnoreCase))
                {
                    query = input.SortOrder?.ToUpper() == "ASC"
                        ? query.OrderBy(x => x.Device.DeviceCode)
                        : query.OrderByDescending(x => x.Device.DeviceCode);
                }
                else if (input.SortField.Equals("DeviceName", StringComparison.OrdinalIgnoreCase))
                {
                    query = input.SortOrder?.ToUpper() == "ASC"
                        ? query.OrderBy(x => x.Device.DeviceName)
                        : query.OrderByDescending(x => x.Device.DeviceName);
                }
                else
                {
                    query = input.SortOrder?.ToUpper() == "ASC"
                        ? query.OrderBy(x => x.Device.CreationTime)
                        : query.OrderByDescending(x => x.Device.CreationTime);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.Device.CreationTime);
            }

            return query;
        }

        private async Task<DeviceDto> MapToDeviceDto(Devices device, Guid? typeId, string typeName, Guid? supplierId, string supplierName)
        {
            var dto = new DeviceDto
            {
                Id = device.Id,
                DeviceCode = device.DeviceCode,
                QrCode = device.QrCode,
                DeviceName = device.DeviceName,
                Specification = device.Specification,
                DeviceLevel = device.DeviceLevel,
                IsKeyDevice = device.IsKeyDevice,
                TypeId = typeId,
                TypeName = typeName,
                SupplierId = supplierId,
                SupplierName = supplierName,
                LogisticsNo = device.LogisticsNo,
                FactoryNo = device.FactoryNo,
                Manufacturer = device.Manufacturer,
                ManufactureDate = device.ManufactureDate,
                PurchaseNo = device.PurchaseNo,
                SourceType = device.SourceType,
                Location = device.Location,
                DeviceStatus = device.DeviceStatus,
                EnableDate = device.EnableDate,
                Creator = device.Creator,
                CreationTime = device.CreationTime
            };

            if (!string.IsNullOrEmpty(device.TechnicalParameters))
                dto.TechnicalParameters = JsonConvert.DeserializeObject<List<TechnicalParameterItem>>(device.TechnicalParameters);

            if (!string.IsNullOrEmpty(device.CustomerRequirements))
                dto.CustomerRequirements = JsonConvert.DeserializeObject<List<CustomerRequirementItem>>(device.CustomerRequirements);

            var factoryNode = await GetFactoryNodeAsync(device.Id);
            dto.FactoryNodeId = factoryNode?.NodeId;
            dto.FactoryNodeName = factoryNode?.NodeName;
            dto.FactoryNodeFullPath = factoryNode?.FullPath;

            dto.MaintainUsers = await GetUsersByTypeAsync(device.Id, "维修人员");
            dto.MaintenanceUsers = await GetUsersByTypeAsync(device.Id, "保养人员");

            // 获取附件
            var attachmentsResult = await _attachmentAppService.GetBusinessAttachmentsWithCategory(
                new GetBusinessAttachmentsInput
                {
                    BusinessId = device.Id,
                    BusinessType = "Device"
                });

            if (attachmentsResult.IsSuccess && attachmentsResult.Data != null)
            {
                dto.TechnicalAttachments = attachmentsResult.Data
                    .GroupBy(x => x.Category)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => new AttachmentInfo
                        {
                            Id = x.Id,
                            FileName = x.FileName,
                            FileSize = x.FileSize,
                            FileSizeFormat = x.FileSizeFormat,
                            FileUrl = x.FileUrl,
                            Category = x.Category,
                            CreationTime = x.CreationTime
                        }).ToList()
                    );
            }

            return dto;
        }

        private async Task<DeviceFactoryNodeInfo> GetFactoryNodeAsync(Guid deviceId)
        {
            var relation = await _factoryNodeRelationRepository.GetAll()
                .FirstOrDefaultAsync(x => x.DeviceId == deviceId);

            if (relation == null)
                return null;

            var node = await _factoryNodeRepository.FirstOrDefaultAsync(relation.FactoryNodeId);
            if (node == null)
                return null;

            var fullPath = await BuildFactoryNodeFullPath(node.Id);

            return new DeviceFactoryNodeInfo
            {
                NodeId = node.Id,
                NodeName = node.Name,
                NodeType = node.NodeType,
                FullPath = fullPath
            };
        }

        private async Task<string> BuildFactoryNodeFullPath(Guid nodeId)
        {
            var path = new List<string>();
            var currentNodeId = nodeId;

            while (currentNodeId != Guid.Empty)
            {
                var node = await _factoryNodeRepository.FirstOrDefaultAsync(currentNodeId);
                if (node == null) break;

                path.Insert(0, node.Name);
                currentNodeId = node.ParentId ?? Guid.Empty;
            }

            return string.Join(" / ", path);
        }

        private async Task<List<UserInfo>> GetUsersByTypeAsync(Guid deviceId, string userType)
        {
            var users = await (from r in _userRelationRepository.GetAll()
                               where r.DeviceId == deviceId && r.UserType == userType
                               select new UserInfo
                               {
                                   UserId = r.UserId
                               }).ToListAsync();

            foreach (var user in users)
            {
                user.UserName = await _userAppService.GetNameByUserId(user.UserId);
            }

            return users;
        }

        /// <summary>
        /// 获取设备变更历史
        /// </summary>
        private async Task<List<DeviceChangeHistoryDto>> GetDeviceChangeHistoryAsync(Guid deviceId)
        {
            var historyList = new List<DeviceChangeHistoryDto>();

            var relations = await _deviceAndChangeApplicationRelationsRepository.GetAll().AsNoTracking()
                .Where(x => x.DeviceId == deviceId)
                .OrderByDescending(x => x.SubmitTime)
                .ToListAsync();

            foreach (var relation in relations)
            {
                var apply = await _changeApplyRepository.FirstOrDefaultAsync(relation.DeviceChangeApplicationId);
                if (apply == null) continue;

                var historyItem = new DeviceChangeHistoryDto
                {
                    ChangeApplyId = relation.DeviceChangeApplicationId,
                    ChangeType = relation.ChangeType,
                    SubmitTime = relation.SubmitTime,
                    SubmitterName = relation.SubmitterName,
                    ApplyReason = relation.ApplyReason,
                    ApplicationStatus = apply.ApplicationStatus,
                    FlowInstanceId = relation.FlowInstanceId
                };

                if (!string.IsNullOrEmpty(apply.Snapshot))
                {
                    historyItem.Snapshot = DeviceJsonHelper.DeserializeDeviceEditInput(apply.Snapshot);
                }

                if (!string.IsNullOrEmpty(apply.NewData))
                {
                    historyItem.NewData = DeviceJsonHelper.DeserializeDeviceEditInput(apply.NewData);
                }

                // 生成字段级变更对比
                historyItem.ChangeDetails = GenerateChangeDetails(apply.Snapshot, apply.NewData);

                if (relation.FlowInstanceId.HasValue)
                    historyItem.FlowInfo = await GetFlowInfoAsync(relation.FlowInstanceId.Value);

                historyList.Add(historyItem);
            }

            return historyList;
        }

        /// <summary>
        /// 生成字段级变更对比
        /// </summary>
        private List<FieldChangeItem> GenerateChangeDetails(string snapshotJson, string newDataJson)
        {
            var changes = new List<FieldChangeItem>();
            var newData = new JObject();
            if (string.IsNullOrEmpty(snapshotJson))
            {
                // 新增操作，所有字段都是新增
                if (!string.IsNullOrEmpty(newDataJson))
                {
                    newData = JObject.Parse(newDataJson);
                    foreach (var prop in newData.Properties())
                    {
                        changes.Add(new FieldChangeItem
                        {
                            FieldName = prop.Name,
                            FieldLabel = GetFieldLabel(prop.Name),
                            OldValue = null,
                            NewValue = prop.Value,
                            ChangeType = "add"
                        });
                    }
                }
                return changes;
            }

            if (string.IsNullOrEmpty(newDataJson))
                return changes;

            var snapshot = JObject.Parse(snapshotJson);
            newData = JObject.Parse(newDataJson);

            // 对比每个字段
            var allFields = snapshot.Properties().Select(p => p.Name)
                .Union(newData.Properties().Select(p => p.Name))
                .Distinct();

            foreach (var field in allFields)
            {
                var oldValue = snapshot[field];
                var newValue = newData[field];

                // 跳过复杂类型（如 technicalParameters 会单独处理）
                if (field == "technicalParameters" || field == "customerRequirements")
                    continue;

                if (oldValue == null && newValue != null)
                {
                    changes.Add(new FieldChangeItem
                    {
                        FieldName = field,
                        FieldLabel = GetFieldLabel(field),
                        OldValue = null,
                        NewValue = newValue,
                        ChangeType = "add"
                    });
                }
                else if (oldValue != null && newValue == null)
                {
                    changes.Add(new FieldChangeItem
                    {
                        FieldName = field,
                        FieldLabel = GetFieldLabel(field),
                        OldValue = oldValue,
                        NewValue = null,
                        ChangeType = "delete"
                    });
                }
                else if (oldValue != null && newValue != null && !JToken.DeepEquals(oldValue, newValue))
                {
                    changes.Add(new FieldChangeItem
                    {
                        FieldName = field,
                        FieldLabel = GetFieldLabel(field),
                        OldValue = oldValue,
                        NewValue = newValue,
                        ChangeType = "edit"
                    });
                }
            }

            // 处理技术参数变更
            var oldParams = snapshot["technicalParameters"]?.ToObject<List<TechnicalParameterItem>>() ?? new List<TechnicalParameterItem>();
            var newParams = newData["technicalParameters"]?.ToObject<List<TechnicalParameterItem>>() ?? new List<TechnicalParameterItem>();

            var paramChanges = CompareTechnicalParameters(oldParams, newParams);
            changes.AddRange(paramChanges);

            // 处理客户要求变更
            var oldReqs = snapshot["customerRequirements"]?.ToObject<List<CustomerRequirementItem>>() ?? new List<CustomerRequirementItem>();
            var newReqs = newData["customerRequirements"]?.ToObject<List<CustomerRequirementItem>>() ?? new List<CustomerRequirementItem>();

            var reqChanges = CompareCustomerRequirements(oldReqs, newReqs);
            changes.AddRange(reqChanges);

            return changes;
        }

        private string GetFieldLabel(string fieldName)
        {
            return _fieldLabels.ContainsKey(fieldName) ? _fieldLabels[fieldName] : fieldName;
        }

        private List<FieldChangeItem> CompareTechnicalParameters(List<TechnicalParameterItem> oldList, List<TechnicalParameterItem> newList)
        {
            var changes = new List<FieldChangeItem>();

            // 按参数名建立字典
            var oldDict = oldList.ToDictionary(x => x.ParameterName, x => x);
            var newDict = newList.ToDictionary(x => x.ParameterName, x => x);

            var allNames = oldDict.Keys.Union(newDict.Keys).Distinct();

            foreach (var name in allNames)
            {
                oldDict.TryGetValue(name, out var oldItem);
                newDict.TryGetValue(name, out var newItem);

                if (oldItem == null && newItem != null)
                {
                    changes.Add(new FieldChangeItem
                    {
                        FieldName = $"technicalParameters.{name}",
                        FieldLabel = $"技术参数-{name}",
                        OldValue = null,
                        NewValue = newItem.ParameterValue,
                        ChangeType = "add"
                    });
                }
                else if (oldItem != null && newItem == null)
                {
                    changes.Add(new FieldChangeItem
                    {
                        FieldName = $"technicalParameters.{name}",
                        FieldLabel = $"技术参数-{name}",
                        OldValue = oldItem.ParameterValue,
                        NewValue = null,
                        ChangeType = "delete"
                    });
                }
                else if (oldItem != null && newItem != null && oldItem.ParameterValue != newItem.ParameterValue)
                {
                    changes.Add(new FieldChangeItem
                    {
                        FieldName = $"technicalParameters.{name}",
                        FieldLabel = $"技术参数-{name}",
                        OldValue = oldItem.ParameterValue,
                        NewValue = newItem.ParameterValue,
                        ChangeType = "edit"
                    });
                }
            }

            return changes;
        }

        private List<FieldChangeItem> CompareCustomerRequirements(List<CustomerRequirementItem> oldList, List<CustomerRequirementItem> newList)
        {
            var changes = new List<FieldChangeItem>();

            for (int i = 0; i < Math.Max(oldList.Count, newList.Count); i++)
            {
                if (i >= oldList.Count && i < newList.Count)
                {
                    // 新增
                    changes.Add(new FieldChangeItem
                    {
                        FieldName = $"customerRequirements[{i}]",
                        FieldLabel = $"客户要求-{newList[i].RequirementName}",
                        OldValue = null,
                        NewValue = newList[i],
                        ChangeType = "add"
                    });
                }
                else if (i < oldList.Count && i >= newList.Count)
                {
                    // 删除
                    changes.Add(new FieldChangeItem
                    {
                        FieldName = $"customerRequirements[{i}]",
                        FieldLabel = $"客户要求-{oldList[i].RequirementName}",
                        OldValue = oldList[i],
                        NewValue = null,
                        ChangeType = "delete"
                    });
                }
                else
                {
                    // 对比
                    var oldItem = oldList[i];
                    var newItem = newList[i];

                    if (oldItem.CustomerName != newItem.CustomerName ||
                        oldItem.RequirementName != newItem.RequirementName ||
                        oldItem.RequirementValue != newItem.RequirementValue ||
                        oldItem.ActualValue != newItem.ActualValue ||
                        oldItem.IsQualified != newItem.IsQualified)
                    {
                        changes.Add(new FieldChangeItem
                        {
                            FieldName = $"customerRequirements[{i}]",
                            FieldLabel = $"客户要求-{newItem.RequirementName}",
                            OldValue = oldItem,
                            NewValue = newItem,
                            ChangeType = "edit"
                        });
                    }
                }
            }

            return changes;
        }

        private async Task<FlowInfo> GetFlowInfoAsync(Guid flowInstanceId)
        {
            var flowInfo = new FlowInfo
            {
                NodeRecords = new List<FlowNodeRecordDto>()
            };

            var flowInstance = await _flowInstanceAppService.GetFlowInstanceAsync(flowInstanceId);
            if (flowInstance == null)
                return flowInfo;

            flowInfo.FlowDefId = flowInstance.FlowDefinitionId;
            flowInfo.FlowName = flowInstance.FlowName;
            flowInfo.Status = flowInstance.Status;
            flowInfo.StatusName = GetFlowStatusName(flowInstance.Status);
            flowInfo.BeginTime = flowInstance.BeginTime;
            flowInfo.EndTime = flowInstance.EndTime;
            flowInfo.CurrentNodeName = flowInstance.CurrentNodeName;

            var histories = await _historyRepository.GetAll()
                .Where(x => x.FlowInstanceId == flowInstanceId)
                .OrderBy(x => x.OperateTime)
                .ToListAsync();

            foreach (var history in histories)
            {
                flowInfo.NodeRecords.Add(new FlowNodeRecordDto
                {
                    NodeName = history.NodeName,
                    NodeType = history.NodeType,
                    NodeTypeName = GetNodeTypeName(history.NodeType),
                    FlowCmd = history.FlowCmd,
                    FlowCmdName = GetCmdName(history.FlowCmd),
                    OperatorName = history.OperatorName,
                    OperateTime = history.OperateTime,
                    Comment = history.Comment,
                    Underway = false
                });
            }

            var pendingTasks = await _taskRepository.GetAll()
                .Where(x => x.FlowInstanceId == flowInstanceId && x.Status == 0)
                .ToListAsync();

            foreach (var task in pendingTasks)
            {
                flowInfo.NodeRecords.Add(new FlowNodeRecordDto
                {
                    NodeName = task.NodeName,
                    NodeType = task.NodeType,
                    NodeTypeName = GetNodeTypeName(task.NodeType),
                    Underway = true
                });
            }

            return flowInfo;
        }

        private string GetCmdName(int cmd)
        {
            return cmd switch
            {
                0 => "发起",
                1 => "自动拒绝",
                2 => "自动通过",
                3 => "拒绝",
                4 => "通过",
                5 => "撤销",
                7 => "回退",
                12 => "抄送",
                _ => "操作"
            };
        }

        private string GetFlowStatusName(int status)
        {
            return status switch
            {
                0 => "审批中",
                1 => "已通过",
                2 => "不通过",
                3 => "已撤销",
                _ => "未知"
            };
        }

        private string GetNodeTypeName(int nodeType)
        {
            return nodeType switch
            {
                1 => "审批",
                2 => "抄送",
                5 => "办理",
                _ => "未知"
            };
        }

        private void UpdateDeviceEntity(Devices device, DeviceEditInput input)
        {
            device.DeviceCode = input.DeviceCode;
            device.DeviceName = input.DeviceName;
            device.Specification = input.Specification;
            device.DeviceLevel = input.DeviceLevel;
            device.IsKeyDevice = input.IsKeyDevice;
            device.TechnicalParameters = JsonConvert.SerializeObject(input.TechnicalParameters ?? new List<TechnicalParameterItem>());
            device.CustomerRequirements = JsonConvert.SerializeObject(input.CustomerRequirements ?? new List<CustomerRequirementItem>());
            device.LogisticsNo = input.LogisticsNo;
            device.FactoryNo = input.FactoryNo;
            device.Manufacturer = input.Manufacturer;
            device.ManufactureDate = input.ManufactureDate;
            device.PurchaseNo = input.PurchaseNo;
            device.SourceType = input.SourceType;
            device.Location = input.Location;
            device.DeviceStatus = input.DeviceStatus;
            device.EnableDate = input.EnableDate;
        }

        private async Task SaveDeviceRelations(Guid deviceId, DeviceEditInput input)
        {
            // 先删除原有关系
            await _deviceTypeRepository.DeleteAsync(x => x.DeviceId == deviceId);
            await _supplierRelationRepository.DeleteAsync(x => x.DeviceId == deviceId);
            await _factoryNodeRelationRepository.DeleteAsync(x => x.DeviceId == deviceId);
            await _userRelationRepository.DeleteAsync(x => x.DeviceId == deviceId);

            // 保存新关系
            if (input.TypeId.HasValue)
            {
                await _deviceTypeRepository.InsertAsync(new DeviceTypeRelations
                {
                    DeviceId = deviceId,
                    TypeId = input.TypeId.Value
                });
            }

            if (input.SupplierId.HasValue)
            {
                await _supplierRelationRepository.InsertAsync(new DeviceSupplierRelations
                {
                    DeviceId = deviceId,
                    SupplierId = input.SupplierId.Value
                });
            }

            if (input.FactoryNodeId.HasValue)
            {
                var node = await _factoryNodeRepository.FirstOrDefaultAsync(input.FactoryNodeId.Value);
                await _factoryNodeRelationRepository.InsertAsync(new DeviceFactoryNodeRelations
                {
                    DeviceId = deviceId,
                    FactoryNodeId = input.FactoryNodeId.Value,
                    NodeType = node?.NodeType
                });
            }

            if (input.MaintainUserIds != null)
            {
                foreach (var uid in input.MaintainUserIds)
                {
                    await _userRelationRepository.InsertAsync(new DeviceUserRelations
                    {
                        DeviceId = deviceId,
                        UserId = uid,
                        UserType = "维修人员"
                    });
                }
            }

            if (input.MaintenanceUserIds != null)
            {
                foreach (var uid in input.MaintenanceUserIds)
                {
                    await _userRelationRepository.InsertAsync(new DeviceUserRelations
                    {
                        DeviceId = deviceId,
                        UserId = uid,
                        UserType = "保养人员"
                    });
                }
            }
        }

        private async Task<Guid?> GetFlowDefinitionByBusinessForm(string changeType)
        {
            try
            {
                string formCode = changeType == "新增" ? "device-add" : "device-edit";

                var businessForm = await _formRepository.GetAll()
                    .Where(f => f.FormCode == formCode && f.Status == true)
                    .Select(f => new { f.FlowDefId, f.FlowName })
                    .FirstOrDefaultAsync();

                if (businessForm == null || !businessForm.FlowDefId.HasValue)
                {
                    // 如果没有找到对应的表单，尝试使用通用表单
                    businessForm = await _formRepository.GetAll()
                        .Where(f => f.FormCode == "device" && f.Status == true)
                        .Select(f => new { f.FlowDefId, f.FlowName })
                        .FirstOrDefaultAsync();
                }

                return businessForm?.FlowDefId;
            }
            catch (Exception ex)
            {
                Logger.Error($"获取{changeType}设备流程定义失败", ex);
                return null;
            }
        }


        /// <summary>
        /// 根据用户ID列表获取用户信息
        /// </summary>
        private async Task<List<UserInfo>> GetUsersFromIds(List<long> userIds)
        {
            if (userIds == null || !userIds.Any())
                return new List<UserInfo>();

            var result = new List<UserInfo>();
            foreach (var userId in userIds)
            {
                var userName = await _userAppService.GetNameByUserId(userId);
                result.Add(new UserInfo
                {
                    UserId = userId,
                    UserName = userName
                });
            }
            return result;
        }

        /// <summary>
        /// 获取工厂节点信息
        /// </summary>
        private async Task<DeviceFactoryNodeInfo> GetFactoryNodeInfo(Guid nodeId)
        {
            var node = await _factoryNodeRepository.FirstOrDefaultAsync(nodeId);
            if (node == null)
                return null;

            var fullPath = await BuildFactoryNodeFullPath(node.Id);

            return new DeviceFactoryNodeInfo
            {
                NodeId = node.Id,
                NodeName = node.Name,
                NodeType = node.NodeType,
                FullPath = fullPath
            };
        }




        /// <summary>
        /// 内部设备编码唯一性校验
        /// </summary>
        private async Task<bool> CheckDeviceCodeUniqueInternal(string deviceCode, Guid? excludeDeviceId, Guid? excludeApplyId)
        {
            if (string.IsNullOrWhiteSpace(deviceCode))
                return true;

            // 1. 检查设备主表
            var deviceQuery = _deviceRepository.GetAll()
                .Where(d => d.DeviceCode == deviceCode);

            if (excludeDeviceId.HasValue && excludeDeviceId.Value != Guid.Empty)
            {
                deviceQuery = deviceQuery.Where(d => d.Id != excludeDeviceId.Value);
            }

            var existsInDevice = await deviceQuery.AnyAsync();
            if (existsInDevice)
                return false;

            // 2. 检查正在审核的申请（状态为：待审核、审核中）
            // 获取所有非草稿状态的申请
            var applyingApplies = await _changeApplyRepository.GetAll()
                .Where(a => a.ApplicationStatus == "审核中" || a.ApplicationStatus == "待审核")
                .ToListAsync();

            foreach (var apply in applyingApplies)
            {
                try
                {
                    // 解析 NewData 中的设备编码
                    var newData = DeviceJsonHelper.DeserializeDeviceEditInput(apply.NewData);
                    if (newData != null && newData.DeviceCode == deviceCode)
                    {
                        // 如果是编辑自身，且是同一个申请ID，则忽略
                        if (excludeApplyId.HasValue && apply.Id == excludeApplyId.Value)
                        {
                            continue;
                        }

                        return false;
                    }
                }
                catch
                {
                    // 解析失败忽略
                    continue;
                }
            }

            return true;
        }



        /// <summary>
        /// 判断申请是否可编辑
        /// </summary>
        private static bool CanEditApply(DeviceChangeApplications apply)
        {
            if (apply == null) return false;

            // 草稿、已撤销、已拒绝都可以编辑
            var editableStatus = new[] { "草稿", "已撤销", "已拒绝" };

            // 删除类型的申请不能编辑(只能重新发起删除)
            if (apply.ChangeType == "删除")
                return false;

            return editableStatus.Contains(apply.ApplicationStatus);
        }

        /// <summary>
        /// 判断申请是否可重新发起(重新申请)
        /// </summary>
        private static bool CanReapply(DeviceChangeApplications apply)
        {
            if (apply == null) return false;

            // 已撤销、已拒绝状态可以重新发起
            var reapplyStatus = new[] { "已撤销", "已拒绝" };

            // 删除类型即使在这些状态也只能重新发起删除，不能编辑
            return reapplyStatus.Contains(apply.ApplicationStatus);
        }

        /// <summary>
        /// 判断申请是否可删除
        /// </summary>
        private static bool CanDeleteApply(DeviceChangeApplications apply)
        {
            if (apply == null) return false;

            // 草稿、已撤销、已拒绝可以删除
            var deletableStatus = new[] { "草稿", "已撤销", "已拒绝" };
            return deletableStatus.Contains(apply.ApplicationStatus);
        }

        /// <summary>
        /// 获取申请可执行的操作列表
        /// </summary>
        private static List<string> GetAvailableActions(DeviceChangeApplications apply)
        {
            var actions = new List<string>();
            if (apply == null) return actions;

            // 详情 - 总是可用
            actions.Add("view");

            // 流程 - 非草稿状态可用
            if (apply.ApplicationStatus != "草稿")
                actions.Add("viewFlow");

            // 编辑 - 非删除类型且状态为草稿/已撤销/已拒绝
            if (apply.ChangeType != "删除" &&
                new[] { "草稿", "已撤销", "已拒绝" }.Contains(apply.ApplicationStatus))
            {
                actions.Add("edit");
            }

            // 重新申请 - 已撤销/已拒绝状态
            if (new[] { "已撤销", "已拒绝" }.Contains(apply.ApplicationStatus))
            {
                actions.Add("reapply");
            }

            // 撤销 - 审核中状态
            if (apply.ApplicationStatus == "审核中")
            {
                actions.Add("cancel");
            }

            // 删除 - 草稿/已撤销/已拒绝状态
            if (new[] { "草稿", "已撤销", "已拒绝" }.Contains(apply.ApplicationStatus))
            {
                actions.Add("delete");
            }

            return actions;
        }

        #endregion
    }

    /// <summary>
    /// 设备工厂节点信息扩展
    /// </summary>
    public class DeviceFactoryNodeInfo
    {
        /// <summary>
        /// 节点Id
        /// </summary>
        public Guid NodeId { get; set; }
        
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 节点类型（工厂、车间、产线、工位）
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// 完整路径（格式：工厂 / 车间 / 产线 / 工位）
        /// </summary>
        public string FullPath { get; set; }
    }
}