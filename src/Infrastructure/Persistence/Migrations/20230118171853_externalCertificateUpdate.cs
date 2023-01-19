using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class externalCertificateUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VARCHAR(100)",
                table: "Certificates",
                newName: "institute");

            migrationBuilder.AlterColumn<string>(
                name: "institute",
                table: "Certificates",
                type: "VARCHAR(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "institute",
                table: "Certificates",
                newName: "VARCHAR(100)");

            migrationBuilder.UpdateData(
                table: "Certificates",
                keyColumn: "VARCHAR(100)",
                keyValue: null,
                column: "VARCHAR(100)",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "VARCHAR(100)",
                table: "Certificates",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
