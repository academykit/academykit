using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class updateQuestionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentMCQSubmissions_Users_UserId",
                table: "AssignmentMCQSubmissions");

            migrationBuilder.DropColumn(
                name: "Hint",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AssignmentMCQSubmissions",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_AssignmentMCQSubmissions_UserId",
                table: "AssignmentMCQSubmissions",
                newName: "IX_AssignmentMCQSubmissions_user_id");

            migrationBuilder.AlterColumn<int>(
                name: "type",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "hints",
                table: "Assignments",
                type: "VARCHAR(250)",
                maxLength: 250,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "assignment_id",
                table: "AssignmentMCQSubmissions",
                type: "VARCHAR(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)",
                oldMaxLength: 20)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentMCQSubmissions_Users_user_id",
                table: "AssignmentMCQSubmissions",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentMCQSubmissions_Users_user_id",
                table: "AssignmentMCQSubmissions");

            migrationBuilder.DropColumn(
                name: "hints",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "AssignmentMCQSubmissions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignmentMCQSubmissions_user_id",
                table: "AssignmentMCQSubmissions",
                newName: "IX_AssignmentMCQSubmissions_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "type",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "Hint",
                table: "Assignments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "assignment_id",
                table: "AssignmentMCQSubmissions",
                type: "VARCHAR(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentMCQSubmissions_Users_UserId",
                table: "AssignmentMCQSubmissions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id");
        }
    }
}
