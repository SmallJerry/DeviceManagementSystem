using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Users.Dto
{
    /// <summary>
    /// 业务数据统计输出对象
    /// </summary>
    public class BizDataCountDto
    {
        /// <summary>
        /// 用户数量
        /// </summary>
        public int UserCount { get; set; } = 0; 


        /// <summary>
        /// 角色数量
        /// </summary>
        public int RoleCount { get; set; } = 0;


        /// <summary>
        /// 职位数量
        /// </summary>
        public int PositionCount { get; set; } = 0;


        /// <summary>
        /// 组织数量
        /// </summary>
        public int OrgCount { get; set; } = 0;

    }
}
