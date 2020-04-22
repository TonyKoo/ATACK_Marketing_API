using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ATACK_Marketing_API.Migrations
{
    public partial class UserAuditAdjust : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditDateTime",
                table: "UserAudit");

            migrationBuilder.DropColumn(
                name: "Elevate",
                table: "UserAudit");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "UserAudit");

            migrationBuilder.AddColumn<DateTime>(
                name: "EventDateTime",
                table: "UserAudit",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "GrantPermission",
                table: "UserAudit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "GranterEmail",
                table: "UserAudit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GranterUid",
                table: "UserAudit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedEmail",
                table: "UserAudit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedUid",
                table: "UserAudit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermissionType",
                table: "UserAudit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventDateTime",
                table: "UserAudit");

            migrationBuilder.DropColumn(
                name: "GrantPermission",
                table: "UserAudit");

            migrationBuilder.DropColumn(
                name: "GranterEmail",
                table: "UserAudit");

            migrationBuilder.DropColumn(
                name: "GranterUid",
                table: "UserAudit");

            migrationBuilder.DropColumn(
                name: "ModifiedEmail",
                table: "UserAudit");

            migrationBuilder.DropColumn(
                name: "ModifiedUid",
                table: "UserAudit");

            migrationBuilder.DropColumn(
                name: "PermissionType",
                table: "UserAudit");

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDateTime",
                table: "UserAudit",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Elevate",
                table: "UserAudit",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "UserAudit",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
