using Abp.Zero.EntityFrameworkCore;
using DeviceManagementSystem.Authorization.Organizations;
using DeviceManagementSystem.Authorization.Positions;
using DeviceManagementSystem.Authorization.Resources;
using DeviceManagementSystem.Authorization.Roles;
using DeviceManagementSystem.Authorization.Users;
using DeviceManagementSystem.BasicDataManagement;
using DeviceManagementSystem.DeviceInfos;
using DeviceManagementSystem.FileInfos;
using DeviceManagementSystem.FlowManagement;
using DeviceManagementSystem.Maintenances;
using DeviceManagementSystem.MultiTenancy;
using DeviceManagementSystem.Systems;
using DeviceManagementSystem.WorkFlows;
using Microsoft.EntityFrameworkCore;
using System;

namespace DeviceManagementSystem.EntityFrameworkCore
{
    public class DeviceManagementSystemDbContext : AbpZeroDbContext<Tenant, Role, User, DeviceManagementSystemDbContext>
    {
        /* Define a DbSet for each entity of the application */

        /// <summary>
        /// 系统配置表
        /// </summary>
        public DbSet<SystemConfig> SystemConfig { get; set; }


        /// <summary>
        /// 数据字典表
        /// </summary>
        public DbSet<Dict> Dict { get; set; }



        /// <summary>
        /// 组织表
        /// </summary>
        public DbSet<Organization> Organization { get; set; }


        /// <summary>
        /// 职位表
        /// </summary>
        public DbSet<Position> Position { get; set; }


        /// <summary>
        /// 资源表
        /// </summary>
        public DbSet<Authorization.Resources.Resource> Resource { get; set; }


        /// <summary>
        /// 系统关系表
        /// </summary>
        public DbSet<SystemRelation> SystemRelation { get; set; }


        /// <summary>
        /// 类型管理表
        /// </summary>
        public DbSet<Types> Types { get; set; }



        /// <summary>
        /// 供应商信息表
        /// </summary>
        public DbSet<Suppliers> Suppliers { get; set; }


        /// <summary>
        /// 工厂建模表
        /// </summary>
        public DbSet<FactoryNodes> FactoryNodes { get; set; }




        /// <summary>
        /// 技术规格参数模板表
        /// </summary>
        public DbSet<TechnicalParameterTemplates> TechnicalParameterTemplates { get; set; }




        /// <summary>
        /// 业务表单表
        /// </summary>
        public DbSet<BusinessForms> BusinessForms { get; set; }



        /// <summary>
        /// 流程历史版本表
        /// </summary>
        public DbSet<FlowDefinitionHistories> FlowDefinitionHistories { get; set; }



        /// <summary>
        /// 流程定义表
        /// </summary>
        public DbSet<FlowDefinitions> FlowDefinitions { get; set; }


        /// <summary>
        /// 附件表
        /// </summary>
        public DbSet<Attachments> Attachments { get; set; }


        /// <summary>
        /// 文件块表
        /// </summary>
        public DbSet<FileChunks> FileChunks { get; set; }


        /// <summary>
        /// 业务附件关系表
        /// </summary>
        public DbSet<BusinessAttachmentRelations> BusinessAttachmentRelations { get; set; }



        /// <summary>
        /// 流程实例表
        /// </summary>
        public DbSet<FlowInstances> FlowInstances { get; set; }


        /// <summary>
        /// 节点任务表
        /// </summary>
        public DbSet<FlowNodeTasks> FlowNodeTasks { get; set; }

        /// <summary>
        /// 流程实例历史记录
        /// </summary>
        public DbSet<FlowInstanceHistories> FlowInstanceHistories { get; set; }


        /// <summary>
        /// 设备表
        /// </summary>
        public DbSet<Devices> Devices { get; set; }


        /// <summary>
        /// 设备与变更申请关系表
        /// </summary>
        public DbSet<DeviceAndChangeApplicationRelations> DeviceAndChangeApplicationRelations { get; set; }


        /// <summary>
        /// 设备变更申请表
        /// </summary>
        public DbSet<DeviceChangeApplications> DeviceChangeApplications { get; set; }


        /// <summary>
        /// 设备工厂建模关系表
        /// </summary>
        public DbSet<DeviceFactoryNodeRelations> DeviceFactoryNodeRelations { get; set; }



        /// <summary>
        /// 设备与供应商关系表
        /// </summary>
        public DbSet<DeviceSupplierRelations> DeviceSupplierRelations { get; set; }


        /// <summary>
        /// 设备与类型关系表
        /// </summary>
        public DbSet<DeviceTypeRelations> DeviceTypeRelations { get; set; }


        /// <summary>
        /// 设备与用户关系表
        /// </summary>
        public DbSet<DeviceUserRelations> DeviceUserRelations { get; set; }






        #region 保养模块相关实体表



        /// <summary>
        /// 设备与保养模板关系表
        /// </summary>
        public DbSet<DeviceMaintenanceTemplateRelation> DeviceMaintenanceTemplateRelations { get; set; }


        /// <summary>
        /// 保养项目表
        /// </summary>
        public DbSet<MaintenanceItems> MaintenanceItems { get; set; }


        /// <summary>
        /// 保养计划表
        /// </summary>
        public DbSet<MaintenancePlans> MaintenancePlans { get; set; }



        /// <summary>
        /// 保养工单与执行人关系表
        /// </summary>
        public DbSet<MaintenanceTaskExecutorRelation> MaintenanceTaskExecutorRelations { get; set; }


        /// <summary>
        /// 保养任务整合组表
        /// </summary>
        public DbSet<MaintenanceTaskGroups> MaintenanceTaskGroups { get; set; }


        /// <summary>
        /// 保养工单项目执行记录表
        /// </summary>
        public DbSet<MaintenanceTaskItems> MaintenanceTaskItems { get; set; }



        /// <summary>
        /// 保养工单表
        /// </summary>
        public DbSet<MaintenanceTasks> MaintenanceTasks { get; set; }


        /// <summary>
        /// 保养模板表
        /// </summary>
        public DbSet<MaintenanceTemplates> MaintenanceTemplates { get; set; }


        /// <summary>
        /// 设备与保养计划关系表
        /// </summary>
        public DbSet<DeviceMaintenancePlanRelation> DeviceMaintenancePlanRelation { get; set; }



        /// <summary>
        /// 保养项目分组（点检部位）
        /// </summary>
        public DbSet<MaintenanceItemGroups> MaintenanceItemGroups { get; set; }



        /// <summary>
        /// 保养标准表
        /// </summary>
        public DbSet<MaintenanceStandards> MaintenanceStandards { get; set; }

        #endregion



        public DeviceManagementSystemDbContext(DbContextOptions<DeviceManagementSystemDbContext> options)
            : base(options)
        {
        }
    }
}
