using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffAttendanceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    ClockInTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClockOutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClockInLatitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    ClockInLongitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    ClockOutLatitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    ClockOutLongitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    ClockInPhotoUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ClockOutPhotoUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsAnomaly = table.Column<bool>(type: "bit", nullable: false),
                    AnomalyReason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TotalHoursWorked = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffAttendances_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staffs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffAttendances_StaffId",
                table: "StaffAttendances",
                column: "StaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffAttendances");
        }
    }
}
