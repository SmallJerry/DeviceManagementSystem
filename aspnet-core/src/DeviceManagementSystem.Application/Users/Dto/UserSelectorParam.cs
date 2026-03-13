using DeviceManagementSystem.Organizations.Dto;
using DeviceManagementSystem.Roles.Dto;
using DeviceManagementSystem.Utils.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Users.Dto
{
    /// <summary>
    /// 用户选择器查询参数
    /// </summary>
    public class UserSelectorParam
    {
        /// <summary>
        /// 组织ID
        /// </summary>
        public Guid? OrgId { get; set; }

        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Current { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        [Range(1, 100)]
        public int Size { get; set; } = 20;
    }

    /// <summary>
    /// 用户选择器返回的用户信息（简化版）
    /// </summary>
    public class UserSelectorDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public Guid? OrgId { get; set; }

        /// <summary>
        /// 职位ID
        /// </summary>
        public Guid? PositionId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }


    }

}
