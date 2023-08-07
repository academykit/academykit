using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class zoom_table_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "api_secret",
                table: "ZoomSettings",
                newName: "oauth_client_secret");

            migrationBuilder.RenameColumn(
                name: "api_key",
                table: "ZoomSettings",
                newName: "oauth_client_id");

            migrationBuilder.AddColumn<string>(
                name: "oauth_account_id",
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
                columns: new[] { "oauth_account_id", "oauth_client_id", "oauth_client_secret" },
                values: new object[] { "OAuth account id", "OAuth client id", "OAuth client secret" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "oauth_account_id",
                table: "ZoomSettings");

            migrationBuilder.RenameColumn(
                name: "oauth_client_secret",
                table: "ZoomSettings",
                newName: "api_secret");

            migrationBuilder.RenameColumn(
                name: "oauth_client_id",
                table: "ZoomSettings",
                newName: "api_key");

            migrationBuilder.UpdateData(
                table: "ZoomSettings",
                keyColumn: "id",
                keyValue: "f41a902f-fabd-4749-ac28-91137f685cb8",
                columns: new[] { "api_key", "api_secret" },
                values: new object[] { "api_key value", "api_secret value" });
        }
    }
}
