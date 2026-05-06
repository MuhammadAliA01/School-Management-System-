using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class addedFieldsToStaffTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankLetterLocation",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CVLocation",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractLocation",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdCopyLocation",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PoliceClearanceLocation",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SACELocation",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankLetterLocation",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "CVLocation",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "ContractLocation",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "IdCopyLocation",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "PoliceClearanceLocation",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "SACELocation",
                table: "Staffs");
        }
    }
}
