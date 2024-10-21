using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class monkas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyRequests_AspNetUsers_EmployeeId",
                table: "CompanyRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseRequests_AspNetUsers_EmployeeId",
                table: "ExpenseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseRequests_Companies_CompanyId",
                table: "ExpenseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Companies_CompanyId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_AspNetUsers_EmployeeId",
                table: "Leaves");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyRequests_AspNetUsers_EmployeeId",
                table: "CompanyRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseRequests_AspNetUsers_EmployeeId",
                table: "ExpenseRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseRequests_Companies_CompanyId",
                table: "ExpenseRequests",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Companies_CompanyId",
                table: "LeaveRequests",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_AspNetUsers_EmployeeId",
                table: "Leaves",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyRequests_AspNetUsers_EmployeeId",
                table: "CompanyRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseRequests_AspNetUsers_EmployeeId",
                table: "ExpenseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseRequests_Companies_CompanyId",
                table: "ExpenseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Companies_CompanyId",
                table: "LeaveRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_AspNetUsers_EmployeeId",
                table: "Leaves");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyRequests_AspNetUsers_EmployeeId",
                table: "CompanyRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseRequests_AspNetUsers_EmployeeId",
                table: "ExpenseRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseRequests_Companies_CompanyId",
                table: "ExpenseRequests",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_AspNetUsers_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Companies_CompanyId",
                table: "LeaveRequests",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_AspNetUsers_EmployeeId",
                table: "Leaves",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
