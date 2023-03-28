using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class AssignmentTableUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentMCQSubmissions_AssignmentQuestions_assignment_ques~",
                table: "AssignmentMCQSubmissions");

            migrationBuilder.DropTable(
                name: "AssignmentQuestions");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentMCQSubmissions_assignment_question_id",
                table: "AssignmentMCQSubmissions");

            migrationBuilder.DropColumn(
                name: "assignment_question_id",
                table: "AssignmentMCQSubmissions");

            migrationBuilder.AddColumn<string>(
                name: "Hint",
                table: "Assignments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AssignmentQuestionOptions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    assignment_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    option = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_correct = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    order = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentQuestionOptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_AssignmentQuestionOptions_Assignments_assignment_id",
                        column: x => x.assignment_id,
                        principalTable: "Assignments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_AssignmentQuestionOptions_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentQuestionOptions_assignment_id",
                table: "AssignmentQuestionOptions",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentQuestionOptions_created_by",
                table: "AssignmentQuestionOptions",
                column: "created_by");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentQuestionOptions");

            migrationBuilder.DropColumn(
                name: "Hint",
                table: "Assignments");

            migrationBuilder.AddColumn<string>(
                name: "assignment_question_id",
                table: "AssignmentMCQSubmissions",
                type: "VARCHAR(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AssignmentQuestions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    assignment_id = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    question_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentQuestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_AssignmentQuestions_Assignments_assignment_id",
                        column: x => x.assignment_id,
                        principalTable: "Assignments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_AssignmentQuestions_Questions_question_id",
                        column: x => x.question_id,
                        principalTable: "Questions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_AssignmentQuestions_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentMCQSubmissions_assignment_question_id",
                table: "AssignmentMCQSubmissions",
                column: "assignment_question_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentQuestions_assignment_id",
                table: "AssignmentQuestions",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentQuestions_created_by",
                table: "AssignmentQuestions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentQuestions_question_id",
                table: "AssignmentQuestions",
                column: "question_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentMCQSubmissions_AssignmentQuestions_assignment_ques~",
                table: "AssignmentMCQSubmissions",
                column: "assignment_question_id",
                principalTable: "AssignmentQuestions",
                principalColumn: "id");
        }
    }
}
