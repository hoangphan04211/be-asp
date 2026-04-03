using System;
using System.Linq;
using System.Threading.Tasks;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task IncreaseInventoryAsync(int productId, int warehouseId, string? lotNumber, decimal quantity, decimal costPrice, string referenceCode)
        {
            if (quantity <= 0) throw new ArgumentException("Số lượng nhập phải lớn hơn 0");

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) throw new Exception("Không tìm thấy sản phẩm");

            var inventories = await _unitOfWork.Inventories.FindAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId && i.LotNumber == lotNumber);
            var inventory = inventories.FirstOrDefault();
            
            decimal beforeQty = inventory?.QuantityOnHand ?? 0;

            if (inventory == null)
            {
                inventory = new Inventory
                {
                    ProductId = productId,
                    WarehouseId = warehouseId,
                    LotNumber = lotNumber,
                    QuantityOnHand = quantity
                };
                await _unitOfWork.Inventories.AddAsync(inventory);
            }
            else
            {
                inventory.QuantityOnHand += quantity;
                _unitOfWork.Inventories.Update(inventory);
            }

            // Tính giá vốn bình quân gia quyền theo sản phẩm
            var allInventories = await _unitOfWork.Inventories.FindAsync(i => i.ProductId == productId);
            decimal totalOldQty = allInventories.Sum(i => i.QuantityOnHand) - quantity;
            if (totalOldQty < 0) totalOldQty = 0;
            
            decimal oldTotalValue = totalOldQty * (product.CostPrice ?? 0);
            decimal newTotalValue = oldTotalValue + (quantity * costPrice);
            decimal newCostPrice = totalOldQty + quantity > 0 ? newTotalValue / (totalOldQty + quantity) : costPrice;
            
            product.CostPrice = newCostPrice;
            _unitOfWork.Products.Update(product);

            // Ghi thẻ kho bằng Repository
            var stockCard = new StockCard
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                LotNumber = lotNumber,
                TransactionType = "Inbound",
                ReferenceCode = referenceCode,
                BeforeQuantity = beforeQty,
                ChangeQuantity = quantity,
                AfterQuantity = beforeQty + quantity,
                Notes = $"Nhập kho từ phiếu {referenceCode}"
            };
            await _unitOfWork.StockCards.AddAsync(stockCard);
        }

        public async Task DecreaseInventoryAsync(int productId, int warehouseId, string? lotNumber, decimal quantity, string referenceCode)
        {
            if (quantity <= 0) throw new ArgumentException("Số lượng xuất phải lớn hơn 0");

            var inventories = await _unitOfWork.Inventories.FindAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId && i.LotNumber == lotNumber);
            var inventory = inventories.FirstOrDefault();

            if (inventory == null || inventory.QuantityOnHand < quantity)
            {
                throw new Exception($"Tồn kho không đủ cho sản phẩm ID {productId}. Tồn hiện tại: {inventory?.QuantityOnHand ?? 0}, yêu cầu: {quantity}");
            }

            decimal beforeQty = inventory.QuantityOnHand;
            inventory.QuantityOnHand -= quantity;
            _unitOfWork.Inventories.Update(inventory);

            // Ghi thẻ kho
            var stockCard = new StockCard
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                LotNumber = lotNumber,
                TransactionType = "Outbound",
                ReferenceCode = referenceCode,
                BeforeQuantity = beforeQty,
                ChangeQuantity = -quantity,
                AfterQuantity = beforeQty - quantity,
                Notes = $"Xuất kho từ phiếu {referenceCode}"
            };
            await _unitOfWork.StockCards.AddAsync(stockCard);
        }
    }
}
