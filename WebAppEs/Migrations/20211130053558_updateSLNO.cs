using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class updateSLNO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SLNO",
                table: "MobileRND_StoreDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SLNO",
                table: "MobileRND_StoreDetails",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
