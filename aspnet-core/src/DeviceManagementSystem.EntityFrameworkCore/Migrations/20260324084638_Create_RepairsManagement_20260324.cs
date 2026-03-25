using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Create_RepairsManagement_20260324 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RepairAcceptance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepairTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepairRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcceptanceCriteriaJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    BeforeRepairParams = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AfterRepairParams = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AcceptanceConclusion = table.Column<int>(type: "int", nullable: false),
                    AcceptanceOpinion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AcceptorId = table.Column<long>(type: "bigint", maxLength: 20, nullable: false),
                    AcceptorName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AcceptanceTime = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_RepairAcceptance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RepairType = table.Column<int>(type: "int", nullable: true),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesterId = table.Column<long>(type: "bigint", maxLength: 20, nullable: false),
                    RequesterName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FaultFoundTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FaultLevel = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    ExpectedCompleteTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FaultDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequestStatus = table.Column<int>(type: "int", maxLength: 10, nullable: true),
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
                    table.PrimaryKey("PK_RepairRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairRequestDeviceTypeRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepairRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairRequestDeviceTypeRelation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairRequestRepairerRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepairRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepairerId = table.Column<long>(type: "bigint", maxLength: 20, nullable: false),
                    RepairerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairRequestRepairerRelation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RepairRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesterId = table.Column<long>(type: "bigint", maxLength: 20, nullable: false),
                    RequesterName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RequestTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepairerIds = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RepairerNames = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AcceptTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: true),
                    TaskStatus = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    IsOverdue = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    FaultReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QualityImpactAnalysis = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RepairMethodResult = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NotifyMaintenance = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    MaintenancePlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaintenanceTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NotifiedAcceptance = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    AcceptorId = table.Column<long>(type: "bigint", nullable: true),
                    AcceptorName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AcceptanceTime = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_RepairTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairTaskExecutionRecord",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepairTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FaultReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QualityImpactAnalysis = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RepairMethodResult = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SaveType = table.Column<int>(type: "int", maxLength: 20, nullable: false),
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
                    table.PrimaryKey("PK_RepairTaskExecutionRecord", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepairAcceptance");

            migrationBuilder.DropTable(
                name: "RepairRequest");

            migrationBuilder.DropTable(
                name: "RepairRequestDeviceTypeRelation");

            migrationBuilder.DropTable(
                name: "RepairRequestRepairerRelation");

            migrationBuilder.DropTable(
                name: "RepairTask");

            migrationBuilder.DropTable(
                name: "RepairTaskExecutionRecord");
        }
    }
}
