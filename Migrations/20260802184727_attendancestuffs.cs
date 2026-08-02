using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class attendancestuffs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceModels",
                columns: table => new
                {
                    attendanceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentID = table.Column<int>(type: "int", nullable: false),
                    teacherID = table.Column<int>(type: "int", nullable: false),
                    DateAttended = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceModels", x => x.attendanceID);
                    table.ForeignKey(
                        name: "FK_AttendanceModels_Staffs_teacherID",
                        column: x => x.teacherID,
                        principalTable: "Staffs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceModels_Students_studentID",
                        column: x => x.studentID,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceModels_studentID",
                table: "AttendanceModels",
                column: "studentID");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceModels_teacherID",
                table: "AttendanceModels",
                column: "teacherID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceModels");
        }
    }
}
