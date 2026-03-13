using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagementSystem.Maintenances.Dto
{
    /// <summary>
    /// 保养模板列表项DTO
    /// </summary>
    [AutoMap(typeof(MaintenanceTemplates))]
    public class MaintenanceTemplateListDto : EntityDto<Guid>
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }
        
        /// <summary>
        /// 模板编码
        /// </summary>
        public string TemplateCode { get; set; }

        /// <summary>
        /// 保养等级（月度：Monthly/季度：Quarter/半年度：Half-Yearly/年度：Annual）
        /// </summary>
        public string MaintenanceLevel { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public bool? IsActive { get; set; }
        
        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 项目数量（冗余字段，方便前端展示）
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// 使用数量（冗余字段，方便前端展示）
        /// </summary>
        public int UseCount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 设备类型列表（冗余字段，方便前端展示）
        /// </summary>
        public List<DeviceTypeInfoDto> DeviceTypes { get; set; }
    }


    /// <summary>
    /// 设备类型信息DTO
    /// </summary>
    public class DeviceTypeInfoDto
    {
        /// <summary>
        /// 类型Id
        /// </summary>
        public Guid Id { get; set; }

        
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }
    }


    /// <summary>
    /// 保养模板详情DTO
    /// </summary>
    public class MaintenanceTemplateDto : EntityDto<Guid>
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 模板编码
        /// </summary>
        public string TemplateCode { get; set; }

        /// <summary>
        /// 保养等级（月度：Monthly/季度：Quarter/半年度：Half-Yearly/年度：Annual）
        /// </summary>
        public string MaintenanceLevel { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public bool? IsActive { get; set; }
        
        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 设备类型列表（冗余字段，方便前端展示）
        /// </summary>
        public List<DeviceTypeInfoDto> DeviceTypes { get; set; }

        /// <summary>
        /// 保养项目列表
        /// </summary>
        public List<MaintenanceItemDto> Items { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }


    /// <summary>
    /// 保养项目DTO
    /// </summary>
    [AutoMap(typeof(MaintenanceItems))]
    public class MaintenanceItemDto : EntityDto<Guid>
    {

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 保养方式（目测、清洁、气吹、更换）
        /// </summary>
        public string MaintenanceMethod { get; set; }

        /// <summary>
        /// 要求内容
        /// </summary>
        public string Requirement { get; set; }

        /// <summary>
        /// 标准值/参考值
        /// </summary>
        public string StandardValue { get; set; }

        /// <summary>
        /// 上限
        /// </summary>
        public double? UpperLimit { get; set; }
        
        /// <summary>
        /// 下限
        /// </summary>
        public double? LowerLimit { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 是否需要上传图片（冗余字段，方便前端展示）
        /// </summary>
        public bool NeedImage { get; set; }

        /// <summary>
        /// 是否需要输入数值（冗余字段，方便前端展示）
        /// </summary>
        public bool? NeedValue { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }

}
