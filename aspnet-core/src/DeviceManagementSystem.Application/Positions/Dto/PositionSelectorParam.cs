using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Positions.Dto
{
    /// <summary>
    /// 职位树 DTO
    /// </summary>
    public class PositionSelectorParam
    {
        /// <summary>
        /// 组织Id
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
    
}
