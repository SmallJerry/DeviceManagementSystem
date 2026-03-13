using Abp.Application.Services;
using DeviceManagementSystem.Sessions.Dto;
using DeviceManagementSystem.Utils.Common;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Sessions
{
    /// <summary>
    /// 会话服务接口
    /// </summary>
    public interface ISessionAppService : IApplicationService
    {

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        /// <returns></returns>
        Task<CommonResult<GetCurrentLoginInformationsOutput>> GetCurrentLoginInformations();
    }
}
