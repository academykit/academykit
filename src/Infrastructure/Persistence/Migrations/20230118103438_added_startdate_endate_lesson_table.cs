using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class added_startdate_endate_lesson_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_date",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "Assignments");

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "Lessons",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "Lessons",
                type: "DATETIME",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_date",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "Lessons");

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
    }
}
