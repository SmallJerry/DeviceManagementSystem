using DeviceManagementSystem.Authorization.Organizations;
using DeviceManagementSystem.Organizations.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Organizations
{
    /// <summary>
    /// 组织服务类接口
    /// </summary>
    public interface IOrganizationAppService
    {

        /// <summary>
        /// 获取组织分页
        /// </summary>
        /// <param name="input">分页参数</param>
        /// <returns>分页结果</returns>
        Task<CommonResult<Page<Organization>>> GetPageList(OrganizationPageInput input);


        /// <summary>
        /// 获取组织树
        /// </summary>
        /// <returns>树形结构</returns>
        Task<CommonResult<List<object>>> GetTreeList();




        /// <summary>
        /// 添加组织       
        /// </summary>
        /// <param name="input">添加参数</param>
        /// <returns>操作结果</returns>
        Task<CommonResult> Create(OrganizationAddInput input);




        /// <summary>
        /// 编辑组织
        /// </summary>
        /// <param name="input">编辑参数</param>
        /// <returns>操作结果</returns>
        Task<CommonResult> UpdateById(OrganizationEditInput input);




        /// <summary>
        /// 删除组织
        /// </summary>
        /// <param name="ids">ID列表</param>
        /// <returns>操作结果</returns>
        Task<CommonResult> DeleteByIds([FromBody] List<OrganizationIdInput> ids);



        /// <summary>
        /// 获取组织详情
        /// </summary>
        /// <param name="id">组织ID</param>
        /// <returns>组织详情</returns>
        Task<CommonResult<Organization>> GetById(Guid id);



        /// <summary>
        /// 获取组织树选择器
        /// </summary>
        /// <returns></returns>
        Task<CommonResult<List<TreeNode>>> GetOrgTreeSelector();

    }
}
