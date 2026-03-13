using DeviceManagementSystem.Authorization.Resources;
using DeviceManagementSystem.Resources.Modules.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Resources.Modules
{
    /// <summary>
    /// /模块类服务接口
    /// </summary>
    public interface IModuleAppService
    {

        /// <summary>
        /// 获取模块分页
        /// </summary>
        /// <param name="input">分页参数</param>
        /// <returns>分页结果</returns>
        Task<CommonResult<Page<Resource>>> GetPageList(ModulePageInput input);



        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="input">添加参数</param>
        /// <returns>操作结果</returns>
        Task<CommonResult> Create(ModuleAddInput input);


        /// <summary>
        /// 编辑模块
        /// </summary>
        /// <param name="input">编辑参数</param>
        /// <returns>操作结果</returns>
        Task<CommonResult> UpdateById(ModuleEditInput input);



        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="ids">ID列表</param>
        /// <returns>操作结果</returns>
        Task<CommonResult> DeleteByIds([FromBody] List<Guid> ids);




        /// <summary>
        /// 获取模块详情     
        /// </summary>
        /// <param name="id">模块ID</param>
        /// <returns>模块详情</returns>
        Task<CommonResult<Resource>> GetById(string id);
    }
}
