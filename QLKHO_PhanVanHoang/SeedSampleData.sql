-- SEED SAMPLE DATA FOR WMS PROJECT - PHAN VAN HOANG
USE [QLKHO_PhanVanHoang];
GO

SET QUOTED_IDENTIFIER ON;
SET ARITHABORT ON;
SET NUMERIC_ROUNDABORT OFF;
SET CONCAT_NULL_YIELDS_NULL ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
GO

BEGIN TRANSACTION;

-- 1. XÓA DỮ LIỆU CŨ (Nếu có - Trừ Roles và SystemUsers)
DELETE FROM AuditLogs;
DELETE FROM StockCards;
DELETE FROM InventoryAdjustments;
DELETE FROM CountingSheetDetails;
DELETE FROM CountingSheets;
DELETE FROM TransferVoucherDetails;
DELETE FROM TransferVouchers;
DELETE FROM DeliveryVoucherDetails;
DELETE FROM DeliveryVouchers;
DELETE FROM ReceivingVoucherDetails;
DELETE FROM ReceivingVouchers;
DELETE FROM Inventories;
DELETE FROM Products;
DELETE FROM Warehouses;
DELETE FROM Suppliers;
DELETE FROM Customers;
DELETE FROM Categories;

-- 2. CATEGORIES
SET IDENTITY_INSERT Categories ON;
INSERT INTO Categories (Id, Name, Description, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, N'Điện thoại & Máy tính', N'Các thiết bị công nghệ cao', GETDATE(), 'admin', 0),
(2, N'Đồ gia dụng', N'Thiết bị dùng trong gia đình', GETDATE(), 'admin', 0),
(3, N'Thực phẩm khô', N'Các loại thực phẩm đóng gói', GETDATE(), 'admin', 0),
(4, N'Linh kiện điện tử', N'Chip, Ram, Mainboard', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Categories OFF;

-- 3. SUPPLIERS
SET IDENTITY_INSERT Suppliers ON;
INSERT INTO Suppliers (Id, Code, Name, PhoneNumber, Email, Address, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 'NCC-SS', N'Samsung Vietnam Co., Ltd', '0241-123456', 'contact@samsung.vn', N'KCN Yên Phong, Bắc Ninh', GETDATE(), 'admin', 0),
(2, 'NCC-HP', N'Tập đoàn Hòa Phát', '0243-987654', 'info@hoaphat.com.vn', N'66 Nguyễn Du, Hà Nội', GETDATE(), 'admin', 0),
(3, 'NCC-AC', N'Acecook Việt Nam', '0283-111222', 'care@acecook.vn', N'KCN Tân Bình, TP.HCM', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Suppliers OFF;

-- 4. CUSTOMERS
SET IDENTITY_INSERT Customers ON;
INSERT INTO Customers (Id, Code, Name, PhoneNumber, Email, Address, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 'KH-TGDD', N'Thế Giới Di Động', '18001060', 'cskh@thegioididong.com', N'P. Tân Thới Nhất, Quận 12, TP.HCM', GETDATE(), 'admin', 0),
(2, 'KH-DMX', N'Điện Máy Xanh', '18001061', 'cskh@dienmayxanh.com', N'62 Trần Quang Khải, Hà Nội', GETDATE(), 'admin', 0),
(3, 'KH-LE', N'Khách lẻ vãng lai', '0900000000', 'retail@example.com', N'Toàn quốc', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Customers OFF;

-- 5. WAREHOUSES
SET IDENTITY_INSERT Warehouses ON;
INSERT INTO Warehouses (Id, Name, Location, PhoneNumber, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, N'Kho Tổng Hà Nội', N'Số 1 Quang Trung, Hà Đông, Hà Nội', '0243-111111', GETDATE(), 'admin', 0),
(2, N'Kho Miền Nam', N'KCN Sóng Thần, Bình Dương', '0283-222222', GETDATE(), 'admin', 0),
(3, N'Kho Trung chuyển Miền Trung', N'KCN Hòa Khánh, Đà Nẵng', '0236-333333', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Warehouses OFF;

-- 6. PRODUCTS
SET IDENTITY_INSERT Products ON;
INSERT INTO Products (Id, SkuCode, Name, CategoryId, Unit, MinStockLevel, IsLotManaged, CostPrice, SellingPrice, Description, ImageUrl, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 'IP15PM-256-BLK', N'iPhone 15 Pro Max 256GB Black', 1, N'Cái', 5, 1, 28000000, 32000000, N'Hàng chính hãng Apple VN', 'https://example.com/ip15.jpg', GETDATE(), 'admin', 0),
(2, 'SS-S24U-512', N'Samsung Galaxy S24 Ultra 512GB', 1, N'Cái', 5, 1, 25000000, 29000000, N'Hàng chính hãng Samsung VN', 'https://example.com/s24u.jpg', GETDATE(), 'admin', 0),
(3, 'TL-PAN-400', N'Tủ lạnh Panasonic 400L Inverter', 2, N'Chiếc', 2, 1, 12000000, 15500000, N'Tiết kiệm điện năng', 'https://example.com/tulanhipan.jpg', GETDATE(), 'admin', 0),
(4, 'MI-HH-TOM', N'Mì Hảo Hảo Tôm Chua Cay', 3, N'Thùng', 50, 1, 105000, 120000, N'30 gói/thùng', 'https://example.com/mihh.jpg', GETDATE(), 'admin', 0),
(5, 'RAM-COR-16', N'RAM Corsair Vengeance 16GB DDR5', 4, N'Thanh', 20, 1, 1200000, 1550000, N'6000MHz RGB', 'https://example.com/ramcor.jpg', GETDATE(), 'admin', 0),
(6, 'MAC-M3-14', N'MacBook Pro 14 inch M3 16GB/512GB', 1, N'Bộ', 3, 1, 38000000, 44000000, N'Chip M3 mạnh mẽ', 'https://example.com/macm3.jpg', GETDATE(), 'admin', 0),
(7, 'TV-LG-65', N'Smart TV LG 65 inch 4K Thin Q', 2, N'Cái', 2, 1, 14000000, 18000000, N'Màn hình OLED siêu mỏng', 'https://example.com/tvlg.jpg', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Products OFF;

-- 7. INVENTORIES (Số dư đầu kỳ)
SET IDENTITY_INSERT Inventories ON;
INSERT INTO Inventories (Id, ProductId, WarehouseId, LotNumber, ExpiryDate, QuantityOnHand, ReservedQuantity, LocationInWarehouse, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 1, 1, 'LOT-001', '2025-12-31', 20, 0, 'KBT-A1', GETDATE(), 'admin', 0),
(2, 2, 1, 'LOT-001', '2025-12-31', 15, 0, 'KBT-A2', GETDATE(), 'admin', 0),
(3, 4, 1, 'BATCH-2024', '2025-06-30', 100, 0, 'KE-食品-01', GETDATE(), 'admin', 0),
(4, 5, 2, 'LOT-MARCH', '2028-12-31', 50, 0, 'E-COMP-01', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Inventories OFF;

-- 8. STOCKCARDS (Matching Inventories)
SET IDENTITY_INSERT StockCards ON;
INSERT INTO StockCards (Id, ProductId, WarehouseId, TransactionType, ReferenceCode, LotNumber, BeforeQuantity, ChangeQuantity, AfterQuantity, TransactionDate, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 1, 1, 'Inbound', 'INITIAL-001', 'LOT-001', 0, 20, 20, GETDATE(), GETDATE(), 'admin', 0),
(2, 2, 1, 'Inbound', 'INITIAL-002', 'LOT-001', 0, 15, 15, GETDATE(), GETDATE(), 'admin', 0),
(3, 4, 1, 'Inbound', 'INITIAL-003', 'BATCH-2024', 0, 100, 100, GETDATE(), GETDATE(), 'admin', 0),
(4, 5, 2, 'Inbound', 'INITIAL-004', 'LOT-MARCH', 0, 50, 50, GETDATE(), GETDATE(), 'admin', 0);
SET IDENTITY_INSERT StockCards OFF;

-- 9. RECEIVING VOUCHERS (Inbound)
SET IDENTITY_INSERT ReceivingVouchers ON;
INSERT INTO ReceivingVouchers (Id, Code, WarehouseId, SupplierId, Status, ReceivingDate, Notes, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 'PN-202403-001', 1, 1, 'Completed', GETDATE(), N'Nhập hàng iPhone định kỳ', GETDATE(), 'admin', 0),
(2, 'PN-202403-002', 1, 3, 'Draft', GETDATE(), N'Nhập thêm mì tôm dự phòng', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT ReceivingVouchers OFF;

SET IDENTITY_INSERT ReceivingVoucherDetails ON;
INSERT INTO ReceivingVoucherDetails (Id, ReceivingVoucherId, ProductId, LotNumber, ExpiryDate, Quantity, UnitPrice, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 1, 1, 'LOT-002', '2026-01-01', 10, 28000000, GETDATE(), 'admin', 0),
(2, 1, 6, 'MAC-BATCH', '2026-01-01', 5, 38000000, GETDATE(), 'admin', 0),
(3, 2, 4, 'BATCH-JUNE', '2025-06-30', 200, 105000, GETDATE(), 'admin', 0);
SET IDENTITY_INSERT ReceivingVoucherDetails OFF;

-- 10. DELIVERY VOUCHERS (Outbound)
SET IDENTITY_INSERT DeliveryVouchers ON;
INSERT INTO DeliveryVouchers (Id, Code, WarehouseId, CustomerId, Status, DeliveryDate, Notes, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 'PX-202403-001', 1, 1, 'Dispatched', GETDATE(), N'Xuất hàng cho TGDD chi nhánh 1', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT DeliveryVouchers OFF;

SET IDENTITY_INSERT DeliveryVoucherDetails ON;
INSERT INTO DeliveryVoucherDetails (Id, DeliveryVoucherId, ProductId, LotNumber, Quantity, SellingPrice, CostPrice, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 1, 1, 'LOT-001', 5, 32000000, 28000000, GETDATE(), 'admin', 0);
SET IDENTITY_INSERT DeliveryVoucherDetails OFF;

-- 11. TRANSFER VOUCHERS
SET IDENTITY_INSERT TransferVouchers ON;
INSERT INTO TransferVouchers (Id, Code, FromWarehouseId, ToWarehouseId, Status, TransferDate, Notes, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 'DC-0001', 1, 2, 'Draft', GETDATE(), N'Điều chuyển iPhone vào miền Nam', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT TransferVouchers OFF;

SET IDENTITY_INSERT TransferVoucherDetails ON;
INSERT INTO TransferVoucherDetails (Id, TransferVoucherId, ProductId, LotNumber, Quantity, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 1, 1, 'LOT-001', 2, GETDATE(), 'admin', 0);
SET IDENTITY_INSERT TransferVoucherDetails OFF;

-- 12. COUNTING SHEETS (Kiểm kê)
SET IDENTITY_INSERT CountingSheets ON;
INSERT INTO CountingSheets (Id, Code, WarehouseId, Status, CountingDate, Notes, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 'PK-202403-001', 1, 'Draft', GETDATE(), N'Kiểm kê định kỳ quý 1', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT CountingSheets OFF;

SET IDENTITY_INSERT CountingSheetDetails ON;
INSERT INTO CountingSheetDetails (Id, CountingSheetId, ProductId, LotNumber, SystemQuantity, ActualQuantity, Note, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 1, 1, 'LOT-001', 15, 15, N'Khớp', GETDATE(), 'admin', 0),
(2, 1, 2, 'LOT-001', 15, 14, N'Lệch 1 (Chuyên môn kiểm tra lại)', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT CountingSheetDetails OFF;

COMMIT TRANSACTION;
GO
