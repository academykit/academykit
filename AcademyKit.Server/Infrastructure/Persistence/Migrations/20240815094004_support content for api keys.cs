using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyKit.Server.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class supportcontentforapikeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .CreateTable(
                    name: "ApiKeys",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        key = table
                            .Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        user_id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                        updated_by = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_ApiKeys", x => x.id);
                        table.ForeignKey(
                            name: "FK_ApiKeys_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_user_id",
                table: "ApiKeys",
                column: "user_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ApiKeys");
        }
    }
}
