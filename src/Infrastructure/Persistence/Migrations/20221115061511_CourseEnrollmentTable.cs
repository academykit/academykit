using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class CourseEnrollmentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSetResults_Users_UserId1",
                table: "QuestionSetResults");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_Users_UserId",
                table: "QuestionSetSubmissionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSetSubmissions_Users_UserId1",
                table: "QuestionSetSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionSetSubmissions_UserId1",
                table: "QuestionSetSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionSetSubmissionAnswers_UserId",
                table: "QuestionSetSubmissionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuestionSetResults_UserId1",
                table: "QuestionSetResults");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "QuestionSetSubmissions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "QuestionSetSubmissionAnswers");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "QuestionSetResults");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "QuestionSetSubmissionAnswers",
                type: "VARCHAR(50)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "CourseEnrollments",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    current_lesson_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    current_lesson_watched = table.Column<int>(type: "int", nullable: false),
                    percentage = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    status = table.Column<int>(type: "int", nullable: false),
                    activity_reason = table.Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    course_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    enrollment_date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    is_deleted = table.Column<ulong>(type: "BIT", nullable: false, defaultValue: 0ul),
                    deleted_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_on = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEnrollments", x => x.id);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_Lessons_current_lesson_id",
                        column: x => x.current_lesson_id,
                        principalTable: "Lessons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WatchHistories",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    course_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lesson_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_completed = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchHistories", x => x.id);
                    table.ForeignKey(
                        name: "FK_WatchHistories_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WatchHistories_Lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "Lessons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_WatchHistories_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissions_user_id",
                table: "QuestionSetSubmissions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissionAnswers_CreatedBy",
                table: "QuestionSetSubmissionAnswers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetResults_user_id",
                table: "QuestionSetResults",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_course_id",
                table: "CourseEnrollments",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_current_lesson_id",
                table: "CourseEnrollments",
                column: "current_lesson_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_user_id",
                table: "CourseEnrollments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_course_id",
                table: "WatchHistories",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_lesson_id",
                table: "WatchHistories",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_user_id",
                table: "WatchHistories",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSetResults_Users_user_id",
                table: "QuestionSetResults",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_Users_CreatedBy",
                table: "QuestionSetSubmissionAnswers",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSetSubmissions_Users_user_id",
                table: "QuestionSetSubmissions",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSetResults_Users_user_id",
                table: "QuestionSetResults");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_Users_CreatedBy",
                table: "QuestionSetSubmissionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSetSubmissions_Users_user_id",
                table: "QuestionSetSubmissions");

            migrationBuilder.DropTable(
                name: "CourseEnrollments");

            migrationBuilder.DropTable(
                name: "WatchHistories");

            migrationBuilder.DropIndex(
                name: "IX_QuestionSetSubmissions_user_id",
                table: "QuestionSetSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionSetSubmissionAnswers_CreatedBy",
                table: "QuestionSetSubmissionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuestionSetResults_user_id",
                table: "QuestionSetResults");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "QuestionSetSubmissions",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "QuestionSetSubmissionAnswers",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "QuestionSetSubmissionAnswers",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "QuestionSetResults",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissions_UserId1",
                table: "QuestionSetSubmissions",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetSubmissionAnswers_UserId",
                table: "QuestionSetSubmissionAnswers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetResults_UserId1",
                table: "QuestionSetResults",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSetResults_Users_UserId1",
                table: "QuestionSetResults",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_Users_UserId",
                table: "QuestionSetSubmissionAnswers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSetSubmissions_Users_UserId1",
                table: "QuestionSetSubmissions",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
