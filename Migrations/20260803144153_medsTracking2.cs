using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class medsTracking2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    medName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "medsAdministered",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentID = table.Column<int>(type: "int", nullable: false),
                    teacherID = table.Column<int>(type: "int", nullable: false),
                    medID = table.Column<int>(type: "int", nullable: false),
                    dateAdministered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medsAdministered", x => x.ID);
                    table.ForeignKey(
                        name: "FK_medsAdministered_Medications_medID",
                        column: x => x.medID,
                        principalTable: "Medications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_medsAdministered_Staffs_teacherID",
                        column: x => x.teacherID,
                        principalTable: "Staffs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_medsAdministered_Students_studentID",
                        column: x => x.studentID,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_medsAdministered_medID",
                table: "medsAdministered",
                column: "medID");

            migrationBuilder.CreateIndex(
                name: "IX_medsAdministered_studentID",
                table: "medsAdministered",
                column: "studentID");

            migrationBuilder.CreateIndex(
                name: "IX_medsAdministered_teacherID",
                table: "medsAdministered",
                column: "teacherID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "medsAdministered");

            migrationBuilder.DropTable(
                name: "Medications");
        }
    }
}
