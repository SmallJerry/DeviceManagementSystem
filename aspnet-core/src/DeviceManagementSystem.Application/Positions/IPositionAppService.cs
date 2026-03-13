using DeviceManagementSystem.Authorization.Positions;
using DeviceManagementSystem.Positions.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Positions
{
    /// <summary>
    /// 职位服务接口
    /// </summary>
    public interface IPositionAppService
    {


        /// <summary>
        /// 获取职位分页
        /// </summary>
        /// <param name="input">分页参数</param>
        /// <returns>分页结果</returns>
        Task<CommonResult<Page<Position>>> GetPageList(PositionPageInput input);




        /// <summary>
        /// 添加职位
        /// </summary>
        /// <param name="input">添加参数</param>
        /// <returns>操作结果</returns>
        Task<CommonResult> Create(PositionAddInput input);


        /// <summary>
        /// 编辑职位
        /// </summary>
        /// <param name="input">编辑参数</param>
        /// <returns>操作结果</returns>
        Task<CommonResult> UpdateById(PositionEditInput input);



        /// <summary>
        /// 删除职位
        /// </summary>
        /// <param name="ids">ID列表</param>
        /// <returns>操作结果</returns>
        Task<CommonResult> DeleteByIds([FromBody] List<PositionIdInput> ids);



        /// <summary>
        /// 获取职位详情
        /// </summary>
        /// <param name="id">职位ID</param>
        /// <returns>职位详情</returns>
        Task<CommonResult<Position>> GetById(Guid id);





    }
}
