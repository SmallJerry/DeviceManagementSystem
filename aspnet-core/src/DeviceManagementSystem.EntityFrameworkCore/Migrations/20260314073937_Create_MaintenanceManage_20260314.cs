using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Create_MaintenanceManage_20260314 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceMaintenancePlanRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenancePlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CycleType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceMaintenancePlanRelation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceMaintenanceTemplateRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenanceLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceMaintenanceTemplateRelation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaintenanceMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StandardValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: true),
                    EstimatedMinutes = table.Column<int>(type: "int", nullable: true),
                    RequiredTools = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequiredSpareParts = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SafetyNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenancePlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenanceLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CycleType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CycleDays = table.Column<int>(type: "int", nullable: false),
                    FirstMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    HasGeneratedTask = table.Column<bool>(type: "bit", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenancePlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaskName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenanceLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PlanStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlanEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemindDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutorIds = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExecutorNames = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OriginalExecutorIds = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DelegatorId = table.Column<long>(type: "bigint", nullable: true),
                    DelegatorName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DelegateReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CompletionRemark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreateType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsReminded = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceTaskAttachmentRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTaskAttachmentRelation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceTaskExecutorRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExecutorId = table.Column<long>(type: "bigint", nullable: false),
                    ExecutorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTaskExecutorRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceTaskGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GroupName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RemindDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlanStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlanEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutorIds = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExecutorNames = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTaskGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceTaskItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaintenanceMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StandardValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Result = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ActualValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTaskItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeviceTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenanceLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTemplate", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceMaintenancePlanRelation");

            migrationBuilder.DropTable(
                name: "DeviceMaintenanceTemplateRelation");

            migrationBuilder.DropTable(
                name: "MaintenanceItem");

            migrationBuilder.DropTable(
                name: "MaintenancePlan");

            migrationBuilder.DropTable(
                name: "MaintenanceTask");

            migrationBuilder.DropTable(
                name: "MaintenanceTaskAttachmentRelation");

            migrationBuilder.DropTable(
                name: "MaintenanceTaskExecutorRelations");

            migrationBuilder.DropTable(
                name: "MaintenanceTaskGroup");

            migrationBuilder.DropTable(
                name: "MaintenanceTaskItem");

            migrationBuilder.DropTable(
                name: "MaintenanceTemplate");
        }
    }
}
