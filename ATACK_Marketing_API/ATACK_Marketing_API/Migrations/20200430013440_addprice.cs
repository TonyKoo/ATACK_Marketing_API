using Microsoft.EntityFrameworkCore.Migrations;

namespace ATACK_Marketing_API.Migrations
{
    public partial class addprice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductDetails",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "ProductPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductPrice",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "ProductDetails",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
