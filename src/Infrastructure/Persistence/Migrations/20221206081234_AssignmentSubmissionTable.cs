using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class AssignmentSubmissionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentMCQSubmissions");

            migrationBuilder.CreateTable(
                name: "AssignmentSubmissions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    assignment_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_correct = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    selected_option = table.Column<string>(type: "VARCHAR(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    answer = table.Column<string>(type: "VARCHAR(5000)", maxLength: 5000, nullable: true)
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
                    table.PrimaryKey("PK_AssignmentSubmissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_AssignmentSubmissions_Assignments_assignment_id",
                        column: x => x.assignment_id,
                        principalTable: "Assignments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_AssignmentSubmissions_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AssignmentSubmissionAttachments",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    assignment_submission_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mime_type = table.Column<string>(type: "VARCHAR(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_url = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
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
                    table.PrimaryKey("PK_AssignmentSubmissionAttachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_AssignmentSubmissionAttachments_AssignmentSubmissions_assign~",
                        column: x => x.assignment_submission_id,
                        principalTable: "AssignmentSubmissions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_AssignmentSubmissionAttachments_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissionAttachments_assignment_submission_id",
                table: "AssignmentSubmissionAttachments",
                column: "assignment_submission_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissionAttachments_created_by",
                table: "AssignmentSubmissionAttachments",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_assignment_id",
                table: "AssignmentSubmissions",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_user_id",
                table: "AssignmentSubmissions",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentSubmissionAttachments");

            migrationBuilder.DropTable(
                name: "AssignmentSubmissions");

            migrationBuilder.CreateTable(
                name: "AssignmentMCQSubmissions",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    assignment_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    is_correct = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    selected_option = table.Column<string>(type: "VARCHAR(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentMCQSubmissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_AssignmentMCQSubmissions_Assignments_assignment_id",
                        column: x => x.assignment_id,
                        principalTable: "Assignments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_AssignmentMCQSubmissions_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentMCQSubmissions_assignment_id",
                table: "AssignmentMCQSubmissions",
                column: "assignment_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentMCQSubmissions_user_id",
                table: "AssignmentMCQSubmissions",
                column: "user_id");
        }
    }
}
