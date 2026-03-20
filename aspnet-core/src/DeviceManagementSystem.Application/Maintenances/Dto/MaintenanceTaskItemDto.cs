using DeviceManagementSystem.Attachment.Dto;
using DeviceManagementSystem.Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances.Dto
{
    #region 保养工单相关DTO

    /// <summary>
    /// 保养工单DTO
    /// </summary>
    public class MaintenanceTaskDto
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
        /// 工单名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 整合任务ID
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// 整合组名称
        /// </summary>
        public string GroupName { get; set; }

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
        /// 设备位置
        /// </summary>
        public string DeviceLocation { get; set; }



        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceTypeName { get; set; }

        /// <summary>
        /// 保养计划ID
        /// </summary>
        public Guid PlanId { get; set; }

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
        /// 工单状态
        /// </summary>
        public string Status { get; set; }



        /// <summary>附件列表</summary>
        public List<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();

        /// <summary>
        /// 计划开始日期
        /// </summary>
        public DateTime PlanStartDate { get; set; }

        /// <summary>
        /// 计划完成日期
        /// </summary>
        public DateTime PlanEndDate { get; set; }

        /// <summary>
        /// 提醒日期
        /// </summary>
        public DateTime? RemindDate { get; set; }

        /// <summary>
        /// 实际开始时间
        /// </summary>
        public DateTime? ActualStartTime { get; set; }

        /// <summary>
        /// 实际完成时间
        /// </summary>
        public DateTime? ActualEndTime { get; set; }

        /// <summary>
        /// 执行人ID列表
        /// </summary>
        public List<long> ExecutorIds { get; set; } = new List<long>();

        /// <summary>
        /// 执行人姓名列表
        /// </summary>
        public List<string> ExecutorNames { get; set; } = new List<string>();

        /// <summary>
        /// 原执行人ID（委派前）
        /// </summary>
        public string OriginalExecutorIds { get; set; }

        /// <summary>
        /// 委派人ID
        /// </summary>
        public long? DelegatorId { get; set; }

        /// <summary>
        /// 委派人姓名
        /// </summary>
        public string DelegatorName { get; set; }

        /// <summary>
        /// 委派原因
        /// </summary>
        public string DelegateReason { get; set; }

        /// <summary>
        /// 执行小结
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 完成备注
        /// </summary>
        public string CompletionRemark { get; set; }

        /// <summary>
        /// 创建方式
        /// </summary>
        public string CreateType { get; set; }

        /// <summary>
        /// 保养项目列表
        /// </summary>
        public List<MaintenanceTaskItemDto> Items { get; set; } = new List<MaintenanceTaskItemDto>();

        /// <summary>
        /// 附件列表
        /// </summary>
        public List<Guid> AttachmentIds { get; set; } = new List<Guid>();


        /// <summary>
        /// 是否为合并工单
        /// </summary>
        public bool IsMergedTask { get; set; } = false;

        /// <summary>
        /// 合并的计划ID列表
        /// </summary>
        public string MergedPlanIds { get; set; }


        /// <summary>
        /// 剩余天数
        /// </summary>
        public int RemainingDays { get; set; }

        /// <summary>
        /// 是否逾期
        /// </summary>
        public bool IsOverdue { get; set; }

        /// <summary>
        /// 是否可执行
        /// </summary>
        public bool CanExecute => Status == "待执行" || Status == "计划";

        /// <summary>
        /// 是否可委派
        /// </summary>
        public bool CanDelegate => Status == "待执行" || Status == "计划";

        /// <summary>
        /// 是否可取消
        /// </summary>
        public bool CanCancel => Status != "已完成" && Status != "已取消";

        /// <summary>
        /// 获取状态颜色
        /// </summary>
        public string StatusColor => Status switch
        {
            "计划" => "yellow",
            "待执行" => "orange",
            "执行中" => "blue",
            "已完成" => "green",
            "已取消" => "red",
            "逾期" => "red",
            _ => "default"
        };
    }

    /// <summary>
    /// 保养工单项目DTO
    /// </summary>
    public class MaintenanceTaskItemDto
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid? TaskId { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 保养方式
        /// </summary>
        public string MaintenanceMethod { get; set; }

        /// <summary>
        /// 保养内容及要求
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 标准值/参考值
        /// </summary>
        public string StandardValue { get; set; }

        /// <summary>
        /// 执行结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 实际测量值
        /// </summary>
        public string ActualValue { get; set; }

        /// <summary>
        /// 执行备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 结果颜色
        /// </summary>
        public string ResultColor
        {
            get
            {
                return Result switch
                {
                    "合格" => "success",
                    "不合格" => "error",
                    "未执行" => "default",
                    _ => "default"
                };
            }
        }



        /// <summary>
        /// 来源计划ID
        /// </summary>
        public Guid? SourcePlanId { get; set; }

        /// <summary>
        /// 来源保养等级（前端用于区分显示）
        /// </summary>
        public string SourceMaintenanceLevel { get; set; }
    }


    /// <summary>
    /// 待办工单DTO（简化版）
    /// </summary>
    public class PendingTaskDto
    {
        /// <summary>
        /// 保养工单
        /// </summary>
        public MaintenanceTaskDto Task { get; set; }
        
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 计划开始时间
        /// </summary>
        public DateTime PlanStartDate { get; set; }
    }


    /// <summary>
    /// 保存工单执行进度输入（不改变状态）
    /// </summary>
    public class SaveMaintenanceTaskExecutionInput
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 执行小结
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 完成备注
        /// </summary>
        public string CompletionRemark { get; set; }

        /// <summary>
        /// 附件ID列表（执行图片等）
        /// </summary>
        public List<Guid> AttachmentIds { get; set; }

        /// <summary>
        /// 项目执行结果
        /// </summary>
        public List<TaskItemExecuteInput> Items { get; set; }
    }


    /// <summary>
    /// 完成保养工单输入（带结束时间）
    /// </summary>
    public class CompleteMaintenanceTaskInput
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 执行小结
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 完成备注
        /// </summary>
        public string CompletionRemark { get; set; }

        /// <summary>
        /// 附件ID列表（执行图片等）
        /// </summary>
        public List<Guid> AttachmentIds { get; set; }

        /// <summary>
        /// 项目执行结果
        /// </summary>
        public List<TaskItemExecuteInput> Items { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }




    /// <summary>
    /// 保养工单执行输入
    /// </summary>
    public class ExecuteMaintenanceTaskInput
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 执行小结
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 完成备注
        /// </summary>
        public string CompletionRemark { get; set; }

        /// <summary>
        /// 附件ID列表（执行图片等）
        /// </summary>
        public List<Guid> AttachmentIds { get; set; }

        /// <summary>
        /// 项目执行结果
        /// </summary>
        public List<TaskItemExecuteInput> Items { get; set; }
    }

    /// <summary>
    /// 项目执行输入
    /// </summary>
    public class TaskItemExecuteInput
    {
        /// <summary>
        /// 记录ID（编辑时传入）
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// 执行结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 实际测量值
        /// </summary>
        public string ActualValue { get; set; }

        /// <summary>
        /// 执行备注
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 保养工单委派输入
    /// </summary>
    public class DelegateMaintenanceTaskInput
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 新执行人ID列表
        /// </summary>
        public List<long> NewExecutorIds { get; set; }

        /// <summary>
        /// 委派原因
        /// </summary>
        public string Reason { get; set; }
    }

    /// <summary>
    /// 保养工单分页查询输入
    /// </summary>
    public class MaintenanceTaskPageInput : PageRequest
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid? DeviceId { get; set; }


        /// <summary>
        /// 设备类型ID
        /// </summary>
        public Guid? DeviceTypeId { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 工单状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 开始日期范围-开始
        /// </summary>
        public DateTime? StartDateBegin { get; set; }

        /// <summary>
        /// 开始日期范围-结束
        /// </summary>
        public DateTime? StartDateEnd { get; set; }

        /// <summary>
        /// 执行人ID
        /// </summary>
        public long? ExecutorId { get; set; }

        /// <summary>
        /// 是否仅看我的待办
        /// </summary>
        public bool? OnlyMyPending { get; set; }
    }

    /// <summary>
    /// 待办任务整合组DTO
    /// </summary>
    public class PendingTaskGroupDto
    {
        /// <summary>
        /// 组ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 组编号
        /// </summary>
        public string GroupNo { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 计划开始日期
        /// </summary>
        public DateTime PlanStartDate { get; set; }

        /// <summary>
        /// 计划完成日期
        /// </summary>
        public DateTime PlanEndDate { get; set; }

        /// <summary>
        /// 设备数量
        /// </summary>
        public int DeviceCount { get; set; }

        /// <summary>
        /// 工单列表
        /// </summary>
        public List<MaintenanceTaskDto> Tasks { get; set; }
    }

    #endregion

}
