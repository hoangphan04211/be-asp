using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QLKHO_PhanVanHoang.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionsSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Group = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "Description", "Group", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "PRODUCT_VIEW", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(2985), "system", null, "Sản phẩm", false, "Xem sản phẩm", null, null },
                    { 2, "PRODUCT_EDIT", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3007), "system", null, "Sản phẩm", false, "Sửa/Xóa sản phẩm", null, null },
                    { 3, "MASTER_DATA_VIEW", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3011), "system", null, "Danh mục", false, "Xem danh mục chung", null, null },
                    { 4, "MASTER_DATA_EDIT", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3016), "system", null, "Danh mục", false, "Quản lý danh mục chung", null, null },
                    { 5, "WAREHOUSE_VIEW", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3020), "system", null, "Danh mục", false, "Xem kho hàng", null, null },
                    { 6, "INBOUND_VIEW", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3024), "system", null, "Vận hành", false, "Xem phiếu nhập", null, null },
                    { 7, "INBOUND_CREATE", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3028), "system", null, "Vận hành", false, "Tạo phiếu nhập", null, null },
                    { 8, "INBOUND_APPROVE", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3032), "system", null, "Vận hành", false, "Duyệt phiếu nhập", null, null },
                    { 9, "OUTBOUND_VIEW", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3036), "system", null, "Vận hành", false, "Xem phiếu xuất", null, null },
                    { 10, "OUTBOUND_CREATE", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3040), "system", null, "Vận hành", false, "Tạo phiếu xuất", null, null },
                    { 11, "OUTBOUND_APPROVE", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3044), "system", null, "Vận hành", false, "Duyệt phiếu xuất", null, null },
                    { 12, "TRANSFER_VIEW", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3049), "system", null, "Vận hành", false, "Xem điều chuyển", null, null },
                    { 13, "TRANSFER_CREATE", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3053), "system", null, "Vận hành", false, "Tạo điều chuyển", null, null },
                    { 14, "COUNTING_VIEW", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3057), "system", null, "Vận hành", false, "Xem kiểm kê", null, null },
                    { 15, "COUNTING_APPROVE", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3061), "system", null, "Vận hành", false, "Duyệt kiểm kê", null, null },
                    { 16, "REPORT_VIEW", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3065), "system", null, "Báo cáo", false, "Xem báo cáo tổn kho", null, null },
                    { 17, "STOCK_CARD_VIEW", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3069), "system", null, "Báo cáo", false, "Xem thẻ kho", null, null },
                    { 18, "USER_MANAGEMENT", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3182), "system", null, "Hệ thống", false, "Quản lý nhân viên", null, null },
                    { 19, "SYSTEM_LOGS", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3188), "system", null, "Hệ thống", false, "Xem nhật ký hệ thống", null, null },
                    { 20, "SYSTEM_TRASH", new DateTime(2026, 4, 14, 18, 44, 31, 871, DateTimeKind.Local).AddTicks(3192), "system", null, "Hệ thống", false, "Quản lý thùng rác", null, null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 7, 1 },
                    { 8, 1 },
                    { 9, 1 },
                    { 10, 1 },
                    { 11, 1 },
                    { 12, 1 },
                    { 13, 1 },
                    { 14, 1 },
                    { 15, 1 },
                    { 16, 1 },
                    { 17, 1 },
                    { 18, 1 },
                    { 19, 1 },
                    { 20, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Permissions");
        }
    }
}
