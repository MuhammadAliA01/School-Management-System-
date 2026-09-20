using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class addedBankAccountModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employeeID = table.Column<int>(type: "int", nullable: false),
                    accountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    accountName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.id);
                    table.ForeignKey(
                        name: "FK_BankAccounts_Staffs_employeeID",
                        column: x => x.employeeID,
                        principalTable: "Staffs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    accTransferredTo = table.Column<int>(type: "int", nullable: false),
                    amtTransferred = table.Column<double>(type: "float", nullable: false),
                    transferDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Transactions_BankAccounts_accTransferredTo",
                        column: x => x.accTransferredTo,
                        principalTable: "BankAccounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_employeeID",
                table: "BankAccounts",
                column: "employeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_accTransferredTo",
                table: "Transactions",
                column: "accTransferredTo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "BankAccounts");
        }
    }
}
