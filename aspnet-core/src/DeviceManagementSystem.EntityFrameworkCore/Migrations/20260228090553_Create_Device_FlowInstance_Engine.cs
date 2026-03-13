using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Create_Device_FlowInstance_Engine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QrCode = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeviceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Specification = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TechnicalParameters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogisticsNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FactoryNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ManufactureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PurchaseNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeviceStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EnableDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BusinessStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Creator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_Device", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceAndChangeApplicationRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceChangeApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceAndChangeApplicationRelation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceChangeApplication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApplicantUserId = table.Column<long>(type: "bigint", nullable: false),
                    ApplyUserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FlowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FlowStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Snapshot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewData = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_DeviceChangeApplication", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceFactoryNodeRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FactoryNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceFactoryNodeRelation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceSupplierRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceSupplierRelation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceTypeRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceTypeRelation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceUserRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceUserRelation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlowInstance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FlowName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FlowDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InitiatorId = table.Column<long>(type: "bigint", nullable: false),
                    InitiatorName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CurrentNodeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CurrentNodeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentNodeType = table.Column<int>(type: "int", nullable: true),
                    CurrentTaskId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CurrentAssigneeId = table.Column<long>(type: "bigint", nullable: true),
                    CurrentAssigneeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BeginTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NodeConfig = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cancelable = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_FlowInstance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlowInstanceHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NodeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NodeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NodeType = table.Column<int>(type: "int", nullable: false),
                    FlowCmd = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<long>(type: "bigint", nullable: true),
                    OperatorName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OperateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Editable = table.Column<bool>(type: "bit", nullable: false),
                    BeforeFormData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AfterFormData = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_FlowInstanceHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlowNodeTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NodeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NodeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NodeType = table.Column<int>(type: "int", nullable: false),
                    AssigneeId = table.Column<long>(type: "bigint", nullable: true),
                    AssigneeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MultiInstanceType = table.Column<int>(type: "int", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HandleTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HandleCmd = table.Column<int>(type: "int", nullable: true),
                    HandleComment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FormAuths = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_FlowNodeTask", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "DeviceAndChangeApplicationRelation");

            migrationBuilder.DropTable(
                name: "DeviceChangeApplication");

            migrationBuilder.DropTable(
                name: "DeviceFactoryNodeRelation");

            migrationBuilder.DropTable(
                name: "DeviceSupplierRelation");

            migrationBuilder.DropTable(
                name: "DeviceTypeRelation");

            migrationBuilder.DropTable(
                name: "DeviceUserRelation");

            migrationBuilder.DropTable(
                name: "FlowInstance");

            migrationBuilder.DropTable(
                name: "FlowInstanceHistory");

            migrationBuilder.DropTable(
                name: "FlowNodeTask");
        }
    }
}
