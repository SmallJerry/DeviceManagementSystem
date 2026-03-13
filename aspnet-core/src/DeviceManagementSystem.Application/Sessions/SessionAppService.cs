using Abp.Auditing;
using Abp.Domain.Repositories;
using DeviceManagementSystem.Authorization.Organizations;
using DeviceManagementSystem.Authorization.Positions;
using DeviceManagementSystem.Authorization.Users;
using DeviceManagementSystem.Sessions.Dto;
using DeviceManagementSystem.Utils.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Sessions
{
    /// <summary>
    /// 会话服务类
    /// </summary>
    public class SessionAppService : DeviceManagementSystemAppServiceBase, ISessionAppService
    {

        private readonly IRepository<Organization, Guid> _organizationRepository;

        private readonly IRepository<Position, Guid> _positionRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="organizationRepository"></param>
        /// <param name="positionRepository"></param>
        public SessionAppService(
          IRepository<Organization, Guid> organizationRepository,
          IRepository<Position, Guid> positionRepository)
        {
            _organizationRepository = organizationRepository;
            _positionRepository = positionRepository;
        }


        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        /// <returns></returns>
        [DisableAuditing]
        public async Task<CommonResult<GetCurrentLoginInformationsOutput>> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                }
            };

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
            }

            if (AbpSession.UserId.HasValue)
            {
                var result = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
                if (result.OrgId != null && result.OrgId != Guid.Empty)
                {
                    result.OrgName = await GetOrganizationNameByOrgId((Guid)result.OrgId);
                }
                if (result.PositionId != null && result.PositionId != Guid.Empty)
                {
                    result.PositionName = await GetPositionNameByPositionId((Guid)result.PositionId);
                }
                output.User = result;
            }

            return CommonResult.Ok(output);
        }


        /// <summary>
        /// 根据id获取组织名称
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private async Task<string> GetOrganizationNameByOrgId(Guid Id)
        {
            var organization = await _organizationRepository.FirstOrDefaultAsync(Id);
            if (organization != null)
            {
                return organization.Name;
            }
            return null;
        }



        /// <summary>
        /// 根据id获取组织名称
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private async Task<string> GetPositionNameByPositionId(Guid Id)
        {
            var position = await _positionRepository.FirstOrDefaultAsync(Id);
            if (position != null)
            {
                return position.Name;
            }
            return null;
        }
    }
}
