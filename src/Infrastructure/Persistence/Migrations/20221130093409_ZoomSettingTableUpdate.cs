using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class ZoomSettingTableUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "secret_key",
                table: "ZoomSettings");

            migrationBuilder.AlterColumn<string>(
                name: "api_key",
                table: "ZoomSettings",
                type: "VARCHAR(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(250)",
                oldMaxLength: 250)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "api_secret",
                table: "ZoomSettings",
                type: "VARCHAR(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "sdk_key",
                table: "ZoomSettings",
                type: "VARCHAR(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "sdk_secret",
                table: "ZoomSettings",
                type: "VARCHAR(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "ZoomSettings",
                keyColumn: "id",
                keyValue: "f41a902f-fabd-4749-ac28-91137f685cb8",
                columns: new[] { "api_secret", "sdk_key", "sdk_secret" },
                values: new object[] { "api_secret value", "sdk key value", "sdk secret value" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "api_secret",
                table: "ZoomSettings");

            migrationBuilder.DropColumn(
                name: "sdk_key",
                table: "ZoomSettings");

            migrationBuilder.DropColumn(
                name: "sdk_secret",
                table: "ZoomSettings");

            migrationBuilder.AlterColumn<string>(
                name: "api_key",
                table: "ZoomSettings",
                type: "VARCHAR(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "secret_key",
                table: "ZoomSettings",
                type: "VARCHAR(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "ZoomSettings",
                keyColumn: "id",
                keyValue: "f41a902f-fabd-4749-ac28-91137f685cb8",
                column: "secret_key",
                value: "secret key value");
        }
    }
}
