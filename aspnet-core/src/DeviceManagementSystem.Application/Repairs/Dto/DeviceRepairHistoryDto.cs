using System;
using System.Collections.Generic;

namespace DeviceManagementSystem.Repairs.Dto
{
    #region 维修历史相关DTO

    /// <summary>
    /// 设备维修历史DTO
    /// </summary>
    public class DeviceRepairHistoryDto
    {
        /// <summary>
        /// 维修工单ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 工单编号
        /// </summary>
        public string TaskNo { get; set; }

        /// <summary>
        /// 维修类型
        /// </summary>
        public int RepairType { get; set; }

        /// <summary>
        /// 维修类型文本
        /// </summary>
        public string RepairTypeText { get; set; }

        /// <summary>
        /// 故障发现时间
        /// </summary>
        public DateTime FaultFoundTime { get; set; }

        /// <summary>
        /// 故障处理等级
        /// </summary>
        public int FaultLevel { get; set; }

        /// <summary>
        /// 故障处理等级文本
        /// </summary>
        public string FaultLevelText { get; set; }

        /// <summary>
        /// 故障现象描述
        /// </summary>
        public string FaultDescription { get; set; }

        /// <summary>
        /// 故障原因
        /// </summary>
        public string FaultReason { get; set; }

        /// <summary>
        /// 维修方法及结果
        /// </summary>
        public string RepairMethodResult { get; set; }

        /// <summary>
        /// 维修开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 维修结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 维修时长（分钟）
        /// </summary>
        public int? DurationMinutes { get; set; }

        /// <summary>
        /// 维修人员姓名列表
        /// </summary>
        public List<string> RepairerNames { get; set; }

        /// <summary>
        /// 工单状态
        /// </summary>
        public int TaskStatus { get; set; }

        /// <summary>
        /// 工单状态文本
        /// </summary>
        public string TaskStatusText { get; set; }

        /// <summary>
        /// 验收结论
        /// </summary>
        public int? AcceptanceConclusion { get; set; }

        /// <summary>
        /// 验收结论文本
        /// </summary>
        public string AcceptanceConclusionText { get; set; }

        /// <summary>
        /// 验收时间
        /// </summary>
        public DateTime? AcceptanceTime { get; set; }

        /// <summary>
        /// 验收人姓名
        /// </summary>
        public string AcceptorName { get; set; }
    }

    /// <summary>
    /// 故障现象历史选项DTO
    /// </summary>
    public class FaultDescriptionOptionDto
    {
        /// <summary>
        /// 故障现象描述
        /// </summary>
        public string FaultDescription { get; set; }

        /// <summary>
        /// 出现次数
        /// </summary>
        public int OccurrenceCount { get; set; }

        /// <summary>
        /// 最后出现时间
        /// </summary>
        public DateTime LastOccurrenceTime { get; set; }
    }

    /// <summary>
    /// 故障原因历史选项DTO
    /// </summary>
    public class FaultReasonOptionDto
    {
        /// <summary>
        /// 故障原因
        /// </summary>
        public string FaultReason { get; set; }

        /// <summary>
        /// 出现次数
        /// </summary>
        public int OccurrenceCount { get; set; }

        /// <summary>
        /// 最后出现时间
        /// </summary>
        public DateTime LastOccurrenceTime { get; set; }
    }

    /// <summary>
    /// 维修方法历史选项DTO
    /// </summary>
    public class RepairMethodOptionDto
    {
        /// <summary>
        /// 维修方法及结果
        /// </summary>
        public string RepairMethodResult { get; set; }

        /// <summary>
        /// 使用次数
        /// </summary>
        public int UsageCount { get; set; }

        /// <summary>
        /// 最后使用时间
        /// </summary>
        public DateTime LastUsageTime { get; set; }
    }

    /// <summary>
    /// 设备维修统计DTO
    /// </summary>
    public class DeviceRepairStatisticsDto
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 总维修次数
        /// </summary>
        public int TotalRepairCount { get; set; }

        /// <summary>
        /// 维修类型分布
        /// </summary>
        public Dictionary<int, int> RepairTypeDistribution { get; set; }

        /// <summary>
        /// 故障等级分布
        /// </summary>
        public Dictionary<int, int> FaultLevelDistribution { get; set; }

        /// <summary>
        /// 平均维修时长（分钟）
        /// </summary>
        public double AvgRepairDuration { get; set; }

        /// <summary>
        /// 超时次数
        /// </summary>
        public int OverdueCount { get; set; }

        /// <summary>
        /// 最近维修时间
        /// </summary>
        public DateTime? LastRepairTime { get; set; }
    }

    #endregion
}