using System;
using System.Linq;
using System.Threading.Tasks;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Services
{
    public class CountingService : ICountingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventoryService _inventoryService;

        public CountingService(IUnitOfWork unitOfWork, IInventoryService inventoryService)
        {
            _unitOfWork = unitOfWork;
            _inventoryService = inventoryService;
        }

        public async Task ApproveCountingSheetAsync(int countingSheetId)
        {
            var sheetList = await _unitOfWork.CountingSheets.GetPagedAsync(1, 1, s => s.Id == countingSheetId, null, "Details");
            var sheet = sheetList.Items.FirstOrDefault();

            if (sheet == null) throw new Exception("Không tìm thấy phiếu kiểm kê.");
            if (sheet.Status != "Draft") throw new Exception("Chỉ duyệt được phiếu ở trạng thái Nháp.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var detail in sheet.Details)
                {
                    if (detail.ActualQuantity == null) continue;

                    decimal diff = (decimal)detail.ActualQuantity - detail.SystemQuantity;
                    if (diff == 0) continue;

                    // 1. Lưu vết điều chỉnh
                    var adjustment = new InventoryAdjustment
                    {
                        AdjustmentType = "Counting",
                        ReferenceId = sheet.Id,
                        ReferenceNumber = sheet.Code,
                        ProductId = detail.ProductId,
                        WarehouseId = sheet.WarehouseId,
                        LotNumber = detail.LotNumber,
                        OldQuantity = detail.SystemQuantity,
                        NewQuantity = (decimal)detail.ActualQuantity,
                        Reason = "Điều chỉnh sau kiểm kê",
                        Status = "Approved",
                        ApprovedAt = DateTime.Now
                    };
                    await _unitOfWork.InventoryAdjustments.AddAsync(adjustment);

                    // 2. Cập nhật tồn kho (Tăng hoặc giảm)
                    if (diff > 0)
                    {
                        // Lấy giá vốn gần nhất để tăng
                        var product = await _unitOfWork.Products.GetByIdAsync(detail.ProductId);
                        await _inventoryService.IncreaseInventoryAsync(
                            detail.ProductId, 
                            sheet.WarehouseId, 
                            detail.LotNumber, 
                            diff, 
                            product?.CostPrice ?? 0, 
                            sheet.Code);
                    }
                    else
                    {
                        await _inventoryService.DecreaseInventoryAsync(
                            detail.ProductId, 
                            sheet.WarehouseId, 
                            detail.LotNumber, 
                            Math.Abs(diff), 
                            sheet.Code);
                    }
                }

                sheet.Status = "Approved";
                _unitOfWork.CountingSheets.Update(sheet);

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
