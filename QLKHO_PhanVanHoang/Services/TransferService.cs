using System;
using System.Linq;
using System.Threading.Tasks;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.DTOs;

namespace QLKHO_PhanVanHoang.Services
{
    public class TransferService : ITransferService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventoryService _inventoryService;
        private readonly ICodeGeneratorService _codeGenerator;

        public TransferService(IUnitOfWork unitOfWork, IInventoryService inventoryService, ICodeGeneratorService codeGenerator)
        {
            _unitOfWork = unitOfWork;
            _inventoryService = inventoryService;
            _codeGenerator = codeGenerator;
        }
        public async Task CreateTransferVoucherAsync(CreateTransferDto dto)
        {
            if (dto.FromWarehouseId == dto.ToWarehouseId)
            {
                throw new ArgumentException("Kho gửi và kho nhận phải khác nhau.");
            }

            var voucher = new TransferVoucher
            {
                FromWarehouseId = dto.FromWarehouseId,
                ToWarehouseId = dto.ToWarehouseId,
                Code = string.IsNullOrEmpty(dto.Code) ? await _codeGenerator.GenerateTransferCodeAsync() : dto.Code,
                Status = "Draft",
                TransferDate = DateTime.Now,
                Notes = dto.Notes
            };

            foreach (var detail in dto.Details)
            {
                voucher.Details.Add(new TransferVoucherDetail
                {
                    ProductId = detail.ProductId,
                    LotNumber = detail.LotNumber,
                    Quantity = detail.Quantity
                });
            }

            await _unitOfWork.TransferVouchers.AddAsync(voucher);
            await _unitOfWork.CompleteAsync();
        }

        public async Task ApproveTransferVoucherAsync(int voucherId)
        {
            var voucherList = await _unitOfWork.TransferVouchers.GetPagedAsync(1, 1, v => v.Id == voucherId, null, "Details");
            var voucher = voucherList.Items.FirstOrDefault();

            if (voucher == null) throw new Exception("Không tìm thấy phiếu chuyển kho.");
            if (voucher.Status != "Draft") throw new Exception("Chỉ có thể phê duyệt phiếu ở trạng thái Nháp.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var detail in voucher.Details)
                {
                    // 1. Giảm tồn tại kho gửi
                    await _inventoryService.DecreaseInventoryAsync(
                        detail.ProductId, 
                        voucher.FromWarehouseId, 
                        detail.LotNumber, 
                        detail.Quantity, 
                        voucher.Code);

                    // 2. Lấy thông tin sản phẩm để lấy giá vốn hiện tại (cho kho nhận)
                    var product = await _unitOfWork.Products.GetByIdAsync(detail.ProductId);
                    decimal costPrice = product?.CostPrice ?? 0;

                    // 3. Tăng tồn tại kho nhận
                    await _inventoryService.IncreaseInventoryAsync(
                        detail.ProductId, 
                        voucher.ToWarehouseId, 
                        detail.LotNumber, 
                        detail.Quantity, 
                        costPrice, 
                        voucher.Code);
                }

                voucher.Status = "Completed";
                _unitOfWork.TransferVouchers.Update(voucher);

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
