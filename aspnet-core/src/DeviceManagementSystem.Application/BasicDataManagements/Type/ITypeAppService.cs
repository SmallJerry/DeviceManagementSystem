using DeviceManagementSystem.BasicDataManagements.Type.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.BasicDataManagements.Type
{
    /// <summary>
    /// 类型管理服务接口
    /// </summary>
    public interface ITypeAppService
    {

        /// <summary>
        /// 获取类型分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult<Page<TypeDto>>> GetPageList(TypePageInput input);



        /// <summary>
        /// 获取类型列表
        /// </summary>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        Task<CommonResult<List<TypeDto>>> GetList(string searchKey = null);



        /// <summary>
        /// 获取类型详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CommonResult<TypeDto>> GetById(Guid id);



        /// <summary>
        /// 创建类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> Create(TypeAddInput input);


        /// <summary>
        /// 更新类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CommonResult> UpdateById(TypeEditInput input);



        /// <summary>
        /// 删除类型
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<CommonResult> DeleteByIds([FromBody] List<TypeIdInput> ids);
    }
}
