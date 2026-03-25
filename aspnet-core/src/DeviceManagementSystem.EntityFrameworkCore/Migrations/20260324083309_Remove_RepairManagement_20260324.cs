using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Remove_RepairManagement_20260324 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcceptanceRecord");

            migrationBuilder.DropTable(
                name: "RepairExecution");

            migrationBuilder.DropTable(
                name: "RepairRequest");

            migrationBuilder.DropTable(
                name: "RepairWorkOrder");

            migrationBuilder.DropTable(
                name: "RepairWorkOrderMaintenanceTaskRelation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcceptanceRecord",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcceptanceBasis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcceptanceConclusion = table.Column<int>(type: "int", maxLength: 10, nullable: true),
                    AcceptanceTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptorId = table.Column<long>(type: "bigint", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    PostRepairParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreRepairParams = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreventiveMeasures = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WorkOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptanceRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairExecution",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    FaultReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FaultReasonDictId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    QualityImpactAnalysis = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RepairMethod = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RepairResult = table.Column<int>(type: "int", nullable: true),
                    WorkOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairExecution", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpectedCompleteTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FaultDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FaultFoundTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FaultLevel = table.Column<int>(type: "int", maxLength: 10, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    RepairType = table.Column<int>(type: "int", maxLength: 10, nullable: true),
                    ReporterId = table.Column<long>(type: "bigint", nullable: false),
                    RequestNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairWorkOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsOverdue = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    OverdueReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceiveTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RepairDuration = table.Column<int>(type: "int", nullable: true),
                    RepairRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepairerId = table.Column<long>(type: "bigint", nullable: true),
                    ReportTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReporterId = table.Column<long>(type: "bigint", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    WorkOrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairWorkOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairWorkOrderMaintenanceTaskRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaintenanceTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RepairWorkOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairWorkOrderMaintenanceTaskRelation", x => x.Id);
                });
        }
    }
}
