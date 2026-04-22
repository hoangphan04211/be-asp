using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Services
{
    public class OutboundService : IOutboundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventoryService _inventoryService;
        private readonly INotificationService _notificationService;

        public OutboundService(IUnitOfWork unitOfWork, IInventoryService inventoryService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _inventoryService = inventoryService;
            _notificationService = notificationService;
        }

        public async Task ApproveDeliveryVoucherAsync(int voucherId)
        {
            var strategy = _unitOfWork.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var voucher = await _unitOfWork.DeliveryVouchers.GetByIdAsync(voucherId);
                    if (voucher == null) throw new Exception("Không tìm thấy phiếu xuất");
                    if (voucher.Status != "Draft") throw new Exception("Chỉ duyệt được phiếu nháp (Draft)");

                    var details = await _unitOfWork.DeliveryVoucherDetails.FindAsync(d => d.DeliveryVoucherId == voucherId);
                    if (!details.Any()) throw new Exception("Phiếu xuất không có chi tiết sản phẩm");

                    foreach (var detail in details)
                    {
                        // Lấy giá vốn hiện tại để ghi nhận vào Chi tiết phiếu xuất
                        var product = await _unitOfWork.Products.GetByIdAsync(detail.ProductId);
                        detail.CostPrice = product?.CostPrice ?? 0;
                        _unitOfWork.DeliveryVoucherDetails.Update(detail);

                        // Trừ tồn kho và ghi thẻ kho thông qua InventoryService
                        await _inventoryService.DecreaseInventoryAsync(detail.ProductId, voucher.WarehouseId, detail.LotNumber, detail.Quantity, voucher.Code);
                    }

                    voucher.Status = "Dispatched";
                    _unitOfWork.DeliveryVouchers.Update(voucher);

                    await _unitOfWork.CommitTransactionAsync();

                    // Gửi thông báo SignalR
                    try {
                        await _notificationService.SendNotificationToAllAsync("🚀 Xuất kho thành công", 
                            $"Phiếu xuất {voucher.Code} đã được duyệt bởi {voucher.UpdatedBy}.");
                    } catch {}
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            });
        }
    }
}
