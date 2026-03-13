using Abp;
using Abp.Authorization;
using Abp.Dependency;
using Abp.UI;
using System;

namespace DeviceManagementSystem.Authorization
{
    public class AbpLoginResultTypeHelper : AbpServiceBase, ITransientDependency
    {
        public AbpLoginResultTypeHelper()
        {
            LocalizationSourceName = DeviceManagementSystemConsts.LocalizationSourceName;
        }

        public Exception CreateExceptionForFailedLoginAttempt(AbpLoginResultType result, string usernameOrEmailAddress)
        {
            switch (result)
            {
                case AbpLoginResultType.Success:
                    return new Exception("Don't call this method with a success result!");
                case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                    return new UserFriendlyException("输入的邮箱账号无效，请重试或联系管理员创建账号！");
                case AbpLoginResultType.InvalidPassword:
                    return new UserFriendlyException("输入的密码有误，请重试！");
                case AbpLoginResultType.UserIsNotActive:
                    return new UserFriendlyException("用户未激活，无法登录！");
                case AbpLoginResultType.UserEmailIsNotConfirmed:
                    return new UserFriendlyException("用户邮件未确认，无法登录！");
                case AbpLoginResultType.LockedOut:
                    return new UserFriendlyException("该账号已锁定！");
                default: // Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    Logger.Warn(L("Unhandled login fail reason: " + result));
                    return new UserFriendlyException("登录失败！");
            }
        }

        public string CreateLocalizedMessageForFailedLoginAttempt(AbpLoginResultType result, string usernameOrEmailAddress, string tenancyName)
        {
            switch (result)
            {
                case AbpLoginResultType.Success:
                    throw new Exception("Don't call this method with a success result!");
                case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                case AbpLoginResultType.InvalidPassword:
                    return L("InvalidUserNameOrPassword");
                case AbpLoginResultType.InvalidTenancyName:
                    return L("ThereIsNoTenantDefinedWithName{0}", tenancyName);
                case AbpLoginResultType.TenantIsNotActive:
                    return L("TenantIsNotActive", tenancyName);
                case AbpLoginResultType.UserIsNotActive:
                    return L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress);
                case AbpLoginResultType.UserEmailIsNotConfirmed:
                    return L("UserEmailIsNotConfirmedAndCanNotLogin");
                default: // Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    Logger.Warn("Unhandled login fail reason: " + result);
                    return L("LoginFailed");
            }
        }
    }
}
