using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Update_Device_DeviceChangeApplications_DeviceAndChangeApplicationRelations_20260303 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlowStatus",
                table: "DeviceAndChangeApplicationRelation");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationStatus",
                table: "DeviceChangeApplication",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplyReason",
                table: "DeviceChangeApplication",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmitTime",
                table: "DeviceChangeApplication",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SubmitterId",
                table: "DeviceChangeApplication",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmitterName",
                table: "DeviceChangeApplication",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceLevel",
                table: "Device",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsKeyDevice",
                table: "Device",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationStatus",
                table: "DeviceChangeApplication");

            migrationBuilder.DropColumn(
                name: "ApplyReason",
                table: "DeviceChangeApplication");

            migrationBuilder.DropColumn(
                name: "SubmitTime",
                table: "DeviceChangeApplication");

            migrationBuilder.DropColumn(
                name: "SubmitterId",
                table: "DeviceChangeApplication");

            migrationBuilder.DropColumn(
                name: "SubmitterName",
                table: "DeviceChangeApplication");

            migrationBuilder.DropColumn(
                name: "DeviceLevel",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "IsKeyDevice",
                table: "Device");

            migrationBuilder.AddColumn<string>(
                name: "FlowStatus",
                table: "DeviceAndChangeApplicationRelation",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
