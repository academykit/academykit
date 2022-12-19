using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class settingtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    key = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    value = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.key);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "key", "value" },
                values: new object[,]
                {
                    { "AWS_AccessKey", null },
                    { "AWS_CloudFront", null },
                    { "AWS_FileBucket", null },
                    { "AWS_RegionEndpoint", null },
                    { "AWS_SecretKey", null },
                    { "AWS_VideoBucket", null },
                    { "Server_FilePath", null },
                    { "Server_Password", null },
                    { "Server_Url", null },
                    { "Server_UserName", null },
                    { "Server_VideoPath", null },
                    { "Storage", "AWS" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
