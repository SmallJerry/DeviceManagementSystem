using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Update_MaintenanceItems_20260316 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "MaintenanceItem",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupSortOrder",
                table: "MaintenanceItem",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InspectionContent",
                table: "MaintenanceItem",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InspectionMethod",
                table: "MaintenanceItem",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemSortOrder",
                table: "MaintenanceItem",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PointName",
                table: "MaintenanceItem",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PointNo",
                table: "MaintenanceItem",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "GroupSortOrder",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "InspectionContent",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "InspectionMethod",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "ItemSortOrder",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "PointName",
                table: "MaintenanceItem");

            migrationBuilder.DropColumn(
                name: "PointNo",
                table: "MaintenanceItem");
        }
    }
}
