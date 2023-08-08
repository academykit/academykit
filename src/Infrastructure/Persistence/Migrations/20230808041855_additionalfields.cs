using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class additionalfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "order",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "custom_configuration",
                table: "GeneralSettings",
                type: "VARCHAR(5000)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "optional_cost",
                table: "Certificates",
                type: "DECIMAL(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "GeneralSettings",
                keyColumn: "id",
                keyValue: "2d7867fc-b7e7-461d-9257-d0990b5ac991",
                column: "custom_configuration",
                value: null);

            migrationBuilder.InsertData(
                table: "Levels",
                columns: new[] { "id", "created_by", "created_on", "name", "slug", "updated_by", "updated_on" },
                values: new object[,]
                {
                    { "7df8d749-6172-482b-b5a1-016fbe478795", "30fcd978-f256-4733-840f-759181bc5e63", new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004), "Intermediate", "intermediate", null, null },
                    { "7e6ff101-cfa2-4aec-bd25-42780be476c3", "30fcd978-f256-4733-840f-759181bc5e63", new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004), "Beginner", "beginner", null, null },
                    { "9be84cd8-1566-4af5-8442-61cb1796dc46", "30fcd978-f256-4733-840f-759181bc5e63", new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004), "Advanced", "advanced", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Levels",
                keyColumn: "id",
                keyValue: "7df8d749-6172-482b-b5a1-016fbe478795");

            migrationBuilder.DeleteData(
                table: "Levels",
                keyColumn: "id",
                keyValue: "7e6ff101-cfa2-4aec-bd25-42780be476c3");

            migrationBuilder.DeleteData(
                table: "Levels",
                keyColumn: "id",
                keyValue: "9be84cd8-1566-4af5-8442-61cb1796dc46");

            migrationBuilder.DropColumn(
                name: "order",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "custom_configuration",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "optional_cost",
                table: "Certificates");
        }
    }
}
