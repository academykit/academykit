using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class CourseCertificateSampleUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sample_url",
                table: "CourseCertificate",
                type: "VARCHAR(500)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCertificate_created_by",
                table: "CourseCertificate",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseCertificate_Users_created_by",
                table: "CourseCertificate",
                column: "created_by",
                principalTable: "Users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseCertificate_Users_created_by",
                table: "CourseCertificate");

            migrationBuilder.DropIndex(
                name: "IX_CourseCertificate_created_by",
                table: "CourseCertificate");

            migrationBuilder.DropColumn(
                name: "sample_url",
                table: "CourseCertificate");
        }
    }
}
