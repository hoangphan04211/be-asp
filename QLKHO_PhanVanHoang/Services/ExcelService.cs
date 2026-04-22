using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Services
{
    public class ExcelService : IExcelService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExcelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<byte[]> ExportInventoryReportAsync()
        {
            var inventories = await _unitOfWork.Inventories.FindAsync(null, "Product.Category,Warehouse");
            
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Báo Cáo Tồn Kho");
            
            // Header
            worksheet.Cell(1, 1).Value = "Mã Sản Phẩm (SKU)";
            worksheet.Cell(1, 2).Value = "Tên Sản Phẩm";
            worksheet.Cell(1, 3).Value = "Danh Mục";
            worksheet.Cell(1, 4).Value = "Kho Hàng";
            worksheet.Cell(1, 5).Value = "Số Lô (Lot)";
            worksheet.Cell(1, 6).Value = "Tồn Thực Tế";
            worksheet.Cell(1, 7).Value = "Hàng Đặt Trước";
            worksheet.Cell(1, 8).Value = "Tồn Có Thể Bán";
            worksheet.Cell(1, 9).Value = "Vị Trí";

            // Format Header
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.AirForceBlue;
            headerRow.Style.Font.FontColor = XLColor.White;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Data rows
            int row = 2;
            foreach (var item in inventories)
            {
                worksheet.Cell(row, 1).Value = item.Product?.SkuCode ?? "N/A";
                worksheet.Cell(row, 2).Value = item.Product?.Name ?? "N/A";
                worksheet.Cell(row, 3).Value = item.Product?.Category?.Name ?? "N/A";
                worksheet.Cell(row, 4).Value = item.Warehouse?.Name ?? "N/A";
                worksheet.Cell(row, 5).Value = item.LotNumber ?? "N/A";
                worksheet.Cell(row, 6).Value = item.QuantityOnHand;
                worksheet.Cell(row, 7).Value = item.ReservedQuantity;
                worksheet.Cell(row, 8).Value = item.AvailableQuantity;
                worksheet.Cell(row, 9).Value = item.LocationInWarehouse ?? "";
                row++;
            }

            worksheet.Columns().AdjustToContents();
            worksheet.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<(int SuccessCount, List<string> Errors)> ImportProductsAsync(IFormFile file)
        {
            var errors = new List<string>();
            int successCount = 0;

            if (file == null || file.Length == 0)
            {
                errors.Add("File lỗi hoặc nội dung trống.");
                return (0, errors);
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1); // Mặc định đọc Sheet đầu tiên
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Dòng đầu là Header

            var newProducts = new List<Product>();

            foreach (var row in rows)
            {
                try
                {
                    string skuCode = row.Cell(1).GetString().Trim();
                    string name = row.Cell(2).GetString().Trim();
                    string unit = row.Cell(3).GetString().Trim();
                    if (!decimal.TryParse(row.Cell(4).GetString(), out decimal costPrice)) costPrice = 0;
                    if (!decimal.TryParse(row.Cell(5).GetString(), out decimal sellingPrice)) sellingPrice = 0;
                    
                    if (string.IsNullOrEmpty(skuCode) || string.IsNullOrEmpty(name))
                    {
                        errors.Add($"Row {row.RowNumber()}: Mã SKU và Tên SP không được để khoảng trắng.");
                        continue;
                    }

                    int categoryId = 1; // Tạm giả định tất cả SP nhét vào category 1 cho bản MVP import
                    
                    var product = new Product
                    {
                        SkuCode = skuCode,
                        Name = name,
                        Unit = string.IsNullOrEmpty(unit) ? "Chiếc" : unit,
                        CostPrice = costPrice,
                        SellingPrice = sellingPrice,
                        CategoryId = categoryId
                    };
                    newProducts.Add(product);
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {row.RowNumber()}: Lỗi cấu trúc data -> {ex.Message}");
                }
            }

            // Ghi vào CSDL
            if (newProducts.Any())
            {
                foreach (var p in newProducts)
                {
                    var isExist = await _unitOfWork.Products.FindAsync(x => x.SkuCode == p.SkuCode);
                    if (!isExist.Any())
                    {
                        await _unitOfWork.Products.AddAsync(p);
                        successCount++;
                    }
                    else
                    {
                        errors.Add($"Mã SKU {p.SkuCode} đã tồn tại trên DB, Skip.");
                    }
                }
                
                await _unitOfWork.CompleteAsync();
            }

            return (successCount, errors);
        }

        public async Task<(int SuccessCount, List<string> Errors)> ImportInventoryAsync(IFormFile file)
        {
            var errors = new List<string>();
            int successCount = 0;

            if (file == null || file.Length == 0)
            {
                errors.Add("File lỗi hoặc nội dung trống.");
                return (0, errors);
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

            // Cần IInventoryService để xử lý logic tăng tồn & ghi thẻ kho
            // Vì IExcelService không inject IInventoryService để tránh circular dependency, 
            // ta sẽ xử lý trực tiếp qua UnitOfWork hoặc dùng Service Locator nếu cần.
            // Tuy nhiên, logic IncreaseInventory khá phức tạp, tốt nhất là inject IInventoryService vào ExcelService.

            foreach (var row in rows)
            {
                try
                {
                    string skuCode = row.Cell(1).GetString().Trim();
                    if (!decimal.TryParse(row.Cell(2).GetString(), out decimal quantity)) quantity = 0;
                    if (!int.TryParse(row.Cell(3).GetString(), out int warehouseId)) warehouseId = 1;
                    string lotNumber = row.Cell(4).GetString().Trim();

                    if (string.IsNullOrEmpty(skuCode) || quantity <= 0)
                    {
                        errors.Add($"Row {row.RowNumber()}: SKU trống hoặc số lượng <= 0.");
                        continue;
                    }

                    var products = await _unitOfWork.Products.FindAsync(p => p.SkuCode == skuCode);
                    var product = products.FirstOrDefault();
                    if (product == null)
                    {
                        errors.Add($"Row {row.RowNumber()}: Không tìm thấy sản phẩm SKU {skuCode}.");
                        continue;
                    }

                    // Thực hiện tăng tồn kho (Logic tương tự InventoryService nhưng làm tại đây)
                    var inventories = await _unitOfWork.Inventories.FindAsync(i => i.ProductId == product.Id && i.WarehouseId == warehouseId && i.LotNumber == lotNumber);
                    var inv = inventories.FirstOrDefault();

                    decimal currentQty = inv?.QuantityOnHand ?? 0;
                    if (inv == null)
                    {
                        inv = new Inventory { ProductId = product.Id, WarehouseId = warehouseId, LotNumber = lotNumber, QuantityOnHand = quantity };
                        await _unitOfWork.Inventories.AddAsync(inv);
                    }
                    else
                    {
                        inv.QuantityOnHand += quantity;
                        _unitOfWork.Inventories.Update(inv);
                    }

                    // Ghi thẻ kho
                    await _unitOfWork.StockCards.AddAsync(new StockCard
                    {
                        ProductId = product.Id,
                        WarehouseId = warehouseId,
                        LotNumber = lotNumber,
                        TransactionType = "Inbound",
                        ReferenceCode = "IMPORT-EXCEL",
                        BeforeQuantity = currentQty,
                        ChangeQuantity = quantity,
                        AfterQuantity = currentQty + quantity,
                        Notes = "Nhập tồn kho hàng loạt từ file Excel"
                    });

                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Row {row.RowNumber()}: Lỗi hệ thống -> {ex.Message}");
                }
            }

            await _unitOfWork.CompleteAsync();
            return (successCount, errors);
        }

        public async Task<byte[]> GetInventoryTemplateAsync()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Mẫu Nhập Tồn Kho");

            // Header columns
            worksheet.Cell(1, 1).Value = "Mã SKU";
            worksheet.Cell(1, 2).Value = "Số lượng";
            worksheet.Cell(1, 3).Value = "ID Kho";
            worksheet.Cell(1, 4).Value = "Số Lô (Lot)";
            worksheet.Cell(1, 5).Value = "Vị trí (Location)";

            // Style Header
            var headerRange = worksheet.Range("A1:E1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.AirForceBlue;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // Example data (row 2)
            worksheet.Cell(2, 1).Value = "SP001";
            worksheet.Cell(2, 2).Value = 100;
            worksheet.Cell(2, 3).Value = 1;
            worksheet.Cell(2, 4).Value = "LOT-2024-001";
            worksheet.Cell(2, 5).Value = "A-01-01";

            // Add some comments/instructions
            worksheet.Cell(4, 1).Value = "HƯỚNG DẪN NHẬP LIỆU:";
            worksheet.Cell(4, 1).Style.Font.Bold = true;
            worksheet.Cell(5, 1).Value = "1. Mã SKU: Phải khớp chính xác với mã trong danh mục sản phẩm.";
            worksheet.Cell(6, 1).Value = "2. Số lượng: Phải là số dương (ví dụ: 10, 20.5).";
            worksheet.Cell(7, 1).Value = "3. ID Kho: Lấy ID từ danh mục kho (thường là 1, 2, 3...).";
            worksheet.Cell(8, 1).Value = "4. Số Lô: Có thể để trống nếu sản phẩm không quản lý theo lô.";

            worksheet.Columns().AdjustToContents();
            
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
