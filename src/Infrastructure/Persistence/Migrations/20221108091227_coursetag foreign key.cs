using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class coursetagforeignkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTags_Users_UserId",
                table: "CourseTags");

            migrationBuilder.DropIndex(
                name: "IX_CourseTags_UserId",
                table: "CourseTags");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CourseTags");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTags_created_by",
                table: "CourseTags",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTags_Users_created_by",
                table: "CourseTags",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTags_Users_created_by",
                table: "CourseTags");

            migrationBuilder.DropIndex(
                name: "IX_CourseTags_created_by",
                table: "CourseTags");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CourseTags",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTags_UserId",
                table: "CourseTags",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTags_Users_UserId",
                table: "CourseTags",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
