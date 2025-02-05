using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyKit.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreatedByNotRequiredInMailNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .AlterColumn<Guid>(
                    name: "CreatedBy",
                    table: "UserSkills",
                    type: "char(36)",
                    nullable: true,
                    collation: "ascii_general_ci",
                    oldClrType: typeof(Guid),
                    oldType: "char(36)"
                )
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder
                .AlterColumn<string>(
                    name: "created_by",
                    table: "MailNotifications",
                    type: "varchar(50)",
                    maxLength: 50,
                    nullable: true,
                    oldClrType: typeof(string),
                    oldType: "varchar(50)",
                    oldMaxLength: 50
                )
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .AlterColumn<Guid>(
                    name: "CreatedBy",
                    table: "UserSkills",
                    type: "char(36)",
                    nullable: false,
                    defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                    collation: "ascii_general_ci",
                    oldClrType: typeof(Guid),
                    oldType: "char(36)",
                    oldNullable: true
                )
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "MailNotifications",
                keyColumn: "created_by",
                keyValue: null,
                column: "created_by",
                value: ""
            );

            migrationBuilder
                .AlterColumn<string>(
                    name: "created_by",
                    table: "MailNotifications",
                    type: "varchar(50)",
                    maxLength: 50,
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "varchar(50)",
                    oldMaxLength: 50,
                    oldNullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
