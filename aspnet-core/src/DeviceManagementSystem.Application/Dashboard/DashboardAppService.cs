using Abp.Auditing;
using Abp.Domain.Repositories;
using DeviceManagementSystem.Dashboard.Dto;
using DeviceManagementSystem.Maintenances;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Dashboard
{
    /// <summary>
    /// 仪表盘服务
    /// </summary>
    [Authorize]
    [Audited]
    public class DashboardAppService : DeviceManagementSystemAppServiceBase, IDashboardAppService
    {
        private readonly IRepository<MaintenanceTasks, Guid> _maintenanceTaskRepository;
        private readonly IRepository<MaintenancePlans, Guid> _maintenancePlanRepository;

        public DashboardAppService(
            IRepository<MaintenanceTasks, Guid> maintenanceTaskRepository,
            IRepository<MaintenancePlans, Guid> maintenancePlanRepository)
        {
            _maintenanceTaskRepository = maintenanceTaskRepository;
            _maintenancePlanRepository = maintenancePlanRepository;
        }

        /// <summary>
        /// 获取保养任务统计信息
        /// </summary>
        [HttpGet]
        [DisableAuditing]
        public async Task<CommonResult<MaintenanceTaskStatsDto>> GetMaintenanceTaskStats([FromQuery] string timeRange = "today")
        {
            try
            {
                DateTime now = DateTime.Now;
                var today = now.Date;
                DateTime startDate;
                DateTime endDate;

                // 根据时间范围计算起止日期
                switch (timeRange)
                {
                    case "today":
                        startDate = today;
                        endDate = today.AddDays(1).AddSeconds(-1);
                        break;
                    case "week":
                        var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1); // 周一为开始
                        startDate = startOfWeek;
                        endDate = startOfWeek.AddDays(7).AddSeconds(-1);
                        break;
                    case "month":
                        startDate = new DateTime(today.Year, today.Month, 1);
                        endDate = startDate.AddMonths(1).AddSeconds(-1);
                        break;
                    default:
                        startDate = today;
                        endDate = today.AddDays(1).AddSeconds(-1);
                        break;
                }

                // 查询时间范围内的工单
                var query = _maintenanceTaskRepository.GetAll()
                    .Where(x => x.PlanStartDate >= startDate && x.PlanStartDate <= endDate);

                var tasks = await query.ToListAsync();
                var total = tasks.Count;

                // 获取当前用户ID
                var currentUserId = AbpSession.UserId;

                // 统计各状态数量
                var pendingCount = tasks.Count(x => x.Status == "待执行");
                var executingCount = tasks.Count(x => x.Status == "执行中");
                var overdueCount = tasks.Count(x => x.Status != "已完成"
                    && x.Status != "已取消"
                    && x.PlanEndDate < today);
                var notExecutedCount = tasks.Count(x => x.Status == "计划");
                var completedCount = tasks.Count(x => x.Status == "已完成");

                // 计算完成率
                var completedRate = total > 0 ? (int)Math.Round((double)completedCount / total * 100) : 0;

                // 获取待办任务数量（当前用户相关）
                var myTodoCount = 0;
                if (currentUserId.HasValue)
                {
                    myTodoCount = await _maintenanceTaskRepository.GetAll()
                        .Where(x => x.Status == "待执行" || x.Status == "执行中")
                        .Where(x => x.ExecutorNames != null && x.ExecutorNames.Contains(currentUserId.Value.ToString()))
                        .CountAsync();
                }

                var result = new MaintenanceTaskStatsDto
                {
                    Total = total,
                    CompletedRate = completedRate,
                    PendingCount = pendingCount,
                    ExecutingCount = executingCount,
                    OverdueCount = overdueCount,
                    NotExecutedCount = notExecutedCount,
                    CompletedCount = completedCount,
                    MyTodoCount = myTodoCount,
                    StartDate = startDate,
                    EndDate = endDate
                };

                return CommonResult<MaintenanceTaskStatsDto>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取保养任务统计失败", ex);
                return CommonResult<MaintenanceTaskStatsDto>.Error($"获取保养任务统计失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取计划管理统计
        /// </summary>
        [HttpGet]
        [DisableAuditing]
        public async Task<CommonResult<PlanStatsDto>> GetPlanStats()
        {
            try
            {
                var today = DateTime.Today;

                // 获取保养计划统计
                var plans = await _maintenancePlanRepository.GetAll().ToListAsync();
                var activePlans = plans.Count(x => x.Status == "启用");
                var expiredPlans = plans.Count(x =>  x.NextMaintenanceDate < today
                    && x.Status == "启用");

                // 获取工单统计
                var tasks = await _maintenanceTaskRepository.GetAll().ToListAsync();
                var pendingTasks = tasks.Count(x => x.Status == "待执行");
                var executingTasks = tasks.Count(x => x.Status == "执行中");

                var result = new PlanStatsDto
                {
                    TotalPlans = plans.Count,
                    ActivePlans = activePlans,
                    ExpiredPlans = expiredPlans,
                    PendingTasks = pendingTasks,
                    ExecutingTasks = executingTasks
                };

                return CommonResult<PlanStatsDto>.Success(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取计划管理统计失败", ex);
                return CommonResult<PlanStatsDto>.Error($"获取计划管理统计失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取任务状态分布（用于图表）
        /// </summary>
        [HttpGet]
        [DisableAuditing]
        public async Task<CommonResult<List<TaskStatusDistributionDto>>> GetTaskStatusDistribution([FromQuery] string timeRange = "today")
        {
            try
            {
                var today = DateTime.Today;
                DateTime startDate;
                DateTime endDate;

                switch (timeRange)
                {
                    case "today":
                        startDate = today;
                        endDate = today.AddDays(1).AddSeconds(-1);
                        break;
                    case "week":
                        var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
                        startDate = startOfWeek;
                        endDate = startOfWeek.AddDays(7).AddSeconds(-1);
                        break;
                    case "month":
                        startDate = new DateTime(today.Year, today.Month, 1);
                        endDate = startDate.AddMonths(1).AddSeconds(-1);
                        break;
                    default:
                        startDate = today;
                        endDate = today.AddDays(1).AddSeconds(-1);
                        break;
                }

                var query = _maintenanceTaskRepository.GetAll()
                    .Where(x => x.PlanStartDate >= startDate && x.PlanStartDate <= endDate);

                var tasks = await query.ToListAsync();

                var distribution = new List<TaskStatusDistributionDto>
                {
                    new TaskStatusDistributionDto
                    {
                        Name = "待执行",
                        Count = tasks.Count(x => x.Status == "待执行"),
                        Color = "#ff4d4f",
                        Key = "pending"
                    },
                    new TaskStatusDistributionDto
                    {
                        Name = "保养中",
                        Count = tasks.Count(x => x.Status == "执行中"),
                        Color = "#faad14",
                        Key = "executing"
                    },
                    new TaskStatusDistributionDto
                    {
                        Name = "逾期",
                        Count = tasks.Count(x => x.Status != "已完成"
                            && x.Status != "已取消"
                            && x.PlanEndDate < today),
                        Color = "#ff4d4f",
                        Key = "overdue"
                    },
                    new TaskStatusDistributionDto
                    {
                        Name = "未执行",
                        Count = tasks.Count(x => x.Status == "计划"),
                        Color = "#bfbfbf",
                        Key = "notExecuted"
                    },
                    new TaskStatusDistributionDto
                    {
                        Name = "已完成",
                        Count = tasks.Count(x => x.Status == "已完成"),
                        Color = "#52c41a",
                        Key = "completed"
                    }
                };

                return CommonResult<List<TaskStatusDistributionDto>>.Success(distribution);
            }
            catch (Exception ex)
            {
                Logger.Error("获取任务状态分布失败", ex);
                return CommonResult<List<TaskStatusDistributionDto>>.Error($"获取任务状态分布失败: {ex.Message}");
            }
        }
    }
}
