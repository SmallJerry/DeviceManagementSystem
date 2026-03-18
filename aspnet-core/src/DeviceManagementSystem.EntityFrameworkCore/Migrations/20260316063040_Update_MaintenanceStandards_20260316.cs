using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Update_MaintenanceStandards_20260316 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PointNo",
                table: "MaintenanceStandard");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PointNo",
                table: "MaintenanceStandard",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
