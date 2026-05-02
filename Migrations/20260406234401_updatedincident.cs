using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class updatedincident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AIAdvice",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AIEvaluation",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AIImpact",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AISeverity",
                table: "Incidents",
                type: "int",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIAdvice",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "AIEvaluation",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "AIImpact",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "AISeverity",
                table: "Incidents");

        }
    }
}
