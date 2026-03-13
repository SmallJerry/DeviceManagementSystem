using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Update_DevicveAndChangeApplicationRelation_DeviceChangeApplications_20260302 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicantUserId",
                table: "DeviceChangeApplication");

            migrationBuilder.DropColumn(
                name: "ApplyUserName",
                table: "DeviceChangeApplication");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "DeviceChangeApplication");

            migrationBuilder.DropColumn(
                name: "FlowInstanceId",
                table: "DeviceChangeApplication");

            migrationBuilder.DropColumn(
                name: "FlowStatus",
                table: "DeviceChangeApplication");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "DeviceChangeApplication");

            migrationBuilder.AddColumn<string>(
                name: "ApplyReason",
                table: "DeviceAndChangeApplicationRelation",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChangeType",
                table: "DeviceAndChangeApplicationRelation",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FlowInstanceId",
                table: "DeviceAndChangeApplicationRelation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlowStatus",
                table: "DeviceAndChangeApplicationRelation",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmitTime",
                table: "DeviceAndChangeApplicationRelation",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "SubmitterId",
                table: "DeviceAndChangeApplicationRelation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "SubmitterName",
                table: "DeviceAndChangeApplicationRelation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyReason",
                table: "DeviceAndChangeApplicationRelation");

            migrationBuilder.DropColumn(
                name: "ChangeType",
                table: "DeviceAndChangeApplicationRelation");

            migrationBuilder.DropColumn(
                name: "FlowInstanceId",
                table: "DeviceAndChangeApplicationRelation");

            migrationBuilder.DropColumn(
                name: "FlowStatus",
                table: "DeviceAndChangeApplicationRelation");

            migrationBuilder.DropColumn(
                name: "SubmitTime",
                table: "DeviceAndChangeApplicationRelation");

            migrationBuilder.DropColumn(
                name: "SubmitterId",
                table: "DeviceAndChangeApplicationRelation");

            migrationBuilder.DropColumn(
                name: "SubmitterName",
                table: "DeviceAndChangeApplicationRelation");

            migrationBuilder.AddColumn<long>(
                name: "ApplicantUserId",
                table: "DeviceChangeApplication",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ApplyUserName",
                table: "DeviceChangeApplication",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeviceId",
                table: "DeviceChangeApplication",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FlowInstanceId",
                table: "DeviceChangeApplication",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlowStatus",
                table: "DeviceChangeApplication",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "DeviceChangeApplication",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
