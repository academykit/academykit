using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class mcqtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionPools",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slug = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                    table.PrimaryKey("PK_QuestionPools", x => x.id);
                    table.ForeignKey(
                        name: "FK_QuestionPools_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    description = table.Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    hints = table.Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: false)
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
                    table.PrimaryKey("PK_Questions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Questions_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    option = table.Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order = table.Column<int>(type: "int", nullable: false),
                    question_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_correct = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_Questions_question_id",
                        column: x => x.question_id,
                        principalTable: "Questions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_QuestionOptions_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestionPoolQuestions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    question_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    question_pool_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
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
                    table.PrimaryKey("PK_QuestionPoolQuestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_QuestionPoolQuestions_QuestionPools_question_pool_id",
                        column: x => x.question_pool_id,
                        principalTable: "QuestionPools",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_QuestionPoolQuestions_Questions_question_id",
                        column: x => x.question_id,
                        principalTable: "Questions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_QuestionPoolQuestions_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestionSetQuestions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    question_set_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    question_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order = table.Column<int>(type: "int", nullable: false),
                    question_pool_question_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
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
                    table.PrimaryKey("PK_QuestionSetQuestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_QuestionSetQuestions_QuestionPoolQuestions_question_pool_que~",
                        column: x => x.question_pool_question_id,
                        principalTable: "QuestionPoolQuestions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_QuestionSetQuestions_Questions_question_id",
                        column: x => x.question_id,
                        principalTable: "Questions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_QuestionSetQuestions_QuestionSets_question_set_id",
                        column: x => x.question_set_id,
                        principalTable: "QuestionSets",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_QuestionSetQuestions_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_created_by",
                table: "QuestionOptions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_question_id",
                table: "QuestionOptions",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPoolQuestions_created_by",
                table: "QuestionPoolQuestions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPoolQuestions_question_id",
                table: "QuestionPoolQuestions",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPoolQuestions_question_pool_id",
                table: "QuestionPoolQuestions",
                column: "question_pool_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPools_created_by",
                table: "QuestionPools",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_created_by",
                table: "Questions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetQuestions_created_by",
                table: "QuestionSetQuestions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetQuestions_question_id",
                table: "QuestionSetQuestions",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetQuestions_question_pool_question_id",
                table: "QuestionSetQuestions",
                column: "question_pool_question_id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSetQuestions_question_set_id",
                table: "QuestionSetQuestions",
                column: "question_set_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropTable(
                name: "QuestionSetQuestions");

            migrationBuilder.DropTable(
                name: "QuestionPoolQuestions");

            migrationBuilder.DropTable(
                name: "QuestionPools");

            migrationBuilder.DropTable(
                name: "Questions");
        }
    }
}
