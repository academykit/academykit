using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class Serverdataupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "key", "value" },
                values: new object[] { "Server_EndPoint", null });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "key", "value" },
                values: new object[] { "Server_PresignedExpiryTime", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_EndPoint");

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_PresignedExpiryTime");
        }
    }
}
