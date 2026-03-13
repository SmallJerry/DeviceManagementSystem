using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using DeviceManagementSystem.Authorization.Users;
using DeviceManagementSystem.MultiTenancy;
using DeviceManagementSystem.Sessions.Dto;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace DeviceManagementSystem
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class DeviceManagementSystemAppServiceBase : ApplicationService
    {

        /// <summary>
        /// 租户管理
        /// </summary>
        public TenantManager TenantManager { get; set; }

        /// <summary>
        /// 用户管理
        /// </summary>
        public UserManager UserManager { get; set; }




        /// <summary>
        /// 构造函数
        /// </summary>
        protected DeviceManagementSystemAppServiceBase()
        {
            LocalizationSourceName = DeviceManagementSystemConsts.LocalizationSourceName;
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("登录状态失效，请重新登录!");
            }

            return user;
        }

        /// <summary>
        /// 获取当前租户信息
        /// </summary>
        /// <returns></returns>
        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        /// <summary>
        /// 校验错误
        /// </summary>
        /// <param name="identityResult"></param>
        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }


    }
}
