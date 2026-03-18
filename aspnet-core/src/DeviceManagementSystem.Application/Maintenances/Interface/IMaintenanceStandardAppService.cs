using DeviceManagementSystem.Maintenances.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances.Interface
{
    public interface IMaintenanceStandardAppService
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        Task<CommonResult<Page<MaintenanceStandardDto>>> GetPageList([FromQuery] MaintenanceStandardPageInput input);

        /// <summary>
        /// 获取详情
        /// </summary>
        Task<CommonResult<MaintenanceStandardDto>> GetById(Guid id);

        /// <summary>
        /// 创建
        /// </summary>
        Task<CommonResult<Guid>> Create(MaintenanceStandardInput input);

        /// <summary>
        /// 更新
        /// </summary>
        Task<CommonResult> Update(MaintenanceStandardInput input);

        /// <summary>
        /// 删除
        /// </summary>
        Task<CommonResult> Delete(Guid id);

        /// <summary>
        /// 批量删除
        /// </summary>
        Task<CommonResult> BatchDelete([FromBody] List<Guid> ids);

        /// <summary>
        /// 获取所有点检部位
        /// </summary>
        Task<CommonResult<List<string>>> GetPointTypes();

        /// <summary>
        /// 批量导入
        /// </summary>
        Task<CommonResult<int>> BatchImport(IFormFile file);


        /// <summary>
        /// 下载导入模板
        /// </summary>
        /// <returns></returns>
        Task<IActionResult> DownloadImportTemplate();
    }
}
