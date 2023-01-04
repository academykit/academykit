using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class ExamResultTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionSetSubmissions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    question_set_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId1 = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_time = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    end_time = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSetSubmissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_QuestionSetSubmissions_QuestionSets_question_set_id",
                        column: x => x.question_set_id,
                        principalTable: "QuestionSets",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_QuestionSetSubmissions_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestionSetResults",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    question_set_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    question_set_submission_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    total_mark = table.Column<decimal>(type: "decimal(20,4)", nullable: false),
                    negative_mark = table.Column<decimal>(type: "decimal(20,4)", nullable: false),
                    UserId1 = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSetResults", x => x.id);
                    table.ForeignKey(
                        name: "FK_QuestionSetResults_QuestionSets_question_set_id",
                        column: x => x.question_set_id,
                        principalTable: "QuestionSets",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_QuestionSetResults_QuestionSetSubmissions_question_set_submi~",
                        column: x => x.question_set_submission_id,
                        principalTable: "QuestionSetSubmissions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_QuestionSetResults_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestionSetSubmissionAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    QuestionSetSubmissionId = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QuestionSetQuestionId = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SelectedAnswers = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsCorrect = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UserId = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedBy = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UpdatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSetSubmissionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionSetSubmissionAnswers_QuestionSetQuestions_QuestionSe~",
                        column: x => x.QuestionSetQuestionId,
                        principalTable: "QuestionSetQuestions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionSetSubmissionAnswers_QuestionSetSubmissions_Question~",
                        column: x => x.QuestionSetSubmissionId,
                        principalTable: "QuestionSetSubmissions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_QuestionSetSubmissionAnswers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetResults_question_set_id",
                table: "QuestionSetResults",
                column: "question_set_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetResults_question_set_submission_id",
                table: "QuestionSetResults",
                column: "question_set_submission_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetResults_UserId1",
                table: "QuestionSetResults",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissionAnswers_QuestionSetQuestionId",
                table: "QuestionSetSubmissionAnswers",
                column: "QuestionSetQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissionAnswers_QuestionSetSubmissionId",
                table: "QuestionSetSubmissionAnswers",
                column: "QuestionSetSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissionAnswers_UserId",
                table: "QuestionSetSubmissionAnswers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissions_question_set_id",
                table: "QuestionSetSubmissions",
                column: "question_set_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissions_UserId1",
                table: "QuestionSetSubmissions",
                column: "UserId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionSetResults");

            migrationBuilder.DropTable(
                name: "QuestionSetSubmissionAnswers");

            migrationBuilder.DropTable(
                name: "QuestionSetSubmissions");
        }
    }
}
