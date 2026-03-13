using System;

namespace DeviceManagementSystem.SystemConfigs.param
{
    /// <summary>
    /// 组织列表选择器参数     
    /// </summary>
    public class ConfigSelectorOrgListParam
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int Current { get; set; }


        /// <summary>
        /// 每页条数
        /// </summary>
        public int Size { get; set; }


        /// <summary>
        /// 父id
        /// </summary>
        public Guid? ParentId { get; set; }


        /// <summary>
        /// 名称关键词
        /// </summary>
        public string SearchKey { get; set; }

    }
}
