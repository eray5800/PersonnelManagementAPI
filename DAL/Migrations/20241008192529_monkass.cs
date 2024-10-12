using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class monkass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CompanyRequests",
                newName: "CompanyName");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "CompanyRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRequests_EmployeeId",
                table: "CompanyRequests",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyRequests_AspNetUsers_EmployeeId",
                table: "CompanyRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyRequests_AspNetUsers_EmployeeId",
                table: "CompanyRequests");

            migrationBuilder.DropIndex(
                name: "IX_CompanyRequests_EmployeeId",
                table: "CompanyRequests");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "CompanyRequests");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "CompanyRequests",
                newName: "Name");
        }
    }
}
