using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class PayslipAndPayslipVersionControlTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payslips",
                columns: table => new
                {
                    PayslipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    PayPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PdfContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentVersion = table.Column<int>(type: "int", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payslips", x => x.PayslipId);
                });

            migrationBuilder.CreateTable(
                name: "PayslipVersionControls",
                columns: table => new
                {
                    PayslipVersionControlId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayslipId = table.Column<int>(type: "int", nullable: false),
                    ArchivedPdfContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ArchivedVersionNumber = table.Column<int>(type: "int", nullable: false),
                    RegenerationReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayslipVersionControls", x => x.PayslipVersionControlId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payslips");

            migrationBuilder.DropTable(
                name: "PayslipVersionControls");
        }
    }
}
