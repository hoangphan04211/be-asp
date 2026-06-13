using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLKHO_PhanVanHoang.Migrations
{
    /// <inheritdoc />
    public partial class Add2FAFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "SystemUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorSecret",
                table: "SystemUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorTempCode",
                table: "SystemUsers",
                type: "varchar(6)",
                maxLength: 6,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "TwoFactorTempCodeExpiry",
                table: "SystemUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorType",
                table: "SystemUsers",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2094));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2115));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2120));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2124));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2129));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2133));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2138));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2142));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2148));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2152));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2156));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2160));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2164));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2168));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2298));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2307));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2312));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2315));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2319));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2026, 5, 17, 14, 31, 21, 649, DateTimeKind.Local).AddTicks(2323));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "SystemUsers");

            migrationBuilder.DropColumn(
                name: "TwoFactorSecret",
                table: "SystemUsers");

            migrationBuilder.DropColumn(
                name: "TwoFactorTempCode",
                table: "SystemUsers");

            migrationBuilder.DropColumn(
                name: "TwoFactorTempCodeExpiry",
                table: "SystemUsers");

            migrationBuilder.DropColumn(
                name: "TwoFactorType",
                table: "SystemUsers");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6292));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6313));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6319));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6323));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6475));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6481));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6486));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6491));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6495));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6500));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6505));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6510));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6515));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6520));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6524));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6529));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6537));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6542));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6546));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2026, 4, 18, 22, 23, 28, 321, DateTimeKind.Local).AddTicks(6551));
        }
    }
}
