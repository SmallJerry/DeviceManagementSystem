
using Abp.Application.Services.Dto;
using DeviceManagementSystem.Systems;
using DeviceManagementSystem.SystemConfigs.param;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManagementSystem.SystemConfigs
{
    /// <summary>
    /// 系统配置接口
    /// </summary>
    public interface ISystemConfigAppService
    {

        /// <summary>
        /// 获取系统基础配置列表
        /// </summary>
        /// <returns></returns>
        Task<CommonResult<List<SystemConfig>>> GetSystemBaseList();



        /// <summary>
        ///  获取配置分页
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<CommonResult<Page<SystemConfig>>> GetPageList(ConfigPageParam param);



        /// <summary>
        /// 获取配置列表
        /// </summary>
        /// <param name="category">分类</param>
        /// <returns></returns>
        Task<CommonResult<List<SystemConfig>>> GetList(string category);



        /// <summary>
        /// 添加业务定义配置
        /// </summary>
        /// <param name="configAddParam">配置参数</param>
        /// <returns></returns>
        Task<CommonResult> Create(ConfigAddParam configAddParam);




        /// <summary>
        /// 编辑参数
        /// </summary>
        /// <param name="param">参数实例</param>
        /// <returns></returns>
        Task<CommonResult> UpdateById(ConfigEditParam param);



        /// <summary>
        /// 删除参数
        /// </summary>
        /// <param name="paramList">参数id列表</param>
        /// <returns></returns>
        Task<CommonResult> DeleteByIds([FromBody] List<ConfigIdParam> paramList);


        /// <summary>
        /// 获取配置详情
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<CommonResult<SystemConfig>> GetById(ConfigIdParam param);


        /// <summary>
        ///  批量更新配置
        /// </summary>
        /// <param name="paramList"></param>
        /// <returns></returns>
        Task<CommonResult> EditBatch(List<ConfigBatchParam> paramList);


        /// <summary>
        /// 根据配置键获取基础配置
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        Task<CommonResult<SystemConfig>> GetBaseConfigByConfigKey(string configKey);

    }
}
