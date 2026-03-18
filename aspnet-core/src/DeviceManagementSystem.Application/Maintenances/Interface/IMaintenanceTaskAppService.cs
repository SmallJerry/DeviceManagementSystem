using DeviceManagementSystem.Maintenances.Dto;
using DeviceManagementSystem.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances.Interface
{
    /// <summary>
    /// 保养工单服务接口
    /// </summary>
    public interface IMaintenanceTaskAppService
    {
        /// <summary>
        /// 获取工单分页列表
        /// </summary>
        Task<CommonResult<Page<MaintenanceTaskDto>>> GetPageList([FromQuery] MaintenanceTaskPageInput input);

        /// <summary>
        /// 获取我的待办工单（按组整合）
        /// </summary>
        Task<CommonResult<List<PendingTaskGroupDto>>> GetMyPendingTasks(long? executorId = null);

        /// <summary>
        /// 获取工单详情
        /// </summary>
        Task<CommonResult<MaintenanceTaskDto>> GetById(Guid id);

        /// <summary>
        /// 生成下周保养工单（定时任务调用）
        /// </summary>
        Task<CommonResult<int>> GenerateNextWeekTasks();

        /// <summary>
        /// 执行保养工单
        /// </summary>
        Task<CommonResult> ExecuteTask(ExecuteMaintenanceTaskInput input);

        /// <summary>
        /// 开始执行工单
        /// </summary>
        Task<CommonResult> StartTask(Guid taskId);

        /// <summary>
        /// 委派工单
        /// </summary>
        Task<CommonResult> DelegateTask(DelegateMaintenanceTaskInput input);

        /// <summary>
        /// 取消工单
        /// </summary>
        Task<CommonResult> CancelTask(Guid taskId, string reason);

        /// <summary>
        /// 获取今日需提醒任务
        /// </summary>
        Task<CommonResult<List<PendingRemindTaskDto>>> GetTodayRemindTasks();

        /// <summary>
        /// 发送提醒（定时任务调用）
        /// </summary>
        Task<CommonResult<int>> SendReminders();



        /// <summary>
        /// 为指定计划生成工单
        /// </summary>
        Task<CommonResult> GenerateTaskForPlan(Guid planId);
    }
}
