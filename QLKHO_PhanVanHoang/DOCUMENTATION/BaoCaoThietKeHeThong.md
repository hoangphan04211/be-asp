# ?? BÁO CÁO ??C T? NGHI?P V? & THI?T K? H? TH?NG

## QLKHO - Warehouse Management System (WMS)

**Phiên b?n:** 1.0  
**Ngày so?n:** Tháng 04/2024  
**Tác gi?:** Phan V?n Hoàng  
**Tr?ng thái:** Chính th?c  
**Build:** .NET 8/9, SQL Server, Hangfire

---

## ?? M?C L?C

1. [T?ng quan tài li?u](#1-t?ng-quan-tài-li?u)
2. [Mô t? t?ng quan h? th?ng](#2-mô-t?-t?ng-quan-h?-th?ng)
3. [??c t? nghi?p v? chi ti?t](#3-??c-t?-nghi?p-v?-chi-ti?t)
4. [Ki?n trúc h? th?ng](#4-ki?n-trúc-h?-th?ng)
5. [Thi?t k? c? s? d? li?u](#5-thi?t-k?-c?-s?-d?-li?u)
6. [Thi?t k? API](#6-thi?t-k?-api)
7. [Thi?t k? b?o m?t](#7-thi?t-k?-b?o-m?t)
8. [Thi?t k? x? lý l?i & logging](#8-thi?t-k?-x?-lý-l?i--logging)
9. [Thi?t k? tri?n khai](#9-thi?t-k?-tri?n-khai)
10. [K? ho?ch ki?m th?](#10-k?-ho?ch-ki?m-th?)
11. [Ph? l?c](#11-ph?-l?c)

---

## 1. T?NG QUAN TÀI LI?U

### 1.1 M?c ?ích tài li?u

Tài li?u này là **b?n ??c t? k? thu?t toàn di?n** cho h? th?ng **QLKHO (Warehouse Management System)**, cung c?p:

- ? ??nh ngh?a rõ ràng v? yêu c?u nghi?p v? (Business Requirements)
- ? Thi?t k? ki?n trúc h? th?ng chi ti?t (System Architecture)
- ? Mô t? c? s? d? li?u ??y ?? (Database Design)
- ? ??c t? API RESTful (API Specification)
- ? H??ng d?n tri?n khai & v?n hành (Deployment & Operations)
- ? K? ho?ch ki?m th? toàn di?n (Test Strategy)

### 1.2 Ph?m vi h? th?ng

**?? Ph?m vi bao g?m:**

| L?nh v?c | Chi ti?t |
|---------|---------|
| **Master Data** | S?n ph?m (Product), Danh m?c (Category), Kho (Warehouse), Nhà cung c?p (Supplier), Khách hàng (Customer) |
| **Inventory** | T?n kho theo lô, theo h?n s? d?ng, theo v? trí trong kho |
| **Inbound** | Phi?u nh?p hàng (ReceivingVoucher), c?p nh?t t?n kho |
| **Outbound** | Phi?u xu?t hàng (DeliveryVoucher), ki?m tra t?n kho kh? d?ng |
| **Transfer** | Chuy?n kho (TransferVoucher) v?i h? tr? split lô |
| **Counting** | Ki?m kê kho (CountingSheet), reconciliation, t?o ?i?u ch?nh (InventoryAdjustment) |
| **Stock Card** | L?ch s? bi?n ??ng t?n kho (StockCard), truy v?t giao d?ch |
| **Alerts** | C?nh báo t?n kho th?p, h?n s? d?ng s?p h?t (Email + Notification) |
| **Audit** | L?u audit log v?i OldValues/NewValues, soft delete |
| **Users & Auth** | JWT authentication, RBAC (Admin, WarehouseManager, WarehouseStaff, Auditor) |

**? Ph?m vi KHÔNG bao g?m:**

| Ph?m vi | Lý do |
|--------|-------|
| Ecommerce/Customer Portal | Phase 2 |
| Advanced Forecasting | Require ML |
| Multi-tenant | Phase 2 |
| Mobile App | Separate project |
| EDI/3PL Integration | Future integration |
| Barcode/RFID Scanning | Hardware dependent |

### 1.3 ??i t??ng s? d?ng

| ?? Ng??i dùng | Vai trò | Nhi?m v? chính | Quy?n h?n |
|---|---|---|---|
| **Admin** | Qu?n tr? h? th?ng | C?u hình, qu?n lý user, c?p quy?n, duy?t all phi?u | Toàn quy?n (Full Access) |
| **WarehouseManager** | Qu?n lý kho | Duy?t phi?u nh?p/xu?t/chuy?n, l?p báo cáo, thi?t l?p ng??ng | Create, Update, Approve |
| **WarehouseStaff** | Nhân viên kho | T?o phi?u nh?p/xu?t, th?c hi?n ki?m kê, ghi nh?n | Create, Edit Draft |
| **Auditor** | Ki?m toán | Xem audit logs, báo cáo truy v?t, không ch?nh s?a | Read-only |

### 1.4 Tài li?u tham kh?o

- ?? [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- ?? [REST API Design Guidelines](https://restfulapi.net/)
- ?? [OWASP Security Cheat Sheet](https://cheatsheetseries.owasp.org/)
- ?? [Hangfire Recurring Jobs](https://docs.hangfire.io/en/latest/background-methods/recurring-jobs.html)
- ?? [SQL Server Best Practices](https://docs.microsoft.com/en-us/sql/relational-databases/sql-server-transaction-locking-and-row-versioning-guide)

---

## 2. MÔ T? T?NG QUAN H? TH?NG

### 2.1 Gi?i thi?u h? th?ng

**QLKHO** (Qu?n Lý KHO) là h? th?ng qu?n lý kho hàng thông minh (**Warehouse Management System - WMS**), ???c xây d?ng trên n?n t?ng **.NET 8** v?i **Entity Framework Core**, **SQL Server**, và các công ngh? hi?n ??i nh? **Hangfire** (background jobs), **SignalR** (real-time notifications), **JWT** (authentication), **Serilog** (logging).

H? th?ng cung c?p m?t gi?i pháp toàn di?n cho:
- ?? **Qu?n lý t?n kho chi ti?t theo lô, h?n s? d?ng, v? trí**
- ?? **Qu?n lý lu?ng nh?p/xu?t/chuy?n kho v?i ki?m soát t?n kho**
- ? **Ki?m kê t? ??ng, reconciliation và ?i?u ch?nh t?n kho**
- ?? **C?nh báo k?p th?i** cho t?n kho th?p và h?n dùng s?p h?t
- ?? **Báo cáo chi ti?t** v?i l?ch s? giao d?ch (Stock Card)
- ?? **Audit trail** ??y ?? (who/when/what) v?i OldValues/NewValues

### 2.2 M?c tiêu h? th?ng

| # | M?c tiêu | Mô t? |
|----|---------|-------|
| ?? **1** | T?i ?u hóa t?n kho | Qu?n lý chính xác t?ng lô hàng, h?n s? d?ng, v? trí kho ? gi?m hàng l?i, t?i thi?u hóa lãng phí |
| ?? **2** | Gi?m sai sót giao d?ch | Ki?m kê t? ??ng, reconciliation, audit log chi ti?t ? t?ng ?? tin c?y d? li?u |
| ?? **3** | C?nh báo k?p th?i | H? th?ng t? ??ng phát hi?n t?n kho th?p, h?n dùng s?p h?t ? nhân viên nh?n ???c alert ngay l?p t?c |
| ?? **4** | Tuân th? pháp lý | L?u tr? audit log (who/when/what/oldValue/newValue), soft delete ? h? tr? ki?m toán, tuân th? quy ??nh |
| ?? **5** | D? m? r?ng & b?o trì | Repository pattern, UnitOfWork, Dependency Injection ? code s?ch, d? test, d? m? r?ng |
| ?? **6** | Hi?u su?t cao | Pagination, indexing DB, transaction management, caching ? x? lý hàng nghìn giao d?ch/ngày |

### 2.3 S? ?? ng? c?nh (Context Diagram)

```
???????????????????????????????????????????????????????????????????
?             ?
?  ???????????????        ????????????????????           ?
?  ? ?? Users    ?        ? ?? Web Browser   ?         ?
?  ? (Roles)     ?????????? (Frontend) ? ?
?  ???????????????        ????????????????????   ?
?         ?       ?
?                API REST ?          ?
?                  ?          ?
?       ???????????????????????????    ?
?            ?  ?? QLKHO System        ?           ?
? ?  (WMS Backend)          ?        ?
?             ???????????????????????????   ?
?  ?             ?
?       ???????????????????????????????????     ?
?              ?  ?         ?      ?
?   ??????????????   ??????????????   ??????????????     ?
?         ? ??? SQL     ?   ? ?? Email   ?   ? ?? SignalR?       ?
?       ? Server     ?   ? Service    ?   ? Hub     ? ?
?         ??????????????   ??????????????   ??????????????       ?
?    ?
?       ???????????????????????????????????                ?
?    ?     ?    ?     ?
?         ??????????????   ??????????????   ??????????????       ?
?         ? ?? Hangfire?   ? ?? Serilog ?   ? ?? Audit   ?       ?
?   ? Jobs       ?   ? Logging  ?   ? Logs       ?       ?
?  ??????????????   ??????????????   ??????????????       ?
?       ?
???????????????????????????????????????????????????????????????????
```

### 2.4 Các module chính

| # | Module | Mô t? | Ch?c n?ng chính | Endpoint | Controllers |
|----|--------|-------|-----------------|----------|-------------|
| 1 | **Master Data** | Qu?n lý d? li?u g?c | CRUD Product, Category, Warehouse, Supplier, Customer | `/api/products`, `/api/warehouses` | `MasterDataControllers.cs`, `WarehousesController.cs` |
| 2 | **Inbound** | Phi?u nh?p kho | T?o/duy?t/hoàn thành phi?u nh?p, c?p nh?t t?n kho | `/api/inbound` | `InboundController.cs` |
| 3 | **Outbound** | Phi?u xu?t kho | T?o/duy?t/hoàn thành phi?u xu?t, ki?m tra t?n | `/api/outbound` | `OutboundController.cs` |
| 4 | **Transfer** | Chuy?n kho | T?o phi?u chuy?n, phê duy?t, c?p nh?t 2 kho | `/api/transfer` | `TransferController.cs` |
| 5 | **Inventory** | Qu?n lý t?n kho | Query t?n kho, filter theo product/warehouse, import Excel | `/api/inventory` | `InventoryController.cs` |
| 6 | **Counting** | Ki?m kê | T?o phi?u ki?m kê, nh?p s? l??ng th?c t?, tính chênh l?ch | `/api/counting` | `CountingController.cs` |
| 7 | **Stock Card** | L?ch s? giao d?ch | Query th? kho, filter theo ngày/s?n ph?m/kho | `/api/inventory/stock-cards` | `InventoryController.cs` |
| 8 | **Audit** | Ki?m toán | Query audit logs, xem who/when/what | `/api/audit` | `AuditController.cs` |
| 9 | **Dashboard** | B?ng ?i?u khi?n | Th?ng kê t?ng quát, kênh báo cáo | `/api/dashboard` | `DashboardController.cs` |
| 10 | **Authentication** | ??ng nh?p/Phân quy?n | JWT login, user management | `/api/auth` | `AuthController.cs` |

### 2.5 Các bên liên quan & vai trò

```
Biz Owner (Warehouse Manager)
    ?
    ??? Define Requirements
    ?
Admin (System Manager)
    ?
    ??? Configure, Approve Policies
    ?
Warehouse Staff (Data Entry)
    ?
    ??? Create/Edit Vouchers
    ?
Auditor (Compliance)
    ?
    ??? Review Audit Logs, Generate Reports
```

---

## 3. ??C T? NGHI?P V? CHI TI?T

### 3.1 Module Qu?n lý S?n ph?m (Master Data)

#### 3.1.1 Use case specification

| Use Case | Mô t? | Pre-condition | Flow | Post-condition |
|----------|-------|--------------|------|----------------|
| **Create Product** | T?o s?n ph?m m?i | User là Admin/Manager | 1. Input SKU, Name, Category, Unit, MinStockLevel, IsLotManaged, CostPrice<br/>2. Validate (SKU unique, Name required)<br/>3. Save to DB<br/>4. Audit log "Create" | Product ???c t?o, ID ???c tr? v? |
| **Edit Product** | S?a thông tin s?n ph?m | Product t?n t?i | 1. Get Product by ID<br/>2. Update fields<br/>3. Check RowVersion (concurrency)<br/>4. Save<br/>5. Audit log "Update" | Product c?p nh?t, OldValues/NewValues recorded |
| **View Product** | Xem danh sách/chi ti?t s?n ph?m | Authenticated | 1. Query with paging<br/>2. Filter by Category<br/>3. Include related Inventory count | Return PagedResult with total count |
| **Delete Product** | Xóa s?n ph?m | Product not used in Inbound/Outbound | 1. Set IsDeleted = true<br/>2. Global query filter excludes it<br/>3. Audit log "Delete" | Product soft-deleted, không hi?n th? ? list |
| **Import Product** | Import danh sách t? Excel | File format valid | 1. Parse Excel<br/>2. Batch insert<br/>3. Return SuccessCount + Errors | Bulk create products |

#### 3.1.2 Business rules

| # | Quy t?c | Chi ti?t |
|----|---------|---------|
| BR1 | **SKU Unique** | Mã SKU ph?i duy nh?t trong h? th?ng, không cho phép trùng l?p |
| BR2 | **Category Required** | M?i s?n ph?m ph?i thu?c 1 danh m?c, không cho null |
| BR3 | **MinStockLevel >= 0** | Ng??ng t?n t?i thi?u ph?i >= 0 |
| BR4 | **Lot Management** | N?u `IsLotManaged=true` thì MUST cung c?p LotNumber khi nh?p/xu?t |
| BR5 | **Soft Delete** | Khi xóa product, ch? set `IsDeleted=true`, gi? l?i data cho audit |
| BR6 | **Price Validation** | CostPrice, SellingPrice ph?i >= 0 ho?c NULL |
| BR7 | **Cannot Delete if In-Use** | N?u product có inventory > 0 ho?c trong phi?u ch?a hoàn thành, không cho xóa |

#### 3.1.3 Validation rules

| Field | Constraint | Error Message |
|-------|-----------|----------------|
| `SkuCode` | MaxLength(50), Required, Unique | "SKU ph?i < 50 ký t?, không trùng" |
| `Name` | MaxLength(200), Required | "Tên s?n ph?m b?t bu?c, < 200 ký t?" |
| `CategoryId` | Required, FK exists | "Danh m?c không t?n t?i" |
| `Unit` | MaxLength(50), Required | "??n v? tính b?t bu?c" |
| `MinStockLevel` | >= 0 | "Ng??ng t?n >= 0" |
| `CostPrice` | Decimal(18,2), >= 0 | "Giá v?n >= 0" |
| `SellingPrice` | Decimal(18,2), >= 0 | "Giá bán >= 0" |

### 3.2 Module Qu?n lý Nh?p kho (Inbound)

#### 3.2.1 Chi ti?t lu?ng phê duy?t phi?u nh?p

**Ti?n ?i?u ki?n:**
- Phi?u nh?p ? tr?ng thái **Draft**
- Có ít nh?t 1 dòng chi ti?t s?n ph?m
- S?n ph?m ph?i t?n t?i
- Kho ph?i t?n t?i

**Lu?ng x? lý:**

```
1. Manager opens ReceivingVoucher (Status = "Draft")
   ?? Validate: Status == "Draft" ?
   ?? Validate: Details not empty ?
   ?? START TRANSACTION

2. For each detail in RV.Details:
   ?? Get Product (?? l?y CostPrice)
   ?? Find or CREATE Inventory (ProductId, WarehouseId, LotNumber)
   ?? Update Inventory:
   ?  ?? QuantityOnHand += detail.Quantity
   ?  ?? Update CostPrice (Weighted Average):
   ?  ?  newCostPrice = (oldCostPrice * oldQty + unitPrice * newQty) / (oldQty + newQty)
   ?  ?? Call InventoryService.IncreaseInventoryAsync()
 ?? Record StockCard:
   ?  ?? TransactionType = "Inbound"
   ?  ?? ReferenceCode = RV.Code
   ?  ?? BeforeQuantity = old qty
   ?  ?? ChangeQuantity = +detail.Quantity
   ?  ?? AfterQuantity = new qty
   ?  ?? TransactionDate = DateTime.UtcNow

3. Update RV.Status = "Completed"
   ?? RV.UpdatedAt = DateTime.UtcNow
   ?? RV.UpdatedBy = currentUser
 ?? AuditLog: "Update" with OldValue="Draft", NewValue="Completed"

4. COMMIT TRANSACTION ?

5. Send notification (SignalR + Email):
   ?? Title: "? Nh?p kho thành công"
   ?? Message: "Phi?u {RV.Code} ?ã ???c duy?t và c?p nh?t {count} s?n ph?m"
   ?? Recipients: All WarehouseManagers
```

#### 3.2.2 Business rules (Inbound)

| # | Quy t?c | Chi ti?t |
|----|---------|---------|
| IB1 | **Status Draft Only** | Ch? phi?u Draft m?i ???c duy?t, không ???c duy?t l?i |
| IB2 | **Quantity > 0** | M?i dòng chi ti?t ph?i có quantity > 0 |
| IB3 | **Product Must Exist** | ProductId ph?i t?n t?i trong b?ng Products |
| IB4 | **Lot Number Required** | N?u Product.IsLotManaged=true MUST cung c?p LotNumber |
| IB5 | **Create or Update Inventory** | N?u Inventory ch?a t?n t?i thì t?o, n?u t?n t?i thì c?ng |
| IB6 | **Update Cost Price** | Tính l?i CostPrice bình quân gia quy?n (Weighted Average Cost) |
| IB7 | **Record StockCard** | Ghi nh?n th? kho v?i type="Inbound", before/after qty |
| IB8 | **One Transaction** | T?t c? thao tác trong 1 DB transaction, n?u l?i ROLLBACK all |
| IB9 | **Audit Log** | L?u audit log v?i user th?c hi?n, th?i gian, old/new values |
| IB10 | **Notification** | G?i thông báo real-time cho all users |

#### 3.2.3 Validation rules (Inbound)

| Field | Constraint | Error |
|-------|-----------|-------|
| `RV.Code` | Max(50), Unique, Required | "Mã phi?u b?t bu?c, < 50 ký t?, không trùng" |
| `RV.WarehouseId` | FK exists, Required | "Kho không t?n t?i" |
| `RVDetail.ProductId` | FK exists, Required | "S?n ph?m không t?n t?i" |
| `RVDetail.Quantity` | Decimal > 0 | "S? l??ng ph?i > 0" |
| `RVDetail.UnitPrice` | Decimal >= 0 | "Giá nh?p >= 0" |
| `RVDetail.LotNumber` | Max(50) | "S? lô < 50 ký t?" |

### 3.3 Module Qu?n lý Xu?t kho (Outbound)

#### 3.3.1 Chi ti?t lu?ng phê duy?t phi?u xu?t

**Ti?n ?i?u ki?n:**
- Phi?u xu?t ? tr?ng thái **Draft**
- Có ít nh?t 1 dòng chi ti?t
- T?n kho kh? d?ng (`AvailableQuantity = QuantityOnHand - ReservedQuantity`) >= yêu c?u

**Lu?ng x? lý:**

```
1. Manager opens DeliveryVoucher (Status = "Draft")
   ?? Validate: Status == "Draft" ?
   ?? Validate: Details not empty ?
   ?? START TRANSACTION

2. For each detail in DV.Details:
   ?? Find Inventory (ProductId, WarehouseId, LotNumber)
   ?? Check: AvailableQuantity >= detail.Quantity
   ?  ?? IF insufficient ? ROLLBACK & throw Exception
   ?  ?? IF sufficient ?
   ?? Update Inventory:
   ?  ?? QuantityOnHand -= detail.Quantity
   ?  ?? Call InventoryService.DecreaseInventoryAsync()
   ?? Get CostPrice t? Product ? store vào detail.CostPrice
   ?? Record StockCard:
   ?  ?? TransactionType = "Outbound"
   ?  ?? ReferenceCode = DV.Code
   ?  ?? BeforeQuantity = old qty
   ?  ?? ChangeQuantity = -detail.Quantity
   ?  ?? AfterQuantity = new qty
   ?  ?? Calculate Profit = (SellingPrice - CostPrice) * Quantity

3. Check MinStockLevel Alert:
   ?? Get total Inventory of product (all lots, all warehouses)
   ?? IF total <= Product.MinStockLevel
   ?  ?? Send Alert: "?? T?n kho th?p h?n ng??ng"
   ?  ?? Message: "S?n ph?m {Name} ch? còn {qty}, ng??ng {minLevel}"

4. Update DV.Status = "Dispatched"
   ?? DV.UpdatedAt = DateTime.UtcNow
   ?? DV.UpdatedBy = currentUser

5. COMMIT TRANSACTION ?

6. Audit log: "Update" DV Status
```

#### 3.3.2 Business rules (Outbound)

| # | Quy t?c | Chi ti?t |
|----|---------|---------|
| OB1 | **Check Available Qty** | AvailableQuantity = QuantityOnHand - ReservedQuantity, ph?i >= yêu c?u xu?t |
| OB2 | **FIFO/FEFO** | ?u tiên xu?t lô c? nh?t ho?c h?n dùng g?n nh?t tr??c |
| OB3 | **Record Cost Price** | L?u cost price t?i th?i ?i?m xu?t vào detail.CostPrice |
| OB4 | **One Transaction** | T?t c? thao tác trong 1 DB transaction |
| OB5 | **Low Stock Alert** | Sau khi xu?t, n?u t?n <= MinStockLevel thì alert |
| OB6 | **Cannot Reverse** | Khi Dispatched, không ???c s?a l?i, ph?i t?o phi?u ?i?u ch?nh |

#### 3.3.3 Công th?c FIFO/FEFO

```
FIFO: First In, First Out
- Xu?t lô c? nh?t tr??c
- var inventory = inventories
      .Where(i => i.AvailableQuantity > 0)
      .OrderBy(i => i.CreatedAt)
      .ThenBy(i => i.LotNumber)
  .ToList();

FEFO: First Expire, First Out
- Xu?t h?n s?m nh?t tr??c
- var inventory = inventories
      .Where(i => i.AvailableQuantity > 0)
      .OrderBy(i => i.ExpiryDate ?? DateTime.MaxValue)
      .ThenBy(i => i.CreatedAt)
      .ToList();
```

### 3.4 Module Qu?n lý Chuy?n kho (Transfer)

#### 3.4.1 Chi ti?t lu?ng chuy?n kho

**Ti?n ?i?u ki?n:**
- Kho g?i ? Kho nh?n
- Có ít nh?t 1 dòng chi ti?t
- T?n kho ? kho g?i >= yêu c?u chuy?n

**Lu?ng x? lý:**

```
1. Manager opens TransferVoucher (Status = "Draft")
   ?? Validate: FromWarehouseId ? ToWarehouseId ?
   ?? Validate: Details not empty ?
   ?? START TRANSACTION

2. For each detail in TV.Details:
   ?? 1?? Decrease Inventory at FromWarehouse
   ?  ?? Call InventoryService.DecreaseInventoryAsync()
   ?
   ?? 2?? Increase Inventory at ToWarehouse
   ?  ?? Call InventoryService.IncreaseInventoryAsync()
 ?
   ?? 3?? Record 2 StockCards
      ?? OUT: TransactionType="TransferOut", warehouseId=FromWarehouse
      ?? IN:  TransactionType="TransferIn", warehouseId=ToWarehouse

3. Update TV.Status = "Completed"

4. COMMIT TRANSACTION ?

5. Send notification
```

#### 3.4.2 Business rules (Transfer)

| # | Quy t?c | Chi ti?t |
|----|---------|---------|
| TR1 | **Different Warehouses** | Kho g?i ? Kho nh?n, không ???c chuy?n trong cùng 1 kho |
| TR2 | **Lot Number** | Có th? gi? nguyên lot ho?c tách lot khi chuy?n (split) |
| TR3 | **Record 2 StockCards** | Ghi 2 th? kho: OUT (kho g?i) + IN (kho nh?n) cùng reference code |
| TR4 | **Cost Price Unchanged** | Gi? cost price, không tính l?i giá v?n |
| TR5 | **One Transaction** | T?t c? thao tác atomic, rollback n?u l?i |

### 3.5 Module Ki?m kê (Inventory Counting)

#### 3.5.1 Quy trình ki?m kê

**B??c 1: T?o phi?u ki?m kê (Draft)**
- Staff t?o CountingSheet v?i Status="Draft"
- H? th?ng t? ??ng load t?t c? Inventory t? warehouse
- SystemQuantity = QuantityOnHand hi?n t?i

**B??c 2: Nh?p s? l??ng th?c t? (ActualQuantity)**
- Staff ghi nh?n ActualQuantity cho m?i lô hàng
- H? th?ng t? ??ng tính Diff = ActualQuantity - SystemQuantity

**B??c 3: Duy?t & x? lý chênh l?ch**
- Manager review phi?u
- Nh?n approve ? h? th?ng x? lý t?ng dòng:
  - N?u Diff > 0: T?ng t?n (hàng th?a)
  - N?u Diff < 0: Gi?m t?n (hàng thi?u)
  - N?u Diff = 0: Không làm gì
- T?o InventoryAdjustment record
- Record StockCard v?i type="Adjustment"

#### 3.5.2 X? lý chênh l?ch t?n kho

| K?ch b?n | SystemQty | ActualQty | Diff | Hành ??ng | K?t qu? |
|---------|-----------|-----------|------|----------|---------|
| **Hàng th?a** | 100 | 110 | +10 | IncreaseInventory(+10) | T?n = 110, ghi StockCard |
| **Hàng thi?u** | 100 | 85 | -15 | DecreaseInventory(-15) | T?n = 85, ghi StockCard |
| **Kh?p** | 100 | 100 | 0 | Không làm gì | Không ghi audit |
| **Sai lô** | - | - | - | Split to correct lot | T?o 2 adjustment |

#### 3.5.3 Business rules (Counting)

| # | Quy t?c | Chi ti?t |
|----|---------|---------|
| CT1 | **Sheet Status Draft** | Ch? phi?u Draft m?i ???c phê duy?t |
| CT2 | **ActualQuantity Required** | Ph?i nh?p actual qty, không ?? null |
| CT3 | **Auto-calculate Diff** | T? ??ng tính Diff = Actual - System |
| CT4 | **Create Adjustment** | T?o InventoryAdjustment record cho m?i diff ? 0 |
| CT5 | **Reason Field** | L?u reason (damaged, lost, wrong qty, etc.) |
| CT6 | **One Transaction** | T?t c? adjustment trong 1 DB transaction |

### 3.6 Module C?nh báo (Alerts)

#### 3.6.1 Hangfire Job - InventoryAlertJob

**Th?i gian ch?y:** Hàng ngày 23:59 (UTC)

**Query 1: Hàng s?p h?t h?n**
```sql
SELECT * FROM Inventories
WHERE ExpiryDate IS NOT NULL
  AND ExpiryDate <= DATEADD(DAY, 30, GETUTCDATE())
  AND QuantityOnHand > 0
  AND IsDeleted = 0
```

**Query 2: Hàng t?n th?p**
```sql
SELECT * FROM Inventories
WHERE QuantityOnHand > 0
  AND QuantityOnHand <= 10  -- Threshold configurable
  AND IsDeleted = 0
```

**Email Alert:**
- Recipient: `admin@wms.com` (configurable)
- Template: HTML with product list
- Via: `EmailService.SendEmailAsync()`

**SignalR Notification:**
- Hub: `NotificationHub`
- Message: "?? C?NH BÁO T?N KHO & H?N S? D?NG"

#### 3.6.2 Business rules (Alerts)

| # | Quy t?c | Chi ti?t |
|----|---------|---------|
| AL1 | **Threshold 30 Days** | C?nh báo h?n dùng s?p h?t trong vòng 30 ngày |
| AL2 | **Low Stock Threshold** | C?nh báo t?n <= 10 (configurable) |
| AL3 | **Exclude Deleted** | Không c?nh báo hàng ?ã xóa (IsDeleted=true) |
| AL4 | **Daily Schedule** | Ch?y 1 l?n/ngày vào 23:59 UTC |
| AL5 | **Retry Logic** | N?u email fail, retry t?i ?a 3 l?n |
| AL6 | **Log Success/Failure** | Ghi Serilog khi alert g?i thành công/th?t b?i |

---

## 4. KI?N TRÚC H? TH?NG

### 4.1 S? ?? ki?n trúc t?ng th?

```
???????????????????????????????????????????????????????????????????
?  ?
?  ??? Presentation Layer (Controllers, DTOs)           ?
?     ??? AuthController             ?
?     ??? ProductsController          ?
?     ??? InventoryController          ?
?     ??? InboundController?
?     ??? OutboundController                 ?
?   ??? ... (10+ controllers)       ?
?    ?
??? HTTP/JSON           ?
?  ?
?  ?? Business Logic Layer (Services, Validators)        ?
?     ??? IInventoryService + InventoryService        ?
? ??? IInboundService + InboundService  ?
?     ??? IOutboundService + OutboundService ?
?     ??? ICountingService + CountingService           ?
?     ??? ITransferService + TransferService                   ?
?     ??? IEmailService + EmailService    ?
?     ??? INotificationService + NotificationService             ?
?     ??? IAuditService + AuditService  ?
?     ??? ... (FluentValidation)              ?
?           ?
?          ?? IUnitOfWork       ?
?            ?
?  ?? Data Access Layer (Repositories, UnitOfWork)             ?
?     ??? IGenericRepository<T>        ?
?   ??? GenericRepository<T>   ?
?     ??? IUnitOfWork ?
?     ??? UnitOfWork     ?
?                 ?
?             ?? EF Core           ?
?                 ?
?  ??? Data Layer (DbContext, Models, Migrations)  ?
?     ??? ApplicationDbContext          ?
?     ??? Product, Inventory, Vouchers, StockCard   ?
?     ??? Migrations (EF Core migrations)          ?
?          ?
?     ?? ADO.NET    ?
?              ?
?  ??? Database (SQL Server)      ?
?     ?
???????????????????????????????????????????????????????????????????

Cross-cutting concerns:
??? ?? Authentication (JWT)
??? ?? Authorization (RBAC)
??? ?? Notifications (SignalR)
??? ?? Email (MailKit)
??? ?? Background Jobs (Hangfire)
??? ?? Logging (Serilog)
??? ??? Exception Handling (Middleware)
```

### 4.2 Phân l?p (Layered Architecture)

| L?p | Thành ph?n | Trách nhi?m |
|-----|-----------|-----------|
| **Presentation** | Controllers, DTOs, ApiResponse | Receive HTTP requests, validate inputs, return responses |
| **Business Logic** | Services, Validators, DomainLogic | Implement business rules, transactions, notifications |
| **Data Access** | Repositories, UnitOfWork, GenericRepository | Abstract DB access, query construction |
| **Data** | DbContext, Models, Migrations | Entity mapping, DB configuration, schema versioning |
| **Infrastructure** | Email, Notifications, JobScheduler, Logger | Cross-cutting services |

### 4.3 Công ngh? s? d?ng

| # | Công ngh? | Phiên b?n | M?c ?ích | Vai trò |
|----|----------|----------|---------|--------|
| 1 | **.NET Core** | 8/9 | Runtime | Backend runtime, async/await |
| 2 | **Entity Framework Core** | Latest | ORM | Database mapping, migrations |
| 3 | **SQL Server** | 2019+ | Database | Data persistence |
| 4 | **JWT** | - | Authentication | Stateless auth tokens |
| 5 | **Hangfire** | Latest | Job Scheduler | Background jobs (alerts) |
| 6 | **SignalR** | Latest | Real-time | Push notifications |
| 7 | **Serilog** | Latest | Logging | Structured logging |
| 8 | **MailKit** | Latest | Email | Send email alerts |
| 9 | **AutoMapper** | Latest | Mapping | DTO ? Model mapping |
| 10 | **FluentValidation** | Latest | Validation | Request DTO validation |
| 11 | **xUnit** | Latest | Testing | Unit tests |
| 12 | **Moq** | Latest | Mocking | Mock dependencies |

### 4.4 Design Patterns áp d?ng

| # | Pattern | N?i s? d?ng | L?i ích |
|----|---------|-----------|---------|
| 1 | **Repository** | `GenericRepository<T>` | Abstraction DB access, testable |
| 2 | **Unit of Work** | `IUnitOfWork` + `UnitOfWork` | Transaction management, consistency |
| 3 | **Dependency Injection** | `Program.cs` | Loose coupling, testable |
| 4 | **Service Layer** | `IInventoryService`, etc. | Business logic centralization |
| 5 | **DTO Pattern** | `*Dto` classes | Request/response schema |
| 6 | **Factory** | `ICodeGeneratorService` | Code generation |
| 7 | **Observer** | `INotificationService` | Event-driven notifications |
| 8 | **Middleware** | `ExceptionMiddleware` | Global error handling |
| 9 | **Strategy** | `IEmailService` | Pluggable email providers |
| 10 | **Decorator** | Soft Delete filter | Dynamic query filtering |

---

## 5. THI?T K? C? S? D? LI?U

### 5.1 B?ng chính (Data Dictionary)

#### 5.1.1 Products (S?n ph?m)

| Tr??ng | Ki?u | Constraint | Mô t? |
|-------|------|-----------|-------|
| `Id` | int | PK, IDENTITY | Khóa chính |
| `SkuCode` | varchar(50) | Unique, Not Null | Mã SKU duy nh?t |
| `Name` | varchar(200) | Not Null | Tên s?n ph?m |
| `CategoryId` | int | FK, Not Null | Danh m?c |
| `Unit` | varchar(50) | Not Null | ??n v? (cái, b?, kg) |
| `MinStockLevel` | int | Default 0 | Ng??ng t?i thi?u |
| `IsLotManaged` | bit | Default 1 | Qu?n lý theo lô? |
| `CostPrice` | decimal(18,2) | Nullable | Giá v?n (weighted avg) |
| `SellingPrice` | decimal(18,2) | Nullable | Giá bán |
| `CreatedAt` | datetime | Not Null | Th?i gian t?o |
| `UpdatedAt` | datetime | Nullable | Th?i gian s?a |
| `CreatedBy` | varchar(100) | Not Null | Ng??i t?o |
| `UpdatedBy` | varchar(100) | Nullable | Ng??i s?a |
| `IsDeleted` | bit | Default 0 | Soft delete flag |
| `RowVersion` | timestamp | Concurrency | Optimistic locking |

**Indexes:**
```sql
CREATE UNIQUE INDEX IX_Product_SkuCode 
ON Products(SkuCode) WHERE IsDeleted = 0;
```

#### 5.1.2 Inventories (T?n kho theo lô)

| Tr??ng | Ki?u | Constraint | Mô t? |
|-------|------|-----------|-------|
| `Id` | int | PK | Khóa chính |
| `ProductId` | int | FK, Not Null | S?n ph?m |
| `WarehouseId` | int | FK, Not Null | Kho ch?a |
| `LotNumber` | varchar(50) | Nullable | S? lô hàng |
| `ExpiryDate` | datetime | Nullable | H?n s? d?ng |
| `QuantityOnHand` | decimal(18,2) | Default 0 | T?n th?c t? |
| `ReservedQuantity` | decimal(18,2) | Default 0 | T?n ??t tr??c |
| `LocationInWarehouse` | varchar(50) | Nullable | V? trí (A-01-01) |
| `CreatedAt` | datetime | Not Null | Th?i gian t?o |
| `UpdatedAt` | datetime | Nullable | Th?i gian s?a |
| `CreatedBy` | varchar(100) | Not Null | Ng??i t?o |
| `UpdatedBy` | varchar(100) | Nullable | Ng??i s?a |
| `IsDeleted` | bit | Default 0 | Soft delete |
| `RowVersion` | timestamp | Concurrency | Optimistic locking |

**Computed Column:**
```sql
ALTER TABLE Inventories
ADD AvailableQuantity AS (QuantityOnHand - ReservedQuantity);
```

**Indexes:**
```sql
CREATE UNIQUE INDEX IX_Inventory_ProductWarehouseLot
ON Inventories(ProductId, WarehouseId, LotNumber)
WHERE IsDeleted = 0;

CREATE INDEX IX_Inventory_ExpiryDate ON Inventories(ExpiryDate);
CREATE INDEX IX_Inventory_WarehouseId ON Inventories(WarehouseId);
```

#### 5.1.3 ReceivingVouchers (Phi?u nh?p)

| Tr??ng | Ki?u | Constraint | Mô t? |
|-------|------|-----------|-------|
| `Id` | int | PK | Khóa chính |
| `Code` | varchar(50) | Unique, Not Null | Mã phi?u (PN-20240401-001) |
| `WarehouseId` | int | FK, Not Null | Kho nh?p |
| `SupplierId` | int | FK, Nullable | Nhà cung c?p |
| `Status` | varchar(50) | Default "Draft" | Draft / Completed |
| `ReceivingDate` | datetime | Not Null | Ngày nh?p |
| `TotalQuantity` | decimal(18,2) | Default 0 | T?ng s? l??ng |
| `TotalAmount` | decimal(18,2) | Nullable | T?ng ti?n |
| `Notes` | varchar(500) | Nullable | Ghi chú |
| `CreatedAt` | datetime | Not Null | Th?i gian t?o |
| `UpdatedAt` | datetime | Nullable | Th?i gian s?a |
| `CreatedBy` | varchar(100) | Not Null | Ng??i t?o |
| `UpdatedBy` | varchar(100) | Nullable | Ng??i duy?t |
| `IsDeleted` | bit | Default 0 | Soft delete |
| `RowVersion` | timestamp | Concurrency | Locking |

**Indexes:**
```sql
CREATE UNIQUE INDEX IX_ReceivingVoucher_Code
ON ReceivingVouchers(Code) WHERE IsDeleted = 0;

CREATE INDEX IX_ReceivingVoucher_Status ON ReceivingVouchers(Status);
```

#### 5.1.4 ReceivingVoucherDetails (Chi ti?t phi?u nh?p)

| Tr??ng | Ki?u | Constraint | Mô t? |
|-------|------|-----------|-------|
| `Id` | int | PK | Khóa chính |
| `ReceivingVoucherId` | int | FK, Not Null | Phi?u nh?p |
| `ProductId` | int | FK, Not Null | S?n ph?m |
| `Quantity` | decimal(18,2) | Not Null, > 0 | S? l??ng |
| `UnitPrice` | decimal(18,2) | Nullable | Giá nh?p/??n v? |
| `LotNumber` | varchar(50) | Nullable | S? lô |
| `ExpiryDate` | datetime | Nullable | H?n dùng |
| `CreatedAt` | datetime | Not Null | Th?i gian t?o |
| `UpdatedAt` | datetime | Nullable | Th?i gian s?a |
| `CreatedBy` | varchar(100) | Not Null | Ng??i t?o |
| `UpdatedBy` | varchar(100) | Nullable | Ng??i s?a |
| `IsDeleted` | bit | Default 0 | Soft delete |
| `RowVersion` | timestamp | Concurrency | Locking |

#### 5.1.5 StockCards (Th? kho - L?ch s?)

| Tr??ng | Ki?u | Constraint | Mô t? |
|-------|------|-----------|-------|
| `Id` | int | PK | Khóa chính |
| `ProductId` | int | FK, Not Null | S?n ph?m |
| `WarehouseId` | int | FK, Not Null | Kho |
| `LotNumber` | varchar(50) | Nullable | S? lô |
| `TransactionDate` | datetime | Not Null | Ngày giao d?ch |
| `TransactionType` | varchar(50) | Not Null | Inbound/Outbound/Transfer/Adjustment |
| `ReferenceCode` | varchar(50) | Nullable | Mã phi?u (PN-..., PX-...) |
| `BeforeQuantity` | decimal(18,2) | Not Null | T?n tr??c |
| `ChangeQuantity` | decimal(18,2) | Not Null | Thay ??i (+ or -) |
| `AfterQuantity` | decimal(18,2) | Not Null | T?n sau |
| `Notes` | varchar(500) | Nullable | Ghi chú |
| `CreatedAt` | datetime | Not Null | Th?i gian t?o |
| `CreatedBy` | varchar(100) | Not Null | H? th?ng |
| `IsDeleted` | bit | Default 0 | Soft delete |

**Indexes:**
```sql
CREATE INDEX IX_StockCard_ProductWarehouseLot
ON StockCards(ProductId, WarehouseId, LotNumber);

CREATE INDEX IX_StockCard_TransactionDate
ON StockCards(TransactionDate DESC);

CREATE INDEX IX_StockCard_ReferenceCode
ON StockCards(ReferenceCode);
```

#### 5.1.6 AuditLogs (Ki?m toán)

| Tr??ng | Ki?u | Constraint | Mô t? |
|-------|------|-----------|-------|
| `Id` | int | PK | Khóa chính |
| `EntityName` | varchar(128) | Not Null | Tên entity |
| `EntityId` | varchar(max) | Nullable | JSON c?a keys {Id: 123} |
| `Action` | varchar(50) | Not Null | Create/Update/Delete/HardDelete |
| `OldValues` | varchar(max) | Nullable | JSON c?a giá tr? c? |
| `NewValues` | varchar(max) | Nullable | JSON c?a giá tr? m?i |
| `ChangedBy` | varchar(100) | Not Null | Ng??i thay ??i |
| `ChangedAt` | datetime | Not Null | Th?i gian thay ??i |
| `CreatedAt` | datetime | Not Null | Th?i gian t?o log |

**Indexes:**
```sql
CREATE INDEX IX_AuditLog_EntityName ON AuditLogs(EntityName);
CREATE INDEX IX_AuditLog_ChangedAt ON AuditLogs(ChangedAt DESC);
CREATE INDEX IX_AuditLog_ChangedBy ON AuditLogs(ChangedBy);
```

### 5.2 Relational Constraints

| Constraint | From Table | To Table | Behavior | Mô t? |
|-----------|-----------|----------|----------|-------|
| FK_Product_Category | Products | Categories | Restrict | Không xóa category n?u còn product |
| FK_Inventory_Product | Inventories | Products | Restrict | Không xóa product n?u còn inventory |
| FK_Inventory_Warehouse | Inventories | Warehouses | Restrict | Không xóa warehouse n?u còn inventory |
| FK_RVDetail_RV | ReceivingVoucherDetails | ReceivingVouchers | Cascade | Xóa phi?u thì xóa chi ti?t |
| FK_DVDetail_DV | DeliveryVoucherDetails | DeliveryVouchers | Cascade | Xóa phi?u thì xóa chi ti?t |
| FK_TVDetail_TV | TransferVoucherDetails | TransferVouchers | Cascade | Xóa phi?u thì xóa chi ti?t |

---

## 6. THI?T K? API

### 6.1 Base Response Format

**Success response:**
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "message": "Success",
  "data": {
    "items": [],
    "pageNumber": 1,
"pageSize": 20,
    "totalCount": 100,
    "totalPages": 5
  }
}
```

**Error response:**
```json
{
  "statusCode": 400,
"isSuccess": false,
  "message": "Validation failed",
  "errors": [
    "Field1: Error message 1",
    "Field2: Error message 2"
  ]
}
```

### 6.2 Authentication Endpoints

| Method | Endpoint | Description | Auth | Body |
|--------|----------|-------------|------|------|
| POST | `/api/auth/login` | ??ng nh?p | ? | `{username, password}` |
| POST | `/api/auth/refresh` | Làm m?i token | ? | `{refreshToken}` |
| POST | `/api/auth/logout` | ??ng xu?t | ? | `{}` |
| POST | `/api/auth/change-password` | ??i m?t kh?u | ? | `{oldPassword, newPassword}` |

**Login Request/Response:**

```json
// Request
{
  "username": "admin@wms.com",
  "password": "SecurePass123!"
}

// Response
{
  "statusCode": 200,
  "isSuccess": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
 "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
    "expiresIn": 3600,
    "user": {
      "id": 1,
      "username": "admin@wms.com",
      "roles": ["Admin"]
    }
  }
}
```

### 6.3 Product Endpoints

| Method | Endpoint | Description | Auth | Roles |
|--------|----------|-------------|------|-------|
| GET | `/api/products` | Danh sách s?n ph?m | ? | Admin, Manager, Staff |
| GET | `/api/products/{id}` | Chi ti?t s?n ph?m | ? | Admin, Manager, Staff |
| POST | `/api/products` | T?o s?n ph?m | ? | Admin, Manager |
| PUT | `/api/products/{id}` | S?a s?n ph?m | ? | Admin, Manager |
| DELETE | `/api/products/{id}` | Xóa s?n ph?m | ? | Admin |
| POST | `/api/products/import` | Import t? Excel | ? | Admin, Manager |

**GET /api/products - Query:**
```
?categoryId=1&pageNumber=1&pageSize=20&search=Bia
```

**Response:**
```json
{
  "statusCode": 200,
  "data": {
    "items": [
      {
        "id": 1,
        "skuCode": "BIA001",
     "name": "Bia Heineken 330ml",
   "category": "?? u?ng",
        "minStockLevel": 100,
      "costPrice": 15000,
        "sellingPrice": 25000
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 150,
    "totalPages": 8
  }
}
```

### 6.4 Inventory Endpoints

| Method | Endpoint | Description | Auth | Roles |
|--------|----------|-------------|------|-------|
| GET | `/api/inventory` | Danh sách t?n kho | ? | Admin, Manager, Staff |
| GET | `/api/inventory/stock-cards` | Th? kho / l?ch s? | ? | Admin, Manager, Staff, Auditor |
| POST | `/api/inventory/import` | Import t?n kho | ? | Admin, Manager |

**GET /api/inventory - Query:**
```
?productId=1&warehouseId=1&pageNumber=1&pageSize=20
```

**Response:**
```json
{
  "statusCode": 200,
  "data": {
    "items": [
      {
        "id": 1,
        "productName": "Bia Heineken",
        "warehouseName": "Warehouse A",
        "lotNumber": "LOT-20240101",
        "expiryDate": "2025-01-01",
        "quantityOnHand": 500,
    "reservedQuantity": 50,
      "availableQuantity": 450,
        "locationInWarehouse": "A-01-01"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 250,
    "totalPages": 13
  }
}
```

### 6.5 Inbound (Nh?p kho) Endpoints

| Method | Endpoint | Description | Auth | Roles |
|--------|----------|-------------|------|-------|
| POST | `/api/inbound` | T?o phi?u nh?p | ? | Admin, Manager, Staff |
| GET | `/api/inbound/{id}` | Chi ti?t phi?u | ? | Admin, Manager, Staff |
| PUT | `/api/inbound/{id}` | S?a phi?u (Draft) | ? | Admin, Manager, Staff |
| DELETE | `/api/inbound/{id}` | Xóa phi?u (Draft) | ? | Admin, Manager |
| POST | `/api/inbound/{id}/approve` | Duy?t phi?u | ? | Admin, Manager |
| GET | `/api/inbound` | Danh sách phi?u | ? | Admin, Manager, Staff |

**POST /api/inbound - Create:**
```json
{
  "code": "PN-20240420-001",
  "warehouseId": 1,
  "supplierId": 5,
  "receivingDate": "2024-04-20T10:00:00Z",
  "notes": "Nh?p hàng t? XYZ",
  "details": [
    {
      "productId": 1,
      "quantity": 100,
      "unitPrice": 15000,
      "lotNumber": "LOT-20240420",
"expiryDate": "2025-04-20"
    }
  ]
}
```

**POST /api/inbound/{id}/approve - Approve:**
```json
{}
```

**Response:**
```json
{
  "statusCode": 200,
  "message": "Phi?u nh?p ???c duy?t thành công",
  "data": {
    "id": 1,
    "code": "PN-20240420-001",
    "status": "Completed",
    "approvedAt": "2024-04-20T14:30:00Z",
    "approvedBy": "admin@wms.com"
  }
}
```

### 6.6 Outbound (Xu?t kho) Endpoints

| Method | Endpoint | Description | Auth | Roles |
|--------|----------|-------------|------|-------|
| POST | `/api/outbound` | T?o phi?u xu?t | ? | Admin, Manager, Staff |
| GET | `/api/outbound/{id}` | Chi ti?t phi?u | ? | Admin, Manager, Staff |
| PUT | `/api/outbound/{id}` | S?a phi?u (Draft) | ? | Admin, Manager, Staff |
| DELETE | `/api/outbound/{id}` | Xóa phi?u (Draft) | ? | Admin, Manager |
| POST | `/api/outbound/{id}/approve` | Duy?t phi?u | ? | Admin, Manager |

**POST /api/outbound - Create:**
```json
{
  "code": "PX-20240420-001",
  "warehouseId": 1,
  "customerId": 10,
  "deliveryDate": "2024-04-20",
  "details": [
    {
      "productId": 1,
  "quantity": 50,
  "sellingPrice": 25000,
      "lotNumber": "LOT-20240101"
    }
  ]
}
```

### 6.7 Counting (Ki?m kê) Endpoints

| Method | Endpoint | Description | Auth | Roles |
|--------|----------|-------------|------|-------|
| POST | `/api/counting` | T?o phi?u | ? | Admin, Manager, Staff |
| GET | `/api/counting/{id}` | Chi ti?t | ? | Admin, Manager, Staff |
| PUT | `/api/counting/{id}` | Update actual qty | ? | Admin, Manager, Staff |
| POST | `/api/counting/{id}/approve` | Duy?t & tính | ? | Admin, Manager |

**PUT /api/counting/{id} - Update quantities:**
```json
{
  "details": [
    {
   "productId": 1,
      "lotNumber": "LOT-20240101",
      "systemQuantity": 500,
      "actualQuantity": 480,
      "reason": "Hàng h?ng"
    }
  ]
}
```

### 6.8 Audit Endpoints

| Method | Endpoint | Description | Auth | Roles |
|--------|----------|-------------|------|-------|
| GET | `/api/audit` | Danh sách logs | ? | Admin, Auditor |
| GET | `/api/audit/{id}` | Chi ti?t log | ? | Admin, Auditor |
| GET | `/api/audit/entity/{entityName}` | Logs c?a entity | ? | Admin, Auditor |

**Response:**
```json
{
  "statusCode": 200,
  "data": {
    "items": [
      {
   "id": 1,
        "entityName": "Product",
        "entityId": "{\"Id\": 1}",
        "action": "Update",
        "oldValues": "{\"CostPrice\": 15000}",
  "newValues": "{\"CostPrice\": 14500}",
        "changedBy": "admin@wms.com",
"changedAt": "2024-04-20T14:30:00Z"
      }
    ]
  }
}
```

---

## 7. THI?T K? B?O M?T

### 7.1 Authentication (JWT)

**Token Structure:**
```
Header: {
  "alg": "HS256",
  "typ": "JWT"
}

Payload: {
  "sub": "1",
  "name": "admin@wms.com",
  "role": "Admin",
  "iat": 1640000000,
  "exp": 1640003600,
  "iss": "QLKHO",
  "aud": "WMS-CLIENT"
}

Signature: HMACSHA256(base64UrlEncode(header) + "." + base64UrlEncode(payload), secret)
```

**Config appsettings.json:**
```json
{
  "Jwt": {
    "Key": "your-super-secret-key-at-least-32-chars",
    "Issuer": "QLKHO",
    "Audience": "WMS-CLIENT",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

### 7.2 Authorization (RBAC)

| Role | Quy?n | Endpoints |
|------|-------|----------|
| **Admin** | Full access | T?t c? |
| **WarehouseManager** | Manage warehouse | CRUD + Approve |
| **WarehouseStaff** | Create & edit draft | POST, PUT (draft) |
| **Auditor** | Read-only | GET (audit logs) |

**Controller attributes:**
```csharp
[Authorize]  // Requires authenticated
[Authorize(Roles = "Admin,WarehouseManager")]  // Specific roles
```

### 7.3 Password Policy

| Policy | Requirement |
|--------|-------------|
| **Min Length** | 8 characters |
| **Uppercase** | At least 1 (A-Z) |
| **Lowercase** | At least 1 (a-z) |
| **Digits** | At least 1 (0-9) |
| **Special Chars** | At least 1 (!@#$%^&*) |
| **Expiration** | 90 days |
| **History** | Cannot reuse last 5 |
| **Attempt Limit** | Max 5 failed ? lock 15 min |

### 7.4 Data Protection

**In Transit:**
- ? HTTPS/TLS 1.2+
- ? Certificate pinning (recommended)

**At Rest:**
- ? SQL Server encryption (TDE)
- ? Password hash (bcrypt)
- ? Audit logs preserved

**Application:**
- ? Input validation (FluentValidation)
- ? XSS protection
- ? SQL injection protection (EF Core)
- ? CSRF tokens

### 7.5 CORS Configuration

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
   .AllowAnyMethod()
   .AllowAnyHeader();
    });
});
```

### 7.6 Security Headers

```
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Strict-Transport-Security: max-age=31536000
Content-Security-Policy: default-src 'self'
```

### 7.7 Secrets Management

**Development (.env.local - gitignored):**
```
Jwt__Key=dev-secret-key
ConnectionStrings__DefaultConnection=Server=localhost;...
Smtp__Pass=dev-password
```

**Production (Azure Key Vault):**
- ? Rotate secrets quarterly
- ? Audit access logs
- ? Environment-specific configs

---

## 8. THI?T K? X? L? L?I & LOGGING

### 8.1 Exception Handling

**ExceptionMiddleware flow:**
```
Request ? ExceptionMiddleware
          ?? Try: await _next(context)
          ?? Catch: HandleExceptionAsync()
    ?? ValidationException ? 400 BadRequest
         ?? KeyNotFoundException ? 404 NotFound
  ?? UnauthorizedAccessException ? 401 Unauthorized
       ?? Conflict ? 409 Conflict
    ?? Other ? 500 InternalServerError
```

**Error Response Format:**
```json
{
  "statusCode": 400,
  "isSuccess": false,
  "message": "Validation failed",
  "errors": ["Field: Error message"]
}
```

### 8.2 Logging (Serilog)

**Configuration:**
```json
{
"Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    },
    "WriteTo": [
      {
   "Name": "Console"
      },
      {
        "Name": "File",
 "Args": {
          "path": "logs/qlykho-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
 }
    ]
  }
}
```

**Log Levels:**

| Level | Usage |
|-------|-------|
| **Verbose** | Detailed flow tracing |
| **Debug** | Development debugging |
| **Information** | Important events (login, approve) |
| **Warning** | Potentially harmful (low stock) |
| **Error** | Error occurred (exception) |
| **Fatal** | System cannot continue |

**Code example:**
```csharp
Log.Information("InventoryAlertJob started at {time}", DateTime.UtcNow);
Log.Warning("?? Found {count} products with low stock", count);
Log.Error(ex, "Error approving voucher {voucherId}", voucherId);
```

### 8.3 Audit Log Design

**What to audit:**

| Entity | Events | Data |
|--------|--------|------|
| **Products** | Create, Update, Delete | All fields |
| **Inventories** | Create, Update | QuantityOnHand changes |
| **Vouchers** | Create, Approve, Cancel | Status changes |
| **User** | Login, ChangePassword | Timestamp, IP |

**Audit Log Entry (JSON):**
```json
{
  "entityName": "Product",
  "entityId": "{\"Id\": 1}",
  "action": "Update",
  "oldValues": {"Name": "Old", "CostPrice": 15000},
  "newValues": {"Name": "New", "CostPrice": 14500},
  "changedBy": "admin@wms.com",
  "changedAt": "2024-04-20T14:30:00Z"
}
```

---

## 9. THI?T K? TRI?N KHAI

### 9.1 Deployment Architecture

```
Developer (Local)
    ? (Push)
GitHub Repository
    ? (Trigger)
CI/CD Pipeline (GitHub Actions)
    ?? Build & Test
    ?? Deploy to Staging
  ?? Manual Approval
    ?? Deploy to Production
```

### 9.2 Docker Configuration

**Dockerfile:**
```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["QLKHO_PhanVanHoang.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "QLKHO_PhanVanHoang.dll"]
```

**docker-compose.yml:**
```yaml
version: '3.8'
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
 SA_PASSWORD: "YourPassword123!"
    ACCEPT_EULA: "Y"
    ports:
   - "1433:1433"

  api:
    build: .
    environment:
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=QLKHO;...
      - Jwt__Key=secret-key
    ports:
      - "5059:80"
    depends_on:
      - sqlserver
```

### 9.3 CI/CD Pipeline (GitHub Actions)

**.github/workflows/dotnet.yml:**
```yaml
name: .NET Build & Deploy

on:
  push:
    branches: [ main, develop ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore
 run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release
    
    - name: Run Tests
      run: dotnet test --no-build
    
 - name: Publish
      run: dotnet publish -c Release -o ./publish
    
    - name: Build Docker
      run: docker build -t qlkho:latest .
    
    - name: Deploy
      run: kubectl apply -f deployment.yaml
```

### 9.4 Environment Configuration

**appsettings.Development.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=QLKHO_Dev;..."
  },
  "Jwt": {
    "Key": "dev-key-min-32-chars",
    "AccessTokenExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {"Default": "Debug"}
  }
}
```

**appsettings.Production.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sql-prod.company.com;Database=QLKHO_Prod;..."
  },
  "Jwt": {
    "Key": "${JWT_SECRET_KEY}",
  "AccessTokenExpirationMinutes": 120
  },
"Logging": {
    "LogLevel": {"Default": "Information"}
  }
}
```

### 9.5 Database Migration

```sh
# Create migration
dotnet ef migrations add AddInventoryAdjustment

# Update database
dotnet ef database update

# For production (approved)
dotnet ef database update --environment Production
```

### 9.6 Backup & Disaster Recovery

| Component | Strategy | Frequency | Retention |
|-----------|----------|-----------|-----------|
| **Database** | Full + Incremental | Daily (full), Hourly (inc) | 30 days |
| **Application** | Container versioning | Per release | Latest 5 |
| **Logs** | Archive to blob | Daily | 90 days |
| **Secrets** | Key Vault | Real-time | Versioned |

**RTO/RPO:**
- RTO: 1 hour
- RPO: 1 hour (hourly backups)

---

## 10. K? HO?CH KI?M TH?

### 10.1 Testing Strategy

**Test Pyramid:**
```
         E2E (5%)
      Integration (15%)
   Unit Tests (80%)
```

| Type | Coverage | Tools | Execution | Priority |
|------|----------|-------|-----------|----------|
| **Unit** | 80% | xUnit, Moq | < 1 min | P1 |
| **Integration** | 15% | xUnit, SQL LocalDB | 5-10 min | P1 |
| **E2E** | 5% | Postman, Selenium | 15-30 min | P2 |
| **Performance** | N/A | JMeter | Daily | P2 |
| **Security** | N/A | OWASP ZAP | Weekly | P1 |

### 10.2 Unit Test Example

```csharp
[Fact]
public async Task IncreaseInventoryAsync_NewProduct_ShouldCreateInventory()
{
    // Arrange
  var mockUow = new Mock<IUnitOfWork>();
    var service = new InventoryService(mockUow.Object, null);
    
    mockUow.Setup(u => u.Products.GetByIdAsync(1))
  .ReturnsAsync(new Product { Id = 1 });
    mockUow.Setup(u => u.Inventories.FindAsync(It.IsAny<Expression<Func<Inventory, bool>>>()))
        .ReturnsAsync(new List<Inventory>());
    
    // Act
    await service.IncreaseInventoryAsync(1, 1, "LOT01", 100, 1000, "REF001");
    
    // Assert
    mockUow.Verify(u => u.Inventories.AddAsync(It.IsAny<Inventory>()), Times.Once);
}
```

### 10.3 Test Coverage Goals

| Component | Goal | Current |
|-----------|------|---------|
| **Services** | 85% | 70% |
| **Controllers** | 70% | 50% |
| **Repositories** | 90% | 60% |
| **Overall** | 80% | 60% |

### 10.4 Run Tests

```sh
# Run all
dotnet test

# With coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura

# Specific class
dotnet test --filter "ClassName=InventoryServiceTests"

# Generate report
reportgenerator -reports:"coverage.lcov" -targetdir:"coverage"
```

---

## 11. PH? L?C

### 11.1 Glossary (Gi?i thích thu?t ng?)

| Thu?t ng? | Vi?t t?t | Gi?i thích |
|-----------|---------|-----------|
| **Warehouse** | WH | Kho ch?a hàng |
| **Inventory** | INV | T?n kho |
| **Lot/Batch** | - | Lô hàng, nhóm s?n ph?m cùng ngu?n |
| **SKU** | - | Mã s?n ph?m duy nh?t |
| **Expiry Date** | EXP | H?n s? d?ng |
| **Receiving Voucher** | RV | Phi?u nh?p kho |
| **Delivery Voucher** | DV | Phi?u xu?t kho |
| **Transfer Voucher** | TV | Phi?u chuy?n kho |
| **Stock Card** | SC | Th? kho, l?ch s? giao d?ch |
| **FIFO** | - | First In, First Out |
| **FEFO** | - | First Expire, First Out |
| **Weighted Average Cost** | WAC | Giá v?n bình quân gia quy?n |
| **Soft Delete** | - | ?ánh d?u xóa m?m |
| **Hard Delete** | - | Xóa v?nh vi?n |
| **Audit Trail** | - | L?ch s? ki?m toán |
| **RBAC** | - | Role-Based Access Control |
| **JWT** | - | JSON Web Token |
| **SignalR** | - | Real-time communication |
| **Hangfire** | - | Background job scheduler |
| **ORM** | - | Object-Relational Mapping |
| **DTO** | - | Data Transfer Object |

### 11.2 Auto-Generated Code Format

```
ReceivingVoucher: PN-{YYYYMMDD}-{SEQ}
  ví d?: PN-20240420-001, PN-20240420-002

DeliveryVoucher: PX-{YYYYMMDD}-{SEQ}
  ví d?: PX-20240420-001

TransferVoucher: PK-{YYYYMMDD}-{SEQ}
  ví d?: PK-20240420-001

CountingSheet: KK-{YYYYMMDD}-{SEQ}
  ví d?: KK-20240420-001
```

### 11.3 Công th?c Tính toán

**Weighted Average Cost (WAC):**
```
newCostPrice = (oldCostPrice * oldQty + unitPrice * newQty) / (oldQty + newQty)

Ví d?:
- Lúc ??u: 100 chi?c × 100,000 ?/chi?c
- Nh?p thêm: 50 chi?c × 120,000 ?/chi?c
- newCostPrice = (100,000*100 + 120,000*50) / (100+50)
   = 16,000,000 / 150
              = 106,667 ?/chi?c
```

**AvailableQuantity:**
```
AvailableQuantity = QuantityOnHand - ReservedQuantity

Ví d?:
- T?n th?c t?: 500 chi?c
- ??t tr??c: 50 chi?c
- Kh? d?ng: 450 chi?c
```

**Profit Calculation:**
```
Profit = (SellingPrice - CostPrice) * Quantity

Ví d?:
- SellingPrice: 150,000 ?/chi?c
- CostPrice: 106,667 ?/chi?c
- Quantity: 100 chi?c
- Profit = (150,000 - 106,667) × 100 = 4,333,300 ?
```

### 11.4 Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 0.1 | 2024-04-01 | Initial draft | Phan V?n Hoàng |
| 0.5 | 2024-04-10 | Added API, auth, security | Phan V?n Hoàng |
| 1.0 | 2024-04-20 | Final version | Phan V?n Hoàng |

---

## K?T LU?N

B?n **Báo cáo ??c t? Nghi?p v? & Thi?t k? H? th?ng** này cung c?p tài li?u toàn di?n cho d? án **QLKHO (Warehouse Management System)**.

? ??nh ngh?a rõ ràng các requirement nghi?p v?  
? Thi?t k? ki?n trúc chi ti?t v?i design patterns  
? Mô t? c? s? d? li?u ??y ??  
? API specification cho t?t c? endpoints  
? B?o m?t toàn di?n (JWT, RBAC, password policy)  
? Deployment strategy v?i Docker & CI/CD  
? K? ho?ch ki?m th? toàn di?n  

**Ngày phát hành:** 2024-04-20  
**Tr?ng thái:** Chính th?c ?

---

**END OF DOCUMENT**
