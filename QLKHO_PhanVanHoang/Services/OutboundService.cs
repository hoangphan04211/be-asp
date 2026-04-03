using System;
using System.Linq;
using System.Threading.Tasks;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Services
{
    public class OutboundService : IOutboundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventoryService _inventoryService;

        public OutboundService(IUnitOfWork unitOfWork, IInventoryService inventoryService)
        {
            _unitOfWork = unitOfWork;
            _inventoryService = inventoryService;
        }

        public async Task ApproveDeliveryVoucherAsync(int voucherId)
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
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
