# 📦 QLKHO_PHANVANHOANG - Warehouse Management System (WMS) v2.0
## GIẢI PHÁP QUẢN LÝ KHO DOANH NGHIỆP TOÀN DIỆN (100% COMPLETE)

[![version](https://img.shields.io/badge/version-2.0.0-blue)](https://github.com/hoangphan04211/be-asp)
[![license](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![dotnet](https://img.shields.io/badge/.NET-8.0-blueviolet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Docker](https://img.shields.io/badge/Docker-Enabled-brightgreen)](https://www.docker.com/)

---

### 🌟 Tính năng Mới (New Features)
1. **Phân quyền Động (Dynamic RBAC):** Quản lý quyền hạn chi tiết đến từng nút bấm và menu.
2. **In ấn & Xuất bản (Printing & Reporting):** In phiếu PDF chuyên nghiệp, xuất Excel tồn kho.
3. **Docker Ready:** Triển khai nhanh chóng với Docker Compose.
4. **UX/UI Polish:** Giao diện Glassmorphism hiện đại, thông báo thời gian thực SignalR.

---

##  Kiến trúc Hệ thống (System Architecture)

Dự án được xây dựng dựa trên các tiêu chuẩn **Clean Code** và **Enterprise Design Patterns**, đảm bảo tính mở rộng (Scalability) và bảo trì (Maintainability) lâu dài.

###  Cấu trúc thư mục Chi tiết (Folder Structure)

1.  **`Controllers/` (Tầng Giao Diện - Presentation Layer)**
    - Tiếp nhận HTTP Request, điều hướng đến Service và phản hồi theo chuẩn RESTful API.
    - Bao gồm: `AuthController`, `ProductsController`, `WarehousesController`, `CategoriesController`, `SuppliersController`, `CustomersController` (Master Data), `InboundController`, `OutboundController`, `TransferController`, `CountingController`, `InventoryController`, `DashboardController`, `UsersController`, `AuditController`, `ExcelController`, `FileController`, `TrashController`.

2.  **`Services/` (Tầng Nghiệp Vụ - Business Logic Layer)**
    - Xử lý logic nghiệp vụ cốt lõi: tính **giá vốn bình quân gia quyền (Weighted Average Cost)**, kiểm tra ngưỡng tồn kho, logic kiểm kê, chuyển kho.

3.  **`Repositories/` (Tầng Truy Cập Dữ Liệu - Data Access Layer)**
    - Pattern **Generic Repository** + **Unit of Work** đảm bảo tính nhất quán transaction và dễ dàng đổi database.

4.  **`Models/` (Domain Models)** — Xem chi tiết tại [Cấu trúc Database](#-cấu-trúc-database-database-schema) bên dưới.

5.  **`DTOs/`** — Objects trung gian, bảo vệ Entity gốc và tối ưu payload API.

6.  **`Data/`** — `ApplicationDbContext` với Audit Log, Soft Delete, Concurrency (RowVersion).

7.  **`Jobs/`** — **Hangfire** cho tác vụ định kỳ (cảnh báo tồn thấp cuối ngày).

8.  **`Middlewares/`** — `ExceptionMiddleware` bắt lỗi Runtime, trả về định dạng lỗi thống nhất.

9.  **`Helpers/`** — `ApiResponse<T>`, `PagedResult<T>`, `PaginationParams`.

10. **`Validators/`** — **FluentValidation** cho dữ liệu đầu vào (SKU không trùng, số lượng > 0...).

11. **`Profiles/`** — **AutoMapper** cấu hình ánh xạ Entity ↔ DTO (bao gồm tính `TotalAmount`).

12. **`Hubs/`** — `NotificationHub` qua **SignalR** gửi cảnh báo thời gian thực.

13. **`QLKHO_PhanVanHoang.Tests/`** — **xUnit + Moq**, 11/11 test cases passed.

14. **`HashTool/`** — Công cụ phụ trợ C# hỗ trợ tạo chuỗi băm (BCrypt Hash) để khởi tạo mật khẩu mặc định cho các tài khoản quản trị khi triển khai lần đầu.



##  Tính Năng Nâng Cao (Advanced Features)

Dự án triển khai các kỹ thuật xử lý dữ liệu chuẩn doanh nghiệp để đảm bảo tính toàn vẹn và minh bạch:

### Kiểm soát Đồng thời (Optimistic Concurrency)
- Sử dụng thuộc tính `RowVersion` (Timestamp) trên tất cả các Entity chính.
- Ngăn chặn tình trạng ghi đè dữ liệu nếu hai người dùng cùng chỉnh sửa một bản ghi (ví dụ: cùng duyệt một phiếu nhập) tại cùng một thời điểm.

### Cơ chế Xóa mềm (Soft Delete)
- Tích hợp **Global Query Filter** trong EF Core.
- Khi "xóa", hệ thống chỉ chuyển `IsDeleted = true`. Dữ liệu sẽ biến mất khỏi các truy vấn thông thường nhưng vẫn tồn tại trong database để phục vụ Audit hoặc khôi phục từ **Thùng rác (Trash Bin)**.

### Hệ thống Nhật ký Thay đổi (Audit Logs)
- Tự động ghi lại log cho mọi hành động `INSERT`, `UPDATE`, `DELETE`.
- Lưu trữ giá trị cũ (`OldValues`) và giá trị mới (`NewValues`) dưới dạng JSON, giúp quản trị viên truy vết 100% lịch sử biến động của từng bản ghi.

---

## Quy Tắc Nghiệp Vụ (Business Rules)

### Quy tắc Đặt mã Tự động (Auto-Code Generation)
Hệ thống tự động sinh mã theo quy chuẩn chuyên nghiệp:
- **Sản phẩm (SKU):** Định dạng `SP-XXXXX` (Ví dụ: `SP-00001`).
- **Phiếu Nhập kho:** Định dạng `PN-YYYYMMDD-STT` (Ví dụ: `PN-20240409-001`).
- **Phiếu Xuất kho:** Định dạng `PX-YYYYMMDD-STT` (Ví dụ: `PX-20240409-001`).
- **Phiếu Chuyển kho:** Định dạng `DC-YYYYMMDD-STT`.
- **Phiếu Kiểm kê:** Định dạng `KK-YYYYMMDD-STT`.

### Ngưỡng Tồn kho & Cảnh báo
- Hệ thống gửi thông báo thời gian thực qua **SignalR** khi số lượng hàng trong kho chạm ngưỡng `MinStockLevel` đã thiết lập.
- Job ngầm (**Hangfire**) quét định kỳ hàng đêm để cảnh báo các lô hàng sắp hết hạn sử dụng.



## Cấu trúc Database (Database Schema)

Toàn bộ các bảng đều kế thừa từ `BaseEntity` với các trường audit tự động:

> **BaseEntity**: `Id (PK)`, `CreatedAt`, `UpdatedAt`, `IsDeleted` *(Soft Delete)*

### Nhóm Quản lý Người dùng

```
┌──────────────────────────────────────┐
│              Roles                   │
│  PK  Id          int                 │
│      Name        nvarchar(50)        │  Admin | WarehouseManager | Employee
└──────────────────────┬───────────────┘
                       │ 1:N
┌──────────────────────▼───────────────┐
│            SystemUsers               │
│  PK  Id                int           │
│      Username          nvarchar(50)  │
│      PasswordHash      nvarchar(200) │  BCrypt
│      FullName          nvarchar(100) │
│      Email             nvarchar(200) │
│  FK  RoleId            int           │ → Roles.Id
│      IsActive          bit           │
│      RefreshToken      nvarchar(max) │
│      RefreshTokenExpiry datetime2    │
│      ResetPasswordCode nvarchar(6)   │
│      ResetPasswordExpiry datetime2   │
└──────────────────────────────────────┘
```

### Nhóm Danh mục Hàng hóa

```
┌──────────────────────────────────────┐   ┌───────────────────────────────────┐
│            Categories                │   │            Suppliers              │
│  PK  Id          int                 │   │  PK  Id          int              │
│      Name        nvarchar(100)       │   │      Name        nvarchar(200)    │
│      Description nvarchar(500)       │   │      Phone       nvarchar(20)     │
└──────────────────┬───────────────────┘   │      Email       nvarchar(200)    │
                   │ 1:N                   │      Address     nvarchar(500)    │
┌──────────────────▼───────────────────┐   └───────────────────────────────────┘
│              Products                │
│  PK  Id            int               │   ┌───────────────────────────────────┐
│      SkuCode       nvarchar(50)  UNIQ│   │            Customers              │
│      Name          nvarchar(200)     │   │  PK  Id          int              │
│  FK  CategoryId    int               │   │      Name        nvarchar(200)    │
│      Unit          nvarchar(50)      │   │      Phone       nvarchar(20)     │
│      MinStockLevel int               │   │      Email       nvarchar(200)    │
│      IsLotManaged  bit               │   │      Address     nvarchar(500)    │
│      CostPrice     decimal(18,2)     │   └───────────────────────────────────┘
│      SellingPrice  decimal(18,2)     │
│      Description   nvarchar(1000)    │
│      ImageUrl      nvarchar(500)     │
└──────────────────────────────────────┘
```

### Nhóm Kho hàng & Tồn kho

```
┌──────────────────────────────────────┐
│             Warehouses               │
│  PK  Id            int               │
│      Name          nvarchar(100)     │
│      Location      nvarchar(200)     │
│      ManagerId     int               │
│      PhoneNumber   nvarchar(20)      │
└──────────┬───────────────────────────┘
           │ 1:N
┌──────────▼───────────────────────────┐
│              Inventories             │  (Tồn kho thực tế theo lô)
│  PK  Id                int           │
│  FK  ProductId         int           │ → Products.Id
│  FK  WarehouseId       int           │ → Warehouses.Id
│      LotNumber         nvarchar(50)  │
│      ExpiryDate        datetime2     │
│      QuantityOnHand    decimal(18,2) │
│      ReservedQuantity  decimal(18,2) │
│      LocationInWarehouse nvarchar(50)│
│  [Computed] AvailableQuantity        │  = QuantityOnHand - ReservedQuantity
└──────────────────────────────────────┘

┌──────────────────────────────────────┐
│             StockCards               │  (Thẻ kho - Lịch sử biến động)
│  PK  Id                int           │
│  FK  ProductId         int           │ → Products.Id
│  FK  WarehouseId       int           │ → Warehouses.Id
│      TransactionType   nvarchar(20)  │  IN | OUT
│      ReferenceNumber   nvarchar(50)  │  Mã phiếu tham chiếu
│      LotNumber         nvarchar(50)  │
│      Quantity          decimal(18,2) │
│      UnitCost          decimal(18,2) │
│      BalanceAfter      decimal(18,2) │  Tồn sau giao dịch
│      TransactionDate   datetime2     │
└──────────────────────────────────────┘
```

### Nhóm Phiếu Nhập kho (Inbound)

```
┌──────────────────────────────────────┐
│          ReceivingVouchers           │
│  PK  Id            int               │
│      Code          nvarchar(50)  UNIQ│  PN-20240319-001
│  FK  WarehouseId   int               │ → Warehouses.Id
│  FK  SupplierId    int (nullable)    │ → Suppliers.Id
│      Status        nvarchar(20)      │  Draft | Completed
│      ReceivingDate datetime2         │
│      Notes         nvarchar(500)     │
│      RowVersion    rowversion        │  Concurrency Control
└──────────────────┬───────────────────┘
                   │ 1:N
┌──────────────────▼───────────────────┐
│        ReceivingVoucherDetails       │
│  PK  Id                int           │
│  FK  ReceivingVoucherId int           │ → ReceivingVouchers.Id
│  FK  ProductId         int           │ → Products.Id
│      LotNumber         nvarchar(50)  │
│      ExpiryDate        datetime2     │
│      Quantity          decimal(18,2) │
│      UnitPrice         decimal(18,2) │
└──────────────────────────────────────┘
```

### Nhóm Phiếu Xuất kho (Outbound)

```
┌──────────────────────────────────────┐
│          DeliveryVouchers            │
│  PK  Id            int               │
│      Code          nvarchar(50)  UNIQ│
│  FK  WarehouseId   int               │ → Warehouses.Id
│  FK  CustomerId    int (nullable)    │ → Customers.Id
│      Status        nvarchar(20)      │  Draft | Dispatched
│      DeliveryDate  datetime2         │
│      Notes         nvarchar(500)     │
└──────────────────┬───────────────────┘
                   │ 1:N
┌──────────────────▼───────────────────┐
│        DeliveryVoucherDetails        │
│  PK  Id                int           │
│  FK  DeliveryVoucherId int           │ → DeliveryVouchers.Id
│  FK  ProductId         int           │ → Products.Id
│      LotNumber         nvarchar(50)  │
│      Quantity          decimal(18,2) │
│      SellingPrice      decimal(18,2) │
│      CostPrice         decimal(18,2) │  Ghi nhận giá vốn lúc xuất
└──────────────────────────────────────┘
```

### Nhóm Chuyển kho (Transfer)

```
┌──────────────────────────────────────┐
│          TransferVouchers            │
│  PK  Id              int             │
│      Code            nvarchar(50)    │
│  FK  FromWarehouseId int             │ → Warehouses.Id
│  FK  ToWarehouseId   int             │ → Warehouses.Id
│      Status          nvarchar(20)    │  Draft | Completed
│      TransferDate    datetime2       │
│      Notes           nvarchar(500)   │
└──────────────────┬───────────────────┘
                   │ 1:N
┌──────────────────▼───────────────────┐
│        TransferVoucherDetails        │
│  PK  Id               int            │
│  FK  TransferVoucherId int           │ → TransferVouchers.Id
│  FK  ProductId         int           │ → Products.Id
│      LotNumber         nvarchar(50)  │
│      Quantity          decimal(18,2) │
└──────────────────────────────────────┘
```

### Nhóm Kiểm kê kho (Inventory Counting)

```
┌──────────────────────────────────────┐
│           CountingSheets             │
│  PK  Id            int               │
│      Code          nvarchar(50)      │
│  FK  WarehouseId   int               │ → Warehouses.Id
│      Status        nvarchar(20)      │  Draft | Approved
│      CountingDate  datetime2         │
│      Notes         nvarchar(500)     │
└──────────────────┬───────────────────┘
                   │ 1:N
┌──────────────────▼───────────────────┐
│         CountingSheetDetails         │
│  PK  Id               int            │
│  FK  CountingSheetId  int            │ → CountingSheets.Id
│  FK  ProductId        int            │ → Products.Id
│      LotNumber        nvarchar(50)   │
│      SystemQuantity   decimal(18,2)  │  Tồn theo hệ thống
│      ActualQuantity   decimal(18,2)  │  Tồn kiểm thực tế
│      Note             nvarchar(500)  │
└──────────────────────────────────────┘

┌──────────────────────────────────────┐
│        InventoryAdjustments          │  (Lịch sử cân đối sau kiểm kê)
│  PK  Id               int            │
│      AdjustmentType   nvarchar(50)   │  Counting | Damage | Return | Manual
│      ReferenceId      int            │  ID phiếu tham chiếu
│      ReferenceNumber  nvarchar(50)   │
│  FK  ProductId        int            │ → Products.Id
│  FK  WarehouseId      int            │ → Warehouses.Id
│      LotNumber        nvarchar(50)   │
│      OldQuantity      decimal(18,2)  │
│      NewQuantity      decimal(18,2)  │
│      Reason           nvarchar(500)  │
│      ApprovedBy       int            │
│      ApprovedAt       datetime2      │
│      Status           nvarchar(20)   │  Pending | Approved | Rejected
└──────────────────────────────────────┘
```

### Nhóm Hệ thống & Audit

```
┌──────────────────────────────────────┐
│             AuditLogs                │  (Ghi lại mọi thay đổi dữ liệu)
│  PK  Id            int               │
│      TableName     nvarchar(100)     │  Tên bảng bị thay đổi
│      RecordId      nvarchar(50)      │  ID bản ghi bị tác động
│      Action        nvarchar(20)      │  INSERT | UPDATE | DELETE
│      OldValues     nvarchar(max)     │  JSON giá trị cũ
│      NewValues     nvarchar(max)     │  JSON giá trị mới
│      ChangedBy     nvarchar(100)     │  Username thực hiện
│      ChangedAt     datetime2         │
└──────────────────────────────────────┘
```

### Sơ đồ quan hệ tổng quát (ERD Overview)

```
Categories ──< Products >── Inventories >── Warehouses
                  │                              │
                  ├── ReceivingVoucherDetails ──< ReceivingVouchers >── Suppliers
                  ├── DeliveryVoucherDetails  ──< DeliveryVouchers  >── Customers
                  ├── TransferVoucherDetails  ──< TransferVouchers  (From/To Warehouses)
                  ├── CountingSheetDetails    ──< CountingSheets    >── Warehouses
                  └── InventoryAdjustments
                  
StockCards  (ProductId + WarehouseId – lịch sử biến động theo dòng thời gian)
AuditLogs   (Ghi log toàn bộ thay đổi DB của mọi bảng trên)
SystemUsers >── Roles
```

---

## Tính năng & Công nghệ (Features & Technologies)

### Tính năng Nổi bật
- **Nhập / Xuất / Chuyển kho:** Vòng đời phiếu từ Draft → Completed/Dispatched trong database transaction.
- **Quản lý tồn kho theo lô (Lot):** Theo dõi chi tiết từng lô hàng, ngày nhập và hạn sử dụng.
- **Giá vốn bình quân gia quyền (Weighted Average Cost):** Tự động tính toán và cập nhật giá vốn khi nhập hàng.
- **Thẻ kho (Stock Card):** Truy vết 100% lịch sử xuất nhập biến động theo dòng thời gian.
- **Kiểm kê kho:** Xác định chênh lệch → Tạo điều chỉnh → Cập nhật tồn kho tự động.
- **Hệ thống Audit Log:** Ghi lại mọi hành động thay đổi dữ liệu (Ai, Bảng nào, Giá trị cũ/mới).
- **Phân quyền RBAC:** Admin (Toàn quyền), WarehouseManager (Quản lý), Employee (Thực thi).
- **Quản lý người dùng:** Admin CRUD tài khoản nhân viên, gán Role, reset mật khẩu.
- **Thông báo thời gian thực:** Cảnh báo tồn kho thấp và thông báo qua **SignalR**.
- **Bảo mật nâng cao:** Refresh Token, Quên mật khẩu qua OTP 6 chữ số gửi Email.
- **Tự động đặt mã (Auto-Code):** Hệ thống thông minh tự động sinh mã SKU cho Sản phẩm (`SP-XXXXX`) và mã chứng từ (`PN`, `PX`, `DC`, `KK`) theo định dạng `MÃ-YYYYMMDD-STT`.
- **Quản lý Thùng rác (Trash Bin):** Lưu trữ các mục đã xóa tạm (Products, Suppliers, Customers, Warehouses, Categories). Hỗ trợ khôi phục hoặc xóa vĩnh viễn (Chỉ dành cho Admin).
- **Lưu trữ Ảnh Cloud đa kênh:** Tích hợp **Cloudinary API** để lưu trữ và tối ưu hóa hình ảnh sản phẩm, giúp giảm tải cho server và tăng tốc độ hiển thị.
- **Xuất/Nhập Excel:** Import sản phẩm hàng loạt, xuất báo cáo tồn kho (ClosedXML).

### Công nghệ lõi

| Thành phần | Công nghệ | Chi tiết |
|---|---|---|
| Framework | .NET 8.0, ASP.NET Core 8 | C# 12, N-Tier Architecture |
| ORM | Entity Framework Core 8 | LINQ, Generic Repository, Unit of Work |
| Database | SQL Server | SQL Server 2022+, Azure SQL compatible |
| Security | JWT Bearer, Refresh Token | BCrypt, Role-Based Access Control (RBAC) |
| Real-time | SignalR (WebSockets) | Thông báo tức thời (Low stock, alerts) |
| Background Jobs | Hangfire | Quản lý tác vụ định kỳ 23:59 hàng đêm |
| Mapping | AutoMapper | Ánh xạ Domain Entity ↔ DTO tự động |
| Validation | FluentValidation | Validate dữ liệu đầu vào chuẩn REST |
| Testing | xUnit, Moq | 11/11 Test cases passed  |
| Excel | ClosedXML | Xuất báo cáo, Import sản phẩm hàng loạt |
| Media Store | Cloudinary | Lưu trữ ảnh sản phẩm trên Cloud |
| Docs | Swagger UI (OpenAPI v3) | Tài liệu API tương tác trực quan |

---

## API Endpoints Tổng quan

| Module | Endpoint | Mô tả |
|---|---|---|
| **Auth** | `POST /api/Auth/login` | Đăng nhập hệ thống |
| **Auth** | `POST /api/Auth/refresh-token` | Làm mới phiên đăng nhập |
| **Auth** | `GET /api/Auth/profile` | Lấy thông tin tài khoản hiện tại |
| **Auth** | `POST /api/Auth/forgot-password` | Gửi yêu cầu quên mật khẩu |
| **Auth** | `POST /api/Auth/reset-password` | Đặt lại mật khẩu qua OTP |
| **Users** *(Admin)* | `GET/POST/PUT/DELETE /api/Users` | Quản trị tài khoản & phân quyền |
| **Master Data** | `GET/POST/PUT/DELETE /api/Categories` | Quản lý loại hàng hóa |
| **Master Data** | `GET/POST/PUT/DELETE /api/Suppliers` | Quản lý nhà cung cấp |
| **Master Data** | `GET/POST/PUT/DELETE /api/Customers` | Quản lý khách hàng |
| **Products** | `GET/POST/PUT/DELETE /api/Products` | Quản lý sản phẩm & giá vốn |
| **Warehouses** | `GET/POST/PUT/DELETE /api/Warehouses` | Quản lý hệ thống kho bãi |
| **Inbound** | `GET /api/Inbound` | Tra cứu phiếu nhập kho |
| **Inbound** | `POST /api/Inbound/draft` | Tạo phiếu nhập nháp |
| **Inbound** | `POST /api/Inbound/approve/{id}` | Duyệt phiếu & Tăng tồn |
| **Outbound** | `GET /api/Outbound` | Tra cứu phiếu xuất kho |
| **Outbound** | `POST /api/Outbound/draft` | Tạo phiếu xuất nháp |
| **Outbound** | `POST /api/Outbound/approve/{id}` | Duyệt phiếu & Giảm tồn |
| **Transfer** | `GET /api/Transfer` | Tra cứu phiếu chuyển kho |
| **Transfer** | `POST /api/Transfer/create` | Lập lệnh điều chuyển |
| **Transfer** | `POST /api/Transfer/approve/{id}` | Xác nhận điều chuyển hoàn tất |
| **Counting** | `GET /api/Counting` | Danh sách phiếu kiểm kê |
| **Counting** | `POST /api/Counting/draft` | Lập lịch kiểm kê |
| **Counting** | `POST /api/Counting/approve/{id}` | Duyệt chênh lệch & Cân đối kho |
| **Inventory** | `GET /api/Inventory` | Xem báo cáo tồn kho hiện thời |
| **Inventory** | `GET /api/Inventory/stock-cards` | Xem lịch sử thẻ kho chi tiết |
| **Dashboard** | `GET /api/Dashboard/summary` | Thống kê số liệu tổng hợp |
| **Dashboard** | `GET /api/Dashboard/low-stock` | Cảnh báo hàng dưới ngưỡng tồn |
| **Trash** | `GET /api/Trash` | Xem danh mục đã xóa tạm |
| **Trash** | `POST /api/Trash/restore/{type}/{id}` | Khôi phục dữ liệu đã xóa |
| **Trash** | `DELETE /api/Trash/hard-delete/{type}/{id}` | Xóa vĩnh viễn (Chỉ Admin) |
| **Audit** | `GET /api/Audit` | Nhật ký truy vết thay đổi dữ liệu |
| **Excel** | `GET /api/Excel/export-inventory` | Xuất báo cáo tồn kho ra Excel |
| **Excel** | `POST /api/Excel/import-products` | Nhập sản phẩm hàng loạt từ Excel |

---

## Unit Testing

Dự án sử dụng **xUnit** + **Moq** để kiểm thử toàn bộ logic nghiệp vụ.

```bash
cd QLKHO_PhanVanHoang.Tests
dotnet test
```

| Test Class | Test Cases | Mô tả |
|---|---|---|
| `InventoryServiceTests` | 3 | Tăng tồn, giảm tồn, cảnh báo tồn thấp |
| `InboundServiceTests` | 2 | Duyệt phiếu nhập, phiếu đã hoàn thành |
| `OutboundServiceTests` | 2 | Duyệt phiếu xuất, phiếu đã xuất |
| `TransferServiceTests` | 2 | Chuyển kho hợp lệ, kho giống nhau |
| `CountingServiceTests` | 2 | Tăng tồn sau kiểm kê, giảm tồn sau kiểm kê |
| **Tổng cộng** | **11** | **11/11 PASSED** |

---

## 🚀 Cài đặt & Vận hành (Deployment & Setup)

1.  **Môi trường:** Cài đặt .NET 8 SDK và SQL Server.
2.  **Cấu hình:** Sao chép `appsettings.Example.json` → `appsettings.json`, cập nhật:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=.;Database=QLKHO_DB;Trusted_Connection=True;"
      },
      "CloudinarySettings": {
        "CloudName": "YOUR_CLOUD_NAME",
        "ApiKey": "YOUR_API_KEY",
        "ApiSecret": "YOUR_API_SECRET"
      },
      "Jwt": {
        "Key": "your-secret-key-min-32-chars",
        "Issuer": "QLKHO_API",
        "Audience": "QLKHO_Client",
        "DurationInMinutes": "120"
      },
      "EmailSettings": {
        "SmtpServer": "YOUR_SMTP_HOST",
        "Port": 587,
        "SenderEmail": "YOUR_SENDER_EMAIL",
        "SenderPassword": "YOUR_SMTP_PASSWORD"
      }
    }
    ```
3.  **Migration:** `dotnet ef database update`
4.  **Running:** `dotnet run` → Truy cập Swagger UI tại: [http://localhost:9000/](http://localhost:9000/)

---

---

## Giao diện Người dùng (Frontend)

Hệ thống đi kèm với giao diện Web hiện đại, tối ưu cho trải nghiệm người dùng doanh nghiệp:

- **Công nghệ Base:** React 18, TypeScript, Vite.
- **UI Library:** Ant Design (Enterprise UI kit).
- **State Management:** Redux Toolkit / React Context.
- **Tính năng chính:** Dashboard trực quan, Form nhập liệu đa dòng, In phiếu trực tiếp, quản lý kho kéo-thả.

> Thư mục nguồn: `frontend-wms/`

---

## Tác giả & Đóng góp


- **Chủ dự án:** Phan Van Hoang
- **GitHub:** [hoangphan04211](https://github.com/hoangphan04211)
- **Email:** phan21828@gmail.com

---

*Dự án được xây dựng với tâm thế mang lại giá trị thực tế nhất cho quy trình quản lý kho hiện đại.*
