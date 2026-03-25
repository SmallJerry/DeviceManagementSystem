using Abp.Auditing;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using DeviceManagementSystem.Attachment;
using DeviceManagementSystem.Attachment.Dto;
using DeviceManagementSystem.BasicDataManagement;
using DeviceManagementSystem.DeviceInfos;
using DeviceManagementSystem.Dicts;
using DeviceManagementSystem.Repairs.Dto;
using DeviceManagementSystem.Repairs.Interface;
using DeviceManagementSystem.Users;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Repairs
{
    /// <summary>
    /// 维修申报服务实现
    /// </summary>
    [AbpAuthorize]
    [Audited]
    public class RepairRequestAppService : DeviceManagementSystemAppServiceBase, IRepairRequestAppService
    {
        private readonly IRepository<RepairRequests, Guid> _repairRequestRepository;
        private readonly IRepository<RepairRequestDeviceTypeRelation, Guid> _requestDeviceTypeRelationRepository;
        private readonly IRepository<Devices, Guid> _deviceRepository;
        private readonly IRepository<DeviceTypeRelations, Guid> _deviceTypeRelationRepository;
        private readonly IRepository<Types, Guid> _typeRepository;
        private readonly IUserAppService _userAppService;
        private readonly IAttachmentAppService _attachmentAppService;
        private readonly IRepairTaskAppService _repairTaskAppService;

        // 维修类型常量
        private const int REPAIR_TYPE_REPAIR = 0;   // 维修
        private const int REPAIR_TYPE_UPGRADE = 1;  // 升级

        // 申报状态常量
        private const int REQUEST_STATUS_PENDING_DISPATCH = 0;   // 待派单
        private const int REQUEST_STATUS_DISPATCHED = 1;         // 已派单
        private const int REQUEST_STATUS_REPAIRING = 2;          // 维修中
        private const int REQUEST_STATUS_COMPLETED = 3;          // 已完成
        private const int REQUEST_STATUS_CANCELLED = 4;          // 已取消

        public RepairRequestAppService(
            IRepository<RepairRequests, Guid> repairRequestRepository,
            IRepository<RepairRequestDeviceTypeRelation, Guid> requestDeviceTypeRelationRepository,
            IRepository<Devices, Guid> deviceRepository,
            IRepository<DeviceTypeRelations, Guid> deviceTypeRelationRepository,
            IRepository<Types, Guid> typeRepository,
            IUserAppService userAppService,
            IAttachmentAppService attachmentAppService,
            IRepairTaskAppService repairTaskAppService)
        {
            _repairRequestRepository = repairRequestRepository;
            _requestDeviceTypeRelationRepository = requestDeviceTypeRelationRepository;
            _deviceRepository = deviceRepository;
            _deviceTypeRelationRepository = deviceTypeRelationRepository;
            _typeRepository = typeRepository;
            _userAppService = userAppService;
            _attachmentAppService = attachmentAppService;
            _repairTaskAppService = repairTaskAppService;
        }

        /// <summary>
        /// 获取维修申报分页列表
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<Page<RepairRequestDto>>> GetPageList([FromQuery] RepairRequestPageInput input)
        {
            try
            {
                if (input.Size > 100) input.Size = 100;

                var query = from r in _repairRequestRepository.GetAll().AsNoTracking()
                            join d in _deviceRepository.GetAll().AsNoTracking() on r.DeviceId equals d.Id into deviceJoin
                            from d in deviceJoin.DefaultIfEmpty()
                            join dtr in _deviceTypeRelationRepository.GetAll().AsNoTracking() on d.Id equals dtr.DeviceId into deviceTypeJoin
                            from dtr in deviceTypeJoin.DefaultIfEmpty()
                            join t in _typeRepository.GetAll().AsNoTracking() on dtr.TypeId equals t.Id into typeJoin
                            from t in typeJoin.DefaultIfEmpty()
                            select new { Request = r, Device = d, DeviceType = t };

                // 应用过滤条件
                if (!string.IsNullOrWhiteSpace(input.SearchKey))
                {
                    query = query.Where(x =>
                        x.Request.RequestNo.Contains(input.SearchKey) ||
                        (x.Device != null && x.Device.DeviceName.Contains(input.SearchKey)) ||
                        (x.Device != null && x.Device.DeviceCode.Contains(input.SearchKey)));
                }

                if (input.RepairType.HasValue)
                {
                    query = query.Where(x => x.Request.RepairType == input.RepairType.Value);
                }

                if (input.RequestStatus.HasValue)
                {
                    query = query.Where(x => x.Request.RequestStatus == input.RequestStatus.Value);
                }

                if (input.DeviceId.HasValue)
                {
                    query = query.Where(x => x.Request.DeviceId == input.DeviceId.Value);
                }

                if (input.FaultFoundTimeBegin.HasValue)
                {
                    query = query.Where(x => x.Request.FaultFoundTime >= input.FaultFoundTimeBegin.Value);
                }

                if (input.FaultFoundTimeEnd.HasValue)
                {
                    var endDate = input.FaultFoundTimeEnd.Value.AddDays(1).AddSeconds(-1);
                    query = query.Where(x => x.Request.FaultFoundTime <= endDate);
                }

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Request.CreationTime)
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var result = new List<RepairRequestDto>();

                foreach (var item in items)
                {
                    var dto = await MapToDto(item.Request, item.Device, item.DeviceType);
                    result.Add(dto);
                }

                var page = new Page<RepairRequestDto>(input.Current, input.Size, total)
                {
                    Records = result
                };

                return CommonResult<Page<RepairRequestDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取维修申报分页列表失败", ex);
                return CommonResult<Page<RepairRequestDto>>.Error($"获取维修申报分页列表失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取维修申报详情
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<RepairRequestDto>> GetById(Guid id)
        {
            try
            {
                var request = await _repairRequestRepository.FirstOrDefaultAsync(id);
                if (request == null)
                {
                    return CommonResult<RepairRequestDto>.Error("维修申报不存在");
                }

                var device = await _deviceRepository.FirstOrDefaultAsync(request.DeviceId);
                Guid? deviceTypeId = null;
                string deviceTypeName = null;

                if (device != null)
                {
                    var deviceTypeRelation = await _deviceTypeRelationRepository
                        .FirstOrDefaultAsync(x => x.DeviceId == device.Id);
                    if (deviceTypeRelation != null)
                    {
                        deviceTypeId = deviceTypeRelation.TypeId;
                        var type = await _typeRepository.FirstOrDefaultAsync(deviceTypeId.Value);
                        deviceTypeName = type?.TypeName;
                    }
                }

                var dto = await MapToDto(request, device, null);
                dto.DeviceTypeId = deviceTypeId;
                dto.DeviceTypeName = deviceTypeName;

                // 获取故障图片附件
                var attachmentsResult = await _attachmentAppService.GetBusinessAttachments(
                    new GetBusinessAttachmentsInput
                    {
                        BusinessId = id,
                        BusinessType = "RepairRequests"
                    });

                if (attachmentsResult.IsSuccess && attachmentsResult.Data != null)
                {
                    dto.FaultImages = attachmentsResult.Data.Select(a => new AttachmentInfoDto
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        FileSize = a.FileSize,
                        FileSizeFormat = a.FileSizeFormat,
                        FileUrl = a.FileUrl,
                        CreationTime = a.CreationTime
                    }).ToList();
                }

                return CommonResult<RepairRequestDto>.Success(dto);
            }
            catch (Exception ex)
            {
                Logger.Error("获取维修申报详情失败", ex);
                return CommonResult<RepairRequestDto>.Error($"获取维修申报详情失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建维修申报
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult<Guid>> Create(RepairRequestInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;
                var userName = await _userAppService.GetNameByUserId(userId);

                var device = await _deviceRepository.FirstOrDefaultAsync(input.DeviceId);
                if (device == null)
                {
                    return CommonResult<Guid>.Error("设备不存在");
                }

                // 生成申报编号
                var requestNo = await GenerateRequestNo();

                var request = new RepairRequests
                {
                    RequestNo = requestNo,
                    RepairType = input.RepairType,
                    DeviceId = input.DeviceId,
                    RequesterId = userId,
                    RequesterName = userName,
                    FaultFoundTime = input.FaultFoundTime,
                    FaultLevel = input.FaultLevel,
                    ExpectedCompleteTime = input.ExpectedCompleteTime,
                    FaultDescription = input.FaultDescription,
                    RequestStatus = REQUEST_STATUS_PENDING_DISPATCH,
                    Remark = input.Remark
                };

                var requestId = await _repairRequestRepository.InsertAndGetIdAsync(request);
                await CurrentUnitOfWork.SaveChangesAsync();

                // 保存设备类型关联
                if (device != null)
                {
                    var deviceTypeRelation = await _deviceTypeRelationRepository
                        .FirstOrDefaultAsync(x => x.DeviceId == device.Id);
                    if (deviceTypeRelation != null)
                    {
                        var relation = new RepairRequestDeviceTypeRelation
                        {
                            RepairRequestId = requestId,
                            DeviceTypeId = deviceTypeRelation.TypeId
                        };
                        await _requestDeviceTypeRelationRepository.InsertAsync(relation);
                    }
                }

                // 保存故障图片附件
                if (input.FaultImageIds != null && input.FaultImageIds.Any())
                {
                    await _attachmentAppService.SetBusinessAttachments(new SetBusinessAttachmentInput
                    {
                        BusinessId = requestId,
                        BusinessType = "RepairRequests",
                        AttachmentIds = input.FaultImageIds
                    });
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult<Guid>.Success(requestId);
            }
            catch (Exception ex)
            {
                Logger.Error("创建维修申报失败", ex);
                return CommonResult<Guid>.Error($"创建维修申报失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新维修申报
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Update(RepairRequestInput input)
        {
            try
            {
                if (!input.Id.HasValue)
                {
                    return CommonResult.Error("申报ID不能为空");
                }

                var request = await _repairRequestRepository.FirstOrDefaultAsync(input.Id.Value);
                if (request == null)
                {
                    return CommonResult.Error("维修申报不存在");
                }

                // 只有待派单状态可以修改
                if (request.RequestStatus != REQUEST_STATUS_PENDING_DISPATCH)
                {
                    return CommonResult.Error($"当前状态({GetRequestStatusText((int)request.RequestStatus)})不可修改");
                }

                request.RepairType = input.RepairType;
                request.DeviceId = input.DeviceId;
                request.FaultFoundTime = input.FaultFoundTime;
                request.FaultLevel = input.FaultLevel;
                request.ExpectedCompleteTime = input.ExpectedCompleteTime;
                request.FaultDescription = input.FaultDescription;
                request.Remark = input.Remark;

                await _repairRequestRepository.UpdateAsync(request);
                await CurrentUnitOfWork.SaveChangesAsync();

                // 更新设备类型关联
                await _requestDeviceTypeRelationRepository.DeleteAsync(x => x.RepairRequestId == request.Id);
                var device = await _deviceRepository.FirstOrDefaultAsync(request.DeviceId);
                if (device != null)
                {
                    var deviceTypeRelation = await _deviceTypeRelationRepository
                        .FirstOrDefaultAsync(x => x.DeviceId == device.Id);
                    if (deviceTypeRelation != null)
                    {
                        var relation = new RepairRequestDeviceTypeRelation
                        {
                            RepairRequestId = request.Id,
                            DeviceTypeId = deviceTypeRelation.TypeId
                        };
                        await _requestDeviceTypeRelationRepository.InsertAsync(relation);
                    }
                }

                // 更新故障图片附件
                if (input.FaultImageIds != null)
                {
                    await _attachmentAppService.SetBusinessAttachments(new SetBusinessAttachmentInput
                    {
                        BusinessId = request.Id,
                        BusinessType = "RepairRequests",
                        AttachmentIds = input.FaultImageIds
                    });
                }

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("更新成功");
            }
            catch (Exception ex)
            {
                Logger.Error("更新维修申报失败", ex);
                return CommonResult.Error($"更新维修申报失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除维修申报
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Delete(Guid id)
        {
            try
            {
                var request = await _repairRequestRepository.FirstOrDefaultAsync(id);
                if (request == null)
                {
                    return CommonResult.Error("维修申报不存在");
                }

                // 只有待派单状态可以删除
                if (request.RequestStatus != REQUEST_STATUS_PENDING_DISPATCH)
                {
                    return CommonResult.Error($"当前状态({GetRequestStatusText((int)request.RequestStatus)})不可删除");
                }

                // 删除附件关系
                await _attachmentAppService.SetBusinessAttachments(new SetBusinessAttachmentInput
                {
                    BusinessId = id,
                    BusinessType = "RepairRequests",
                    AttachmentIds = new List<Guid>()
                });

                // 删除设备类型关联
                await _requestDeviceTypeRelationRepository.DeleteAsync(x => x.RepairRequestId == id);

                // 删除申报
                await _repairRequestRepository.DeleteAsync(id);

                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("删除成功");
            }
            catch (Exception ex)
            {
                Logger.Error("删除维修申报失败", ex);
                return CommonResult.Error($"删除维修申报失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 取消维修申报
        /// </summary>
        [UnitOfWork]
        public async Task<CommonResult> Cancel(Guid id, string reason)
        {
            try
            {
                var request = await _repairRequestRepository.FirstOrDefaultAsync(id);
                if (request == null)
                {
                    return CommonResult.Error("维修申报不存在");
                }

                // 只有待派单和已派单状态可以取消
                if (request.RequestStatus != REQUEST_STATUS_PENDING_DISPATCH &&
                    request.RequestStatus != REQUEST_STATUS_DISPATCHED)
                {
                    return CommonResult.Error($"当前状态({GetRequestStatusText((int)request.RequestStatus)})不可取消");
                }

                request.RequestStatus = REQUEST_STATUS_CANCELLED;
                request.Remark = reason ?? "用户取消";

                await _repairRequestRepository.UpdateAsync(request);
                await CurrentUnitOfWork.SaveChangesAsync();

                return CommonResult.Ok("取消成功");
            }
            catch (Exception ex)
            {
                Logger.Error("取消维修申报失败", ex);
                return CommonResult.Error($"取消维修申报失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取设备故障现象历史选项
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<List<FaultDescriptionOptionDto>>> GetFaultDescriptionOptions(Guid deviceId)
        {
            try
            {
                var requests = await _repairRequestRepository.GetAll()
                    .Where(x => x.DeviceId == deviceId && !string.IsNullOrEmpty(x.FaultDescription))
                    .GroupBy(x => x.FaultDescription)
                    .Select(g => new FaultDescriptionOptionDto
                    {
                        FaultDescription = g.Key,
                        OccurrenceCount = g.Count(),
                        LastOccurrenceTime = g.Max(x => x.FaultFoundTime)
                    })
                    .OrderByDescending(x => x.OccurrenceCount)
                    .Take(10)
                    .ToListAsync();

                return CommonResult<List<FaultDescriptionOptionDto>>.Success(requests);
            }
            catch (Exception ex)
            {
                Logger.Error("获取故障现象历史选项失败", ex);
                return CommonResult<List<FaultDescriptionOptionDto>>.Error($"获取故障现象历史选项失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取我的维修申报列表
        /// </summary>
        [DisableAuditing]
        public async Task<CommonResult<Page<RepairRequestDto>>> GetMyRequests([FromQuery] RepairRequestPageInput input)
        {
            try
            {
                var userId = AbpSession.UserId.Value;

                if (input.Size > 100) input.Size = 100;

                var query = from r in _repairRequestRepository.GetAll().AsNoTracking()
                            where r.RequesterId == userId
                            join d in _deviceRepository.GetAll().AsNoTracking() on r.DeviceId equals d.Id into deviceJoin
                            from d in deviceJoin.DefaultIfEmpty()
                            select new { Request = r, Device = d };

                // 应用过滤条件
                if (!string.IsNullOrWhiteSpace(input.SearchKey))
                {
                    query = query.Where(x =>
                        x.Request.RequestNo.Contains(input.SearchKey) ||
                        (x.Device != null && x.Device.DeviceName.Contains(input.SearchKey)));
                }

                if (input.RepairType.HasValue)
                {
                    query = query.Where(x => x.Request.RepairType == input.RepairType.Value);
                }

                if (input.RequestStatus.HasValue)
                {
                    query = query.Where(x => x.Request.RequestStatus == input.RequestStatus.Value);
                }

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Request.CreationTime)
                    .Skip((input.Current - 1) * input.Size)
                    .Take(input.Size)
                    .ToListAsync();

                var result = new List<RepairRequestDto>();

                foreach (var item in items)
                {
                    var dto = await MapToDto(item.Request, item.Device, null);
                    result.Add(dto);
                }

                var page = new Page<RepairRequestDto>(input.Current, input.Size, total)
                {
                    Records = result
                };

                return CommonResult<Page<RepairRequestDto>>.Success(page);
            }
            catch (Exception ex)
            {
                Logger.Error("获取我的维修申报列表失败", ex);
                return CommonResult<Page<RepairRequestDto>>.Error($"获取我的维修申报列表失败: {ex.Message}");
            }
        }

        #region 辅助方法

        /// <summary>
        /// 生成申报编号
        /// </summary>
        private async Task<string> GenerateRequestNo()
        {
            var today = DateTime.Now.ToString("yyyyMMdd");
            var lastRequest = await _repairRequestRepository.GetAll()
                .Where(x => x.RequestNo.StartsWith($"RR{today}"))
                .OrderByDescending(x => x.RequestNo)
                .FirstOrDefaultAsync();

            if (lastRequest != null && lastRequest.RequestNo.Length >= 14)
            {
                var lastSeq = int.Parse(lastRequest.RequestNo.Substring(11, 3));
                var newSeq = (lastSeq + 1).ToString("D3");
                return $"RR{today}{newSeq}";
            }

            return $"RR{today}001";
        }

        /// <summary>
        /// 映射为DTO
        /// </summary>
        private async Task<RepairRequestDto> MapToDto(RepairRequests request, Devices device, Types deviceType)
        {
            var dto = new RepairRequestDto
            {
                Id = request.Id,
                RequestNo = request.RequestNo,
                RepairType = (int)request.RepairType,
                RepairTypeText = GetRepairTypeText((int)request.RepairType),
                DeviceId = request.DeviceId,
                DeviceCode = device?.DeviceCode,
                DeviceName = device?.DeviceName,
                DeviceTypeName = deviceType?.TypeName,
                RequesterId = request.RequesterId,
                RequesterName = request.RequesterName,
                FaultFoundTime = request.FaultFoundTime,
                FaultLevel = request.FaultLevel,
                FaultLevelText = await GetDictLabelAsync("FAULT_LEVEL", request.FaultLevel.ToString()),
                ExpectedCompleteTime = request.ExpectedCompleteTime,
                FaultDescription = request.FaultDescription,
                RequestStatus = (int)request.RequestStatus,
                RequestStatusText = GetRequestStatusText((int)request.RequestStatus),
                CreationTime = request.CreationTime,
                Remark = request.Remark
            };

            return dto;
        }

        /// <summary>
        /// 获取维修类型文本
        /// </summary>
        private string GetRepairTypeText(int repairType)
        {
            return repairType switch
            {
                REPAIR_TYPE_REPAIR => "维修",
                REPAIR_TYPE_UPGRADE => "升级",
                _ => "维修"
            };
        }

        /// <summary>
        /// 获取申报状态文本
        /// </summary>
        private string GetRequestStatusText(int status)
        {
            return status switch
            {
                REQUEST_STATUS_PENDING_DISPATCH => "待派单",
                REQUEST_STATUS_DISPATCHED => "已派单",
                REQUEST_STATUS_REPAIRING => "维修中",
                REQUEST_STATUS_COMPLETED => "已完成",
                REQUEST_STATUS_CANCELLED => "已取消",
                _ => "未知"
            };
        }

        /// <summary>
        /// 获取字典标签
        /// </summary>
        private async Task<string> GetDictLabelAsync(string category, string value)
        {
            try
            {
                var dictAppService = IocManager.Instance.Resolve<DictAppService>();
                var result = await dictAppService.GetDictLabelAsync(category, value);
                return result.Data;
            }
            catch
            {
                return value;
            }
        }

        #endregion
    }
}