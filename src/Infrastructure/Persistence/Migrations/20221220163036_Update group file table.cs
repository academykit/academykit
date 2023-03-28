using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class Updategroupfiletable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentReviews_AssignmentSubmissions_AssignmentSubmission~",
                table: "AssignmentReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentReviews_Users_created_by",
                table: "AssignmentReviews");

            migrationBuilder.DropTable(
                name: "GroupStorages");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentReviews_created_by",
                table: "AssignmentReviews");

            migrationBuilder.RenameColumn(
                name: "AssignmentSubmissionId",
                table: "AssignmentReviews",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_AssignmentReviews_AssignmentSubmissionId",
                table: "AssignmentReviews",
                newName: "IX_AssignmentReviews_user_id");

            migrationBuilder.AddColumn<string>(
                name: "lesson_id",
                table: "AssignmentReviews",
                type: "VARCHAR(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GroupFile",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GroupId = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    key = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Size = table.Column<double>(type: "double", nullable: false),
                    url = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mime_type = table.Column<string>(type: "VARCHAR(50)", nullable: false)
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
                    table.PrimaryKey("PK_GroupFile", x => x.id);
                    table.ForeignKey(
                        name: "FK_GroupFile_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_GroupFile_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentReviews_lesson_id",
                table: "AssignmentReviews",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupFile_created_by",
                table: "GroupFile",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_GroupFile_GroupId",
                table: "GroupFile",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentReviews_Lessons_lesson_id",
                table: "AssignmentReviews",
                column: "lesson_id",
                principalTable: "Lessons",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentReviews_Users_user_id",
                table: "AssignmentReviews",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentReviews_Lessons_lesson_id",
                table: "AssignmentReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentReviews_Users_user_id",
                table: "AssignmentReviews");

            migrationBuilder.DropTable(
                name: "GroupFile");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentReviews_lesson_id",
                table: "AssignmentReviews");

            migrationBuilder.DropColumn(
                name: "lesson_id",
                table: "AssignmentReviews");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AssignmentReviews",
                newName: "AssignmentSubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignmentReviews_user_id",
                table: "AssignmentReviews",
                newName: "IX_AssignmentReviews_AssignmentSubmissionId");

            migrationBuilder.CreateTable(
                name: "GroupStorages",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GroupId = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    key = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mime_type = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    url = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupStorages", x => x.id);
                    table.ForeignKey(
                        name: "FK_GroupStorages_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_GroupStorages_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentReviews_created_by",
                table: "AssignmentReviews",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_GroupStorages_created_by",
                table: "GroupStorages",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_GroupStorages_GroupId",
                table: "GroupStorages",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentReviews_AssignmentSubmissions_AssignmentSubmission~",
                table: "AssignmentReviews",
                column: "AssignmentSubmissionId",
                principalTable: "AssignmentSubmissions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentReviews_Users_created_by",
                table: "AssignmentReviews",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id");
        }
    }
}
