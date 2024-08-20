using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyKit.Server.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLicenseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "customerName",
                table: "Licenses",
                newName: "customer_name"
            );

            migrationBuilder.RenameColumn(
                name: "customerEmail",
                table: "Licenses",
                newName: "customer_email"
            );

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "Licenses",
                newName: "updated_on"
            );

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Licenses",
                newName: "updated_by"
            );

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Licenses",
                newName: "created_on"
            );

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Licenses",
                newName: "created_by"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_on",
                table: "Licenses",
                type: "DATETIME",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true
            );

            migrationBuilder
                .AlterColumn<string>(
                    name: "updated_by",
                    table: "Licenses",
                    type: "VARCHAR(50)",
                    maxLength: 50,
                    nullable: true,
                    oldClrType: typeof(Guid),
                    oldType: "char(36)",
                    oldNullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_on",
                table: "Licenses",
                type: "DATETIME",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)"
            );

            migrationBuilder
                .AlterColumn<string>(
                    name: "created_by",
                    table: "Licenses",
                    type: "VARCHAR(50)",
                    maxLength: 50,
                    nullable: false,
                    oldClrType: typeof(Guid),
                    oldType: "char(36)"
                )
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_on",
                table: "Licenses",
                newName: "UpdatedOn"
            );

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "Licenses",
                newName: "UpdatedBy"
            );

            migrationBuilder.RenameColumn(
                name: "customer_name",
                table: "Licenses",
                newName: "customerName"
            );

            migrationBuilder.RenameColumn(
                name: "customer_email",
                table: "Licenses",
                newName: "customerEmail"
            );

            migrationBuilder.RenameColumn(
                name: "created_on",
                table: "Licenses",
                newName: "CreatedOn"
            );

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "Licenses",
                newName: "CreatedBy"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedOn",
                table: "Licenses",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME",
                oldNullable: true
            );

            migrationBuilder
                .AlterColumn<Guid>(
                    name: "UpdatedBy",
                    table: "Licenses",
                    type: "char(36)",
                    nullable: true,
                    collation: "ascii_general_ci",
                    oldClrType: typeof(string),
                    oldType: "VARCHAR(50)",
                    oldMaxLength: 50,
                    oldNullable: true
                )
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Licenses",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME"
            );

            migrationBuilder
                .AlterColumn<Guid>(
                    name: "CreatedBy",
                    table: "Licenses",
                    type: "char(36)",
                    nullable: false,
                    collation: "ascii_general_ci",
                    oldClrType: typeof(string),
                    oldType: "VARCHAR(50)",
                    oldMaxLength: 50
                )
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
