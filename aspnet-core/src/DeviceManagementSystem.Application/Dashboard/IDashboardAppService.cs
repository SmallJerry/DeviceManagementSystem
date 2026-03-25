using DeviceManagementSystem.Dashboard.Dto;
using DeviceManagementSystem.Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Dashboard
{
    /// <summary>
    /// 仪表盘服务接口
    /// </summary>
    public interface IDashboardAppService
    {
        /// <summary>
        /// 获取保养任务统计信息
        /// </summary>
        Task<CommonResult<MaintenanceTaskStatsDto>> GetMaintenanceTaskStats(string timeRange = "today");

        /// <summary>
        /// 获取计划管理统计
        /// </summary>
        Task<CommonResult<PlanStatsDto>> GetPlanStats();

        /// <summary>
        /// 获取任务状态分布
        /// </summary>
        Task<CommonResult<List<TaskStatusDistributionDto>>> GetTaskStatusDistribution(string timeRange = "today");
    }
}
