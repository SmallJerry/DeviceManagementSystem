using DeviceManagementSystem.BasicDataManagements.TechnicalParameterTemplate.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.BasicDataManagements.TechnicalParameterTemplate
{
    /// <summary>
    /// 技术参数模板管理服务接口
    /// </summary>
    public interface ITechnicalParameterTemplateAppService
    {

        /// <summary>
        /// 获取技术参数模板分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<Page<TechnicalParameterTemplateDto>>> GetPageList(TechnicalParameterTemplatePageInput input);



        /// <summary>
        /// 获取技术参数模板选择器列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<List<TechnicalParameterTemplateSelectorDto>>> GetListForSelector(TechnicalParameterTemplateSelectorInput input);


        /// <summary>
        /// 获取技术参数模板详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CommonResult<TechnicalParameterTemplateDto>> GetById(Guid id);


        /// <summary>
        /// 创建技术参数模板
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> Create(TechnicalParameterTemplateAddInput input);


        /// <summary>
        /// 更新技术参数模板
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> UpdateById(TechnicalParameterTemplateEditInput input);


        /// <summary>
        /// 删除技术参数模板
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<CommonResult> DeleteByIds([FromBody] List<TechnicalParameterTemplateIdInput> ids);
    }
}
