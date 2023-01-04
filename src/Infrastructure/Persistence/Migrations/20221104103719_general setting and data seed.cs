using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class generalsettinganddataseed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "password_change_token",
                table: "Users",
                type: "VARCHAR(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "department_id",
                table: "Users",
                type: "VARCHAR(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "mail_port",
                table: "SMTPSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "sender_name",
                table: "SMTPSettings",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.id);
                    table.ForeignKey(
                        name: "FK_Departments_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GeneralSettings",
                columns: table => new
                {
                    id = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    logo_url = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    company_name = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    company_address = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    company_contact_number = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email_signature = table.Column<string>(type: "VARCHAR(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_on = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    updated_by = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updated_on = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralSettings", x => x.id);
                    table.ForeignKey(
                        name: "FK_GeneralSettings_Users_created_by",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "id", "address", "bio", "created_by", "created_on", "department_id", "email", "first_name", "hash_password", "image_url", "is_active", "last_name", "middle_name", "mobile_number", "password_change_token", "password_reset_token", "password_reset_token_expiry", "profession", "public_urls", "role", "updated_by", "updated_on" },
                values: new object[] { "30fcd978-f256-4733-840f-759181bc5e63", "ADDRESS", null, "30fcd978-f256-4733-840f-759181bc5e63", new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004), null, "vuriloapp@gmail.com", "ABC", "+gURQgHBT1zJz5AljZhAMyaNRFQBVorq5HIlEmhf+ZQ=:BBLvXedGXzdz0ZlypoKQxQ==", null, true, "XYZ", null, "1234567890", null, null, null, null, null, 1, "30fcd978-f256-4733-840f-759181bc5e63", new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004) });

            migrationBuilder.InsertData(
                table: "GeneralSettings",
                columns: new[] { "id", "company_address", "company_contact_number", "company_name", "created_by", "created_on", "email_signature", "logo_url", "updated_by", "updated_on" },
                values: new object[] { "2d7867fc-b7e7-461d-9257-d0990b5ac991", "company address", "company contact number", "company name", "30fcd978-f256-4733-840f-759181bc5e63", new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004), "company default email signature", "image path", "30fcd978-f256-4733-840f-759181bc5e63", new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004) });

            migrationBuilder.InsertData(
                table: "SMTPSettings",
                columns: new[] { "id", "created_by", "created_on", "mail_port", "mail_server", "password", "replay_to", "sender_email", "sender_name", "updated_by", "updated_on", "use_ssl", "user_name" },
                values: new object[] { "d3c343d8-adf8-45d4-afbe-e09c3285da24", "30fcd978-f256-4733-840f-759181bc5e63", new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004), 123, "email-smtp.ap-south-1.amazonaws.com", "password", "support@vurilo.com", "noreply@vurilo.com", "Vurilo", "30fcd978-f256-4733-840f-759181bc5e63", new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004), true, "username" });

            migrationBuilder.InsertData(
                table: "ZoomSettings",
                columns: new[] { "id", "api_key", "created_by", "created_on", "secret_key", "updated_by", "updated_on" },
                values: new object[] { "f41a902f-fabd-4749-ac28-91137f685cb8", "api_key value", "30fcd978-f256-4733-840f-759181bc5e63", new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004), "secret key value", "30fcd978-f256-4733-840f-759181bc5e63", new DateTime(2022, 11, 4, 10, 35, 19, 307, DateTimeKind.Utc).AddTicks(3004) });

            migrationBuilder.CreateIndex(
                name: "IX_Users_department_id",
                table: "Users",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_created_by",
                table: "Departments",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralSettings_created_by",
                table: "GeneralSettings",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Departments_department_id",
                table: "Users",
                column: "department_id",
                principalTable: "Departments",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Departments_department_id",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "GeneralSettings");

            migrationBuilder.DropIndex(
                name: "IX_Users_department_id",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "SMTPSettings",
                keyColumn: "id",
                keyValue: "d3c343d8-adf8-45d4-afbe-e09c3285da24");

            migrationBuilder.DeleteData(
                table: "ZoomSettings",
                keyColumn: "id",
                keyValue: "f41a902f-fabd-4749-ac28-91137f685cb8");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "id",
                keyValue: "30fcd978-f256-4733-840f-759181bc5e63");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "sender_name",
                table: "SMTPSettings");

            migrationBuilder.AlterColumn<string>(
                name: "password_change_token",
                table: "Users",
                type: "VARCHAR(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "mail_port",
                table: "SMTPSettings",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
