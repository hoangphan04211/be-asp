using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Services
{
    public class CountingService : ICountingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventoryService _inventoryService;
        private readonly INotificationService _notificationService;

        public CountingService(IUnitOfWork unitOfWork, IInventoryService inventoryService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _inventoryService = inventoryService;
            _notificationService = notificationService;
        }

        public async Task ApproveCountingSheetAsync(int countingSheetId)
        {
            var strategy = _unitOfWork.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
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

                        // Lấy tồn kho hiện tại thực tế nhất trong DB (không phụ thuộc vào số cũ trên phiếu)
                        var inventories = await _unitOfWork.Inventories.FindAsync(i => i.ProductId == detail.ProductId && i.WarehouseId == sheet.WarehouseId && i.LotNumber == detail.LotNumber);
                        var inventory = inventories.FirstOrDefault();
                        decimal currentQty = inventory?.QuantityOnHand ?? 0;
                        decimal targetQty = (decimal)detail.ActualQuantity;

                        if (currentQty == targetQty) continue;

                        // 1. Lưu vết điều chỉnh (Adjustment)
                        var adjustment = new InventoryAdjustment
                        {
                            AdjustmentType = "Counting",
                            ReferenceId = sheet.Id,
                            ReferenceNumber = sheet.Code,
                            ProductId = detail.ProductId,
                            WarehouseId = sheet.WarehouseId,
                            LotNumber = detail.LotNumber,
                            OldQuantity = currentQty,
                            NewQuantity = targetQty,
                            Reason = $"Cân đối kiểm kê (Chênh lệch thực tế: {targetQty - currentQty})",
                            Status = "Approved",
                            ApprovedAt = DateTime.Now
                        };
                        await _unitOfWork.InventoryAdjustments.AddAsync(adjustment);

                        // 2. Đồng bộ tồn kho
                        if (inventory == null)
                        {
                            inventory = new Inventory
                            {
                                ProductId = detail.ProductId,
                                WarehouseId = sheet.WarehouseId,
                                LotNumber = detail.LotNumber,
                                QuantityOnHand = targetQty
                            };
                            await _unitOfWork.Inventories.AddAsync(inventory);
                        }
                        else
                        {
                            inventory.QuantityOnHand = targetQty;
                            _unitOfWork.Inventories.Update(inventory);
                        }

                        // 3. Ghi thẻ kho (StockCard)
                        var stockCard = new StockCard
                        {
                            ProductId = detail.ProductId,
                            WarehouseId = sheet.WarehouseId,
                            LotNumber = detail.LotNumber,
                            TransactionType = "Kiểm Kê",
                            ReferenceCode = sheet.Code,
                            BeforeQuantity = currentQty,
                            ChangeQuantity = targetQty - currentQty,
                            AfterQuantity = targetQty,
                            Notes = $"Điều chỉnh kiểm kê phiếu {sheet.Code}"
                        };
                        await _unitOfWork.StockCards.AddAsync(stockCard);
                    }

                    sheet.Status = "Approved";
                    _unitOfWork.CountingSheets.Update(sheet);

                    await _unitOfWork.CommitTransactionAsync();

                    // Gửi thông báo SignalR
                    try {
                        await _notificationService.SendNotificationToAllAsync("📊 Kiểm kê hoàn tất", 
                            $"Phiếu kiểm kê {sheet.Code} đã được duyệt bởi {sheet.UpdatedBy}. Tồn kho đã được cân đối.");
                    } catch {}
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            });
        }
    }
}
