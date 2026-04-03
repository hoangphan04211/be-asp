# 📦 QLKHO_PHANVANHOANG

[![version](https://img.shields.io/badge/version-1.0.0-blue)](https://github.com/hoangphan04211/be-asp)
[![license](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![dotnet](https://img.shields.io/badge/.NET-8.0-blueviolet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen)](https://github.com/hoangphan04211/be-asp/pulls)

**Mô tả ngắn gọn**: Hệ thống API Backend Quản lý Kho (WMS) thông minh giúp doanh nghiệp tối ưu hóa quy trình lưu trữ, theo dõi hàng hóa và giá vốn theo thời gian thực.

<p align="center">
  <img src="https://via.placeholder.com/800x400?text=Warehouse+Management+System+API+Dashboard" alt="Demo Screenshot" width="800">
</p>

## 📋 Mục lục
- [Tổng quan](#tổng-quan)
- [Tính năng](#tính-năng)
- [Công nghệ sử dụng](#công-nghệ-sử-dụng)
- [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
- [Cài đặt](#cài-đặt)
- [Cấu hình](#cấu-hình)
- [Chạy dự án](#chạy-dự-án)
- [Cấu trúc thư mục](#cấu-trúc-thư-mục)
- [Kiến trúc hệ thống](#kiến-trúc-hệ-thống)
- [API Documentation](#api-documentation)
- [Database Schema](#database-schema)
- [Hướng dẫn sử dụng](#hướng-dẫn-sử-dụng)
- [Testing](#testing)
- [Triển khai](#triển-khai)
- [Tác giả](#tác-giả)

---

## Tổng quan
Dự án được phát triển nhằm cung cấp một giải pháp quản lý kho mạnh mẽ, bảo mật và dễ mở rộng. Hệ thống hỗ trợ đầy đủ các quy trình từ nhập hàng, xuất hàng đến điều chuyển nội bộ và kiểm kê định kỳ.

## Tính năng
- **Quản lý Danh mục (Master Data)**: Sản phẩm, Kho hàng, Nhà cung cấp, Khách hàng.
- **Quản lý Nhập kho (Inbound)**: Hỗ trợ nhập hàng theo lô (Lot), tự động tính giá vốn theo phương pháp **Bình quân gia quyền (Weighted Average Cost)**.
- **Quản lý Xuất kho (Outbound)**: Kiểm soát tồn kho thực tế, đảm bảo không xuất quá số lượng hiện có.
- **Điều chuyển nội bộ (Transfer)**: Quản lý di chuyển hàng hóa giữa các kho.
- **Kiểm kê (Stock Take)**: Lập phiếu kiểm kê, đối soát và tự động điều chỉnh tồn kho.
- **Thẻ kho (Stock Card)**: Truy xuất chi tiết lịch sử biến động của từng mặt hàng.
- **Báo cáo & Cảnh báo**: Tích hợp Hangfire để gửi cảnh báo hàng sắp hết hạn hoặc dưới ngưỡng tồn tối thiểu.
- **Bảo mật**: Xác thực JWT, phân quyền Role-based (Admin, Manager, Employee).
- **Log & Audit**: Ghi lại mọi thay đổi dữ liệu (ai sửa, sửa gì, lúc nào).

## Công nghệ sử dụng
- **Backend**: ASP.NET Core 8.0 (Web API)
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT (JSON Web Token)
- **Background Jobs**: Hangfire
- **Logging**: Serilog
- **Validation**: FluentValidation
- **Tài liệu**: Swagger / OpenAPI

## Yêu cầu hệ thống
- .NET 8.0 SDK
- SQL Server (hoặc Docker chạy SQL Server)
- Visual Studio 2022 hoặc VS Code

## Cài đặt
1. Clone repository:
   ```bash
   git clone https://github.com/hoangphan04211/be-asp.git
   ```
2. Cài đặt các thư viện:
   ```bash
   dotnet restore
   ```

## Cấu hình
1. Sao chép file cấu hình mẫu:
   ```bash
   cp appsettings.Example.json appsettings.json
   ```
2. Cập nhật chuỗi kết nối Database và khóa JWT trong `appsettings.json`.

## Chạy dự án
1. Cập nhật Database (Migration):
   ```bash
   dotnet ef database update
   ```
2. Chạy ứng dụng:
   ```bash
   dotnet run
   ```

## Cấu trúc thư mục
- `Controllers/`: Xử lý các yêu cầu API.
- `Services/`: Logic nghiệp vụ chính của hệ thống.
- `Repositories/`: Tương tác với cơ sở dữ liệu (Pattern Repository & Unit of Work).
- `Models/`: Các thực thể database và cấu hình mapping.
- `DTOs/`: Data Transfer Objects cho đầu vào/đầu ra API.
- `Data/`: ApplicationDbContext và dữ liệu khởi tạo (Seed data).

## API Documentation
Sau khi chạy dự án, bạn có thể truy cập Swagger UI tại:
`https://localhost:xxxx/index.html` (Trang chủ Swagger)

## Tác giả
**Phan Van Hoang** - *Dẫn dắt phát triển* - [GitHub Profile](https://github.com/hoangphan04211)

---
*Cảm ơn bạn đã quan tâm đến dự án này!*
