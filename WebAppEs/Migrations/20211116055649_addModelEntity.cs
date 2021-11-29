using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class addModelEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MobileRND_Models",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ModelName = table.Column<string>(maxLength: 150, nullable: false),
                    Supplier = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileRND_Models", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MobileRND_Models");
        }
    }
}
