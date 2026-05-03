using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentDocumentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfPath",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PdfPath",
                table: "StudentRegistrations");

            migrationBuilder.CreateTable(
                name: "StudentDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentRegistrationId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentDocuments_StudentRegistrations_StudentRegistrationId",
                        column: x => x.StudentRegistrationId,
                        principalTable: "StudentRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentDocuments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_StudentId",
                table: "StudentDocuments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_StudentRegistrationId",
                table: "StudentDocuments",
                column: "StudentRegistrationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentDocuments");

            migrationBuilder.AddColumn<string>(
                name: "PdfPath",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PdfPath",
                table: "StudentRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
