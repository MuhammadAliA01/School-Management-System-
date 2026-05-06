using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class addedQuizQuestionsToClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MCQ1",
                table: "EmploymentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MCQ2",
                table: "EmploymentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MCQ3",
                table: "EmploymentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MCQ4",
                table: "EmploymentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OEQ1",
                table: "EmploymentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OEQ2",
                table: "EmploymentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OEQ3",
                table: "EmploymentApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MCQ1",
                table: "EmploymentApplications");

            migrationBuilder.DropColumn(
                name: "MCQ2",
                table: "EmploymentApplications");

            migrationBuilder.DropColumn(
                name: "MCQ3",
                table: "EmploymentApplications");

            migrationBuilder.DropColumn(
                name: "MCQ4",
                table: "EmploymentApplications");

            migrationBuilder.DropColumn(
                name: "OEQ1",
                table: "EmploymentApplications");

            migrationBuilder.DropColumn(
                name: "OEQ2",
                table: "EmploymentApplications");

            migrationBuilder.DropColumn(
                name: "OEQ3",
                table: "EmploymentApplications");
        }
    }
}
