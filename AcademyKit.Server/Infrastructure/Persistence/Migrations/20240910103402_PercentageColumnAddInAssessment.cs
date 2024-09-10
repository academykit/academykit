using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyKit.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class PercentageColumnAddInAssessment : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "pass_percentage",
            table: "Assessments",
            type: "int",
            nullable: true
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "pass_percentage", table: "Assessments");
    }
}
