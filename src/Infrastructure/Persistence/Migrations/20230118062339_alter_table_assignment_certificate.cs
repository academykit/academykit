using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class alter_table_assignment_certificate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_verified",
                table: "Certificates");

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "Certificates",
                type: "DATETIME",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Certificates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "Assignments",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "Assignments",
                type: "DATETIME",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_date",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "end_date",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "Assignments");

            migrationBuilder.AddColumn<bool>(
                name: "is_verified",
                table: "Certificates",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
