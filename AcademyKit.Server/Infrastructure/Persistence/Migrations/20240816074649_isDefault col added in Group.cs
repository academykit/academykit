using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyKit.Server.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class isDefaultcoladdedinGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_default",
                table: "Groups",
                type: "tinyint(1)",
                nullable: true,
                defaultValue: false
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "is_default", table: "Groups");
        }
    }
}
