using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class questionseterrortableupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_submission_error",
                table: "QuestionSetSubmissions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "submission_error_message",
                table: "QuestionSetSubmissions",
                type: "VARCHAR(250)",
                maxLength: 250,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_submission_error",
                table: "QuestionSetSubmissions");

            migrationBuilder.DropColumn(
                name: "submission_error_message",
                table: "QuestionSetSubmissions");
        }
    }
}
