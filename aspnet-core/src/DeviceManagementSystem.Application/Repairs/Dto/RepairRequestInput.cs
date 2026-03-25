using DeviceManagementSystem.Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Repairs.Dto
{
    /// <summary>
    /// 维修申报输入DTO
    /// </summary>
    public class RepairRequestInput
    {
        /// <summary>
        /// 申报ID（编辑时传入）
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 维修类型：0-维修，1-升级
        /// </summary>
        public int RepairType { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 故障发现时间
        /// </summary>
        public DateTime FaultFoundTime { get; set; }

        /// <summary>
        /// 故障处理等级：0-一般，1-紧急，2-特急
        /// </summary>
        public int FaultLevel { get; set; }

        /// <summary>
        /// 期望完成时间（必填）
        /// </summary>
        public DateTime ExpectedCompleteTime { get; set; }

        /// <summary>
        /// 故障现象描述
        /// </summary>
        public string FaultDescription { get; set; }

        /// <summary>
        /// 故障图片附件ID列表
        /// </summary>
        public List<Guid> FaultImageIds { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 维修申报输出DTO
    /// </summary>
    public class RepairRequestDto
    {
        /// <summary>
        /// 申报ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 申报编号
        /// </summary>
        public string RequestNo { get; set; }

        /// <summary>
        /// 维修类型：0-维修，1-升级
        /// </summary>
        public int RepairType { get; set; }

        /// <summary>
        /// 维修类型文本
        /// </summary>
        public string RepairTypeText { get; set; }

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
        /// 报修人ID
        /// </summary>
        public long RequesterId { get; set; }

        /// <summary>
        /// 报修人姓名
        /// </summary>
        public string RequesterName { get; set; }

        /// <summary>
        /// 故障发现时间
        /// </summary>
        public DateTime FaultFoundTime { get; set; }

        /// <summary>
        /// 故障处理等级：0-一般，1-紧急，2-特急
        /// </summary>
        public int FaultLevel { get; set; }

        /// <summary>
        /// 故障处理等级文本
        /// </summary>
        public string FaultLevelText { get; set; }

        /// <summary>
        /// 期望完成时间
        /// </summary>
        public DateTime ExpectedCompleteTime { get; set; }

        /// <summary>
        /// 故障现象描述
        /// </summary>
        public string FaultDescription { get; set; }

        /// <summary>
        /// 故障图片附件列表
        /// </summary>
        public List<AttachmentInfoDto> FaultImages { get; set; }

        /// <summary>
        /// 申报状态：0-待派单，1-已派单，2-维修中，3-已完成，4-已取消
        /// </summary>
        public int RequestStatus { get; set; }

        /// <summary>
        /// 申报状态文本
        /// </summary>
        public string RequestStatusText { get; set; }

        /// <summary>
        /// 关联的维修工单ID
        /// </summary>
        public Guid? RepairTaskId { get; set; }

        /// <summary>
        /// 维修工单编号
        /// </summary>
        public string RepairTaskNo { get; set; }

        /// <summary>
        /// 维修人员列表
        /// </summary>
        public List<RepairerInfoDto> Repairers { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 维修申报分页查询输入
    /// </summary>
    public class RepairRequestPageInput : PageRequest
    {
        /// <summary>
        /// 搜索关键字（申报编号、设备名称）
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 维修类型筛选
        /// </summary>
        public int? RepairType { get; set; }

        /// <summary>
        /// 申报状态筛选
        /// </summary>
        public int? RequestStatus { get; set; }

        /// <summary>
        /// 设备ID筛选
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// 故障发现时间开始
        /// </summary>
        public DateTime? FaultFoundTimeBegin { get; set; }

        /// <summary>
        /// 故障发现时间结束
        /// </summary>
        public DateTime? FaultFoundTimeEnd { get; set; }
    }
}
