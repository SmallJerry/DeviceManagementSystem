using DeviceManagementSystem.Maintenances.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances.Interface
{
    /// <summary>
    /// 保养模板服务接口
    /// </summary>
    public interface IMaintenanceTemplateAppService
    {
        /// <summary>
        /// 获取保养模板分页列表
        /// </summary>
        Task<CommonResult<Page<MaintenanceTemplateDto>>> GetPageList([FromQuery] MaintenanceTemplatePageInput input);

        /// <summary>
        /// 获取所有保养模板（用于选择器）
        /// </summary>
        Task<CommonResult<List<MaintenanceTemplateDto>>> GetListForSelector(Guid? deviceTypeId = null, string level = null);

        /// <summary>
        /// 获取保养模板详情
        /// </summary>
        Task<CommonResult<MaintenanceTemplateDetailDto>> GetById(Guid id);

        /// <summary>
        /// 创建保养模板
        /// </summary>
        Task<CommonResult<Guid>> Create([FromBody] MaintenanceTemplateFullInput input);

        /// <summary>
        /// 更新保养模板
        /// </summary>
        Task<CommonResult> Update([FromBody] MaintenanceTemplateFullInput input);

        /// <summary>
        /// 删除保养模板
        /// </summary>
        Task<CommonResult> Delete(Guid id);

        /// <summary>
        /// 批量删除保养模板
        /// </summary>
        Task<CommonResult> BatchDelete([FromBody] List<Guid> ids);


        /// <summary>
        /// 调整分组顺序
        /// </summary>
        Task<CommonResult> ReorderGroups(Guid templateId, List<Guid> groupIds);

        /// <summary>
        /// 调整项目顺序
        /// </summary>
        Task<CommonResult> ReorderItems(Guid groupId, List<Guid> itemIds);
    }
}
