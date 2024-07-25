using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserSkillsConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSkills_Skills_SkillsId",
                table: "UserSkills"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_UserSkills", table: "UserSkills");

            migrationBuilder.DropIndex(name: "IX_UserSkills_SkillsId", table: "UserSkills");

            migrationBuilder.DropColumn(name: "SkillsId", table: "UserSkills");

            migrationBuilder
                .AlterColumn<string>(
                    name: "SkillId",
                    table: "UserSkills",
                    type: "VARCHAR(50)",
                    nullable: false,
                    oldClrType: typeof(Guid),
                    oldType: "char(36)"
                )
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSkills",
                table: "UserSkills",
                columns: new[] { "SkillId", "UserId" }
            );

            migrationBuilder
                .CreateTable(
                    name: "SkillsUser",
                    columns: table => new
                    {
                        SkillsId = table
                            .Column<string>(type: "VARCHAR(50)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        UsersId = table
                            .Column<string>(type: "VARCHAR(50)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4")
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_SkillsUser", x => new { x.SkillsId, x.UsersId });
                        table.ForeignKey(
                            name: "FK_SkillsUser_Skills_SkillsId",
                            column: x => x.SkillsId,
                            principalTable: "Skills",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_SkillsUser_Users_UsersId",
                            column: x => x.UsersId,
                            principalTable: "Users",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SkillsUser_UsersId",
                table: "SkillsUser",
                column: "UsersId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkills_Skills_SkillId",
                table: "UserSkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSkills_Skills_SkillId",
                table: "UserSkills"
            );

            migrationBuilder.DropTable(name: "SkillsUser");

            migrationBuilder.DropPrimaryKey(name: "PK_UserSkills", table: "UserSkills");

            migrationBuilder
                .AlterColumn<Guid>(
                    name: "SkillId",
                    table: "UserSkills",
                    type: "char(36)",
                    nullable: false,
                    collation: "ascii_general_ci",
                    oldClrType: typeof(string),
                    oldType: "VARCHAR(50)"
                )
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "SkillsId",
                    table: "UserSkills",
                    type: "VARCHAR(50)",
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSkills",
                table: "UserSkills",
                column: "Id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserSkills_SkillsId",
                table: "UserSkills",
                column: "SkillsId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkills_Skills_SkillsId",
                table: "UserSkills",
                column: "SkillsId",
                principalTable: "Skills",
                principalColumn: "id"
            );
        }
    }
}
