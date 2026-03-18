using DeviceManagementSystem.Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances.Dto
{
    /// <summary>
    /// 保养标准输入DTO
    /// </summary>
    public class MaintenanceStandardInput
    {
        public Guid? Id { get; set; }
        public string PointName { get; set; }
        public string PointType { get; set; }
        public string InspectionContent { get; set; }
        public List<string> InspectionMethod { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 保养标准输出DTO
    /// </summary>
    public class MaintenanceStandardDto
    {

        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 点检项目名称
        /// </summary>
        public string PointName { get; set; }

        /// <summary>
        /// 点检部位
        /// </summary>
        public string PointType { get; set; }

        /// <summary>
        /// 点检内容
        /// </summary>
        public string InspectionContent { get; set; }

        /// <summary>
        /// 点检方法列表
        /// </summary>
        public List<string> InspectionMethod { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }

    /// <summary>
    /// 保养标准分页查询输入
    /// </summary>
    public class MaintenanceStandardPageInput : PageRequest
    {

        /// <summary>
        /// 点检项目名称（模糊搜索）
        /// </summary>
        public string PointType { get; set; }
    }
}
