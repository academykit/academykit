using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class updateenrollmenttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "Feedbacks");

            migrationBuilder.AddColumn<DateTime>(
                name: "certificate_issued_date",
                table: "CourseEnrollments",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "certificate_url",
                table: "CourseEnrollments",
                type: "VARCHAR(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "has_certificate_issued",
                table: "CourseEnrollments",
                type: "tinyint(1)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "certificate_issued_date",
                table: "CourseEnrollments");

            migrationBuilder.DropColumn(
                name: "certificate_url",
                table: "CourseEnrollments");

            migrationBuilder.DropColumn(
                name: "has_certificate_issued",
                table: "CourseEnrollments");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Feedbacks",
                type: "VARCHAR(5000)",
                maxLength: 5000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
