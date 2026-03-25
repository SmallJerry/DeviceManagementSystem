using DeviceManagementSystem.Utils.Common;
using System;
using System.Collections.Generic;

namespace DeviceManagementSystem.Repairs.Dto
{
    #region 维修工单相关DTO

    /// <summary>
    /// 维修工单输入DTO
    /// </summary>
    public class RepairTaskInput
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 维修人员ID列表
        /// </summary>
        public List<long> RepairerIds { get; set; }
    }

    /// <summary>
    /// 维修工单输出DTO
    /// </summary>
    public class RepairTaskDto
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 工单编号
        /// </summary>
        public string TaskNo { get; set; }

        /// <summary>
        /// 关联的维修申报ID
        /// </summary>
        public Guid RepairRequestId { get; set; }

        /// <summary>
        /// 申报编号
        /// </summary>
        public string RequestNo { get; set; }

        /// <summary>
        /// 申报人ID
        /// </summary>
        public long RequesterId { get; set; }

        /// <summary>
        /// 申报人姓名
        /// </summary>
        public string RequesterName { get; set; }

        /// <summary>
        /// 报修时间
        /// </summary>
        public DateTime RequestTime { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid? DeviceTypeId { get; set; }

        /// <summary>
        /// 设备类型名称
        /// </summary>
        public string DeviceTypeName { get; set; }

        /// <summary>
        /// 维修人员ID列表
        /// </summary>
        public List<long> RepairerIds { get; set; }

        /// <summary>
        /// 维修人员姓名列表
        /// </summary>
        public List<string> RepairerNames { get; set; }

        /// <summary>
        /// 接单时间
        /// </summary>
        public DateTime? AcceptTime { get; set; }

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
        /// 工单状态：0-待接单，1-维修中，2-待验收，3-已完成，4-已取消
        /// </summary>
        public int TaskStatus { get; set; }

        /// <summary>
        /// 工单状态文本
        /// </summary>
        public string TaskStatusText { get; set; }

        /// <summary>
        /// 超时标记：0-未超时，1-已超时
        /// </summary>
        public int IsOverdue { get; set; }

        /// <summary>
        /// 故障原因
        /// </summary>
        public string FaultReason { get; set; }

        /// <summary>
        /// 对产品品质影响分析及处理方案
        /// </summary>
        public string QualityImpactAnalysis { get; set; }

        /// <summary>
        /// 维修方法及结果
        /// </summary>
        public string RepairMethodResult { get; set; }

        /// <summary>
        /// 是否通知保养
        /// </summary>
        public int NotifyMaintenance { get; set; }

        /// <summary>
        /// 保养计划ID
        /// </summary>
        public Guid? MaintenancePlanId { get; set; }

        /// <summary>
        /// 保养计划名称
        /// </summary>
        public string MaintenancePlanName { get; set; }

        /// <summary>
        /// 保养工单ID
        /// </summary>
        public Guid? MaintenanceTaskId { get; set; }

        /// <summary>
        /// 保养工单编号
        /// </summary>
        public string MaintenanceTaskNo { get; set; }

        /// <summary>
        /// 是否已通知验收
        /// </summary>
        public int NotifiedAcceptance { get; set; }

        /// <summary>
        /// 验收人ID
        /// </summary>
        public long? AcceptorId { get; set; }

        /// <summary>
        /// 验收人姓名
        /// </summary>
        public string AcceptorName { get; set; }

        /// <summary>
        /// 验收时间
        /// </summary>
        public DateTime? AcceptanceTime { get; set; }

        /// <summary>
        /// 维修申报信息
        /// </summary>
        public RepairRequestDto RepairRequest { get; set; }

        /// <summary>
        /// 验收记录
        /// </summary>
        public RepairAcceptanceDto Acceptance { get; set; }
    }

    /// <summary>
    /// 维修工单分页查询输入
    /// </summary>
    public class RepairTaskPageInput : PageRequest
    {
        /// <summary>
        /// 搜索关键字（工单编号、设备名称）
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 工单状态筛选
        /// </summary>
        public int? TaskStatus { get; set; }

        /// <summary>
        /// 设备ID筛选
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// 维修人员ID筛选
        /// </summary>
        public long? RepairerId { get; set; }
    }

    /// <summary>
    /// 维修工单执行记录输入DTO（用于保存草稿）
    /// </summary>
    public class RepairTaskExecutionInput
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 故障原因
        /// </summary>
        public string FaultReason { get; set; }

        /// <summary>
        /// 对产品品质影响分析及处理方案
        /// </summary>
        public string QualityImpactAnalysis { get; set; }

        /// <summary>
        /// 维修方法及结果
        /// </summary>
        public string RepairMethodResult { get; set; }

        /// <summary>
        /// 保存类型：0-草稿，1-正式提交
        /// </summary>
        public int SaveType { get; set; } = 0;
    }

    /// <summary>
    /// 维修工单接单输入
    /// </summary>
    public class AcceptRepairTaskInput
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 接单时间（可选，默认当前时间）
        /// </summary>
        public DateTime? AcceptTime { get; set; }
    }

    /// <summary>
    /// 维修工单开始执行输入
    /// </summary>
    public class StartRepairTaskInput
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 开始时间（可选，默认当前时间）
        /// </summary>
        public DateTime? StartTime { get; set; }
    }

    /// <summary>
    /// 维修工单完成输入
    /// </summary>
    public class CompleteRepairTaskInput
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 故障原因
        /// </summary>
        public string FaultReason { get; set; }

        /// <summary>
        /// 对产品品质影响分析及处理方案
        /// </summary>
        public string QualityImpactAnalysis { get; set; }

        /// <summary>
        /// 维修方法及结果
        /// </summary>
        public string RepairMethodResult { get; set; }

        /// <summary>
        /// 结束时间（可选，默认当前时间）
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 是否通知保养
        /// </summary>
        public int NotifyMaintenance { get; set; }

        /// <summary>
        /// 保养计划ID（当NotifyMaintenance=1时必填）
        /// </summary>
        public Guid? MaintenancePlanId { get; set; }
    }

    #endregion
}