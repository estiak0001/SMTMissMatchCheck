using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class addScannerDetails3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "MobileRND_Store_ScanHistory");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAndTime",
                table: "MobileRND_Store_ScanHistory",
                maxLength: 50,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAndTime",
                table: "MobileRND_Store_ScanHistory");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "MobileRND_Store_ScanHistory",
                type: "datetime2",
                maxLength: 50,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
