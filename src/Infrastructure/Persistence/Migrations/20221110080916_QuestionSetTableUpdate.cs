using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class QuestionSetTableUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lessons_meeting_id",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "question_set_type",
                table: "QuestionSets");

            migrationBuilder.DropColumn(
                name: "total_question",
                table: "QuestionSets");

            migrationBuilder.AddColumn<decimal>(
                name: "passing_weightage",
                table: "QuestionSets",
                type: "decimal(10,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "is_mandatory",
                table: "Lessons",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "question_set_id",
                table: "Lessons",
                type: "VARCHAR(50)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_meeting_id",
                table: "Lessons",
                column: "meeting_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_question_set_id",
                table: "Lessons",
                column: "question_set_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_QuestionSets_question_set_id",
                table: "Lessons",
                column: "question_set_id",
                principalTable: "QuestionSets",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_QuestionSets_question_set_id",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_meeting_id",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_question_set_id",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "passing_weightage",
                table: "QuestionSets");

            migrationBuilder.DropColumn(
                name: "is_mandatory",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "question_set_id",
                table: "Lessons");

            migrationBuilder.AddColumn<int>(
                name: "question_set_type",
                table: "QuestionSets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_question",
                table: "QuestionSets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_meeting_id",
                table: "Lessons",
                column: "meeting_id");
        }
    }
}
