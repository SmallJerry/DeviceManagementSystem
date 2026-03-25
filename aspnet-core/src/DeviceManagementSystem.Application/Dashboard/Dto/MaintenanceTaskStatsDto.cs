using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Dashboard.Dto
{
    /// <summary>
    /// 保养任务统计DTO
    /// </summary>
    public class MaintenanceTaskStatsDto
    {
        /// <summary>
        /// 任务总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 完成率（百分比）
        /// </summary>
        public int CompletedRate { get; set; }

        /// <summary>
        /// 待执行数量
        /// </summary>
        public int PendingCount { get; set; }

        /// <summary>
        /// 执行中数量
        /// </summary>
        public int ExecutingCount { get; set; }

        /// <summary>
        /// 逾期数量
        /// </summary>
        public int OverdueCount { get; set; }

        /// <summary>
        /// 未执行数量（计划状态）
        /// </summary>
        public int NotExecutedCount { get; set; }

        /// <summary>
        /// 已完成数量
        /// </summary>
        public int CompletedCount { get; set; }

        /// <summary>
        /// 我的待办数量
        /// </summary>
        public int MyTodoCount { get; set; }

        /// <summary>
        /// 统计开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 统计结束日期
        /// </summary>
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// 计划管理统计DTO
    /// </summary>
    public class PlanStatsDto
    {
        /// <summary>
        /// 计划总数
        /// </summary>
        public int TotalPlans { get; set; }

        /// <summary>
        /// 启用计划数
        /// </summary>
        public int ActivePlans { get; set; }

        /// <summary>
        /// 逾期计划数
        /// </summary>
        public int ExpiredPlans { get; set; }

        /// <summary>
        /// 待执行工单数
        /// </summary>
        public int PendingTasks { get; set; }

        /// <summary>
        /// 执行中工单数
        /// </summary>
        public int ExecutingTasks { get; set; }
    }

    /// <summary>
    /// 任务状态分布DTO
    /// </summary>
    public class TaskStatusDistributionDto
    {
        /// <summary>
        /// 状态名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 状态键
        /// </summary>
        public string Key { get; set; }
    }
}
