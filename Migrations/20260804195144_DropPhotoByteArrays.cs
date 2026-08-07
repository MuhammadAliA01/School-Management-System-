using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class DropPhotoByteArrays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClockInPhoto",
                table: "StaffAttendances");

            migrationBuilder.DropColumn(
                name: "ClockOutPhoto",
                table: "StaffAttendances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ClockInPhoto",
                table: "StaffAttendances",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ClockOutPhoto",
                table: "StaffAttendances",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
