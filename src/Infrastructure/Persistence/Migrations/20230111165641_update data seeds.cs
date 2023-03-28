using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class updatedataseeds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                keyValue: "Server_VideoBucket");

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "key", "value" },
                values: new object[,]
                {
                    { "Server_Bucket", null },
                    { "Server_PrivatePath", null },
                    { "Server_PublicPath", null },
                    { "Server_VideoPath", null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "key",
                keyValue: "Server_Bucket");

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

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "key", "value" },
                values: new object[] { "Server_PrivateBucket", null });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "key", "value" },
                values: new object[] { "Server_PublicBucket", null });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "key", "value" },
                values: new object[] { "Server_VideoBucket", null });
        }
    }
}
