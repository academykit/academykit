using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class Questionsettable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PassCode",
                table: "Meetings",
                newName: "passcode");

            migrationBuilder.AddColumn<int>(
                name: "duration",
                table: "Meetings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QuestionSets",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slug = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    thumbnail_url = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    total_question = table.Column<int>(type: "int", nullable: false),
                    negative_marking = table.Column<decimal>(type: "decimal(10,4)", nullable: false, defaultValue: 0m),
                    question_marking = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    question_set_type = table.Column<int>(type: "int", nullable: false),
                    allowed_retake = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    duration = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    end_time = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSets", x => x.id);
                    table.ForeignKey(
                        name: "FK_QuestionSets_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSets_created_by",
                table: "QuestionSets",
                column: "created_by");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionSets");

            migrationBuilder.DropColumn(
                name: "duration",
                table: "Meetings");

            migrationBuilder.RenameColumn(
                name: "passcode",
                table: "Meetings",
                newName: "PassCode");
        }
    }
}
