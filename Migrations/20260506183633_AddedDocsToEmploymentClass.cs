using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddedDocsToEmploymentClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankLetterLocation",
                table: "EmploymentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractLocation",
                table: "EmploymentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdCopyLocation",
                table: "EmploymentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PoliceClearanceLocation",
                table: "EmploymentApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SACELocation",
                table: "EmploymentApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankLetterLocation",
                table: "EmploymentApplications");

            migrationBuilder.DropColumn(
                name: "ContractLocation",
                table: "EmploymentApplications");

            migrationBuilder.DropColumn(
                name: "IdCopyLocation",
                table: "EmploymentApplications");

            migrationBuilder.DropColumn(
                name: "PoliceClearanceLocation",
                table: "EmploymentApplications");

            migrationBuilder.DropColumn(
                name: "SACELocation",
                table: "EmploymentApplications");
        }
    }
}
