using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class addScannerDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "MobileRND_StoreDetails",
                maxLength: 50,
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "MobileRND_StoreDetails");
        }
    }
}
