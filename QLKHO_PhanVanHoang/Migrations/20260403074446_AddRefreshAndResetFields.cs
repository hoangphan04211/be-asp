using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLKHO_PhanVanHoang.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshAndResetFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "SystemUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "SystemUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordCode",
                table: "SystemUsers",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPasswordExpiry",
                table: "SystemUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "SystemUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "SystemUsers");

            migrationBuilder.DropColumn(
                name: "ResetPasswordCode",
                table: "SystemUsers");

            migrationBuilder.DropColumn(
                name: "ResetPasswordExpiry",
                table: "SystemUsers");

            migrationBuilder.InsertData(
                table: "SystemUsers",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "FullName", "IsActive", "IsDeleted", "PasswordHash", "RoleId", "UpdatedAt", "UpdatedBy", "Username" },
                values: new object[] { 1, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "system", "admin@wms.com", "Hệ thống Quản trị", true, false, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgJRp41f..GjA72t/2R7yV8Z2vKi", 1, null, null, "admin" });
        }
    }
}
