using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateParentFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateViewed",
                table: "ConductReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsViewedByParent",
                table: "ConductReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ParentMessage",
                table: "ConductReports",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateViewed",
                table: "ConductReports");

            migrationBuilder.DropColumn(
                name: "IsViewedByParent",
                table: "ConductReports");

            migrationBuilder.DropColumn(
                name: "ParentMessage",
                table: "ConductReports");
        }
    }
}
