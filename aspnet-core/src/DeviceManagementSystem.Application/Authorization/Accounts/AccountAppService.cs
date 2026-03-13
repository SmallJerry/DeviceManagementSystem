using Abp.Auditing;
using Abp.Configuration;
using Abp.Zero.Configuration;
using DeviceManagementSystem.Authorization.Accounts.Dto;
using DeviceManagementSystem.Authorization.Users;
using DeviceManagementSystem.Identity;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Authorization.Accounts
{
    /// <summary>
    /// 账号服务
    /// </summary>
    public class AccountAppService : DeviceManagementSystemAppServiceBase, IAccountAppService
    {
        // from: http://regexlib.com/REDetails.aspx?regexp_id=1923
        public const string PasswordRegex = "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s)[0-9a-zA-Z!@#$%^&*()]*$";

        private readonly UserRegistrationManager _userRegistrationManager;

        private readonly SignInManager _signInManager;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager,
             SignInManager signInManager)
        {
            _userRegistrationManager = userRegistrationManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// 判断租户名是否可用
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.EmailAddress,
                input.UserName,
                input.Password,
                true // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [DisableAuditing]
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
