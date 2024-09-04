using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyKit.Server.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LicenseTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "activated_on",
                table: "Licenses",
                type: "DATETIME",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "expired_on",
                table: "Licenses",
                type: "DATETIME",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "variant_id",
                table: "Licenses",
                type: "INT",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "variant_name",
                table: "Licenses",
                type: "VARCHAR(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "activated_on",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "expired_on",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "variant_id",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "variant_name",
                table: "Licenses");
        }
    }
}
