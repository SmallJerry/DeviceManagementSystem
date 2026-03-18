using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Update_MaintenanceItems_202603161910 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "EstimatedMinutes",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "MaintenanceMethod",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "RequiredSpareParts",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "RequiredTools",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "SafetyNotes",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "StandardValue",
                table: "MaintenanceItem");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceId",
                table: "DeviceMaintenanceTemplateRelation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ChangeApplyId",
                table: "DeviceMaintenanceTemplateRelation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "DeviceMaintenanceTemplateRelation",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeApplyId",
                table: "DeviceMaintenanceTemplateRelation");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "DeviceMaintenanceTemplateRelation");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "MaintenanceItem",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedMinutes",
                table: "MaintenanceItem",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "MaintenanceItem",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "MaintenanceItem",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceMethod",
                table: "MaintenanceItem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredSpareParts",
                table: "MaintenanceItem",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredTools",
                table: "MaintenanceItem",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SafetyNotes",
                table: "MaintenanceItem",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "MaintenanceItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StandardValue",
                table: "MaintenanceItem",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceId",
                table: "DeviceMaintenanceTemplateRelation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
