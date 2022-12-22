using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class updatezoomsettingtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "webhook_secret",
                table: "ZoomSettings",
                type: "VARCHAR(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "webhook_verification_key",
                table: "ZoomSettings",
                type: "VARCHAR(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "webhook_secret",
                table: "ZoomSettings");

            migrationBuilder.DropColumn(
                name: "webhook_verification_key",
                table: "ZoomSettings");
        }
    }
}
