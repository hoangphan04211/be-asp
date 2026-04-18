USE QLKHO_PhanVanHoang;
GO

PRINT N'BẮT ĐẦU RESET DỮ LIỆU...';

-- 1. Tắt tạm thời kiểm tra ràng buộc khóa ngoại (Foreign Keys)
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all";

-- 2. Dọn dẹp dữ liệu các bảng nghiệp vụ và danh mục (KHÔNG chạm vào Users, Roles, Permissions)
PRINT N'Đang xóa dữ liệu cũ...';
DELETE FROM AuditLogs;
DELETE FROM StockCards;
DELETE FROM TransferVoucherDetails;
DELETE FROM TransferVouchers;
DELETE FROM DeliveryVoucherDetails;
DELETE FROM DeliveryVouchers;
DELETE FROM InventoryAdjustments;
DELETE FROM CountingSheetDetails;
DELETE FROM CountingSheets;
DELETE FROM ReceivingVoucherDetails;
DELETE FROM ReceivingVouchers;
DELETE FROM Inventories;
DELETE FROM Products;
DELETE FROM Customers;
DELETE FROM Suppliers;
DELETE FROM Warehouses;
DELETE FROM Categories;

-- 3. Reset lại Identity (để ID tự động bắt đầu lại từ 1)
PRINT N'Đang reset lại Identity (ID)...';
DBCC CHECKIDENT ('Categories', RESEED, 0);
DBCC CHECKIDENT ('Warehouses', RESEED, 0);
DBCC CHECKIDENT ('Suppliers', RESEED, 0);
DBCC CHECKIDENT ('Customers', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Inventories', RESEED, 0);
DBCC CHECKIDENT ('ReceivingVouchers', RESEED, 0);
DBCC CHECKIDENT ('ReceivingVoucherDetails', RESEED, 0);
DBCC CHECKIDENT ('DeliveryVouchers', RESEED, 0);
DBCC CHECKIDENT ('DeliveryVoucherDetails', RESEED, 0);
DBCC CHECKIDENT ('TransferVouchers', RESEED, 0);
DBCC CHECKIDENT ('TransferVoucherDetails', RESEED, 0);
DBCC CHECKIDENT ('CountingSheets', RESEED, 0);
DBCC CHECKIDENT ('CountingSheetDetails', RESEED, 0);
DBCC CHECKIDENT ('InventoryAdjustments', RESEED, 0);
DBCC CHECKIDENT ('StockCards', RESEED, 0);
DBCC CHECKIDENT ('AuditLogs', RESEED, 0);

-- Bật lại kiểm tra ràng buộc khóa ngoại
EXEC sp_MSforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all";

PRINT N'=========================================';
PRINT N'ĐANG THÊM DỮ LIỆU MẪU MỚI...';
PRINT N'=========================================';

-- 4. INSERT Dữ liệu Danh mục (Categories)
SET IDENTITY_INSERT Categories ON;
INSERT INTO Categories (Id, Name, Description, CreatedAt, CreatedBy, IsDeleted) VALUES 
(1, N'Điện thoại thông minh', N'Smartphone các hãng Apple, Samsung, Xiaomi', GETDATE(), 'admin', 0),
(2, N'Laptop & Máy tính', N'Macbook, Laptop Gaming, Laptop Văn phòng', GETDATE(), 'admin', 0),
(3, N'Phụ kiện điện tử', N'Cáp, sạc, tai nghe, ốp lưng', GETDATE(), 'admin', 0),
(4, N'Thiết bị thông minh', N'Đồng hồ thông minh, loa bluetooth', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Categories OFF;

-- 5. INSERT Dữ liệu Kho hàng (Warehouses)
SET IDENTITY_INSERT Warehouses ON;
INSERT INTO Warehouses (Id, Name, Location, PhoneNumber, CreatedAt, CreatedBy, IsDeleted) VALUES 
(1, N'Kho Tổng Miền Bắc', N'KCN Tiên Sơn, Bắc Ninh', '19001001', GETDATE(), 'admin', 0),
(2, N'Kho Trung Tâm Hà Nội', N'123 Cầu Giấy, Hà Nội', '19001002', GETDATE(), 'admin', 0),
(3, N'Kho Miền Nam', N'Quận 9, TP.HCM', '19001003', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Warehouses OFF;

-- 6. INSERT Dữ liệu Nhà cung cấp (Suppliers)
SET IDENTITY_INSERT Suppliers ON;
INSERT INTO Suppliers (Id, Code, Name, ContactPerson, PhoneNumber, Email, Address, CreatedAt, CreatedBy, IsDeleted) VALUES 
(1, 'NCC-APPLE', N'Apple VN', N'Nguyễn Văn A', '0901234567', 'contact@apple.vn', N'Quận 1, TP.HCM', GETDATE(), 'admin', 0),
(2, 'NCC-SS', N'Samsung Elec VN', N'Trần Thị B', '0987654321', 'sales@samsung.com', N'SEVT, Thái Nguyên', GETDATE(), 'admin', 0),
(3, 'NCC-ANKER', N'Anker Official', N'Lê Văn C', '0912345678', 'partner@anker.vn', N'Đống Đa, Hà Nội', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Suppliers OFF;

-- 7. INSERT Dữ liệu Khách hàng (Customers)
SET IDENTITY_INSERT Customers ON;
INSERT INTO Customers (Id, Code, Name, PhoneNumber, Email, Address, CreatedAt, CreatedBy, IsDeleted) VALUES 
(1, 'KH-FPT', N'FPT Shop', '18006601', 'doitac@fptshop.com', N'Hà Nội', GETDATE(), 'admin', 0),
(2, 'KH-TGDD', N'Thế Giới Di Động', '18001060', 'b2b@tgdd.vn', N'Bình Dương', GETDATE(), 'admin', 0),
(3, 'KH-LAA', N'Khách lẻ VIP A', '0933333333', 'khacha@gmail.com', N'Cầu Giấy, HN', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Customers OFF;

-- 8. INSERT Dữ liệu Sản phẩm (Products)
-- Không INSERT RowVersion vì SQl Server tự động quản lý field này
SET IDENTITY_INSERT Products ON;
INSERT INTO Products (Id, SkuCode, Name, Description, Unit, CostPrice, SellingPrice, MinStockLevel, IsLotManaged, CategoryId, CreatedAt, CreatedBy, IsDeleted) VALUES 
(1, 'IP15-PRM-256-VN', N'iPhone 15 Pro Max 256GB VN/A', N'Điện thoại Apple iPhone 15 Pro Max Titan Tự Nhiên', N'Chiếc', 25000000, 29000000, 10, 1, 1, GETDATE(), 'admin', 0),
(2, 'SS-S24U-512', N'Samsung Galaxy S24 Ultra 512GB', N'Điện thoại Samsung AI cao cấp', N'Chiếc', 28000000, 31990000, 5, 1, 1, GETDATE(), 'admin', 0),
(3, 'MAC-M3-PRO-14', N'MacBook Pro 14 M3 Pro 18GB/512GB', N'Laptop Apple Macbook Pro màu Đen (Space Black)', N'Chiếc', 45000000, 49990000, 3, 1, 2, GETDATE(), 'admin', 0),
(4, 'LAP-ASUS-G15', N'Laptop Asus ROG Strix G15', N'Laptop đồ họa / Gaming cao cấp', N'Chiếc', 26000000, 28500000, 5, 1, 2, GETDATE(), 'admin', 0),
(5, 'ANK-20W-PD', N'Củ sạc Anker PowerPort III 20W', N'Sạc nhanh PD cho iPhone', N'Cái', 200000, 350000, 50, 0, 3, GETDATE(), 'admin', 0),
(6, 'AW-S9-41MM', N'Apple Watch Series 9 41mm', N'Đồng hồ viền nhôm dây cao su', N'Chiếc', 9000000, 10500000, 10, 0, 4, GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Products OFF;

-- 9. INSERT Tồn kho ban đầu (Inventories)
SET IDENTITY_INSERT Inventories ON;
INSERT INTO Inventories (Id, ProductId, WarehouseId, LotNumber, QuantityOnHand, ReservedQuantity, LocationInWarehouse, ExpiryDate, CreatedAt, CreatedBy, IsDeleted) VALUES 
(1, 1, 1, 'LOT2026-04A', 50, 0, 'A1-01', NULL, GETDATE(), 'admin', 0),
(2, 2, 1, 'LOT2026-04B', 30, 5, 'A1-02', NULL, GETDATE(), 'admin', 0),
(3, 3, 1, 'LOT2026-04C', 10, 0, 'A2-01', NULL, GETDATE(), 'admin', 0),
(4, 5, 2, 'LOT2026-04D', 200, 20, 'C1-05', NULL, GETDATE(), 'admin', 0),
(5, 1, 3, 'LOT2026-04E', 20, 0, 'K3-01', NULL, GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Inventories OFF;

-- 10. INSERT Dữ liệu Phiếu nhập (ReceivingVouchers) - Đã Hoàn Thành
SET IDENTITY_INSERT ReceivingVouchers ON;
INSERT INTO ReceivingVouchers (Id, Code, WarehouseId, SupplierId, Status, ReceivingDate, Notes, CreatedAt, CreatedBy, IsDeleted) VALUES 
(1, 'PN-20260401-01', 1, 1, 'Completed', GETDATE()-5, N'Nhập lô iPhone T4', GETDATE()-5, 'admin', 0),
(2, 'PN-20260405-02', 2, 3, 'Completed', GETDATE()-2, N'Nhập phụ kiện Anker', GETDATE()-2, 'admin', 0);
SET IDENTITY_INSERT ReceivingVouchers OFF;

-- Chi tiết phiếu nhập
SET IDENTITY_INSERT ReceivingVoucherDetails ON;
INSERT INTO ReceivingVoucherDetails (Id, ReceivingVoucherId, ProductId, Quantity, UnitPrice, LotNumber, CreatedAt, CreatedBy, IsDeleted) VALUES 
(1, 1, 1, 50, 25000000, 'LOT2026-04A', GETDATE()-5, 'admin', 0),
(2, 2, 5, 200, 200000, 'LOT2026-04D', GETDATE()-2, 'admin', 0);
SET IDENTITY_INSERT ReceivingVoucherDetails OFF;

-- 11. INSERT Dữ liệu Thẻ kho (StockCards) tương ứng lịch sử
SET IDENTITY_INSERT StockCards ON;
INSERT INTO StockCards (Id, ProductId, WarehouseId, LotNumber, TransactionDate, TransactionType, ReferenceCode, BeforeQuantity, ChangeQuantity, AfterQuantity, Notes, CreatedAt, CreatedBy, IsDeleted) VALUES 
(1, 1, 1, 'LOT2026-04A', GETDATE()-5, 'Inbound', 'PN-20260401-01', 0, 50, 50, N'Nhập hàng', GETDATE()-5, 'admin', 0),
(2, 5, 2, 'LOT2026-04D', GETDATE()-2, 'Inbound', 'PN-20260405-02', 0, 200, 200, N'Nhập hàng', GETDATE()-2, 'admin', 0);
SET IDENTITY_INSERT StockCards OFF;

PRINT N'=========================================';
PRINT N'✅ THÊM DỮ LIỆU THÀNH CÔNG!';
PRINT N'=========================================';
GO
