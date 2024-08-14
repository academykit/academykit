using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyKit.Server.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SupportExternalUrlForLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .AlterColumn<int>(
                    name: "Id",
                    table: "Logs",
                    type: "int",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "int"
                )
                .Annotation(
                    "MySql:ValueGenerationStrategy",
                    MySqlValueGenerationStrategy.IdentityColumn
                );

            migrationBuilder
                .AddColumn<string>(
                    name: "external_url",
                    table: "Lessons",
                    type: "VARCHAR(500)",
                    maxLength: 500,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "external_url", table: "Lessons");

            migrationBuilder
                .AlterColumn<int>(
                    name: "Id",
                    table: "Logs",
                    type: "int",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "int"
                )
                .OldAnnotation(
                    "MySql:ValueGenerationStrategy",
                    MySqlValueGenerationStrategy.IdentityColumn
                );
        }
    }
}
