using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPayslipDistributionAccessToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "PayslipDistributions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "PayslipDistributions");
        }
    }
}
