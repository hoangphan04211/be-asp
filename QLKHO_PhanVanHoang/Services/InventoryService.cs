using System;
using System.Linq;
using System.Threading.Tasks;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Hubs;

namespace QLKHO_PhanVanHoang.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public InventoryService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
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

            Inventory? inventory = null;

            // 1. Thử tìm theo số lô được chỉ định và phải còn hàng
            if (!string.IsNullOrEmpty(lotNumber))
            {
                var inventories = await _unitOfWork.Inventories.FindAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId && i.LotNumber == lotNumber && i.QuantityOnHand >= quantity);
                inventory = inventories.FirstOrDefault();
            }

            // 2. Nếu không tìm thấy lô được chỉ định (hoặc lô đó không đủ hàng), tìm bất kỳ lô nào còn đủ hàng trong kho đó
            if (inventory == null)
            {
                var availableInventories = await _unitOfWork.Inventories.FindAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId && i.QuantityOnHand >= quantity);
                inventory = availableInventories.OrderByDescending(i => i.QuantityOnHand).FirstOrDefault();
            }

            // 3. Nếu vẫn không thấy lô nào đủ, báo lỗi chi tiết
            if (inventory == null)
            {
                var totalInWarehouse = (await _unitOfWork.Inventories.FindAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId)).Sum(i => i.QuantityOnHand);
                throw new Exception($"Không có lô hàng nào đủ {quantity} sản phẩm. Tổng tồn tất cả các lô trong kho này là: {totalInWarehouse}. Vui lòng kiểm tra lại số lượng xuất.");
            }

            decimal beforeQty = inventory.QuantityOnHand;
            inventory.QuantityOnHand -= quantity;
            _unitOfWork.Inventories.Update(inventory);
            
            // Ghi nhận số lô thực tế đã xuất vào thẻ kho
            string? actualLot = inventory.LotNumber;

            // Ghi thẻ kho
            var stockCard = new StockCard
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                LotNumber = actualLot,
                TransactionType = "Outbound",
                ReferenceCode = referenceCode,
                BeforeQuantity = beforeQty,
                ChangeQuantity = -quantity,
                AfterQuantity = beforeQty - quantity,
                Notes = $"Xuất kho từ phiếu {referenceCode}"
            };
            await _unitOfWork.StockCards.AddAsync(stockCard);

            // Kiểm tra ngưỡng tồn kho tối thiểu sau khi xuất
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product != null)
            {
                var productInventories = await _unitOfWork.Inventories.FindAsync(i => i.ProductId == productId);
                var totalStock = productInventories.Sum(i => i.QuantityOnHand);
                if (totalStock <= product.MinStockLevel)
                {
                    await _notificationService.SendNotificationToAllAsync("⚠️ Cảnh báo tồn kho", 
                        $"Sản phẩm '{product.Name}' ({product.SkuCode}) đang dưới ngưỡng tồn tối thiểu! Tồn hiện tại: {totalStock}");
                }
            }
        }
    }
}
