using Microsoft.EntityFrameworkCore.Migrations;

namespace ATACK_Marketing_API.Migrations
{
    public partial class VendorAuditAsjust : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAuditId",
                table: "VendorAudit");

            migrationBuilder.AddColumn<string>(
                name: "GranterEmail",
                table: "VendorAudit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GranterUid",
                table: "VendorAudit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GranterEmail",
                table: "VendorAudit");

            migrationBuilder.DropColumn(
                name: "GranterUid",
                table: "VendorAudit");

            migrationBuilder.AddColumn<int>(
                name: "UserAuditId",
                table: "VendorAudit",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
