using Abp.Domain.Uow;
using DeviceManagementSystem.Authorization.Resources;
using DeviceManagementSystem.Resources.Menus.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Resources.Menus
{
    /// <summary>
    /// 菜单服务类接口
    /// </summary>
    public interface IMenuAppService
    {
        /// <summary>
        /// 获取菜单分页
        /// </summary>
        /// <param name="input">分页参数</param>
        /// <returns>分页结果</returns>
        Task<CommonResult<Page<Resource>>> GetPageList(MenuPageInput input);


        /// <summary>
        ///  获取菜单树
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<List<object>>> GetTreeList(MenuTreeInput input);

        /// <summary>
        /// 根据ID获取菜单名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<string> GetMenuTitleById(Guid id);


        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="input">添加参数</param>
        /// <returns>操作结果</returns>
        Task<CommonResult> Create(MenuAddInput input);


        /// <summary>
        /// 编辑菜单
        /// </summary>
        /// <param name="input">编辑参数</param>
        /// <returns>操作结果</returns>
        Task<CommonResult> UpdateById(MenuEditInput input);




        /// <summary>
        /// 更改菜单所属模块
        /// </summary>
        /// <param name="input">更改模块参数</param>
        /// <returns>操作结果</returns>
        Task<CommonResult> ChangeModule(MenuChangeModuleInput input);



        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="menuIdInput">ID列表</param>
        /// <returns>操作结果</returns>
        Task<CommonResult> DeleteByIds([FromBody] List<MenuIdInput> menuIdInput);




        /// <summary>
        /// 获取菜单详情
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <returns>菜单详情</returns>

        Task<CommonResult<Resource>> GetById(string id);



        /// <summary>
        /// 获取模块选择器
        /// </summary>
        /// <param name="input">选择器参数</param>
        /// <returns>模块列表</returns>
        Task<CommonResult<List<Resource>>> GetModuleSelector(MenuSelectorModuleInput input);



        /// <summary>
        /// 获取菜单树选择器
        /// </summary>
        /// <param name="input">选择器参数</param>
        /// <returns>菜单树选择器结果</returns>
        Task<CommonResult<List<object>>> GetMenuTreeSelector(MenuSelectorMenuInput input);
    }
}
