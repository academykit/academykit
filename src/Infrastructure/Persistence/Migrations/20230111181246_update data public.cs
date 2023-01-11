using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class updatedatapublic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_PrivatePath");

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_PublicPath");

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_VideoPath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "key", "value" },
                values: new object[] { "Server_PrivatePath", null });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "key", "value" },
                values: new object[] { "Server_PublicPath", null });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "key", "value" },
                values: new object[] { "Server_VideoPath", null });
        }
    }
}
