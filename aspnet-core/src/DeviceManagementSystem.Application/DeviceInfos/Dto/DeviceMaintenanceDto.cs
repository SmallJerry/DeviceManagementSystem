using DeviceManagementSystem.Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.DeviceInfos.Dto
{
    /// <summary>
    /// 保养履历查询输入
    /// </summary>
    public class MaintenanceHistoryQueryInput : PageRequest
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 工单编号
        /// </summary>
        public string TaskNo { get; set; }

        /// <summary>
        /// 开始日期范围-开始
        /// </summary>
        public DateTime? StartDateBegin { get; set; }

        /// <summary>
        /// 开始日期范围-结束
        /// </summary>
        public DateTime? StartDateEnd { get; set; }
    }

  
    /// <summary>
    /// 保养模板分组DTO
    /// </summary>
    public class MaintenanceTemplateGroupDto
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 保养项目列表
        /// </summary>
        public List<MaintenanceTemplateItemDto> Items { get; set; }
    }

    /// <summary>
    /// 保养模板项目DTO
    /// </summary>
    public class MaintenanceTemplateItemDto
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 点检项目
        /// </summary>
        public string PointName { get; set; }

        /// <summary>
        /// 点检内容
        /// </summary>
        public string InspectionContent { get; set; }

        /// <summary>
        /// 点检方法
        /// </summary>
        public string InspectionMethod { get; set; }

        /// <summary>
        /// 标准值
        /// </summary>
        public string StandardValue { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 上限
        /// </summary>
        public decimal? UpperLimit { get; set; }

        /// <summary>
        /// 下限
        /// </summary>
        public decimal? LowerLimit { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// 设备保养模板DTO
    /// </summary>
    public class DeviceMaintenanceTemplateDto
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 保养等级显示文本
        /// </summary>
        public string MaintenanceLevelText { get; set; }

        /// <summary>
        /// 保养计划ID
        /// </summary>
        public Guid PlanId { get; set; }

        /// <summary>
        /// 保养计划名称
        /// </summary>
        public string PlanName { get; set; }
    }
}
