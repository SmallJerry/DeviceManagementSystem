using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Create_DeviceMaintenancePlanRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CycleType",
                table: "DeviceMaintenancePlanRelation",
                newName: "MaintenanceLevel");

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateId",
                table: "DeviceMaintenancePlanRelation",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "DeviceMaintenancePlanRelation");

            migrationBuilder.RenameColumn(
                name: "MaintenanceLevel",
                table: "DeviceMaintenancePlanRelation",
                newName: "CycleType");
        }
    }
}
