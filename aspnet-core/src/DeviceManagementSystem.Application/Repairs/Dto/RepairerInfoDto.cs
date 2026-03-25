using System;
using System.Collections.Generic;

namespace DeviceManagementSystem.Repairs.Dto
{
    #region 通用信息DTO

    /// <summary>
    /// 维修人员信息DTO
    /// </summary>
    public class RepairerInfoDto
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
    /// 附件信息DTO
    /// </summary>
    public class AttachmentInfoDto
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
        /// 上传时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }

    /// <summary>
    /// 保养计划选择项DTO
    /// </summary>
    public class MaintenancePlanOptionDto
    {
        /// <summary>
        /// 计划ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 计划名称
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// 保养等级
        /// </summary>
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 保养等级文本
        /// </summary>
        public string MaintenanceLevelText { get; set; }
    }

    #endregion
}