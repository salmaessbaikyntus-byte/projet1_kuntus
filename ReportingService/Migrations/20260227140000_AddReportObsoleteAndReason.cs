using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportingService.Migrations
{
    public partial class AddReportObsoleteAndReason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Reports",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ObsoleteAt",
                table: "Reports",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExportFormat",
                table: "Reports",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "PDF");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Reason", table: "Reports");
            migrationBuilder.DropColumn(name: "ObsoleteAt", table: "Reports");
            migrationBuilder.DropColumn(name: "ExportFormat", table: "Reports");
        }
    }
}
