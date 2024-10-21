using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class nineEleven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_AspNetUsers_EmployeeId",
                table: "Leaves");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Leaves",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "LeaveRequests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "ExpenseRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_CompanyId",
                table: "Leaves",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseRequests_CompanyId",
                table: "ExpenseRequests",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseRequests_Companies_CompanyId",
                table: "ExpenseRequests",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_AspNetUsers_EmployeeId",
                table: "Leaves",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_Companies_CompanyId",
                table: "Leaves",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseRequests_Companies_CompanyId",
                table: "ExpenseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_AspNetUsers_EmployeeId",
                table: "Leaves");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_Companies_CompanyId",
                table: "Leaves");

            migrationBuilder.DropIndex(
                name: "IX_Leaves_CompanyId",
                table: "Leaves");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseRequests_CompanyId",
                table: "ExpenseRequests");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ExpenseRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "LeaveRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_AspNetUsers_EmployeeId",
                table: "Leaves",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
