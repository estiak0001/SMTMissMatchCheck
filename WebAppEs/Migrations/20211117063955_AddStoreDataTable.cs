using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class AddStoreDataTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MobileRND_StoreHead",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EmployeeID = table.Column<string>(maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(maxLength: 50, nullable: false),
                    ModelID = table.Column<Guid>(maxLength: 50, nullable: false),
                    LotNo = table.Column<string>(maxLength: 50, nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    LUser = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileRND_StoreHead", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileRND_StoreDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HeadID = table.Column<Guid>(nullable: false),
                    EmployeeID = table.Column<string>(maxLength: 50, nullable: false),
                    SLNO = table.Column<string>(maxLength: 50, nullable: true),
                    Moduler = table.Column<string>(maxLength: 50, nullable: true),
                    Feeder = table.Column<string>(maxLength: 150, nullable: true),
                    PartNumber = table.Column<string>(maxLength: 150, nullable: true),
                    FeederName = table.Column<string>(maxLength: 150, nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    LUser = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileRND_StoreDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MobileRND_StoreDetails_MobileRND_StoreHead_HeadID",
                        column: x => x.HeadID,
                        principalTable: "MobileRND_StoreHead",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MobileRND_StoreDetails_HeadID",
                table: "MobileRND_StoreDetails",
                column: "HeadID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MobileRND_StoreDetails");

            migrationBuilder.DropTable(
                name: "MobileRND_StoreHead");
        }
    }
}
