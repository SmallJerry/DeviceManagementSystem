using System;
using System.Collections.Generic;

namespace DeviceManagementSystem.Repairs.Dto
{
    #region 维修验收相关DTO

    /// <summary>
    /// 验收标准项DTO
    /// </summary>
    public class AcceptanceCriteriaItemDto
    {
        /// <summary>
        /// 验收标准名称
        /// </summary>
        public string CriteriaName { get; set; }

        /// <summary>
        /// 验收结论：0-正常，1-不正常
        /// </summary>
        public int Conclusion { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 维修验收输入DTO
    /// </summary>
    public class RepairAcceptanceInput
    {
        /// <summary>
        /// 维修工单ID
        /// </summary>
        public Guid RepairTaskId { get; set; }

        /// <summary>
        /// 验收依据列表（JSON格式）
        /// </summary>
        public List<AcceptanceCriteriaItemDto> AcceptanceCriteria { get; set; }

        /// <summary>
        /// 维修前参数（JSON格式）
        /// </summary>
        public string BeforeRepairParams { get; set; }

        /// <summary>
        /// 维修后参数（JSON格式）
        /// </summary>
        public string AfterRepairParams { get; set; }

        /// <summary>
        /// 验收结论：0-正常，1-不正常
        /// </summary>
        public int AcceptanceConclusion { get; set; }

        /// <summary>
        /// 验收意见/预防措施
        /// </summary>
        public string AcceptanceOpinion { get; set; }
    }

    /// <summary>
    /// 维修验收输出DTO
    /// </summary>
    public class RepairAcceptanceDto
    {
        /// <summary>
        /// 验收ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 维修工单ID
        /// </summary>
        public Guid RepairTaskId { get; set; }

        /// <summary>
        /// 维修申报ID
        /// </summary>
        public Guid RepairRequestId { get; set; }

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
        /// 验收依据列表
        /// </summary>
        public List<AcceptanceCriteriaItemDto> AcceptanceCriteria { get; set; }

        /// <summary>
        /// 维修前参数
        /// </summary>
        public string BeforeRepairParams { get; set; }

        /// <summary>
        /// 维修后参数
        /// </summary>
        public string AfterRepairParams { get; set; }

        /// <summary>
        /// 验收结论：0-正常，1-不正常
        /// </summary>
        public int AcceptanceConclusion { get; set; }

        /// <summary>
        /// 验收结论文本
        /// </summary>
        public string AcceptanceConclusionText { get; set; }

        /// <summary>
        /// 验收意见/预防措施
        /// </summary>
        public string AcceptanceOpinion { get; set; }

        /// <summary>
        /// 验收人ID
        /// </summary>
        public long AcceptorId { get; set; }

        /// <summary>
        /// 验收人姓名
        /// </summary>
        public string AcceptorName { get; set; }

        /// <summary>
        /// 验收时间
        /// </summary>
        public DateTime AcceptanceTime { get; set; }
    }

    #endregion
}