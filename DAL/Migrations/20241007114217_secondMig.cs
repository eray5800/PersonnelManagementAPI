using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class secondMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experience_EmployeeDetails_EmployeeDetailId",
                table: "Experience");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Experience",
                table: "Experience");

            migrationBuilder.RenameTable(
                name: "Experience",
                newName: "Experiences");

            migrationBuilder.RenameIndex(
                name: "IX_Experience_EmployeeDetailId",
                table: "Experiences",
                newName: "IX_Experiences_EmployeeDetailId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Experiences",
                table: "Experiences",
                column: "ExperienceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_EmployeeDetails_EmployeeDetailId",
                table: "Experiences",
                column: "EmployeeDetailId",
                principalTable: "EmployeeDetails",
                principalColumn: "EmployeeDetailId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_EmployeeDetails_EmployeeDetailId",
                table: "Experiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Experiences",
                table: "Experiences");

            migrationBuilder.RenameTable(
                name: "Experiences",
                newName: "Experience");

            migrationBuilder.RenameIndex(
                name: "IX_Experiences_EmployeeDetailId",
                table: "Experience",
                newName: "IX_Experience_EmployeeDetailId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Experience",
                table: "Experience",
                column: "ExperienceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Experience_EmployeeDetails_EmployeeDetailId",
                table: "Experience",
                column: "EmployeeDetailId",
                principalTable: "EmployeeDetails",
                principalColumn: "EmployeeDetailId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
