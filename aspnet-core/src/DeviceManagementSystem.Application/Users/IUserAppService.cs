using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Uow;
using DeviceManagementSystem.Roles.Dto;
using DeviceManagementSystem.Users.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Users
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        /// <summary>
        /// 激活用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<CommonResult> Activate(EntityDto<long> user);
        
        /// <summary>
        /// 禁用用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<CommonResult> DeActivate(EntityDto<long> user);

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        Task<CommonResult<ListResultDto<RoleDto>>> GetRoles();

        /// <summary>
        /// 修改用户语言
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> ChangeLanguage(ChangeUserLanguageDto input);
        
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> ChangePassword(ChangePasswordDto input);
        
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> ResetPassword(ResetPasswordDto input);


        /// <summary>
        /// 根据用户ID获取用户名称
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GetNameByUserId(long userId);


        /// <summary>
        /// 下载用户导入模板
        /// </summary>
        IActionResult GetDownloadImportUserTemplate();



        /// <summary>
        /// 导入用户数据（从Excel文件）
        /// </summary>
        Task<CommonResult<UserImportResultDto>> Import(IFormFile file);


        /// <summary>
        /// 批量导入用户（高性能版）
        /// </summary>
        Task<CommonResult<UserImportResultDto>> ImportBatch(UserImportDto input);


        /// <summary>
        /// 获取登录用户PC端菜单
        /// </summary>
        Task<CommonResult<List<Tree<string>>>> GetLoginMenu();



        /// <summary>
        /// 获取用户拥有资源
        /// </summary>
        Task<CommonResult<SysUserOwnResourceResult>> GetOwnResource(SysUserIdParam sysUserIdParam);


        /// <summary>
        /// 给用户授权资源
        /// </summary>
        Task<CommonResult> GrantResource(SysUserGrantResourceParam sysUserGrantResourceParam);


        /// <summary>
        /// 给用户授权角色
        /// </summary>
        Task<CommonResult> GrantRole(SysUserGrantRoleParam sysUserGrantRoleParam);


        /// <summary>
        /// 获取用户拥有的角色ID列表
        /// </summary>
        Task<List<long>> GetOwnRole(SysUserIdParam sysUserIdParam);



        /// <summary>
        /// 激活/禁用用户状态  
        /// </summary>
        /// <returns></returns>
        Task<CommonResult<string>> UpdateUserStatus(int id);



        /// <summary>
        /// 编辑个人工作台快捷方式
        /// </summary>
        Task<CommonResult<string>> UpdateUserWorkbench(UserUpdateWorkbenchParam userUpdateWorkbenchParam);



        /// <summary>
        /// 获取登录用户个人工作台快捷方式
        /// </summary>
        /// <returns></returns>
        Task<CommonResult<string>> GetLoginUserWorkbench();


        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<UserDto>> CreateUserAsync(CreateUserDto input);


        /// <summary>
        /// 根据id修改用户信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<UserDto>> UpdateByIdAsync(UserDto input);


        /// <summary>
        /// 当前登录用户修改基础用户信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> UpdateUserInfo(BaseUserInfo input);



        /// <summary>
        /// 根据id删除用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> DeleteByIdAsync([FromBody] List<SysUserIdParam> input);



        /// <summary>
        /// 根据id列表获取用户Dto列表
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        Task<CommonResult<List<UserDto>>> GetUserListByIdList([FromBody] List<long> idList);



        /// <summary>
        /// 根据id获取用户Dto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<UserDto>> GetByIdAsync(EntityDto<long> input);




        /// <summary>
        /// 分页查询列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<CommonResult<Page<UserDto>>> GetPageListAsync(UserPageParam param);



        /// <summary>
        /// 统计业务数据
        /// </summary>
        /// <returns></returns>
        Task<CommonResult<BizDataCountDto>> GetBizDataCount();





        /// <summary>
        /// 获取用户选择器
        /// </summary>
        /// <param name="param">查询参数</param>
        /// <returns>用户选择器结果</returns>
        Task<CommonResult<Page<UserSelectorDto>>> GetUserSelector(UserSelectorParam param);
    }
}
