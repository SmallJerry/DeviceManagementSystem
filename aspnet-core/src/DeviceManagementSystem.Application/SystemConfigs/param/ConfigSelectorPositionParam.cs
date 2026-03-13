using System;

namespace DeviceManagementSystem.SystemConfigs.param
{
    /// <summary>
    /// 职位选择器参数
    /// </summary>
    public class ConfigSelectorPositionParam
    {

        /// <summary>
        /// 当前页
        /// </summary>
        public int Current { get; set; }


        /// <summary>
        /// 每页条数
        /// </summary>
        /// 
        public int Size { get; set; }


        /// <summary>
        /// 组织id
        /// </summary>
        public Guid? OrgId { get; set; }


        /// <summary>
        /// 角色分类
        /// </summary>
        public string Category { get; set; }


        /// <summary>
        /// 名称关键词
        /// </summary>
        public string SearchKey { get; set; }

    }
}
