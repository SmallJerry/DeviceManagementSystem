// MyTaskStatisticsDto.cs
using System;
using System.Collections.Generic;

namespace DeviceManagementSystem.Maintenances.Dto
{
    /// <summary>
    /// 我的工单统计数据DTO
    /// </summary>
    public class MyTaskStatisticsDto
    {
        /// <summary>
        /// 全部待办数量
        /// </summary>
        public int AllCount { get; set; }

        /// <summary>
        /// 待执行数量
        /// </summary>
        public int PendingCount { get; set; }

        /// <summary>
        /// 计划数量
        /// </summary>
        public int PlanCount { get; set; }

        /// <summary>
        /// 执行中数量
        /// </summary>
        public int ExecutingCount { get; set; }

        /// <summary>
        /// 已完成数量
        /// </summary>
        public int CompletedCount { get; set; }

        /// <summary>
        /// 逾期数量
        /// </summary>
        public int OverdueCount { get; set; }

        /// <summary>
        /// 工单列表
        /// </summary>
        public List<MaintenanceTaskDto> Tasks { get; set; }
    }
}