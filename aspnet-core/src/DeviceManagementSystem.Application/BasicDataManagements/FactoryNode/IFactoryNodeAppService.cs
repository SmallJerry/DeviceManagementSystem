using DeviceManagementSystem.BasicDataManagements.FactoryNode.Dto;
using DeviceManagementSystem.Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.BasicDataManagements.FactoryNode
{
    /// <summary>
    /// 工厂节点管理服务接口
    /// </summary>
    public interface IFactoryNodeAppService
    {
        /// <summary>
        /// 获取工厂节点分页列表
        /// </summary>
        Task<CommonResult<Page<FactoryNodeDto>>> GetPageList(FactoryNodePageInput input);

        /// <summary>
        /// 获取工厂节点树形结构
        /// </summary>
        Task<CommonResult<List<FactoryNodeTreeDto>>> GetTreeList(FactoryNodeTreeInput input);

        /// <summary>
        /// 获取工厂节点详情
        /// </summary>
        Task<CommonResult<FactoryNodeDto>> GetById(Guid id);

        /// <summary>
        /// 创建工厂节点
        /// </summary>
        Task<CommonResult> Create(FactoryNodeAddInput input);

        /// <summary>
        /// 更新工厂节点
        /// </summary>
        Task<CommonResult> UpdateById(FactoryNodeEditInput input);

        /// <summary>
        /// 删除工厂节点
        /// </summary>
        Task<CommonResult> DeleteByIds(List<FactoryNodeIdInput> ids);
    }

    /// <summary>
    /// 节点数据传输对象
    /// </summary>
    public class NodeDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 父Id
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }
    }
}
