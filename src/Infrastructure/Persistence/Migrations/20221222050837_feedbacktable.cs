using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class feedbacktable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lesson_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "Lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeedbackQuestionOptions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    feedback_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    option = table.Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                    table.PrimaryKey("PK_FeedbackQuestionOptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_FeedbackQuestionOptions_Feedbacks_feedback_id",
                        column: x => x.feedback_id,
                        principalTable: "Feedbacks",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_FeedbackQuestionOptions_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeedbackSubmissions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lesson_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    feedback_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    selected_option = table.Column<string>(type: "VARCHAR(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    answer = table.Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rating = table.Column<int>(type: "int", nullable: true),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackSubmissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_FeedbackSubmissions_Feedbacks_feedback_id",
                        column: x => x.feedback_id,
                        principalTable: "Feedbacks",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_FeedbackSubmissions_Lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "Lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackSubmissions_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackQuestionOptions_created_by",
                table: "FeedbackQuestionOptions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackQuestionOptions_feedback_id",
                table: "FeedbackQuestionOptions",
                column: "feedback_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_created_by",
                table: "Feedbacks",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_lesson_id",
                table: "Feedbacks",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackSubmissions_feedback_id",
                table: "FeedbackSubmissions",
                column: "feedback_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackSubmissions_lesson_id",
                table: "FeedbackSubmissions",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackSubmissions_user_id",
                table: "FeedbackSubmissions",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedbackQuestionOptions");

            migrationBuilder.DropTable(
                name: "FeedbackSubmissions");

            migrationBuilder.DropTable(
                name: "Feedbacks");
        }
    }
}
