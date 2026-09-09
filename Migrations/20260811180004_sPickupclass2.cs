using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class sPickupclass2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_studentPickups_Students_StudentId",
                table: "studentPickups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_studentPickups",
                table: "studentPickups");

            migrationBuilder.RenameTable(
                name: "studentPickups",
                newName: "StudentPickups");

            migrationBuilder.RenameIndex(
                name: "IX_studentPickups_StudentId",
                table: "StudentPickups",
                newName: "IX_StudentPickups_StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentPickups",
                table: "StudentPickups",
                column: "StudentPickupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentPickups_Students_StudentId",
                table: "StudentPickups",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentPickups_Students_StudentId",
                table: "StudentPickups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentPickups",
                table: "StudentPickups");

            migrationBuilder.RenameTable(
                name: "StudentPickups",
                newName: "studentPickups");

            migrationBuilder.RenameIndex(
                name: "IX_StudentPickups_StudentId",
                table: "studentPickups",
                newName: "IX_studentPickups_StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_studentPickups",
                table: "studentPickups",
                column: "StudentPickupId");

            migrationBuilder.AddForeignKey(
                name: "FK_studentPickups_Students_StudentId",
                table: "studentPickups",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
