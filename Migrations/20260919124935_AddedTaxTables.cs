using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddedTaxTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "netSalary",
                table: "EmployeeEarnings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "payeDeduction",
                table: "EmployeeEarnings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "pensionDeduction",
                table: "EmployeeEarnings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "taxableIncome",
                table: "EmployeeEarnings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "totalDeductions",
                table: "EmployeeEarnings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "uifDeduction",
                table: "EmployeeEarnings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "TaxBrackets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinIncome = table.Column<double>(type: "float", nullable: false),
                    MaxIncome = table.Column<double>(type: "float", nullable: false),
                    BaseTaxAmount = table.Column<double>(type: "float", nullable: false),
                    TaxRate = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxBrackets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimaryRebate = table.Column<double>(type: "float", nullable: false),
                    SecondaryRebate = table.Column<double>(type: "float", nullable: false),
                    TertiaryRebate = table.Column<double>(type: "float", nullable: false),
                    PensionContributionRate = table.Column<double>(type: "float", nullable: false),
                    UifRate = table.Column<double>(type: "float", nullable: false),
                    UifMonthlyCap = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxConfigurations", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TaxBrackets",
                columns: new[] { "Id", "BaseTaxAmount", "MaxIncome", "MinIncome", "TaxRate", "TaxYear" },
                values: new object[,]
                {
                    { 1, 0.0, 237100.0, 1.0, 0.17999999999999999, "2026/2027" },
                    { 2, 42678.0, 370500.0, 237101.0, 0.26000000000000001, "2026/2027" },
                    { 3, 77362.0, 512800.0, 370501.0, 0.31, "2026/2027" },
                    { 4, 121475.0, 673000.0, 512801.0, 0.35999999999999999, "2026/2027" },
                    { 5, 179147.0, 857900.0, 673001.0, 0.39000000000000001, "2026/2027" },
                    { 6, 251258.0, 1817000.0, 857901.0, 0.40999999999999998, "2026/2027" },
                    { 7, 644489.0, 99999999.0, 1817001.0, 0.45000000000000001, "2026/2027" }
                });

            migrationBuilder.InsertData(
                table: "TaxConfigurations",
                columns: new[] { "Id", "PensionContributionRate", "PrimaryRebate", "SecondaryRebate", "TaxYear", "TertiaryRebate", "UifMonthlyCap", "UifRate" },
                values: new object[] { 1, 0.074999999999999997, 17235.0, 9444.0, "2026/2027", 3145.0, 177.12, 0.01 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaxBrackets");

            migrationBuilder.DropTable(
                name: "TaxConfigurations");

            migrationBuilder.DropColumn(
                name: "netSalary",
                table: "EmployeeEarnings");

            migrationBuilder.DropColumn(
                name: "payeDeduction",
                table: "EmployeeEarnings");

            migrationBuilder.DropColumn(
                name: "pensionDeduction",
                table: "EmployeeEarnings");

            migrationBuilder.DropColumn(
                name: "taxableIncome",
                table: "EmployeeEarnings");

            migrationBuilder.DropColumn(
                name: "totalDeductions",
                table: "EmployeeEarnings");

            migrationBuilder.DropColumn(
                name: "uifDeduction",
                table: "EmployeeEarnings");
        }
    }
}
