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
            var inventories = await _unitOfWork.Inventories.GetAllAsync();
            
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Báo Cáo Tồn Kho");
            
            // Header
            worksheet.Cell(1, 1).Value = "ID Tồn Kho";
            worksheet.Cell(1, 2).Value = "ID Sản Phẩm";
            worksheet.Cell(1, 3).Value = "Mã Kho";
            worksheet.Cell(1, 4).Value = "Số Lô (Lot)";
            worksheet.Cell(1, 5).Value = "Tồn Thực Tế";
            worksheet.Cell(1, 6).Value = "Hàng Đặt Trước";
            worksheet.Cell(1, 7).Value = "Tồn Có Thể Bán";
            worksheet.Cell(1, 8).Value = "Vị Trí Lưu Trữ";

            // Format Header
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.AirForceBlue;
            headerRow.Style.Font.FontColor = XLColor.White;

            // Data rows
            int row = 2;
            foreach (var item in inventories)
            {
                worksheet.Cell(row, 1).Value = item.Id;
                worksheet.Cell(row, 2).Value = item.ProductId;
                worksheet.Cell(row, 3).Value = item.WarehouseId;
                worksheet.Cell(row, 4).Value = item.LotNumber ?? "N/A";
                worksheet.Cell(row, 5).Value = item.QuantityOnHand;
                worksheet.Cell(row, 6).Value = item.ReservedQuantity;
                worksheet.Cell(row, 7).Value = item.AvailableQuantity;
                worksheet.Cell(row, 8).Value = item.LocationInWarehouse ?? "";
                row++;
            }

            worksheet.Columns().AdjustToContents();

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
    }
}
