using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BinaryDiff.Result.Infrastructure.Database.Migrations
{
    public partial class Initial_migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiffResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DiffId = table.Column<Guid>(nullable: false),
                    Result = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    TriggerInputId = table.Column<string>(nullable: false),
                    TriggerInputPosition = table.Column<int>(nullable: false),
                    OpposingInputId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiffResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Differences",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ResultId = table.Column<Guid>(nullable: false),
                    Offset = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Differences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Differences_DiffResults_ResultId",
                        column: x => x.ResultId,
                        principalTable: "DiffResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Differences_ResultId",
                table: "Differences",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_DiffResults_DiffId",
                table: "DiffResults",
                column: "DiffId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Differences");

            migrationBuilder.DropTable(
                name: "DiffResults");
        }
    }
}
