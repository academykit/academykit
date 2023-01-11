using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class Dataseed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_FilePath");

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_Password");

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_UserName");

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_VideoPath");

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "key", "value" },
                values: new object[,]
                {
                    { "Server_AccessKey", null },
                    { "Server_PrivateBucket", null },
                    { "Server_PublicBucket", null },
                    { "Server_SecretKey", null },
                    { "Server_VideoBucket", null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_AccessKey");

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_PrivateBucket");

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_PublicBucket");

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_SecretKey");

            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_VideoBucket");

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "key", "value" },
                values: new object[,]
                {
                    { "Server_FilePath", null },
                    { "Server_Password", null },
                    { "Server_UserName", null },
                    { "Server_VideoPath", null }
                });
        }
    }
}
