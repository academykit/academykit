using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class update_user_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: "30fcd978-f256-4733-840f-759181bc5e63",
                column: "status",
                value: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: "30fcd978-f256-4733-840f-759181bc5e63",
                column: "is_active",
                value: true);
        }
    }
}
