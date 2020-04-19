using Microsoft.EntityFrameworkCore.Migrations;

namespace ATACK_Marketing_API.Migrations
{
    public partial class AdjustVendor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventVendors_Events_EventId",
                table: "EventVendors");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_EventVendors_EventVendorId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "VendorName",
                table: "Vendors");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EventVendorId",
                table: "Products",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "EventVendors",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventVendors_Events_EventId",
                table: "EventVendors",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_EventVendors_EventVendorId",
                table: "Products",
                column: "EventVendorId",
                principalTable: "EventVendors",
                principalColumn: "EventVendorId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventVendors_Events_EventId",
                table: "EventVendors");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_EventVendors_EventVendorId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Vendors");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorName",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EventVendorId",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "EventVendors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_EventVendors_Events_EventId",
                table: "EventVendors",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_EventVendors_EventVendorId",
                table: "Products",
                column: "EventVendorId",
                principalTable: "EventVendors",
                principalColumn: "EventVendorId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
