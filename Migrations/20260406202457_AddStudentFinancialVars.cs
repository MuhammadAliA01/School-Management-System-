using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentFinancialVars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "Students",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFees",
                table: "Students",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

           // migrationBuilder.AddColumn<string>(
              //  name: "StripeSessionId",
                //table: "StudentRegistrations",
                //type: "nvarchar(max)",
               // nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "TotalFees",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StripeSessionId",
                table: "StudentRegistrations");
        }
    }
}
