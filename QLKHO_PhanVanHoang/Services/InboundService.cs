using System;
using System.Linq;
using System.Threading.Tasks;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Services
{
    public class InboundService : IInboundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventoryService _inventoryService;

        public InboundService(IUnitOfWork unitOfWork, IInventoryService inventoryService)
        {
            _unitOfWork = unitOfWork;
            _inventoryService = inventoryService;
        }

        public async Task ApproveReceivingVoucherAsync(int voucherId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var voucher = await _unitOfWork.ReceivingVouchers.GetByIdAsync(voucherId);
                if (voucher == null) throw new Exception("Không tìm thấy phiếu nhập");
                if (voucher.Status != "Draft") throw new Exception("Chỉ duyệt được phiếu nháp (Draft)");

                var details = await _unitOfWork.ReceivingVoucherDetails.FindAsync(d => d.ReceivingVoucherId == voucherId);
                if (!details.Any()) throw new Exception("Phiếu nhập không có chi tiết sản phẩm");

                foreach (var detail in details)
                {
                     decimal cost = detail.UnitPrice ?? 0;
                     await _inventoryService.IncreaseInventoryAsync(detail.ProductId, voucher.WarehouseId, detail.LotNumber, detail.Quantity, cost, voucher.Code);
                }

                voucher.Status = "Completed";
                _unitOfWork.ReceivingVouchers.Update(voucher);

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
