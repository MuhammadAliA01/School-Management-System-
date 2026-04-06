using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeSessionTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeSessionId",
                table: "StudentRegistrations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeSessionId",
                table: "StudentRegistrations");
        }
    }
}
