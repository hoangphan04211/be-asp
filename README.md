# 📦 QLKHO_PHANVANHOANG - Warehouse Management System API

[![version](https://img.shields.io/badge/version-1.0.0-blue)](https://github.com/hoangphan04211/be-asp)
[![license](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![dotnet](https://img.shields.io/badge/.NET-8.0-blueviolet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen)](https://github.com/hoangphan04211/be-asp/pulls)

**Hệ thống Quản lý Kho (WMS) API thông minh** là giải pháp Backend chuyên nghiệp được thiết kế theo mô hình kiến trúc phân lớp (N-Tier Architecture), hỗ trợ doanh nghiệp tối ưu hóa quy trình lưu trữ, theo dõi biến động hàng hóa và kiểm soát giá vốn theo thời gian thực.

<p align="center">
  <img src="https://via.placeholder.com/800x400?text=Modern+Warehouse+Management+System+Backend" alt="Demo Screenshot" width="800">
</p>

---

## 🏗️ Kiến trúc Hệ thống (System Architecture)

Dự án được xây dựng dựa trên các tiêu chuẩn **Clean Code** và **Enterprise Design Patterns**, đảm bảo tính mở rộng (Scalability) và bảo trì (Maintainability) lâu dài.

### 📂 Cấu trúc thư mục Chi tiết (Folder Structure)

Dưới đây là chi tiết các thành phần cấu thành nên hệ thống "trái tim" của Warehouse Management:

1.  **`Controllers/` (Tầng Giao Diện - Presentation Layer)**
    - *Chức năng:* Tiếp nhận các HTTP Request từ Client (Frontend, Mobile, Third-party).
    - *Chi tiết:* Điều hướng dữ liệu đến các Service tương ứng và phản hồi kết quả theo chuẩn **RESTful API**. Bao gồm các Module: Auth, Products, Inbound, Outbound, Transfer, Audit...

2.  **`Services/` (Tầng Nghiệp Vụ - Business Logic Layer)**
    - *Chức năng:* Xử lý logic nghiệp vụ cốt lõi của hệ thống.
    - *Chi tiết:* Nơi thực thi các quy tắc kinh doanh phức tạp như: tính giá vốn bình quân gia quyền (AQW), kiểm tra ngưỡng tồn kho tối thiểu, logic kiểm kê hàng hóa.

3.  **`Repositories/` (Tầng Truy Cập Dữ Liệu - Data Access Layer)**
    - *Chức năng:* Trừu tượng hóa việc giao tiếp với Cơ sở dữ liệu.
    - *Chi tiết:* Áp dụng Pattern **Generic Repository** và **Unit of Work** để đảm bảo tính nhất quán của dữ liệu (Transaction) và giúp việc chuyển đổi database (nếu cần) trở nên linh hoạt.

4.  **`Models/` (Domain Models)**
    - *Chức năng:* Định nghĩa các thực thể dữ liệu (Entities).
    - *Chi tiết:* Các lớp đại diện cho bảng trong Database (`Product`, `Warehouse`, `Inventory`...). Bao gồm `BaseEntity` hỗ trợ lưu vết lịch sử (Created/Updated) và Xóa mềm (Soft Delete).

5.  **`DTOs/` (Data Transfer Objects)**
    - *Chức năng:* Các lớp trung gian truyền tải dữ liệu.
    - *Chi tiết:* Giúp bảo mật Entity gốc, chỉ truyền tải những thông tin cần thiết giữa các tầng, tối ưu hóa kích thước dữ liệu truyền tải qua mạng.

6.  **`Data/` (Infrastructure Layer)**
    - *Chức năng:* Quản lý hạ tầng dữ liệu.
    - *Chi tiết:* Chứa `ApplicationDbContext`, cấu hình Seeding (dữ liệu mẫu) và quản lý các phiên bản database thông qua Entity Framework Migrations.

7.  **`Jobs/` (Background Tasks)**
    - *Chức năng:* Thực thi các tác vụ định kỳ tự động.
    - *Chi tiết:* Sử dụng **Hangfire** để xử lý các công việc chạy ngầm như: gửi thông báo hàng hết hạn, kiểm tra tồn kho thấp vào cuối ngày.

8.  **`Middlewares/` (Global Middleware)**
    - *Chức năng:* Xử lý các tác vụ xuyên suốt (Cross-cutting Concerns).
    - *Chi tiết:* Tích hợp `ExceptionMiddleware` để bắt lỗi Runtime và trả về định dạng lỗi API thống nhất cho Client.

9.  **`Helpers/` (System Utilities)**
    - *Chức năng:* Các tiện ích bổ trợ hệ thống.
    - *Chi tiết:* Chứa `ApiResponse` chuẩn hóa đầu ra, `Pagination` phục vụ phân trang dữ liệu lớn.

10. **`Validators/` (Data Validation)**
    - *Chức năng:* Kiểm chuẩn dữ liệu đầu vào.
    - *Chi tiết:* Sử dụng **FluentValidation** để cấu hình các ràng buộc dữ liệu chặt chẽ (SKU không trùng, số lượng > 0...).

11. **`Profiles/` (AutoMapper Mapping)**
    - *Chức năng:* Cấu hình ánh xạ đối tượng.
    - *Chi tiết:* Định nghĩa quy tắc chuyển đổi tự động giữa Entity và DTO.

12. **`wwwroot/` (Static Files)**
    - *Chức năng:* Lưu trữ tài nguyên tĩnh.
    - *Chi tiết:* Nơi lưu trữ hình ảnh sản phẩm được tải lên từ hệ thống.

---

## 🛠️ Tính năng & Công nghệ (Features & Technologies)

### Tính năng Nổi bật
- **Quản lý tồn kho theo lô (Lot):** Theo dõi chi tiết từng lô hàng, ngày nhập và hạn sử dụng.
- **Giá vốn thông minh:** Tự động tính toán giá vốn khi nhập hàng.
- **Thẻ kho (Stock Card):** Truy vết lịch sử xuất nhập kho 100% không sai lệch.
- **Hệ thống Kiểm soát (Audit Log):** Ghi lại mọi hành động thay đổi dữ liệu (Ai thay đổi, Giá trị cũ/mới).
- **Phân quyền (RBAC):** Admin (Toàn quyền), WarehouseManager (Quản lý), Employee (Thực thi).

### Công nghệ lõi
- **Core:** .NET 8.0, EF Core 8
- **Security:** JWT Identity, BCrypt Password Hashing
- **Performance:** Hangfire Server, Static File Serving
- **Docs:** Swagger UI (OpenAPI v3)

---

## 🚀 Cài đặt & Vận hành (Deployment & Setup)

1.  **Môi trường:** Đảm bảo máy tính đã cài đặt .NET 8 SDK và SQL Server.
2.  **Cấu hình:** Sao chép `appsettings.Example.json` thành `appsettings.json`, cập nhật `DefaultConnection`.
3.  **Migration:** Mở terminal và chạy `dotnet ef database update`.
4.  **Running:** Chạy lệnh `dotnet run` và truy cập `https://localhost:xxxx` để trải nghiệm Swagger API.

---

## ✍️ Tác giả & Đóng góp
- **Chủ dự án:** Phan Van Hoang
- **Email:** hoangpv@example.com

---
*Dự án này được xây dựng với tâm thế mang lại giá trị thực tế nhất cho quy trình quản lý kho hiện đại.*
