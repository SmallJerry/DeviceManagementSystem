using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Update_MaintenanceTask_MaintenanceTaskItem_20260319 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceMaintenanceLevel",
                table: "MaintenanceTaskItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourcePlanId",
                table: "MaintenanceTaskItem",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMergedTask",
                table: "MaintenanceTask",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MergedPlanIds",
                table: "MaintenanceTask",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceMaintenanceLevel",
                table: "MaintenanceTaskItem");

            migrationBuilder.DropColumn(
                name: "SourcePlanId",
                table: "MaintenanceTaskItem");

            migrationBuilder.DropColumn(
                name: "IsMergedTask",
                table: "MaintenanceTask");

            migrationBuilder.DropColumn(
                name: "MergedPlanIds",
                table: "MaintenanceTask");
        }
    }
}
