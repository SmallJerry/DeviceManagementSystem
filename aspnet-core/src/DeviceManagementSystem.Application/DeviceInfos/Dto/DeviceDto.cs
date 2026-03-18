using DeviceManagementSystem.Attachment.Dto;
using DeviceManagementSystem.Maintenances.Dto;
using DeviceManagementSystem.WorkFlows.FlowInstance.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeviceManagementSystem.DeviceInfos.Dto
{
    /// <summary>
    /// 设备DTO
    /// </summary>
    public class DeviceDto
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 设备编码（管理编号）- 前端输入，必填
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 设备二维码
        /// </summary>
        public string QrCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 设备等级 (如: A级, B级, C级)
        /// </summary>
        public string DeviceLevel { get; set; }

        /// <summary>
        /// 是否为重点设备
        /// </summary>
        public bool IsKeyDevice { get; set; }    

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid? TypeId { get; set; }

        /// <summary>
        /// 设备类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 具体规格参数列表
        /// </summary>
        public List<TechnicalParameterItem> TechnicalParameters { get; set; }

        /// <summary>
        /// 客户要求指标列表
        /// </summary>
        public List<CustomerRequirementItem> CustomerRequirements { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string LogisticsNo { get; set; }

        /// <summary>
        /// 出厂编号
        /// </summary>
        public string FactoryNo { get; set; }

        /// <summary>
        /// 生产厂商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ManufactureDate { get; set; }

        /// <summary>
        /// 申购单号
        /// </summary>
        public string PurchaseNo { get; set; }

        /// <summary>
        /// 来源方式（采购/自制）
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 存放地点
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 设备状态（未验收/使用中/故障/闲置/报废等）
        /// </summary>
        public string DeviceStatus { get; set; }

        /// <summary>
        /// 启用日期
        /// </summary>
        public DateTime? EnableDate { get; set; }

        /// <summary>
        /// 业务状态（草稿、审核中、已确认）
        /// </summary>
        public string BusinessStatus { get; set; }


        /// <summary>
        /// 变更类型（新增/编辑/删除）- 仅在变更申请详情中使用，表示当前数据是新增、编辑还是删除的内容
        /// </summary>
        public string ChangeType { get; set; } // 变更类型（新增/编辑/删除）

        /// <summary>
        /// 关联的供应商ID
        /// </summary>
        public Guid? SupplierId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 工厂节点ID（单选）
        /// </summary>
        public Guid? FactoryNodeId { get; set; }

        /// <summary>
        /// 工厂节点名称
        /// </summary>
        public string FactoryNodeName { get; set; }

        /// <summary>
        /// 工厂节点完整路径
        /// </summary>
        public string FactoryNodeFullPath { get; set; }

        /// <summary>
        /// 维修人员列表
        /// </summary>
        public List<UserInfo> MaintainUsers { get; set; }

        /// <summary>
        /// 保养人员列表
        /// </summary>
        public List<UserInfo> MaintenanceUsers { get; set; }

        /// <summary>
        /// 技术资料附件列表（按分类分组）
        /// </summary>
        public Dictionary<string, List<AttachmentInfo>> TechnicalAttachments { get; set; }

        /// <summary>
        /// 变更历史列表
        /// </summary>
        public List<DeviceChangeHistoryDto> ChangeHistory { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }


        /// <summary>
        /// 变更申请ID（用于草稿和已提交列表）
        /// </summary>
        public Guid? ChangeApplyId { get; set; }

        /// <summary>
        /// 流程实例ID
        /// </summary>
        public Guid? FlowInstanceId { get; set; }

        /// <summary>
        /// 申请原因
        /// </summary>
        public string ApplyReason { get; set; }

        /// <summary>
        /// 是否可撤销
        /// </summary>
        public bool Cancelable { get; set; }

        /// <summary>
        /// 是否可以发起删除申请
        /// </summary>
        public bool CanDelete { get; set; }



        /// <summary>
        /// 月度保养计划
        /// </summary>
        public MaintenancePlanDto MonthlyMaintenance { get; set; }

        /// <summary>
        /// 季度保养计划
        /// </summary>
        public MaintenancePlanDto QuarterlyMaintenance { get; set; }

        /// <summary>
        /// 半年度保养计划
        /// </summary>
        public MaintenancePlanDto HalfYearlyMaintenance { get; set; }

        /// <summary>
        /// 年度保养计划
        /// </summary>
        public MaintenancePlanDto AnnualMaintenance { get; set; }
    }



    /// <summary>
    /// 设备简版DTO
    /// </summary>
    public class DeviceSimpleDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
        
        /// <summary>
        /// 设备状态
        /// </summary>
        public string DeviceStatus { get; set; }

    }


    ///// <summary>
    ///// 保养计划DTO
    ///// </summary>
    //public class MaintenancePlanDto
    //{
    //    /// <summary>
    //    /// 计划ID
    //    /// </summary>
    //    public Guid? PlanId { get; set; }

    //    /// <summary>
    //    /// 模板ID
    //    /// </summary>
    //    public Guid? TemplateId { get; set; }

    //    /// <summary>
    //    /// 模板名称
    //    /// </summary>
    //    public string TemplateName { get; set; }

    //    /// <summary>
    //    /// 保养等级
    //    /// </summary>
    //    public string MaintenanceLevel { get; set; }

    //    /// <summary>
    //    /// 计划状态
    //    /// </summary>
    //    public string Status { get; set; }

    //    /// <summary>
    //    /// 下次保养日期
    //    /// </summary>
    //    public DateTime? NextMaintenanceDate { get; set; }
    //}


    /// <summary>
    /// 设备变更历史DTO
    /// </summary>
    public class DeviceChangeHistoryDto
    {
        /// <summary>
        /// 变更申请ID
        /// </summary>
        public Guid ChangeApplyId { get; set; }

        /// <summary>
        /// 变更类型（新增、编辑、删除）
        /// </summary>
        public string ChangeType { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime SubmitTime { get; set; }

        /// <summary>
        /// 提交人姓名
        /// </summary>
        public string SubmitterName { get; set; }

        /// <summary>
        /// 申请原因
        /// </summary>
        public string ApplyReason { get; set; }


        /// <summary>
        /// 申请状态（草稿、待审核、审核中、已通过、已拒绝、已撤销等）
        /// </summary>
        public string ApplicationStatus { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public string FlowStatus { get; set; }

        /// <summary>
        /// 流程实例ID
        /// </summary>
        public Guid? FlowInstanceId { get; set; }

        /// <summary>
        /// 流程信息
        /// </summary>
        public FlowInfo FlowInfo { get; set; }


        /// <summary>
        /// 变更对比详情（字段级对比）
        /// </summary>
        public List<FieldChangeItem> ChangeDetails { get; set; }

        /// <summary>
        /// 变更前数据快照
        /// </summary>
        public object Snapshot { get; set; }

        /// <summary>
        /// 变更后数据
        /// </summary>
        public object NewData { get; set; }
    }


    /// <summary>
    /// 流程信息
    /// </summary>
    public class FlowInfo
    {
        /// <summary>
        /// 流程定义ID
        /// </summary>
        public Guid FlowDefId { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName { get; set; }

        /// <summary>
        /// 流程状态：0-审批中，1-已通过，2-不通过，3-已撤销
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 流程状态名称
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string CurrentNodeName { get; set; }

        /// <summary>
        /// 流程节点记录
        /// </summary>
        public List<FlowNodeRecordDto> NodeRecords { get; set; }
    }



    /// <summary>
    /// 技术参数项
    /// </summary>
    public class TechnicalParameterItem
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string ParameterValue { get; set; }

        /// <summary>
        /// 参数描述
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 客户要求指标项
    /// </summary>
    public class CustomerRequirementItem
    {
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 要求名称
        /// </summary>
        public string RequirementName { get; set; }

        /// <summary>
        /// 要求值
        /// </summary>
        public string RequirementValue { get; set; }

        /// <summary>
        /// 实际值
        /// </summary>
        public string ActualValue { get; set; }

        /// <summary>
        /// 是否达标
        /// </summary>
        public bool IsQualified { get; set; }
    }

    /// <summary>
    /// 附件信息
    /// </summary>
    public class AttachmentInfo
    {
        /// <summary>
        /// 附件ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 文件大小格式化
        /// </summary>
        public string FileSizeFormat { get; set; }

        /// <summary>
        /// 文件URL
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// 附件分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 上传者
        /// </summary>
        public string Creator { get; set; }
    }


    /// <summary>
    /// 字段变更项
    /// </summary>
    public class FieldChangeItem
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 字段标签（显示名称）
        /// </summary>
        public string FieldLabel { get; set; }

        /// <summary>
        /// 旧值
        /// </summary>
        public object OldValue { get; set; }
        
        /// <summary>
        /// 新值
        /// </summary>
        public object NewValue { get; set; }
    
        /// <summary>
        /// 变更类型
        /// </summary>
        public string ChangeType { get; set; } // add/edit/delete
    }





    /// <summary>
    /// 流程节点信息
    /// </summary>
    public class FlowNodeInfo
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public int NodeType { get; set; }

        /// <summary>
        /// 节点类型名称
        /// </summary>
        public string NodeTypeName { get; set; }

        /// <summary>
        /// 审批人信息
        /// </summary>
        public string AssigneeInfo { get; set; }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }
    }

    /// <summary>
    /// 设备分页查询参数
    /// </summary>
    public class DevicePageInput
    {
        /// <summary>
        /// 搜索关键字（设备名称/编码/规格型号）
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid? TypeId { get; set; }


        /// <summary>
        /// 设备等级 (如: A级, B级, C级)
        /// </summary>
        public string DeviceLevel { get; set; }      // 新增


        /// <summary>
        /// 是否为重点设备
        /// </summary>
        public bool? IsKeyDevice { get; set; }       // 新增

        /// <summary>
        /// 设备状态
        /// </summary>
        public string DeviceStatus { get; set; }

        /// <summary>
        /// 业务状态
        /// </summary>
        public string BusinessStatus { get; set; }

        /// <summary>
        /// 创建时间开始
        /// </summary>
        public DateTime? CreationTimeBegin { get; set; }

        /// <summary>
        /// 创建时间结束
        /// </summary>
        public DateTime? CreationTimeEnd { get; set; }

        /// <summary>
        /// 来源方式（采购/自制）
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 变更类型（新增/编辑/删除）- 仅在变更申请列表中使用，表示查询的变更记录是新增、编辑还是删除的内容
        /// </summary>
        public string ChangeType { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// 排序方式：ASC/DESC
        /// </summary>
        public string SortOrder { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int Current { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public int Size { get; set; } = 10;
    }

    /// <summary>
    /// 设备新增/编辑输入
    /// </summary>
    public class DeviceEditInput
    {
        /// <summary>
        /// 设备ID（编辑时必填）
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 设备编码（前端输入，必填）
        /// </summary>
        [Required(ErrorMessage = "设备编码不能为空")]
        public string DeviceCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [Required(ErrorMessage = "设备名称不能为空")]
        public string DeviceName { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid? TypeId { get; set; }

        /// <summary>
        /// 技术参数模板ID
        /// </summary>
        public Guid? TechnicalParameterTemplateId { get; set; }

        /// <summary>
        /// 具体规格参数列表
        /// </summary>
        public List<TechnicalParameterItem> TechnicalParameters { get; set; }

        /// <summary>
        /// 设备等级 (如: A级, B级, C级)
        /// </summary>
        public string DeviceLevel { get; set; }      // 新增

        /// <summary>
        /// 是否为重点设备
        /// </summary>
        public bool IsKeyDevice { get; set; } 

        /// <summary>
        /// 客户要求指标列表
        /// </summary>
        public List<CustomerRequirementItem> CustomerRequirements { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string LogisticsNo { get; set; }

        /// <summary>
        /// 出厂编号
        /// </summary>
        public string FactoryNo { get; set; }

        /// <summary>
        /// 生产厂商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ManufactureDate { get; set; }

        /// <summary>
        /// 申购单号
        /// </summary>
        public string PurchaseNo { get; set; }

        /// <summary>
        /// 来源方式（采购/自制）
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 存放地点
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 设备状态（未验收/使用中/故障/闲置/报废等）
        /// </summary>
        public string DeviceStatus { get; set; }

        /// <summary>
        /// 启用日期
        /// </summary>
        public DateTime? EnableDate { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public Guid? SupplierId { get; set; }

        /// <summary>
        /// 工厂节点ID（单选）
        /// </summary>
        public Guid? FactoryNodeId { get; set; }

        /// <summary>
        /// 维修人员ID列表
        /// </summary>
        public List<long> MaintainUserIds { get; set; }

        /// <summary>
        /// 保养人员ID列表
        /// </summary>
        public List<long> MaintenanceUserIds { get; set; }

        /// <summary>
        /// 技术资料附件ID列表（按分类）
        /// </summary>
        public List<AttachmentWithCategory> TechnicalAttachmentWithCategories { get; set; }



        /// <summary>
        /// 月度保养计划数据
        /// </summary>
        public MaintenancePlanDto MonthlyMaintenance { get; set; }

        /// <summary>
        /// 季度保养计划数据
        /// </summary>
        public MaintenancePlanDto QuarterlyMaintenance { get; set; }

        /// <summary>
        /// 半年度保养计划数据
        /// </summary>
        public MaintenancePlanDto HalfYearlyMaintenance { get; set; }

        /// <summary>
        /// 年度保养计划数据
        /// </summary>
        public MaintenancePlanDto AnnualMaintenance { get; set; }
    }


    /// <summary>
    /// 保养计划数据
    /// </summary>
    public class MaintenancePlanData
    {
        /// <summary>
        /// 模板id
        /// </summary>
        public Guid TemplateId { get; set; }
        
        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }
        
        /// <summary>
        /// 保养项目列表
        /// </summary>
        public List<MaintenancePlanItemData> Items { get; set; }
    }


    /// <summary>
    /// 保养计划项目数据
    /// </summary>
    public class MaintenancePlanItemData
    {
        /// <summary>
        /// 序号
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
        /// 点检方法列表
        /// </summary>
        public List<string> InspectionMethod { get; set; }
    }


    /// <summary>
    /// 检查设备编码唯一性输入参数
    /// </summary>
    public class CheckDeviceCodeUniqueInput
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 排除的设备ID（编辑时排除自身）
        /// </summary>
        public Guid? ExcludeId { get; set; }

        /// <summary>
        /// 排除的申请ID（草稿编辑时排除自身）
        /// </summary>
        public Guid? ExcludeApplyId { get; set; }
    }



    /// <summary>
    /// 更新草稿输入参数
    /// </summary>
    public class UpdateDraftInput
    {
        /// <summary>
        /// 变更申请ID
        /// </summary>
        [Required(ErrorMessage = "变更申请ID不能为空")]
        public Guid ChangeApplyId { get; set; }

        /// <summary>
        /// 表单数据
        /// </summary>
        [Required(ErrorMessage = "表单数据不能为空")]
        public DeviceEditInput FormData { get; set; }


        /// <summary>
        /// 是否重置为草稿状态（编辑草稿时可选）
        /// </summary>
        public bool? ResetToDraft { get; set; } = false; 
    }

    /// <summary>
    /// 重新申请输入参数（针对已拒绝或已撤销的申请再次提交）
    /// </summary>
    public class ReapplyInput
    {
        /// <summary>
        /// 原申请ID
        /// </summary>
        public Guid ChangeApplyId { get; set; }

        /// <summary>
        /// 表单数据（可选，不传则使用原数据）
        /// </summary>
        public DeviceEditInput FormData { get; set; }

        /// <summary>
        /// 申请原因
        /// </summary>
        public string ApplyReason { get; set; }

        /// <summary>
        /// 是否复制附件
        /// </summary>
        public bool CopyAttachments { get; set; } = true;
    }



    /// <summary>
    /// 删除申请输入参数
    /// </summary>
    public class DeleteApplyInput
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        [Required(ErrorMessage = "设备ID不能为空")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 申请原因
        /// </summary>
        [Required(ErrorMessage = "申请原因不能为空")]
        [StringLength(500, ErrorMessage = "申请原因不能超过500个字符")]
        public string ApplyReason { get; set; }
    }


    /// <summary>
    /// 变更申请详情DTO（增强版）
    /// </summary>
    public class ChangeApplyDetailDto
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public Guid ApplyId { get; set; }

        /// <summary>
        /// 变更类型
        /// </summary>
        public string ChangeType { get; set; }

        /// <summary>
        /// 申请状态
        /// </summary>
        public string ApplicationStatus { get; set; }

        /// <summary>
        /// 申请原因
        /// </summary>
        public string ApplyReason { get; set; }

        /// <summary>
        /// 提交人ID
        /// </summary>
        public long? SubmitterId { get; set; }

        /// <summary>
        /// 提交人姓名
        /// </summary>
        public string SubmitterName { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime? SubmitTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid? DeviceId { get; set; }

        // 设备基本信息
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 设备等级
        /// </summary>
        public string DeviceLevel { get; set; }

        /// <summary>
        /// 是否为重点设备
        /// </summary>
        public bool IsKeyDevice { get; set; }

        /// <summary>
        /// 生产厂商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public DateTime? ManufactureDate { get; set; }

        /// <summary>
        /// 出厂编号
        /// </summary>
        public string FactoryNo { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string LogisticsNo { get; set; }

        /// <summary>
        /// 申购单号
        /// </summary>
        public string PurchaseNo { get; set; }

        /// <summary>
        /// 来源方式
        /// </summary>
        public string SourceType { get; set; }

        /// <summary>
        /// 存放地点
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 设备状态
        /// </summary>
        public string DeviceStatus { get; set; }

        /// <summary>
        /// 启用日期
        /// </summary>
        public DateTime? EnableDate { get; set; }

        // 关联数据ID
        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid? TypeId { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public Guid? SupplierId { get; set; }

        /// <summary>
        /// 工厂节点ID
        /// </summary>
        public Guid? FactoryNodeId { get; set; }

        // 关联数据名称
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 工厂节点完整路径
        /// </summary>
        public string FactoryNodeFullPath { get; set; }

        /// <summary>
        /// 维修人员列表
        /// </summary>
        public List<UserInfo> MaintainUsers { get; set; }

        /// <summary>
        /// 保养人员列表
        /// </summary>
        public List<UserInfo> MaintenanceUsers { get; set; }

        /// <summary>
        /// 技术参数
        /// </summary>
        public List<TechnicalParameterItem> TechnicalParameters { get; set; }

        /// <summary>
        /// 客户要求
        /// </summary>
        public List<CustomerRequirementItem> CustomerRequirements { get; set; }

        /// <summary>
        /// 变更前快照
        /// </summary>
        public DeviceEditInput Snapshot { get; set; }

        /// <summary>
        /// 变更后数据
        /// </summary>
        public DeviceEditInput NewData { get; set; }

        /// <summary>
        /// 字段变更对比
        /// </summary>
        public List<FieldChangeItem> FieldChanges { get; set; }

        /// <summary>
        /// 流程实例ID
        /// </summary>
        public Guid? FlowInstanceId { get; set; }

        /// <summary>
        /// 技术资料附件列表（带分类）
        /// </summary>
        public List<AttachmentWithCategory> TechnicalAttachmentWithCategories { get; set; }

        /// <summary>
        /// 可用操作列表（如：提交、撤销、编辑等）
        /// </summary>
        public List<string> AvailableActions { get; set; }


        /// <summary>
        /// 月度保养计划
        /// </summary>
        public MaintenancePlanData MonthlyMaintenance { get; set; }

        /// <summary>
        /// 季度保养计划
        /// </summary>
        public MaintenancePlanData QuarterlyMaintenance { get; set; }

        /// <summary>
        /// 半年度保养计划
        /// </summary>
        public MaintenancePlanData HalfYearlyMaintenance { get; set; }

        /// <summary>
        /// 年度保养计划
        /// </summary>
        public MaintenancePlanData AnnualMaintenance { get; set; }


    }

    /// <summary>
    /// 提交草稿输入参数
    /// </summary>
    public class SubmitDraftInput
    {
        /// <summary>
        /// 变更申请ID
        /// </summary>
        [Required(ErrorMessage = "变更申请ID不能为空")]
        public Guid ChangeApplyId { get; set; }
    }



    /// <summary>
    /// 撤销申请输入
    /// </summary>
    public class CancelApplyInput
    {
        /// <summary>
        /// 申请Id
        /// </summary>
        [Required(ErrorMessage = "申请ID不能为空")]
        public Guid ChangeApplyId { get; set; }
        
        
        /// <summary>
        /// 申请原因
        /// </summary>
        public string Reason { get; set; }
    }

    /// <summary>
    /// 设备提交申请输入
    /// </summary>
    public class DeviceSubmitInput
    {
        /// <summary>
        /// 变更申请ID（如果是编辑已有草稿）
        /// </summary>
        public Guid? ChangeApplyId { get; set; }

        /// <summary>
        /// 设备ID（如果是编辑已有设备）
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// 变更类型
        /// </summary>
        [Required(ErrorMessage = "变更类型不能为空")]
        public string ChangeType { get; set; }

        /// <summary>
        /// 申请原因（编辑/删除时必填）
        /// </summary>
        public string ApplyReason { get; set; }

        /// <summary>
        /// 表单数据（编辑后的设备数据）
        /// </summary>
        [Required(ErrorMessage = "表单数据不能为空")]
        public DeviceEditInput FormData { get; set; }
    }





    /// <summary>
    /// 设备ID输入
    /// </summary>
    public class DeviceIdInput
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid Id { get; set; }
    }
}