using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class QuestionTagFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTags_Users_UserId",
                table: "QuestionTags");

            migrationBuilder.DropIndex(
                name: "IX_QuestionTags_UserId",
                table: "QuestionTags");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "QuestionTags");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTags_created_by",
                table: "QuestionTags",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTags_Users_created_by",
                table: "QuestionTags",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTags_Users_created_by",
                table: "QuestionTags");

            migrationBuilder.DropIndex(
                name: "IX_QuestionTags_created_by",
                table: "QuestionTags");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "QuestionTags",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTags_UserId",
                table: "QuestionTags",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTags_Users_UserId",
                table: "QuestionTags",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
