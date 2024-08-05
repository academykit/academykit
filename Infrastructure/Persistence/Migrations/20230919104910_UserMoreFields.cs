using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserMoreFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AddressIsSame",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "birth_date_ad",
                table: "Users",
                type: "DATETIME",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "birth_date_bs",
                table: "Users",
                type: "DATETIME",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "blood_group",
                table: "Users",
                type: "int",
                nullable: true
            );

            migrationBuilder
                .AddColumn<string>(
                    name: "branch_id",
                    table: "Users",
                    type: "VARCHAR(50)",
                    maxLength: 50,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "current_address",
                    table: "Users",
                    type: "VARCHAR(200)",
                    maxLength: 200,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "current_city",
                    table: "Users",
                    type: "VARCHAR(200)",
                    maxLength: 200,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "current_country",
                    table: "Users",
                    type: "VARCHAR(200)",
                    maxLength: 200,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "current_district",
                    table: "Users",
                    type: "VARCHAR(200)",
                    maxLength: 200,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "current_municipality",
                    table: "Users",
                    type: "VARCHAR(200)",
                    maxLength: 200,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "current_state",
                    table: "Users",
                    type: "VARCHAR(200)",
                    maxLength: 200,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "current_ward",
                    table: "Users",
                    type: "VARCHAR(200)",
                    maxLength: 200,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "employment_type",
                    table: "Users",
                    type: "VARCHAR(50)",
                    maxLength: 50,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "family_address_same",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder
                .AddColumn<string>(
                    name: "father_name",
                    table: "Users",
                    type: "VARCHAR(100)",
                    maxLength: 100,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "gender",
                table: "Users",
                type: "int",
                nullable: true
            );

            migrationBuilder
                .AddColumn<string>(
                    name: "grandfather_name",
                    table: "Users",
                    type: "VARCHAR(100)",
                    maxLength: 100,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "identity_issued_by",
                    table: "Users",
                    type: "VARCHAR(100)",
                    maxLength: 100,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "identity_issued_on",
                table: "Users",
                type: "DATE",
                maxLength: 100,
                nullable: true
            );

            migrationBuilder
                .AddColumn<string>(
                    name: "identity_number",
                    table: "Users",
                    type: "VARCHAR(50)",
                    maxLength: 50,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "identity_type",
                table: "Users",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "joined_date_ad",
                table: "Users",
                type: "DATETIME",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "joined_date_bs",
                table: "Users",
                type: "DATETIME",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "marital_status",
                table: "Users",
                type: "int",
                nullable: true
            );

            migrationBuilder
                .AddColumn<string>(
                    name: "member_current_address",
                    table: "Users",
                    type: "VARCHAR(100)",
                    maxLength: 100,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "member_permanent_address",
                    table: "Users",
                    type: "VARCHAR(100)",
                    maxLength: 100,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "member_phone",
                    table: "Users",
                    type: "VARCHAR(50)",
                    maxLength: 50,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "mobile_number_secondary",
                    table: "Users",
                    type: "VARCHAR(50)",
                    maxLength: 50,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "mother_name",
                    table: "Users",
                    type: "VARCHAR(100)",
                    maxLength: 100,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "nationality",
                table: "Users",
                type: "int",
                nullable: true,
                defaultValue: 1
            );

            migrationBuilder
                .AddColumn<string>(
                    name: "permanent_city",
                    table: "Users",
                    type: "VARCHAR(200)",
                    maxLength: 200,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "permanent_country",
                    table: "Users",
                    type: "VARCHAR(200)",
                    maxLength: 200,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "permanent_district",
                    table: "Users",
                    type: "VARCHAR(200)",
                    maxLength: 200,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "permanent_municipality",
                    table: "Users",
                    type: "VARCHAR(200)",
                    maxLength: 200,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "permanent_state",
                    table: "Users",
                    type: "VARCHAR(200)",
                    maxLength: 200,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "permanent_ward",
                    table: "Users",
                    type: "VARCHAR(200)",
                    maxLength: 200,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "personal_email",
                    table: "Users",
                    type: "VARCHAR(100)",
                    maxLength: 100,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .AddColumn<string>(
                    name: "spouse_name",
                    table: "Users",
                    type: "VARCHAR(100)",
                    maxLength: 100,
                    nullable: true
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Branch",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        slug = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        name = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        code = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        nepaliname = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        address = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        email = table
                            .Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        mobile_number = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        is_active = table.Column<bool>(
                            type: "tinyint(1)",
                            nullable: false,
                            defaultValue: false
                        ),
                        ip_address = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        remarks = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        location = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        under_branch = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        area_branch = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        regional_branch = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        province = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        valley_type = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        sol_id = table.Column<int>(type: "INT(50)", maxLength: 50, nullable: false),
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
                        table.PrimaryKey("PK_Branch", x => x.id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "UserEducations",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        institution_name = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        degree = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        specialization = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        completion_date = table.Column<DateOnly>(type: "DATE", nullable: false),
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
                        table.PrimaryKey("PK_UserEducations", x => x.id);
                        table.ForeignKey(
                            name: "FK_UserEducations_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "UserWorkExperiences",
                    columns: table => new
                    {
                        id = table
                            .Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        company_name = table
                            .Column<string>(type: "VARCHAR(270)", maxLength: 270, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        job_title = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        job_description = table
                            .Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        joined_date = table.Column<DateOnly>(type: "DATE", nullable: false),
                        end_date = table.Column<DateOnly>(type: "DATE", nullable: false),
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
                        table.PrimaryKey("PK_UserWorkExperiences", x => x.id);
                        table.ForeignKey(
                            name: "FK_UserWorkExperiences_Users_user_id",
                            column: x => x.user_id,
                            principalTable: "Users",
                            principalColumn: "id",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Users_branch_id",
                table: "Users",
                column: "branch_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserEducations_user_id",
                table: "UserEducations",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkExperiences_user_id",
                table: "UserWorkExperiences",
                column: "user_id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Branch_branch_id",
                table: "Users",
                column: "branch_id",
                principalTable: "Branch",
                principalColumn: "id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Users_Branch_branch_id", table: "Users");

            migrationBuilder.DropTable(name: "Branch");

            migrationBuilder.DropTable(name: "UserEducations");

            migrationBuilder.DropTable(name: "UserWorkExperiences");

            migrationBuilder.DropIndex(name: "IX_Users_branch_id", table: "Users");

            migrationBuilder.DropColumn(name: "AddressIsSame", table: "Users");

            migrationBuilder.DropColumn(name: "birth_date_ad", table: "Users");

            migrationBuilder.DropColumn(name: "birth_date_bs", table: "Users");

            migrationBuilder.DropColumn(name: "blood_group", table: "Users");

            migrationBuilder.DropColumn(name: "branch_id", table: "Users");

            migrationBuilder.DropColumn(name: "current_address", table: "Users");

            migrationBuilder.DropColumn(name: "current_city", table: "Users");

            migrationBuilder.DropColumn(name: "current_country", table: "Users");

            migrationBuilder.DropColumn(name: "current_district", table: "Users");

            migrationBuilder.DropColumn(name: "current_municipality", table: "Users");

            migrationBuilder.DropColumn(name: "current_state", table: "Users");

            migrationBuilder.DropColumn(name: "current_ward", table: "Users");

            migrationBuilder.DropColumn(name: "employment_type", table: "Users");

            migrationBuilder.DropColumn(name: "family_address_same", table: "Users");

            migrationBuilder.DropColumn(name: "father_name", table: "Users");

            migrationBuilder.DropColumn(name: "gender", table: "Users");

            migrationBuilder.DropColumn(name: "grandfather_name", table: "Users");

            migrationBuilder.DropColumn(name: "identity_issued_by", table: "Users");

            migrationBuilder.DropColumn(name: "identity_issued_on", table: "Users");

            migrationBuilder.DropColumn(name: "identity_number", table: "Users");

            migrationBuilder.DropColumn(name: "identity_type", table: "Users");

            migrationBuilder.DropColumn(name: "joined_date_ad", table: "Users");

            migrationBuilder.DropColumn(name: "joined_date_bs", table: "Users");

            migrationBuilder.DropColumn(name: "marital_status", table: "Users");

            migrationBuilder.DropColumn(name: "member_current_address", table: "Users");

            migrationBuilder.DropColumn(name: "member_permanent_address", table: "Users");

            migrationBuilder.DropColumn(name: "member_phone", table: "Users");

            migrationBuilder.DropColumn(name: "mobile_number_secondary", table: "Users");

            migrationBuilder.DropColumn(name: "mother_name", table: "Users");

            migrationBuilder.DropColumn(name: "nationality", table: "Users");

            migrationBuilder.DropColumn(name: "permanent_city", table: "Users");

            migrationBuilder.DropColumn(name: "permanent_country", table: "Users");

            migrationBuilder.DropColumn(name: "permanent_district", table: "Users");

            migrationBuilder.DropColumn(name: "permanent_municipality", table: "Users");

            migrationBuilder.DropColumn(name: "permanent_state", table: "Users");

            migrationBuilder.DropColumn(name: "permanent_ward", table: "Users");

            migrationBuilder.DropColumn(name: "personal_email", table: "Users");

            migrationBuilder.DropColumn(name: "spouse_name", table: "Users");
        }
    }
}
