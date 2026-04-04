-- SEED SAMPLE DATA V2 FOR WMS PROJECT - PHAN VAN HOANG
-- RICH DATASET WITH 50+ PRODUCTS, 15 CATEGORIES, 15 SUPPLIERS, 15 CUSTOMERS
-- STATUS: COMPLETED/DISPATCHED FOR TREND CHARTS
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

-- 1. XÓA DỮ LIỆU CŨ (Trừ Roles và SystemUsers)
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

-- 2. CATEGORIES (15)
SET IDENTITY_INSERT Categories ON;
INSERT INTO Categories (Id, Name, Description, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, N'Điện thoại & Tablet', N'Smartphones, iPads, Android Tablets', GETDATE(), 'admin', 0),
(2, N'Laptop & PC', N'Laptops, Desktops, Workstations', GETDATE(), 'admin', 0),
(3, N'Linh kiện máy tính', N'CPU, RAM, GPU, SSD, HDD', GETDATE(), 'admin', 0),
(4, N'Thiết bị mạng', N'Routers, Switches, Access Points', GETDATE(), 'admin', 0),
(5, N'Đồ gia dụng lớn', N'Tủ lạnh, Máy giặt, Điều hòa', GETDATE(), 'admin', 0),
(6, N'Đồ gia dụng nhỏ', N'Nồi cơm điện, Lò vi sóng, Máy xay', GETDATE(), 'admin', 0),
(7, N'Âm thanh', N'Loa, Tai nghe, Soundbars', GETDATE(), 'admin', 0),
(8, N'Phụ kiện công nghệ', N'Sạc cáp, Ốp lưng, Chuột, Bàn phím', GETDATE(), 'admin', 0),
(9, N'Thực phẩm đóng gói', N'Mì tôm, Đồ hộp, Gia vị', GETDATE(), 'admin', 0),
(10, N'Đồ uống', N'Nước ngọt, Bia, Sữa', GETDATE(), 'admin', 0),
(11, N'Hóa mỹ phẩm', N'Dầu gội, Sữa tắm, Bột giặt', GETDATE(), 'admin', 0),
(12, N'Dụng cụ nhà kho', N'Băng dính, Thùng carton, Pallet', GETDATE(), 'admin', 0),
(13, N'Văn phòng phẩm', N'Giấy in, Bút, Sổ tay', GETDATE(), 'admin', 0),
(14, N'Thời trang & Balo', N'Áo thun, Balo kho, Đồng phục', GETDATE(), 'admin', 0),
(15, N'Sức khỏe & Y tế', N'Khẩu trang, Cồn, Kit test', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Categories OFF;

-- 3. SUPPLIERS (15)
SET IDENTITY_INSERT Suppliers ON;
INSERT INTO Suppliers (Id, Code, Name, PhoneNumber, Email, Address, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 'NCC-APPLE', N'Apple Asia Limited', '024-11112222', 'contact@apple.com', N'P. Bến Nghé, Quận 1, TP.HCM', GETDATE(), 'admin', 0),
(2, 'NCC-SAMSUNG', N'Samsung Electronics VN', '0241-123456', 'sales@samsung.vn', N'KCN Yên Phong, Bắc Ninh', GETDATE(), 'admin', 0),
(3, 'NCC-DELL', N'Dell Vietnam Distributor', '028-33334444', 'support@dell.com.vn', N'KCN Tân Thuận, TP.HCM', GETDATE(), 'admin', 0),
(4, 'NCC-ACER', N'Acer Vietnam', '028-55556666', 'service@acer.vn', N'Quận 10, TP.HCM', GETDATE(), 'admin', 0),
(5, 'NCC-TPLL', N'TPLink Vietnam', '024-77778888', 'info@tplink.vn', N'Quận Cầu Giấy, Hà Nội', GETDATE(), 'admin', 0),
(6, 'NCC-ELG', N'LG Electronics VN', '0243-987654', 'lg@lg.vn', N'Số 1 Đại lộ Thăng Long, Hà Nội', GETDATE(), 'admin', 0),
(7, 'NCC-PAN', N'Panasonic Vietnam', '0243-111222', 'pan@panasonic.vn', N'KCN Thăng Long, Hà Nội', GETDATE(), 'admin', 0),
(8, 'NCC-ACE', N'Acecook Việt Nam', '028-32223333', 'cs@acecook.vn', N'KCN Tân Bình, TP.HCM', GETDATE(), 'admin', 0),
(9, 'NCC-MAS', N'Masan Group', '024-44445555', 'masan@masan.vn', N'Mễ Trì, Nam Từ Liêm, Hà Nội', GETDATE(), 'admin', 0),
(10, 'NCC-VIN', N'Vinamilk Vietnam', '028-66667777', 'care@vinamilk.vn', N'P. Tân Phú, Quận 7, TP.HCM', GETDATE(), 'admin', 0),
(11, 'NCC-UNL', N'Unilever Vietnam', '028-88889999', 'care@unilever.vn', N'KCN Tây Bắc Củ Chi, TP.HCM', GETDATE(), 'admin', 0),
(12, 'NCC-HP', N'Tập đoàn Hòa Phát', '024-22223333', 'info@hoaphat.com.vn', N'66 Nguyễn Du, Hà Nội', GETDATE(), 'admin', 0),
(13, 'NCC-FPT', N'FPT Distribution', '024-55550000', 'fpt@fpt.vn', N'Duy Tân, Hà Nội', GETDATE(), 'admin', 0),
(14, 'NCC-PHT', N'Phong Vũ Computer', '024-99998888', 'contact@phongvu.vn', N'Láng Hạ, Hà Nội', GETDATE(), 'admin', 0),
(15, 'NCC-KIM', N'Kimberly-Clark VN', '028-00001111', 'kc@kimberly.vn', N'Quận 1, TP.HCM', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Suppliers OFF;

-- 4. CUSTOMERS (15)
SET IDENTITY_INSERT Customers ON;
INSERT INTO Customers (Id, Code, Name, PhoneNumber, Email, Address, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 'KH-TGDD', N'Thế Giới Di Động', '18001060', 'cskh@thegioididong.com', N'Toàn quốc', GETDATE(), 'admin', 0),
(2, 'KH-DMX', N'Điện Máy Xanh', '18001061', 'cskh@dienmayxanh.com', N'Toàn quốc', GETDATE(), 'admin', 0),
(3, 'KH-FPT', N'FPT Shop', '18006601', 'sh@fpt.com.vn', N'Hà Nội & HCM', GETDATE(), 'admin', 0),
(4, 'KH-CELL', N'CellphoneS', '18002097', 'contact@cellphones.vn', N'Hà Nội & HCM', GETDATE(), 'admin', 0),
(5, 'KH-VIN', N'WinMart+', '024-33334444', 'winmart@vin.vn', N'Toàn quốc', GETDATE(), 'admin', 0),
(6, 'KH-BHX', N'Bách Hóa Xanh', '19001908', 'cskh@bhx.vn', N'TP.HCM & Miền Tây', GETDATE(), 'admin', 0),
(7, 'KH-COOP', N'Co.op Mart', '1900555568', 'chamsockhachhang@coopmart.vn', N'Toàn quốc', GETDATE(), 'admin', 0),
(8, 'KH-BIGC', N'Go! & Big C', '19001880', 'bigc@go.vn', N'Toàn quốc', GETDATE(), 'admin', 0),
(9, 'KH-PX', N'Hệ thống Nhà thuốc Pharmacity', '18006821', 'care@pharmacity.vn', N'Toàn quốc', GETDATE(), 'admin', 0),
(10, 'KH-LOTT', N'Lotte Mart Vietnam', '028-33335555', 'lotte@lotte.vn', N'Hà Nội & HCM', GETDATE(), 'admin', 0),
(11, 'KH-AEON', N'AEON Mall Vietnam', '024-66668888', 'aeon@aeon.vn', N'Hà Nội & HCM', GETDATE(), 'admin', 0),
(12, 'KH-HAPO', N'Hapro Mart', '024-99990000', 'hapro@hapro.vn', N'Miền Bắc', GETDATE(), 'admin', 0),
(13, 'KH-GEAM', N'Gia hàng điện máy Chợ Lớn', '028-38563388', 'cholon@cholon.vn', N'Miền Nam', GETDATE(), 'admin', 0),
(14, 'KH-NHAB', N'Nhà sách FAHASA', '1900636467', 'info@fahasa.vn', N'Toàn quốc', GETDATE(), 'admin', 0),
(15, 'KH-LE', N'Khách mua lẻ vãng lai', '0900000000', 'retail@example.com', N'Cửa hàng', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Customers OFF;

-- 5. WAREHOUSES
SET IDENTITY_INSERT Warehouses ON;
INSERT INTO Warehouses (Id, Name, Location, PhoneNumber, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, N'Kho Tổng Miền Bắc (Hà Nội)', N'Số 1 Quang Trung, Hà Đông, Hà Nội', '0243-111111', GETDATE(), 'admin', 0),
(2, N'Kho Trung tâm Miền Nam (HCM)', N'KCN Sóng Thần, Bình Dương', '0283-222222', GETDATE(), 'admin', 0),
(3, N'Kho Luân chuyển Miền Trung (Đà Nẵng)', N'KCN Hòa Khánh, Đà Nẵng', '0236-333333', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Warehouses OFF;

-- 6. PRODUCTS (50+)
SET IDENTITY_INSERT Products ON;
INSERT INTO Products (Id, SkuCode, Name, CategoryId, Unit, MinStockLevel, IsLotManaged, CostPrice, SellingPrice, Description, ImageUrl, CreatedAt, CreatedBy, IsDeleted) VALUES
-- Category 1: Điện thoại
(1, 'IP15PM-256', N'iPhone 15 Pro Max 256GB Platinum', 1, N'Cái', 5, 1, 30000000, 34500000, N'Hàng chính hãng Apple Vietnam', 'https://example.com/ip15pm.jpg', GETDATE(), 'admin', 0),
(2, 'S24ULTRA-512', N'Samsung S24 Ultra 512GB Titanium', 1, N'Cái', 3, 1, 28000000, 32000000, N'Samsung VN chính hãng', 'https://example.com/s24u.jpg', GETDATE(), 'admin', 0),
(3, 'OPPO-REN-11', N'Oppo Reno 11 5G 128GB', 1, N'Cái', 10, 1, 8000000, 9900000, N'Oppo chính hãng', 'https://example.com/reno11.jpg', GETDATE(), 'admin', 0),
(4, 'iPad-AIR-M2', N'iPad Air M2 11 inch 128GB WiFi', 1, N'Cái', 5, 1, 14000000, 16900000, N'Chip M2 mạnh mẽ', 'https://example.com/ipadair.jpg', GETDATE(), 'admin', 0),
-- Category 2: Laptop
(5, 'MAC-M3-14', N'MacBook Pro 14 M3 8GB/512GB', 2, N'Bộ', 3, 1, 36000000, 42000000, N'Chip M3 Apple', 'https://example.com/macm3.jpg', GETDATE(), 'admin', 0),
(6, 'DELL-XPS-13', N'Dell XPS 13 9315 i7/16/512', 2, N'Bộ', 2, 1, 32000000, 38500000, N'Laptop doanh nhân cao cấp', 'https://example.com/dellxps.jpg', GETDATE(), 'admin', 0),
(7, 'ACER-ASP-5', N'Acer Aspire 5 A515 i5 Gen 13', 2, N'Bộ', 10, 1, 12000000, 14500000, N'Cân bằng hiệu năng và giá', 'https://example.com/aceraspire.jpg', GETDATE(), 'admin', 0),
-- Category 3: Linh kiện
(8, 'SSD-SAM-980-1TB', N'SSD Samsung 980 Pro 1TB NVMe', 3, N'Cái', 20, 0, 1800000, 2250000, N'Tốc độ đọc ghi cực cao', 'https://example.com/ssd980.jpg', GETDATE(), 'admin', 0),
(9, 'RAM-COR-16G-D5', N'Ram Corsair Vengeance 16GB DDR5', 3, N'Thanh', 15, 0, 1400000, 1750000, N'Tương thích Intel/AMD mới nhất', 'https://example.com/ramcor.jpg', GETDATE(), 'admin', 0),
(10, 'CPU-I7-14700K', N'Intel Core i7 14700K 20 Cores', 3, N'Hộp', 5, 1, 9500000, 11200000, N'Thế hệ 14 Raptor Lake Refresh', 'https://example.com/i714700k.jpg', GETDATE(), 'admin', 0),
-- Category 5: Gia dụng lớn
(11, 'LG-TV-65-OLED', N'TV LG 65 inch OLED 4K C3', 5, N'Cái', 2, 1, 42000000, 52000000, N'Màn hình hoàn hảo cho cinema', 'https://example.com/lgoled.jpg', GETDATE(), 'admin', 0),
(12, 'PANA-TL-400L', N'Tủ lạnh Panasonic 400L Prime Fresh', 5, N'Cái', 3, 1, 15000000, 18900000, N'Công nghệ cấp đông mềm', 'https://example.com/panatl.jpg', GETDATE(), 'admin', 0),
(13, 'AQ-MAT-10KG', N'Máy giặt Aqua 10kg cửa trước', 5, N'Cái', 5, 1, 7500000, 9200000, N'Inverter tiết kiệm điện', 'https://example.com/aquamg.jpg', GETDATE(), 'admin', 0),
-- Category 9: Thực phẩm
(14, 'HH-HAOTOM', N'Mì Hảo Hảo Tôm Chua Cay', 9, N'Thùng', 50, 1, 105000, 122000, N'30 gói/thùng chính hiệu', 'https://example.com/hao2.jpg', GETDATE(), 'admin', 0),
(15, 'CHINSU-TC-1KG', N'Tương Ớt Chinsu 1kg', 9, N'Chai', 100, 1, 35000, 42000, N'Cay nồng tự nhiên', 'https://example.com/chinsu.jpg', GETDATE(), 'admin', 0),
(16, 'GAO-ST25-5KG', N'Gạo ST25 chuẩn túi 5kg', 9, N'Túi', 40, 1, 160000, 195000, N'Gạo ngon nhất thế giới', 'https://example.com/gaost25.jpg', GETDATE(), 'admin', 0),
-- Category 10: Đồ uống
(17, 'VINAMILK-IT-1L', N'Sữa tươi Vinamilk ít đường 1L', 10, N'Hộp', 120, 1, 28000, 34000, N'Lốc 12 hộp', 'https://example.com/vinamilk1.jpg', GETDATE(), 'admin', 0),
(18, 'COCA-330-24', N'Coca Cola Lon 330ml 24 lon', 10, N'Thùng', 100, 1, 185000, 215000, N'Nước giải khát có gas', 'https://example.com/coca.jpg', GETDATE(), 'admin', 0),
(19, 'HEINEKEN-24', N'Bia Heineken Silver 24 lon', 10, N'Thùng', 80, 1, 420000, 465000, N'Vị nhẹ êm', 'https://example.com/ken.jpg', GETDATE(), 'admin', 0),
-- Category 11: Mỹ phẩm
(20, 'OMO-MATIC-4KG', N'Nước giặt OMO Matic Cửa trước 4kg', 11, N'Túi', 50, 1, 165000, 198000, N'Bền màu bền sợi', 'https://example.com/omo.jpg', GETDATE(), 'admin', 0),
(21, 'SUNSILK-650G', N'Dầu gội Sunsilk Óng mượt 650g', 11, N'Chai', 30, 1, 115000, 142000, N'Dưỡng tóc mềm mại', 'https://example.com/sunsilk.jpg', GETDATE(), 'admin', 0),
-- Lặp lại hoặc thêm cho đủ 50 sản phẩm (Rút gọn tên để tiết kiệm file)
(22, 'LOGI-MX-MST3', N'Chuột Logitech MX Master 3S', 8, N'Cái', 10, 0, 2100000, 2650000, N'Siêu chuột văn phòng', 'https://example.com/mx3s.jpg', GETDATE(), 'admin', 0),
(23, 'KEY-FL-ES-GP', N'Bàn phím cơ FL-Esports GP75', 8, N'Cái', 8, 1, 1500000, 1950000, N'Bàn phím cơ hotswap', 'https://example.com/fl.jpg', GETDATE(), 'admin', 0),
(24, 'AIRPODS-PRO-2', N'Apple AirPods Pro 2 MagSafe', 7, N'Bộ', 15, 1, 4800000, 5950000, N'Chống ồn hiệu quả', 'https://example.com/ap2.jpg', GETDATE(), 'admin', 0),
(25, 'MARSHALL-EM-2', N'Loa Marshall Emberton II Black', 7, N'Cái', 5, 1, 3200000, 4150000, N'Phong cách cổ điển', 'https://example.com/emberton.jpg', GETDATE(), 'admin', 0),
(26, 'MI-RICE-1.8L', N'Nồi cơm điện Xiaomi 1.8L', 6, N'Cái', 10, 1, 1100000, 1450000, N'Điều khiển qua app', 'https://example.com/mi.jpg', GETDATE(), 'admin', 0),
(27, 'PHILIPS-AFD-4L', N'Nồi chiên không dầu Philips HD9252', 6, N'Cái', 7, 1, 2400000, 3100000, N'Dung tích 4.1L', 'https://example.com/philips.jpg', GETDATE(), 'admin', 0),
(28, 'ROUTER-TP-AX53', N'Router TP-Link Archer AX53 WiFi 6', 4, N'Bộ', 12, 1, 1300000, 1650000, N'Phát wifi xuyên tường', 'https://example.com/ax53.jpg', GETDATE(), 'admin', 0),
(29, 'PAPER-DoubleA', N'Giấy A4 Double A 80gsm 500 tờ', 13, N'Ram', 200, 0, 85000, 105000, N'Giấy in cao cấp', 'https://example.com/doublea.jpg', GETDATE(), 'admin', 0),
(30, 'PEN-TL-027', N'Bút bi Thiên Long 027 Box 20', 13, N'Hộp', 100, 0, 65000, 85000, N'Ngòi 0.5mm', 'https://example.com/pen.jpg', GETDATE(), 'admin', 0),
(31, 'BALO-HIK-WMS', N'Balo Hikeshi chuẩn kho', 14, N'Cái', 50, 0, 250000, 350000, N'Chống nước nhẹ', 'https://example.com/balo.jpg', GETDATE(), 'admin', 0),
(32, 'UNIFORM-XL', N'Đồng phục Kho WMS Size XL', 14, N'Bộ', 100, 0, 120000, 180000, N'Vải kaki thoáng mát', 'https://example.com/ao.jpg', GETDATE(), 'admin', 0),
(33, 'MASK-3D-50', N'Khẩu trang 3D Mask hộp 50 cái', 15, N'Hộp', 200, 0, 45000, 65000, N'Kháng khuẩn chuẩn Nhật', 'https://example.com/mask.jpg', GETDATE(), 'admin', 0),
(34, 'ALCOHOL-70D', N'Cồn y tế 70 độ chai 500ml', 15, N'Chai', 100, 0, 15000, 22000, N'Sát trùng nhanh', 'https://example.com/con.jpg', GETDATE(), 'admin', 0),
-- Thêm các sản phẩm tương tự
(35, 'IP14-128', N'iPhone 14 128GB Blue', 1, N'Cái', 5, 1, 16000000, 18900000, N'', '', GETDATE(), 'admin', 0),
(36, 'SS-A54-BK', N'Samsung A54 5G Black', 1, N'Cái', 10, 1, 7500000, 9200000, N'', '', GETDATE(), 'admin', 0),
(37, 'LAP-LENO-ID-3', N'Lenovo IdeaPad 3 Slim i3', 2, N'Bộ', 5, 1, 9500000, 11500000, N'', '', GETDATE(), 'admin', 0),
(38, 'LAP-HP-VICT-15', N'HP Victus 15 Ryzen 5 RTX 2050', 2, N'Bộ', 3, 1, 15500000, 18500000, N'', '', GETDATE(), 'admin', 0),
(39, 'SSD-WD-500G', N'SSD WD Blue 500GB SATA', 3, N'Cái', 15, 0, 850000, 1150000, N'', '', GETDATE(), 'admin', 0),
(40, 'VGA-RTX-4060', N'ASUS RTX 4060 Dual 8GB', 3, N'Hộp', 4, 1, 7800000, 9200000, N'', '', GETDATE(), 'admin', 0),
(41, 'LG-TV-55-UQ', N'TV LG 55 inch 4K UHD', 5, N'Cái', 5, 1, 9500000, 12500000, N'', '', GETDATE(), 'admin', 0),
(42, 'SAM-MG-8KG', N'Máy giặt Samsung EchoBubble 8kg', 5, N'Cái', 3, 1, 6500000, 8100000, N'', '', GETDATE(), 'admin', 0),
(43, 'BLENDER-T-75', N'Máy xay Tefal BL43B166', 6, N'Cái', 10, 1, 850000, 1150000, N'', '', GETDATE(), 'admin', 0),
(44, 'KETTLE-SUN-1.8L', N'Ấm siêu tốc Sunhouse 1.8L', 6, N'Cái', 20, 1, 150000, 220000, N'', '', GETDATE(), 'admin', 0),
(45, 'NESCAFE-3IN1', N'Cà phê Nescafe 3in1 bịch 20 gói', 10, N'Bịch', 100, 1, 45000, 58000, N'', '', GETDATE(), 'admin', 0),
(46, 'PEPSI-330-24', N'Pepsi Lon 330ml thùng 24', 10, N'Thùng', 50, 1, 180000, 205000, N'', '', GETDATE(), 'admin', 0),
(47, 'CLEAR-MEN-450G', N'Dầu gội Clear Men Sạch gàu 450g', 11, N'Chai', 20, 1, 105000, 135000, N'', '', GETDATE(), 'admin', 0),
(48, 'COLGATE-150G', N'Kem đánh răng Colgate 150g', 11, N'Tuýp', 100, 1, 25000, 32000, N'', '', GETDATE(), 'admin', 0),
(49, 'PALLET-WOOD', N'Pallet gỗ thông 1mx1m2', 12, N'Cái', 300, 0, 150000, 150000, N'', '', GETDATE(), 'admin', 0),
(50, 'TAPE-CLEAR', N'Băng dính trong 5cm 100yard', 12, N'Cuộn', 500, 0, 8000, 12000, N'', '', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Products OFF;

-- 7. INVENTORIES (Thiết lập tồn đầu kỳ cho cả 3 kho)
SET IDENTITY_INSERT Inventories ON;
-- Hà Nội (CH1)
INSERT INTO Inventories (Id, ProductId, WarehouseId, LotNumber, ExpiryDate, QuantityOnHand, ReservedQuantity, LocationInWarehouse, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 1, 1, 'LOT-A-01', '2026-03-01', 20, 0, 'KE-01', GETDATE(), 'admin', 0),
(2, 5, 1, 'BATCH-1', NULL, 10, 0, 'LAP-02', GETDATE(), 'admin', 0),
(3, 14, 1, 'EXP-2025', '2025-12-31', 200, 0, 'FOOD-01', GETDATE(), 'admin', 0),
(4, 33, 1, 'MED-24', '2027-01-01', 500, 0, 'MED-01', GETDATE(), 'admin', 0),
-- HCM (CH2)
(5, 1, 2, 'LOT-A-02', '2026-03-01', 15, 0, 'Q1-A', GETDATE(), 'admin', 0),
(6, 11, 2, 'TV-BATCH', '2028-01-01', 8, 0, 'ELE-01', GETDATE(), 'admin', 0),
-- Đà Nẵng (CH3)
(7, 14, 3, 'EXP-2025', '2025-12-31', 100, 0, 'DN-F-01', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT Inventories OFF;

-- 8. RECEIVING VOUCHERS (Lịch sử 30 ngày)
SET IDENTITY_INSERT ReceivingVouchers ON;
INSERT INTO ReceivingVouchers (Id, Code, WarehouseId, SupplierId, Status, ReceivingDate, Notes, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 'PN-HN-001', 1, 1, 'Completed', DATEADD(day, -25, GETDATE()), N'Lô Apple đầu tháng', DATEADD(day, -25, GETDATE()), 'admin', 0),
(2, 'PN-HN-002', 1, 8, 'Completed', DATEADD(day, -20, GETDATE()), N'Nhập mì tôm định kỳ', DATEADD(day, -20, GETDATE()), 'admin', 0),
(3, 'PN-SG-001', 2, 6, 'Completed', DATEADD(day, -15, GETDATE()), N'Nhập TV LG HCM', DATEADD(day, -15, GETDATE()), 'admin', 0),
(4, 'PN-DN-001', 3, 10, 'Completed', DATEADD(day, -10, GETDATE()), N'Nhập sữa Đà Nẵng', DATEADD(day, -10, GETDATE()), 'admin', 0),
(5, 'PN-HN-003', 1, 2, 'Completed', DATEADD(day, -5, GETDATE()), N'Samsung S24 mới về', DATEADD(day, -5, GETDATE()), 'admin', 0),
(6, 'PN-SG-002', 2, 11, 'Completed', DATEADD(day, -2, GETDATE()), N'Omo & Sunsilk HCM', DATEADD(day, -2, GETDATE()), 'admin', 0);
SET IDENTITY_INSERT ReceivingVouchers OFF;

SET IDENTITY_INSERT ReceivingVoucherDetails ON;
INSERT INTO ReceivingVoucherDetails (Id, ReceivingVoucherId, ProductId, LotNumber, ExpiryDate, Quantity, UnitPrice, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 1, 1, 'LOT-A-01', '2026-03-01', 20, 30000000, GETDATE(), 'admin', 0),
(2, 2, 14, 'EXP-2025', '2025-12-31', 200, 105000, GETDATE(), 'admin', 0),
(3, 3, 11, 'TV-BATCH', '2028-01-01', 8, 42000000, GETDATE(), 'admin', 0),
(4, 4, 17, 'MILK-25', '2025-06-01', 120, 28000, GETDATE(), 'admin', 0),
(5, 5, 2, 'SAM-25', '2026-06-01', 10, 28000000, GETDATE(), 'admin', 0),
(6, 6, 20, 'OMO-25', '2027-01-01', 50, 165000, GETDATE(), 'admin', 0);
SET IDENTITY_INSERT ReceivingVoucherDetails OFF;

-- 9. DELIVERY VOUCHERS (Lịch sử 30 ngày)
SET IDENTITY_INSERT DeliveryVouchers ON;
INSERT INTO DeliveryVouchers (Id, Code, WarehouseId, CustomerId, Status, DeliveryDate, Notes, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 'PX-HN-001', 1, 1, 'Dispatched', DATEADD(day, -22, GETDATE()), N'Giao hàng cho TGDD', GETDATE(), 'admin', 0),
(2, 'PX-SG-001', 2, 2, 'Dispatched', DATEADD(day, -18, GETDATE()), N'Giao máy giặt ĐMX', GETDATE(), 'admin', 0),
(3, 'PX-HN-002', 1, 5, 'Dispatched', DATEADD(day, -12, GETDATE()), N'Cửa hàng WinMart lấy gạo/mì', GETDATE(), 'admin', 0),
(4, 'PX-DN-001', 3, 3, 'Dispatched', DATEADD(day, -7, GETDATE()), N'FPT Shop Đà Nẵng lấy iPad', GETDATE(), 'admin', 0),
(5, 'PX-HN-003', 1, 4, 'Dispatched', DATEADD(day, -3, GETDATE()), N'CellphoneS lấy iPhone 15', GETDATE(), 'admin', 0),
(6, 'PX-SG-002', 2, 6, 'Dispatched', DATEADD(day, -1, GETDATE()), N'Bách Hóa Xanh lấy Omo', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT DeliveryVouchers OFF;

SET IDENTITY_INSERT DeliveryVoucherDetails ON;
INSERT INTO DeliveryVoucherDetails (Id, DeliveryVoucherId, ProductId, LotNumber, Quantity, SellingPrice, CostPrice, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 1, 1, 'LOT-A-01', 5, 34500000, 30000000, GETDATE(), 'admin', 0),
(2, 2, 13, 'AQ-MAT', 3, 9200000, 7500000, GETDATE(), 'admin', 0),
(3, 3, 14, 'EXP-2025', 100, 122000, 105000, GETDATE(), 'admin', 0),
(4, 3, 16, 'SAM-25', 20, 195000, 160000, GETDATE(), 'admin', 0),
(5, 5, 1, 'LOT-A-01', 2, 34500000, 30000000, GETDATE(), 'admin', 0);
SET IDENTITY_INSERT DeliveryVoucherDetails OFF;

-- 10. STOCK CARDS (Sơ lược lịch sử - 1 số sản phẩm chính)
SET IDENTITY_INSERT StockCards ON;
INSERT INTO StockCards (Id, ProductId, WarehouseId, TransactionType, ReferenceCode, LotNumber, BeforeQuantity, ChangeQuantity, AfterQuantity, TransactionDate, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 1, 1, 'Inbound', 'PN-HN-001', 'LOT-A-01', 0, 20, 20, DATEADD(day, -25, GETDATE()), GETDATE(), 'admin', 0),
(2, 1, 1, 'Outbound', 'PX-HN-001', 'LOT-A-01', 20, -5, 15, DATEADD(day, -22, GETDATE()), GETDATE(), 'admin', 0),
(3, 14, 1, 'Inbound', 'PN-HN-002', 'EXP-2025', 0, 200, 200, DATEADD(day, -20, GETDATE()), GETDATE(), 'admin', 0),
(4, 14, 1, 'Outbound', 'PX-HN-002', 'EXP-2025', 200, -100, 100, DATEADD(day, -12, GETDATE()), GETDATE(), 'admin', 0);
SET IDENTITY_INSERT StockCards OFF;

-- 11. TRANSFER VOUCHERS
SET IDENTITY_INSERT TransferVouchers ON;
INSERT INTO TransferVouchers (Id, Code, FromWarehouseId, ToWarehouseId, Status, TransferDate, Notes, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 'DC-001', 1, 2, 'Completed', DATEADD(day, -8, GETDATE()), N'Điều chuyển iPhone từ HN vào SG', GETDATE(), 'admin', 0),
(2, 'DC-002', 2, 3, 'Completed', DATEADD(day, -4, GETDATE()), N'Điều chuyển hàng Gia dụng đi Đà Nẵng', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT TransferVouchers OFF;

SET IDENTITY_INSERT TransferVoucherDetails ON;
INSERT INTO TransferVoucherDetails (Id, TransferVoucherId, ProductId, LotNumber, Quantity, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 1, 1, 'LOT-A-01', 5, GETDATE(), 'admin', 0),
(2, 2, 12, 'PANA-TL', 2, GETDATE(), 'admin', 0);
SET IDENTITY_INSERT TransferVoucherDetails OFF;

-- 12. COUNTING SHEETS
SET IDENTITY_INSERT CountingSheets ON;
INSERT INTO CountingSheets (Id, Code, WarehouseId, Status, CountingDate, Notes, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 'PK-HN-001', 1, 'Completed', DATEADD(day, -15, GETDATE()), N'Kiểm kê định kỳ giữa tháng', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT CountingSheets OFF;

SET IDENTITY_INSERT CountingSheetDetails ON;
INSERT INTO CountingSheetDetails (Id, CountingSheetId, ProductId, LotNumber, SystemQuantity, ActualQuantity, Note, CreatedAt, CreatedBy, IsDeleted) VALUES
(1, 1, 1, 'LOT-A-01', 15, 15, N'Khớp', GETDATE(), 'admin', 0),
(2, 1, 14, 'EXP-2025', 100, 99, N'Hao hụt 1 thùng do va đập', GETDATE(), 'admin', 0);
SET IDENTITY_INSERT CountingSheetDetails OFF;

COMMIT TRANSACTION;
GO
