using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class update_log_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageTemplate",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "Logs",
                newName: "Logged");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Logs",
                type: "VARCHAR(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(1000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Logger",
                table: "Logs",
                type: "VARCHAR(400)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MachineName",
                table: "Logs",
                type: "VARCHAR(200)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logger",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "MachineName",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "Logged",
                table: "Logs",
                newName: "TimeStamp");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Logs",
                type: "VARCHAR(1000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(4000)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MessageTemplate",
                table: "Logs",
                type: "VARCHAR(2000)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
