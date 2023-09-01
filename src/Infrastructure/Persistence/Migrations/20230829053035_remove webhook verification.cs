using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class removewebhookverification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "webhook_verification_key",
                table: "ZoomSettings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "webhook_verification_key",
                table: "ZoomSettings",
                type: "VARCHAR(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "ZoomSettings",
                keyColumn: "id",
                keyValue: "f41a902f-fabd-4749-ac28-91137f685cb8",
                column: "webhook_verification_key",
                value: null);
        }
    }
}
