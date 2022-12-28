using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class recording_tabled_removed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupFile_Groups_GroupId",
                table: "GroupFile");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupFile_Users_created_by",
                table: "GroupFile");

            migrationBuilder.DropTable(
                name: "Recordings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupFile",
                table: "GroupFile");

            migrationBuilder.RenameTable(
                name: "GroupFile",
                newName: "GroupFiles");

            migrationBuilder.RenameIndex(
                name: "IX_GroupFile_GroupId",
                table: "GroupFiles",
                newName: "IX_GroupFiles_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupFile_created_by",
                table: "GroupFiles",
                newName: "IX_GroupFiles_created_by");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupFiles",
                table: "GroupFiles",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupFiles_Groups_GroupId",
                table: "GroupFiles",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupFiles_Users_created_by",
                table: "GroupFiles",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupFiles_Groups_GroupId",
                table: "GroupFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupFiles_Users_created_by",
                table: "GroupFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupFiles",
                table: "GroupFiles");

            migrationBuilder.RenameTable(
                name: "GroupFiles",
                newName: "GroupFile");

            migrationBuilder.RenameIndex(
                name: "IX_GroupFiles_GroupId",
                table: "GroupFile",
                newName: "IX_GroupFile_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupFiles_created_by",
                table: "GroupFile",
                newName: "IX_GroupFile_created_by");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupFile",
                table: "GroupFile",
                column: "id");

            migrationBuilder.CreateTable(
                name: "Recordings",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lesson_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    name = table.Column<string>(type: "VARCHAR(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    slug = table.Column<string>(type: "VARCHAR(170)", maxLength: 170, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    video_url = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recordings", x => x.id);
                    table.ForeignKey(
                        name: "FK_Recordings_Lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "Lessons",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Recordings_lesson_id",
                table: "Recordings",
                column: "lesson_id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupFile_Groups_GroupId",
                table: "GroupFile",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupFile_Users_created_by",
                table: "GroupFile",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id");
        }
    }
}
