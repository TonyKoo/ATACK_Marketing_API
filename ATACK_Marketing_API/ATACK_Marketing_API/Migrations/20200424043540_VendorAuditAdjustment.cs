using Microsoft.EntityFrameworkCore.Migrations;

namespace ATACK_Marketing_API.Migrations
{
    public partial class VendorAuditAdjustment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GranterEmail",
                table: "VendorAudit");

            migrationBuilder.DropColumn(
                name: "GranterUid",
                table: "VendorAudit");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "VendorAudit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUid",
                table: "VendorAudit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "VendorAudit");

            migrationBuilder.DropColumn(
                name: "UserUid",
                table: "VendorAudit");

            migrationBuilder.AddColumn<string>(
                name: "GranterEmail",
                table: "VendorAudit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GranterUid",
                table: "VendorAudit",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
