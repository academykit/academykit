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
                table: "QuestionPoolQuestions",
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
                nullable: true,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order",
                table: "QuestionPoolQuestions");

            migrationBuilder.DropColumn(
                name: "custom_configuration",
                table: "GeneralSettings");

            migrationBuilder.DropColumn(
                name: "optional_cost",
                table: "Certificates");
        }
    }
}
