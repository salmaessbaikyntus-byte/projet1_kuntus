using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanningEngineService.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeWeeklyStatsAndPlanningPss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "AssignmentDate",
                table: "PlanningAssignments",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2025, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "PssCodeValue",
                table: "PlanningAssignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmployeeWeeklyStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeekNumber = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    SensitivePssCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeWeeklyStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeWeeklyStats_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeWeeklyStats_EmployeeId_Year_WeekNumber",
                table: "EmployeeWeeklyStats",
                columns: new[] { "EmployeeId", "Year", "WeekNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanningAssignments_AssignmentDate_EmployeeId",
                table: "PlanningAssignments",
                columns: new[] { "AssignmentDate", "EmployeeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("EmployeeWeeklyStats");
            migrationBuilder.DropIndex("IX_PlanningAssignments_AssignmentDate_EmployeeId", "PlanningAssignments");
            migrationBuilder.DropColumn("PlanningAssignments", "AssignmentDate");
            migrationBuilder.DropColumn("PlanningAssignments", "PssCodeValue");
        }
    }
}
