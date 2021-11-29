using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class AddIconToNavigationManue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "AspNetNavigationMenu",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "AspNetNavigationMenu");
        }
    }
}
