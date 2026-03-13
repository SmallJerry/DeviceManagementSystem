using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Linq;
using Abp.Linq.Extensions;
using DeviceManagementSystem.Authorization;
using DeviceManagementSystem.Authorization.Users;
using DeviceManagementSystem.Journals.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Journals
{
    /// <summary>
    /// 日志服务接口
    /// </summary>
    [AbpAuthorize]
    [DisableAuditing]
    public class JournalAppService : DeviceManagementSystemAppServiceBase
    {
        private readonly IRepository<UserLoginAttempt, long> _repository;
        private readonly IRepository<AuditLog, long> _auditLogRepository;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly IRepository<User, long> _userRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="repository"></param>
        /// <param name="abpLoginResultTypeHelper"></param>
        /// <param name="auditLogRepository"></param>
        public JournalAppService(IRepository<User, long> userRepository, IRepository<UserLoginAttempt, long> repository, AbpLoginResultTypeHelper abpLoginResultTypeHelper, IRepository<AuditLog, long> auditLogRepository)
        {
            _repository = repository;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _auditLogRepository = auditLogRepository;
            _userRepository = userRepository;
        }


        /// <summary>
        /// 获取用户登录日志
        /// </summary>
        public async Task<CommonResult<Page<UserLoginAttemptDto>>> GetAllUserLoginLog(
            PagedLogResultRequestDto input)
        {
            var query = _repository.GetAll()
                .WhereIf(
                    !string.IsNullOrWhiteSpace(input.SearchKey),
                    x => x.UserNameOrEmailAddress.Contains(input.SearchKey)
                )
                .WhereIf(input.Time != null && input.Time.Length == 2, x => x.CreationTime >= input.Time[0] && x.CreationTime <= input.Time[1])
                .WhereIf(
                    input.Result.HasValue,
                    x => input.Result.Value
                        ? x.Result == AbpLoginResultType.Success
                        : x.Result != AbpLoginResultType.Success
                );

            // 总数
            var totalCount = await query.CountAsync();

            // 排序（支持字段扩展）
            query = input.SortField switch
            {
                "UserNameOrEmailAddress" => input.SortOrder == "ASCEND"
                    ? query.OrderBy(x => x.UserNameOrEmailAddress)
                    : query.OrderByDescending(x => x.UserNameOrEmailAddress),

                _ => input.SortOrder == "ASCEND"
                    ? query.OrderBy(x => x.CreationTime)
                    : query.OrderByDescending(x => x.CreationTime)
            };

            // 分页
            var entities = await query
                .Skip((input.Current - 1) * input.Size)
                .Take(input.Size)
                .ToListAsync();

            // 映射 DTO
            var items = entities.Select(u =>
            {
                var dto = ObjectMapper.Map<UserLoginAttemptDto>(u);
                dto.ResultMsg = u.Result == AbpLoginResultType.Success
                    ? "登录成功"
                    : _abpLoginResultTypeHelper.CreateLocalizedMessageForFailedLoginAttempt(
                        u.Result,
                        u.UserNameOrEmailAddress,
                        u.TenancyName
                    );
                return dto;
            }).ToList();

            var page = new Page<UserLoginAttemptDto>
            {
                Current = input.Current,
                Size = input.Size,
                Total = totalCount,
                Records = items
            };

            return CommonResult<Page<UserLoginAttemptDto>>.Success(page);
        }




        /// <summary>
        /// 获取审计日志
        /// </summary>
        public async Task<CommonResult<Page<AuditLogDto>>> GetAllAuditLog(
            PagedLogResultRequestDto input)
        {
            var auditQuery = _auditLogRepository.GetAll()
                .AsNoTracking()
                .Where(x => !x.MethodName.Contains("Get"))
                .WhereIf(input.Time != null && input.Time.Length == 2, x => x.ExecutionTime >= input.Time[0] && x.ExecutionTime <= input.Time[1])
                .WhereIf(
                    input.Result.HasValue,
                    x => input.Result.Value
                        ? x.Exception == null
                        : !string.IsNullOrEmpty(x.Exception)
                );

            var query =
                from auditLog in auditQuery
                join user in _userRepository.GetAll().AsNoTracking()
                    on auditLog.UserId equals user.Id into userJoin
                from user in userJoin.DefaultIfEmpty()
                select new AuditLogDto
                {
                    Id = auditLog.Id,
                    TenantId = auditLog.TenantId,
                    UserId = auditLog.UserId,

                    Name = user != null ? user.UserName : string.Empty,
                    PhoneNumber = user != null ? user.PhoneNumber : string.Empty,

                    ServiceName = auditLog.ServiceName,
                    MethodName = auditLog.MethodName,
                    Parameters = auditLog.Parameters,
                    ReturnValue = auditLog.ReturnValue,

                    BrowserInfo = auditLog.BrowserInfo,
                    ClientIpAddress = auditLog.ClientIpAddress,
                    ClientName = auditLog.ClientName,

                    ExecutionTime = auditLog.ExecutionTime,
                    ExecutionDuration = auditLog.ExecutionDuration,

                    Exception = auditLog.Exception,
                    ExceptionMessage = auditLog.ExceptionMessage,

                    ImpersonatorTenantId = auditLog.ImpersonatorTenantId,
                    ImpersonatorUserId = auditLog.ImpersonatorUserId,
                    CustomData = auditLog.CustomData
                };

            query = query
                 .Where(it => !string.IsNullOrWhiteSpace(it.Name))
                .WhereIf(
                    !string.IsNullOrWhiteSpace(input.SearchKey),
                    x => x.Name.Contains(input.SearchKey)
                      || x.PhoneNumber.Contains(input.SearchKey)
                )
                .OrderByDescending(x => x.ExecutionTime);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((input.Current - 1) * input.Size)
                .Take(input.Size)
                .ToListAsync();

            var page = new Page<AuditLogDto>
            {
                Current = input.Current,
                Size = input.Size,
                Total = totalCount,
                Records = items
            };

            return CommonResult<Page<AuditLogDto>>.Success(page);
        }


    }
}
