using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances.Dto
{
    #region 提醒相关DTO

    /// <summary>
    /// 待提醒任务DTO（用于邮件提醒）
    /// </summary>
    public class PendingRemindTaskDto
    {
        /// <summary>
        /// 工单ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 工单编号
        /// </summary>
        public string TaskNo { get; set; }

        /// <summary>
        /// 工单名称
        /// </summary>
        public string TaskName { get; set; }

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
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 保养等级显示文本
        /// </summary>
        public string MaintenanceLevelText { get; set; }

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
        public DateTime RemindDate { get; set; }

        /// <summary>
        /// 执行人ID列表（逗号分隔）
        /// </summary>
        public string ExecutorIds { get; set; }

        /// <summary>
        /// 执行人姓名列表（逗号分隔）
        /// </summary>
        public string ExecutorNames { get; set; }

        /// <summary>
        /// 是否为合并工单
        /// </summary>
        public bool IsMergedTask { get; set; }

        /// <summary>
        /// 合并的计划ID列表
        /// </summary>
        public string MergedPlanIds { get; set; }

        /// <summary>
        /// 剩余天数（计算属性）
        /// </summary>
        public int RemainingDays => (PlanStartDate.Date - DateTime.Today).Days;

        /// <summary>
        /// 逾期天数（如果已逾期）
        /// </summary>
        public int OverdueDays => DateTime.Today > PlanEndDate.Date ? (DateTime.Today - PlanEndDate.Date).Days : 0;

        /// <summary>
        /// 是否已逾期
        /// </summary>
        public bool IsOverdue => DateTime.Today > PlanEndDate.Date;

        /// <summary>
        /// 工单状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 工单状态颜色
        /// </summary>
        public string StatusColor { get; set; }

        /// <summary>
        /// 创建方式（自动/手动）
        /// </summary>
        public string CreateType { get; set; }

        /// <summary>
        /// 获取执行人ID列表
        /// </summary>
        public List<long> GetExecutorIdList()
        {
            if (string.IsNullOrEmpty(ExecutorIds))
                return new List<long>();

            return ExecutorIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => long.Parse(x))
                .ToList();
        }

        /// <summary>
        /// 获取执行人姓名列表
        /// </summary>
        public List<string> GetExecutorNameList()
        {
            if (string.IsNullOrEmpty(ExecutorNames))
                return new List<string>();

            return ExecutorNames.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
    #endregion
}
