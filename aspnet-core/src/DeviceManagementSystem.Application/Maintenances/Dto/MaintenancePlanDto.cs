using DeviceManagementSystem.Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances.Dto
{
    #region 保养计划相关DTO

    /// <summary>
    /// 保养计划输入DTO（设备建档时使用）
    /// </summary>
    public class MaintenancePlanInput
    {
        /// <summary>
        /// 计划ID（编辑时传入）
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 设备ID（审批通过后才有）
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 首次保养日期
        /// </summary>
        public DateTime FirstMaintenanceDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 设备启用日期（用于计算）
        /// </summary>
        public DateTime? EnableDate { get; set; }
    }

    /// <summary>
    /// 保养计划DTO
    /// </summary>
    public class MaintenancePlanDto
    {
        /// <summary>
        /// 计划ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 计划名称
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

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
        /// 周期类型
        /// </summary>
        public string CycleType { get; set; }

        /// <summary>
        /// 周期值（天数）
        /// </summary>
        public int CycleDays { get; set; }

        /// <summary>
        /// 首次保养日期
        /// </summary>
        public DateTime FirstMaintenanceDate { get; set; }

        /// <summary>
        /// 下次保养日期
        /// </summary>
        public DateTime NextMaintenanceDate { get; set; }

        /// <summary>
        /// 上次保养日期
        /// </summary>
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// 计划状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 是否已生成工单
        /// </summary>
        public bool HasGeneratedTask { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 剩余天数
        /// </summary>
        public int RemainingDays
        {
            get
            {
                return (int)(NextMaintenanceDate - DateTime.Today).TotalDays;
            }
        }

        /// <summary>
        /// 状态显示（带颜色）
        /// </summary>
        public string StatusColor
        {
            get
            {
                if (Status != "启用") return "default";
                if (RemainingDays < 0) return "red";
                if (RemainingDays <= 7) return "orange";
                return "green";
            }
        }
    }

    /// <summary>
    /// 设备保养计划组合DTO（用于设备建档）
    /// </summary>
    public class DeviceMaintenancePlansDto
    {
        /// <summary>
        /// 月度保养计划
        /// </summary>
        public MaintenancePlanInput Monthly { get; set; }

        /// <summary>
        /// 季度保养计划
        /// </summary>
        public MaintenancePlanInput Quarterly { get; set; }

        /// <summary>
        /// 半年度保养计划
        /// </summary>
        public MaintenancePlanInput HalfYearly { get; set; }

        /// <summary>
        /// 年度保养计划
        /// </summary>
        public MaintenancePlanInput Annual { get; set; }
    }

    /// <summary>
    /// 设备变更申请中的保养计划数据
    /// </summary>
    public class ChangeApplyMaintenanceData
    {
        /// <summary>
        /// 月度计划模板ID
        /// </summary>
        public Guid? MonthlyTemplateId { get; set; }

        /// <summary>
        /// 季度计划模板ID
        /// </summary>
        public Guid? QuarterlyTemplateId { get; set; }

        /// <summary>
        /// 半年度计划模板ID
        /// </summary>
        public Guid? HalfYearlyTemplateId { get; set; }

        /// <summary>
        /// 年度计划模板ID
        /// </summary>
        public Guid? AnnualTemplateId { get; set; }

        /// <summary>
        /// 月度首次保养日期
        /// </summary>
        public DateTime? MonthlyFirstDate { get; set; }

        /// <summary>
        /// 季度首次保养日期
        /// </summary>
        public DateTime? QuarterlyFirstDate { get; set; }

        /// <summary>
        /// 半年度首次保养日期
        /// </summary>
        public DateTime? HalfYearlyFirstDate { get; set; }

        /// <summary>
        /// 年度首次保养日期
        /// </summary>
        public DateTime? AnnualFirstDate { get; set; }
    }

    /// <summary>
    /// 保养计划统计DTO（本周、下周、逾期、已生成）
    /// </summary>
    public class MaintenancePlanStatisticsDto
    {
        /// <summary>
        /// 本周
        /// </summary>
        public int ThisWeek { get; set; }

        /// <summary>
        /// 下周
        /// </summary>
        public int NextWeek { get; set; }

        /// <summary>
        /// 逾期
        /// </summary>
        public int Overdue { get; set; }

        /// <summary>
        /// 已生成
        /// </summary>
        public int Generated { get; set; }
    }

    /// <summary>
    /// 保养计划简要DTO（用于待生成工单列表）
    /// </summary>
    public class MaintenancePlanSimpleDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// 计划名称
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }


        /// <summary>
        /// 下次保养日期
        /// </summary>
        public DateTime NextMaintenanceDate { get; set; }
    }

    /// <summary>
    /// 更新计划状态输入DTO
    /// </summary>
    public class UpdatePlanStatusInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
    }



    /// <summary>
    /// 保养计划分页查询输入
    /// </summary>
    public class MaintenancePlanPageInput : PageRequest
    {

        /// <summary>
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 计划状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 下次保养日期开始
        /// </summary>
        public DateTime? NextDateBegin { get; set; }

        /// <summary>
        /// 下次保养日期结束
        /// </summary>
        public DateTime? NextDateEnd { get; set; }
    }


    #endregion

}
