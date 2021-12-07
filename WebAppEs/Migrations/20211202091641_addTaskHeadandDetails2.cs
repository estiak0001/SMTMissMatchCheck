using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class addTaskHeadandDetails2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MobileRND_QcTaskHead",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EmployeeID = table.Column<string>(maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(maxLength: 50, nullable: false),
                    ModelID = table.Column<Guid>(maxLength: 50, nullable: false),
                    LotNo = table.Column<string>(maxLength: 50, nullable: false),
                    StartTime = table.Column<DateTime>(maxLength: 50, nullable: false),
                    EndTime = table.Column<DateTime>(maxLength: 50, nullable: false),
                    TaskType = table.Column<string>(maxLength: 50, nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    LUser = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileRND_QcTaskHead", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileRND_QcTaskHeadDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EmployeeID = table.Column<string>(maxLength: 50, nullable: false),
                    TaskHeadID = table.Column<Guid>(nullable: false),
                    DateAndTime = table.Column<DateTime>(maxLength: 50, nullable: false),
                    StoreDetailsID = table.Column<Guid>(maxLength: 50, nullable: false),
                    Status = table.Column<bool>(maxLength: 50, nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    LUser = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileRND_QcTaskHeadDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MobileRND_QcTaskHeadDetails_MobileRND_QcTaskHead_TaskHeadID",
                        column: x => x.TaskHeadID,
                        principalTable: "MobileRND_QcTaskHead",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MobileRND_QcTaskHeadDetails_TaskHeadID",
                table: "MobileRND_QcTaskHeadDetails",
                column: "TaskHeadID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MobileRND_QcTaskHeadDetails");

            migrationBuilder.DropTable(
                name: "MobileRND_QcTaskHead");
        }
    }
}
