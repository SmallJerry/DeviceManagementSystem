using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances.Dto
{
    /// <summary>
    /// 保养统计查询条件
    /// </summary>
    public class MaintenanceStatisticsInput
    {
        /// <summary>
        /// 设备名称（支持多个关键词空格分隔）
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 设备类型ID列表
        /// </summary>
        public List<Guid> DeviceTypeIds { get; set; }

        /// <summary>
        /// 保养等级列表
        /// </summary>
        public List<string> MaintenanceLevels { get; set; }

        /// <summary>
        /// 保养频次列表
        /// </summary>
        public List<string> MaintenanceFrequencies { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
    }


    /// <summary>
    /// 保养统计看板DTO
    /// </summary>
    public class MaintenanceStatisticsDto
    {
        /// <summary>
        /// 今日待完成任务
        /// </summary>
        public int TodayPendingCount { get; set; }

        /// <summary>
        /// 今日已完成任务
        /// </summary>
        public int TodayCompletedCount { get; set; }

        /// <summary>
        /// 本月已完成任务
        /// </summary>
        public int MonthCompletedCount { get; set; }

        /// <summary>
        /// 本月未完成任务
        /// </summary>
        public int MonthPendingCount { get; set; }

        /// <summary>
        /// 本月待完成任务等级分布
        /// </summary>
        public List<LevelDistributionDto> MonthLevelDistribution { get; set; }

        /// <summary>
        /// 保养完成率分析
        /// </summary>
        public List<CompletionRateDto> CompletionRates { get; set; }

        /// <summary>
        /// 保养次数统计
        /// </summary>
        public List<DeviceMaintenanceCountDto> MaintenanceCounts { get; set; }

        /// <summary>
        /// 保养次数时间趋势
        /// </summary>
        public List<MaintenanceTrendDto> MaintenanceTrends { get; set; }

        /// <summary>
        /// 已完成保养次数与等级分布
        /// </summary>
        public List<CompletedMaintenanceDistributionDto> CompletedDistributions { get; set; }

        /// <summary>
        /// 保养记录明细
        /// </summary>
        public List<MaintenanceRecordDto> Records { get; set; }

        /// <summary>
        /// 记录总数
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// 设备类型选项（用于前端筛选）
        /// </summary>
        public List<DeviceTypeOptionDto> DeviceTypeOptions { get; set; }

        /// <summary>
        /// 设备编码选项（用于前端筛选）
        /// </summary>
        public List<string> DeviceCodeOptions { get; set; }
    }

    /// <summary>
    /// 设备类型选项DTO
    /// </summary>
    public class DeviceTypeOptionDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }
       
        /// <summary>
        /// 设备类型名称
        /// </summary>
        public string TypeName { get; set; }
    }

    /// <summary>
    /// 等级分布DTO
    /// </summary>
    public class LevelDistributionDto
    {
        public string Level { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// 完成率分析DTO
    /// </summary>
    public class CompletionRateDto
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }


        /// <summary>
        /// 设备类型名称
        /// </summary>
        public string DeviceTypeName { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }
        
        /// <summary>
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }
        
        /// <summary>
        /// 保养频率
        /// </summary>
        public string MaintenanceFrequency { get; set; }

        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime StatisticsDate { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }


        /// <summary>
        /// 完成数量
        /// </summary>
        public int CompletedCount { get; set; }
        
        /// <summary>
        /// 完成率
        /// </summary>
        public decimal CompletionRate { get; set; }
    }

    /// <summary>
    /// 设备保养次数统计DTO
    /// </summary>
    public class DeviceMaintenanceCountDto
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }


        /// <summary>
        /// 设备类型名称
        /// </summary>
        public string DeviceTypeName { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 保养趋势DTO
    /// </summary>
    public class MaintenanceTrendDto
    {
        /// <summary>
        /// 月
        /// </summary>
        public DateTime Month { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// 已完成保养分布DTO
    /// </summary>
    public class CompletedMaintenanceDistributionDto
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }


        /// <summary>
        /// 设备类型名称
        /// </summary>
        public string DeviceTypeName { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 保养频率
        /// </summary>
        public string MaintenanceFrequency { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 完成数量
        /// </summary>
        public int CompletedCount { get; set; }

        /// <summary>
        /// 待完成数量
        /// </summary>
        public int PendingCount { get; set; }
    }

    /// <summary>
    /// 保养记录明细DTO
    /// </summary>
    public class MaintenanceRecordDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 任务编号（格式：设备编码_保养等级_保养频率_年月日）
        /// </summary>
        public string TaskNo { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备类型名称
        /// </summary>
        public string DeviceTypeName { get; set; }

        /// <summary>
        /// 保养结果
        /// </summary>
        public string MaintenanceResult { get; set; }

        /// <summary>
        /// 保养时间
        /// </summary>
        public DateTime? MaintenanceTime { get; set; }
        
        /// <summary>
        /// 执行人
        /// </summary>
        public string ExecutorName { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }


        /// <summary>
        /// 设备状态（如：正常、异常、维修中等）
        /// </summary>
        public string DeviceStatus { get; set; }
    }
}
