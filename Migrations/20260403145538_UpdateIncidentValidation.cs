using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIncidentValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentName",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "TeacherName",
                table: "Incidents");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Incidents",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_StudentId",
                table: "Incidents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_TeacherId",
                table: "Incidents",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Students_StudentId",
                table: "Incidents",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Teachers_TeacherId",
                table: "Incidents",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "TeacherId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Students_StudentId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Teachers_TeacherId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_StudentId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_TeacherId",
                table: "Incidents");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeacherName",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
