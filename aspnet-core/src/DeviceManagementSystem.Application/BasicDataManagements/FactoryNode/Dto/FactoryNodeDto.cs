using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeviceManagementSystem.BasicDataManagements.FactoryNode.Dto
{
    /// <summary>
    /// 工厂节点DTO
    /// </summary>
    public class FactoryNodeDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 父级名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// 节点类型名称
        /// </summary>
        public string NodeTypeName { get; set; }

        /// <summary>
        /// 节点编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 状态名称
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int SortCode { get; set; }

        /// <summary>
        /// 拓展信息
        /// </summary>
        public string ExtendInfo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 是否有子节点
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// 完整路径（用于展示）
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<FactoryNodeDto> Children { get; set; } = new List<FactoryNodeDto>();
    }

    /// <summary>
    /// 树形节点DTO
    /// </summary>
    public class FactoryNodeTreeDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// 节点编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int SortCode { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<FactoryNodeTreeDto> Children { get; set; } = new List<FactoryNodeTreeDto>();

        /// <summary>
        /// 是否有子节点
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }


        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }



        /// <summary>
        /// 创建时间
        /// </summary>
        public  DateTime CreationTime { get; set; }



        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath { get; set; }
    }

    /// <summary>
    /// 节点类型层级关系
    /// </summary>
    public class NodeTypeHierarchy
    {
        public string NodeType { get; set; }
        public string Name { get; set; }
        public string ParentType { get; set; }
        public string ChildType { get; set; }
        public int Level { get; set; }
    }

    /// <summary>
    /// 添加工厂节点参数
    /// </summary>
    public class FactoryNodeAddInput
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        [Required(ErrorMessage = "节点类型不能为空")]
        [RegularExpression(@"^(Factory|Workshop|ProductionLine|Workstation)$",
            ErrorMessage = "节点类型只能是Factory|Workshop|ProductionLine|Workstation")]
        public string NodeType { get; set; }

        /// <summary>
        /// 节点编码
        /// </summary>
        [StringLength(100, ErrorMessage = "节点编码长度不能超过100个字符")]
        public string Code { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        [Required(ErrorMessage = "节点名称不能为空")]
        [StringLength(200, ErrorMessage = "节点名称长度不能超过200个字符")]
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(500, ErrorMessage = "地址长度不能超过500个字符")]
        public string Address { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Required(ErrorMessage = "状态不能为空")]
        [RegularExpression(@"^(Enabled|Disabled)$", ErrorMessage = "状态只能是Enabled或Disabled")]
        public string Status { get; set; } = "Enabled";

        /// <summary>
        /// 排序码
        /// </summary>
        public int SortCode { get; set; } = 0;

        /// <summary>
        /// 拓展信息
        /// </summary>
        public string ExtendInfo { get; set; }
    }

    /// <summary>
    /// 编辑工厂节点参数
    /// </summary>
    public class FactoryNodeEditInput : FactoryNodeAddInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }
    }

    /// <summary>
    /// 查询树形结构参数
    /// </summary>
    public class FactoryNodeTreeInput
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// 根节点ID
        /// </summary>
        public Guid? RootId { get; set; }

        /// <summary>
        /// 是否包含停用节点
        /// </summary>
        public bool IncludeDisabled { get; set; } = false;
    }

    /// <summary>
    /// 节点ID参数
    /// </summary>
    public class FactoryNodeIdInput
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public Guid Id { get; set; }
    }

    /// <summary>
    /// 分页查询参数
    /// </summary>
    public class FactoryNodePageInput
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; } = "SortCode";

        /// <summary>
        /// 排序方式：ASC/DESC
        /// </summary>
        public string SortOrder { get; set; } = "ASC";

        /// <summary>
        /// 当前页
        /// </summary>
        public int Current { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public int Size { get; set; } = 10;
    }

    /// <summary>
    /// 父级节点选择器DTO
    /// </summary>
    public class ParentNodeSelectorDto
    {
        /// <summary>
        /// 允许选择的父级节点类型
        /// </summary>
        public string AllowedParentType { get; set; }

        /// <summary>
        /// 树形数据
        /// </summary>
        public List<FactoryNodeTreeDto> TreeData { get; set; }
    }




    /// <summary>
    /// 节点类型层级信息DTO
    /// </summary>
    public class NodeTypeHierarchyDto
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 父级类型
        /// </summary>
        public string ParentType { get; set; }

        /// <summary>
        /// 子级类型
        /// </summary>
        public string ChildType { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// 启用的节点DTO（用于选择器）
    /// </summary>
    public class EnabledNodeDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 是否有子节点
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<EnabledNodeDto> Children { get; set; } = new List<EnabledNodeDto>();
    }
}