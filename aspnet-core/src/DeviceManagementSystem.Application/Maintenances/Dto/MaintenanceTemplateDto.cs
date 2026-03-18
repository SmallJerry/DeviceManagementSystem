using DeviceManagementSystem.Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances.Dto
{
    #region 保养模板相关DTO

    /// <summary>
    /// 保养模板输入DTO
    /// </summary>
    public class MaintenanceTemplateInput
    {
        /// <summary>
        /// 模板ID（编辑时传入）
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid DeviceTypeId { get; set; }

        /// <summary>
        /// 设备类型名称
        /// </summary>
        public string DeviceTypeName { get; set; }

        /// <summary>
        /// 保养等级（月度、季度、半年度、年度）
        /// </summary>
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 模板描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 保养项目列表
        /// </summary>
        public List<MaintenanceItemDto> Items { get; set; } = new List<MaintenanceItemDto>();
    }

    /// <summary>
    /// 保养模板输出DTO
    /// </summary>
    public class MaintenanceTemplateDto
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid DeviceTypeId { get; set; }

        /// <summary>
        /// 设备类型名称
        /// </summary>
        public string DeviceTypeName { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 保养等级显示文本
        /// </summary>
        public string MaintenanceLevelText { get; set; }

        /// <summary>
        /// 模板描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 状态显示文本
        /// </summary>
        public string IsActiveText => IsActive ? "启用" : "停用";

        /// <summary>
        /// 项目数量
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 保养项目列表
        /// </summary>
        public List<MaintenanceItemDto> Items { get; set; }
    }

    /// <summary>
    /// 保养项目DTO
    /// </summary>
    public class MaintenanceItemDto
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid? Id { get; set; }


        /// <summary>
        /// 序号（①、②、③...）
        /// </summary>
        public string PointNo { get; set; }

        /// <summary>
        /// 点检项目名称
        /// </summary>
        public string PointName { get; set; }
        
        /// <summary>
        /// 点检内容
        /// </summary>
        public string InspectionContent { get; set; }

        /// <summary>
        /// 点检方法列表（例如：["清洁","目测"]）
        /// </summary>
        public List<string> InspectionMethod { get; set; }


        /// <summary>
        /// 排序号（用于组内排序）
        /// </summary>
        public int SortOrder { get; set; }
    }



    /// <summary>
    /// 完整输入DTO
    /// </summary>
    public class MaintenanceTemplateFullInput
    {

        /// <summary>
        /// 主键
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid DeviceTypeId { get; set; }

        /// <summary>
        /// 保养等级（月度、季度、半年度、年度）
        /// </summary>
        public string MaintenanceLevel { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否启用（默认为true）
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// 分组列表（点检部位分组）
        /// </summary>
        public List<MaintenanceItemGroupInput> Groups { get; set; }
    }



    /// <summary>
    /// 分组输入DTO
    /// </summary>
    public class MaintenanceItemGroupInput
    {
        /// <summary>
        /// 组别ID（编辑时传入）
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// 点检部位分组名称（例如：清洗工作腔）
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 点检部位（与GroupName相同，用于兼容）
        /// </summary>
        public string PointType { get; set; }
        
        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 项目列表（分组内的点检项目）
        /// </summary>
        public List<MaintenanceItemInput> Items { get; set; }
    }


    /// <summary>
    /// 项目输入DTO
    /// </summary>
    public class MaintenanceItemInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid? Id { get; set; }
        
        /// <summary>
        /// 点检序号
        /// </summary>
        public string PointNo { get; set; }

        /// <summary>
        /// 点检项目名称
        /// </summary>
        public string PointName { get; set; }

        /// <summary>
        /// 点检内容
        /// </summary>
        public string InspectionContent { get; set; }

        /// <summary>
        /// 点检方法列表（例如：["清洁","目测"]）
        /// </summary>
        public List<string> InspectionMethod { get; set; }

        /// <summary>
        /// 序号（用于组内排序）
        /// </summary>
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// 保养模板分页查询输入
    /// </summary>
    public class MaintenanceTemplatePageInput : PageRequest
    {

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid? DeviceTypeId { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? IsActive { get; set; }
    }



    /// <summary>
    /// 保养模板详情DTO（带分组）
    /// </summary>
    public class MaintenanceTemplateDetailDto
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid DeviceTypeId { get; set; }

        /// <summary>
        /// 设备类型名称
        /// </summary>
        public string DeviceTypeName { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 保养等级显示文本
        /// </summary>
        public string MaintenanceLevelText { get; set; }

        /// <summary>
        /// 模板描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 分组列表
        /// </summary>
        public List<MaintenanceItemGroupDetailDto> Groups { get; set; }
    }




    /// <summary>
    /// 保养项目分组详情DTO
    /// </summary>
    public class MaintenanceItemGroupDetailDto
    {
        /// <summary>
        /// 分组ID
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// 分组名称（例如：清洗工作腔）
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 点检部位
        /// </summary>
        public string PointType { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 项目列表
        /// </summary>
        public List<MaintenanceItemDetailDto> Items { get; set; }
    }

    /// <summary>
    /// 保养项目详情DTO
    /// </summary>
    public class MaintenanceItemDetailDto
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 分组ID
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// 点检序号（①、②、③...）
        /// </summary>
        public string PointNo { get; set; }

        /// <summary>
        /// 点检项目名称
        /// </summary>
        public string PointName { get; set; }

        /// <summary>
        /// 点检内容
        /// </summary>
        public string InspectionContent { get; set; }

        /// <summary>
        /// 点检方法（JSON字符串）
        /// </summary>
        public List<string> InspectionMethod { get; set; }

        /// <summary>
        /// 分组排序
        /// </summary>
        public int GroupSortOrder { get; set; }

        /// <summary>
        /// 项目排序
        /// </summary>
        public int ItemSortOrder { get; set; }
    }

   

    #endregion

}
