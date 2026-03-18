using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances.Dto
{
    #region 提醒相关DTO

    /// <summary>
    /// 待提醒任务DTO
    /// </summary>
    public class PendingRemindTaskDto
    {
        /// <summary>
        /// 组ID
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 工单列表
        /// </summary>
        public List<MaintenanceTaskDto> Tasks { get; set; }

        /// <summary>
        /// 设备数量
        /// </summary>
        public int DeviceCount { get;set;}

        /// <summary>
        /// 提醒日期
        /// </summary>
        public DateTime RemindDate { get; set; }

        /// <summary>
        /// 计划开始日期
        /// </summary>
        public DateTime PlanStartDate { get; set; }



        /// <summary>
        /// 计划完成日期
        /// </summary>
        public DateTime PlanEndDate { get; set; }


        /// <summary>
        /// 执行人姓名
        /// </summary>
        public string ExecutorNames { get; set; }
    }

    #endregion
}
