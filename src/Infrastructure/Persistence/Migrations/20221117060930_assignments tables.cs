using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class assignmentstables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_QuestionSetQuestions_QuestionSe~",
                table: "QuestionSetSubmissionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_QuestionSetSubmissions_Question~",
                table: "QuestionSetSubmissionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_Users_CreatedBy",
                table: "QuestionSetSubmissionAnswers");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "QuestionSetSubmissionAnswers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "QuestionSetSubmissionAnswers",
                newName: "updated_on");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "QuestionSetSubmissionAnswers",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "SelectedAnswers",
                table: "QuestionSetSubmissionAnswers",
                newName: "selected_answers");

            migrationBuilder.RenameColumn(
                name: "QuestionSetSubmissionId",
                table: "QuestionSetSubmissionAnswers",
                newName: "question_set_submission_id");

            migrationBuilder.RenameColumn(
                name: "QuestionSetQuestionId",
                table: "QuestionSetSubmissionAnswers",
                newName: "question_set_question_id");

            migrationBuilder.RenameColumn(
                name: "IsCorrect",
                table: "QuestionSetSubmissionAnswers",
                newName: "is_correct");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "QuestionSetSubmissionAnswers",
                newName: "created_on");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "QuestionSetSubmissionAnswers",
                newName: "created_by");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionSetSubmissionAnswers_QuestionSetSubmissionId",
                table: "QuestionSetSubmissionAnswers",
                newName: "IX_QuestionSetSubmissionAnswers_question_set_submission_id");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionSetSubmissionAnswers_QuestionSetQuestionId",
                table: "QuestionSetSubmissionAnswers",
                newName: "IX_QuestionSetSubmissionAnswers_question_set_question_id");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionSetSubmissionAnswers_CreatedBy",
                table: "QuestionSetSubmissionAnswers",
                newName: "IX_QuestionSetSubmissionAnswers_created_by");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "QuestionSetSubmissionAnswers",
                type: "VARCHAR(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_on",
                table: "QuestionSetSubmissionAnswers",
                type: "DATETIME",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "QuestionSetSubmissionAnswers",
                type: "VARCHAR(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "selected_answers",
                table: "QuestionSetSubmissionAnswers",
                type: "VARCHAR(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<bool>(
                name: "is_correct",
                table: "QuestionSetSubmissionAnswers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_on",
                table: "QuestionSetSubmissionAnswers",
                type: "DATETIME",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lesson_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
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
                    table.PrimaryKey("PK_Assignments", x => x.id);
                    table.ForeignKey(
                        name: "FK_Assignments_Lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "Lessons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Assignments_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Recordings",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "VARCHAR(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slug = table.Column<string>(type: "VARCHAR(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lesson_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    video_url = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    duration = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Order = table.Column<int>(type: "int", nullable: false),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "AssignmentAttachments",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    assignment_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_url = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    name = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mime_type = table.Column<string>(type: "VARCHAR(50)", nullable: true)
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
                    table.PrimaryKey("PK_AssignmentAttachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_AssignmentAttachments_Assignments_assignment_id",
                        column: x => x.assignment_id,
                        principalTable: "Assignments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_AssignmentAttachments_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AssignmentQuestions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    question_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    assignment_id = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "AssignmentMCQSubmissions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    assignment_id = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    assignment_question_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_correct = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    selected_option = table.Column<string>(type: "VARCHAR(300)", maxLength: 300, nullable: true)
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
                    table.PrimaryKey("PK_AssignmentMCQSubmissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_AssignmentMCQSubmissions_AssignmentQuestions_assignment_ques~",
                        column: x => x.assignment_question_id,
                        principalTable: "AssignmentQuestions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_AssignmentMCQSubmissions_Assignments_assignment_id",
                        column: x => x.assignment_id,
                        principalTable: "Assignments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_AssignmentMCQSubmissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentAttachments_assignment_id",
                table: "AssignmentAttachments",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentAttachments_created_by",
                table: "AssignmentAttachments",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentMCQSubmissions_assignment_id",
                table: "AssignmentMCQSubmissions",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentMCQSubmissions_assignment_question_id",
                table: "AssignmentMCQSubmissions",
                column: "assignment_question_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentMCQSubmissions_UserId",
                table: "AssignmentMCQSubmissions",
                column: "UserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_created_by",
                table: "Assignments",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_lesson_id",
                table: "Assignments",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "IX_Recordings_lesson_id",
                table: "Recordings",
                column: "lesson_id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_QuestionSetQuestions_question_s~",
                table: "QuestionSetSubmissionAnswers",
                column: "question_set_question_id",
                principalTable: "QuestionSetQuestions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_QuestionSetSubmissions_question~",
                table: "QuestionSetSubmissionAnswers",
                column: "question_set_submission_id",
                principalTable: "QuestionSetSubmissions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_Users_created_by",
                table: "QuestionSetSubmissionAnswers",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_QuestionSetQuestions_question_s~",
                table: "QuestionSetSubmissionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_QuestionSetSubmissions_question~",
                table: "QuestionSetSubmissionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_Users_created_by",
                table: "QuestionSetSubmissionAnswers");

            migrationBuilder.DropTable(
                name: "AssignmentAttachments");

            migrationBuilder.DropTable(
                name: "AssignmentMCQSubmissions");

            migrationBuilder.DropTable(
                name: "Recordings");

            migrationBuilder.DropTable(
                name: "AssignmentQuestions");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "QuestionSetSubmissionAnswers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "QuestionSetSubmissionAnswers",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "QuestionSetSubmissionAnswers",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "selected_answers",
                table: "QuestionSetSubmissionAnswers",
                newName: "SelectedAnswers");

            migrationBuilder.RenameColumn(
                name: "question_set_submission_id",
                table: "QuestionSetSubmissionAnswers",
                newName: "QuestionSetSubmissionId");

            migrationBuilder.RenameColumn(
                name: "question_set_question_id",
                table: "QuestionSetSubmissionAnswers",
                newName: "QuestionSetQuestionId");

            migrationBuilder.RenameColumn(
                name: "is_correct",
                table: "QuestionSetSubmissionAnswers",
                newName: "IsCorrect");

            migrationBuilder.RenameColumn(
                name: "created_on",
                table: "QuestionSetSubmissionAnswers",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "QuestionSetSubmissionAnswers",
                newName: "CreatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionSetSubmissionAnswers_question_set_submission_id",
                table: "QuestionSetSubmissionAnswers",
                newName: "IX_QuestionSetSubmissionAnswers_QuestionSetSubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionSetSubmissionAnswers_question_set_question_id",
                table: "QuestionSetSubmissionAnswers",
                newName: "IX_QuestionSetSubmissionAnswers_QuestionSetQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionSetSubmissionAnswers_created_by",
                table: "QuestionSetSubmissionAnswers",
                newName: "IX_QuestionSetSubmissionAnswers_CreatedBy");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "QuestionSetSubmissionAnswers",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)",
                oldMaxLength: 50)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedOn",
                table: "QuestionSetSubmissionAnswers",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "QuestionSetSubmissionAnswers",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "SelectedAnswers",
                table: "QuestionSetSubmissionAnswers",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(150)",
                oldMaxLength: 150)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "QuestionSetSubmissionAnswers",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "QuestionSetSubmissionAnswers",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_QuestionSetQuestions_QuestionSe~",
                table: "QuestionSetSubmissionAnswers",
                column: "QuestionSetQuestionId",
                principalTable: "QuestionSetQuestions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_QuestionSetSubmissions_Question~",
                table: "QuestionSetSubmissionAnswers",
                column: "QuestionSetSubmissionId",
                principalTable: "QuestionSetSubmissions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSetSubmissionAnswers_Users_CreatedBy",
                table: "QuestionSetSubmissionAnswers",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "id");
        }
    }
}
