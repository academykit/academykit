using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class AssignmentSubmissionTableUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "lesson_id",
                table: "AssignmentSubmissions",
                type: "VARCHAR(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSubmissions_lesson_id",
                table: "AssignmentSubmissions",
                column: "lesson_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmissions_Lessons_lesson_id",
                table: "AssignmentSubmissions",
                column: "lesson_id",
                principalTable: "Lessons",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentSubmissions_Lessons_lesson_id",
                table: "AssignmentSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentSubmissions_lesson_id",
                table: "AssignmentSubmissions");

            migrationBuilder.DropColumn(
                name: "lesson_id",
                table: "AssignmentSubmissions");
        }
    }
}
