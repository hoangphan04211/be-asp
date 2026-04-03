# 📦 QLKHO_PHANVANHOANG - Warehouse Management System API

[![version](https://img.shields.io/badge/version-1.1.0-blue)](https://github.com/hoangphan04211/be-asp)
[![license](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![dotnet](https://img.shields.io/badge/.NET-8.0-blueviolet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![tests](https://img.shields.io/badge/tests-11%2F11%20passed-brightgreen)](#unit-testing)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen)](https://github.com/hoangphan04211/be-asp/pulls)

**Hệ thống Quản lý Kho (WMS) API thông minh** là giải pháp Backend chuyên nghiệp được thiết kế theo mô hình kiến trúc phân lớp (N-Tier Architecture), hỗ trợ doanh nghiệp tối ưu hóa quy trình lưu trữ, theo dõi biến động hàng hóa và kiểm soát giá vốn theo thời gian thực.

---

## 🏗️ Kiến trúc Hệ thống (System Architecture)

Dự án được xây dựng dựa trên các tiêu chuẩn **Clean Code** và **Enterprise Design Patterns**, đảm bảo tính mở rộng (Scalability) và bảo trì (Maintainability) lâu dài.

### 📂 Cấu trúc thư mục Chi tiết (Folder Structure)

1.  **`Controllers/` (Tầng Giao Diện - Presentation Layer)**
    - Tiếp nhận HTTP Request, điều hướng đến Service và phản hồi theo chuẩn RESTful API.
    - Bao gồm: `AuthController`, `ProductsController`, `WarehousesController`, `InboundController`, `OutboundController`, `TransferController`, `CountingController`, `InventoryController`, `DashboardController`, `UsersController`, `AuditController`, `ExcelController`, `FileController`.

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

---

## 🗄️ Cấu trúc Database (Database Schema)

Toàn bộ các bảng đều kế thừa từ `BaseEntity` với các trường audit tự động:

> **BaseEntity**: `Id (PK)`, `CreatedAt`, `UpdatedAt`, `IsDeleted` *(Soft Delete)*

### 👥 Nhóm Quản lý Người dùng

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

### 📦 Nhóm Danh mục Hàng hóa

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

### 🏭 Nhóm Kho hàng & Tồn kho

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

### 📋 Nhóm Phiếu Nhập kho (Inbound)

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

### 📤 Nhóm Phiếu Xuất kho (Outbound)

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

### 🔄 Nhóm Chuyển kho (Transfer)

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

### 🔍 Nhóm Kiểm kê kho (Inventory Counting)

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

### 📝 Nhóm Hệ thống & Audit

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

### 📊 Sơ đồ quan hệ tổng quát (ERD Overview)

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

## 🛠️ Tính năng & Công nghệ (Features & Technologies)

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
- **Xuất/Nhập Excel:** Import sản phẩm hàng loạt, xuất báo cáo tồn kho (ClosedXML).

### Công nghệ lõi

| Thành phần | Công nghệ |
|---|---|
| Framework | .NET 8.0, ASP.NET Core 8 |
| ORM | Entity Framework Core 8 |
| Database | SQL Server |
| Security | JWT Bearer, Refresh Token, BCrypt |
| Real-time | SignalR (WebSockets) |
| Background Jobs | Hangfire |
| Mapping | AutoMapper |
| Validation | FluentValidation |
| Testing | xUnit, Moq |
| Excel | ClosedXML |
| Docs | Swagger UI (OpenAPI v3) |

---

## 🔌 API Endpoints Tổng quan

| Module | Endpoint | Mô tả |
|---|---|---|
| Auth | `POST /api/Auth/login` | Đăng nhập |
| Auth | `POST /api/Auth/refresh-token` | Làm mới Token |
| Auth | `GET /api/Auth/profile` | Lấy thông tin cá nhân |
| Auth | `POST /api/Auth/forgot-password` | Quên mật khẩu |
| Auth | `POST /api/Auth/reset-password` | Đặt lại mật khẩu |
| Users *(Admin)* | `GET/POST/PUT/DELETE /api/Users` | Quản lý nhân viên |
| Products | `GET/POST/PUT/DELETE /api/Products` | Quản lý sản phẩm |
| Warehouses | `GET/POST/PUT/DELETE /api/Warehouses` | Quản lý kho hàng |
| Inbound | `GET /api/Inbound` | Danh sách phiếu nhập |
| Inbound | `POST /api/Inbound/draft` | Tạo phiếu nháp |
| Inbound | `POST /api/Inbound/approve/{id}` | Duyệt phiếu nhập |
| Outbound | `GET /api/Outbound` | Danh sách phiếu xuất |
| Outbound | `POST /api/Outbound/draft` | Tạo phiếu nháp |
| Outbound | `POST /api/Outbound/approve/{id}` | Duyệt phiếu xuất |
| Transfer | `GET /api/Transfer` | Danh sách phiếu chuyển |
| Transfer | `POST /api/Transfer/create` | Tạo phiếu chuyển kho |
| Transfer | `POST /api/Transfer/approve/{id}` | Duyệt phiếu chuyển kho |
| Counting | `GET /api/Counting` | Danh sách phiếu kiểm kê |
| Counting | `POST /api/Counting/draft` | Tạo phiếu kiểm nháp |
| Counting | `POST /api/Counting/approve/{id}` | Duyệt & cân đối kho |
| Inventory | `GET /api/Inventory` | Xem tồn kho hiện tại |
| Inventory | `GET /api/Inventory/stock-cards` | Xem thẻ kho |
| Dashboard | `GET /api/Dashboard/summary` | Thống kê tổng quan |
| Dashboard | `GET /api/Dashboard/low-stock` | Danh sách hàng sắp hết |
| Audit | `GET /api/Audit` | Nhật ký thay đổi dữ liệu |
| Excel | `GET /api/Excel/export-inventory` | Xuất báo cáo tồn kho |
| Excel | `POST /api/Excel/import-products` | Import sản phẩm từ Excel |

---

## 🧪 Unit Testing

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
| **Tổng cộng** | **11** | ✅ **11/11 PASSED** |

---

## 🚀 Cài đặt & Vận hành (Deployment & Setup)

1.  **Môi trường:** Cài đặt .NET 8 SDK và SQL Server.
2.  **Cấu hình:** Sao chép `appsettings.Example.json` → `appsettings.json`, cập nhật:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=.;Database=QLKHO_DB;Trusted_Connection=True;"
      },
      "Jwt": {
        "Key": "your-secret-key-min-32-chars",
        "Issuer": "QLKHO_API",
        "Audience": "QLKHO_Client",
        "DurationInMinutes": "120"
      },
      "EmailSettings": {
        "SmtpServer": "smtp-relay.brevo.com",
        "Port": 587,
        "SenderEmail": "your-verified-email@domain.com",
        "SenderPassword": "your-brevo-api-key"
      }
    }
    ```
3.  **Migration:** `dotnet ef database update`
4.  **Running:** `dotnet run` → Truy cập Swagger UI tại: [http://localhost:9000/](http://localhost:9000/)

---

## ✍️ Tác giả & Đóng góp

- **Chủ dự án:** Phan Van Hoang
- **GitHub:** [hoangphan04211](https://github.com/hoangphan04211)
- **Email:** phan21828@gmail.com

---

*Dự án được xây dựng với tâm thế mang lại giá trị thực tế nhất cho quy trình quản lý kho hiện đại.*
