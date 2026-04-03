using Moq;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Services;
using System.Linq.Expressions;
using Xunit;

namespace QLKHO_PhanVanHoang.Tests
{
    public class InventoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<INotificationService> _mockNotification;
        private readonly InventoryService _inventoryService;

        public InventoryServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockNotification = new Mock<INotificationService>();
            _inventoryService = new InventoryService(_mockUow.Object, _mockNotification.Object);
        }

        [Fact]
        public async Task IncreaseInventoryAsync_ValidProduct_ShouldUpdateInventoryAndCostPrice()
        {
            // Arrange
            int productId = 1;
            int warehouseId = 1;
            string refCode = "REF001";
            decimal quantity = 10;
            decimal costPrice = 100;

            var product = new Product { Id = productId, Name = "Test Product", SkuCode = "P01", CostPrice = 0 };
            var inventory = new List<Inventory>();

            _mockUow.Setup(u => u.Products.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockUow.Setup(u => u.Inventories.FindAsync(It.IsAny<Expression<Func<Inventory, bool>>>()))
                    .ReturnsAsync(inventory);
            _mockUow.Setup(u => u.StockCards.AddAsync(It.IsAny<StockCard>())).Returns(Task.CompletedTask);

            // Act
            await _inventoryService.IncreaseInventoryAsync(productId, warehouseId, "LOT01", quantity, costPrice, refCode);

            // Assert
            _mockUow.Verify(u => u.Inventories.AddAsync(It.IsAny<Inventory>()), Times.Once);
            _mockUow.Verify(u => u.Products.Update(It.Is<Product>(p => p.CostPrice == costPrice)), Times.Once);
            _mockUow.Verify(u => u.StockCards.AddAsync(It.IsAny<StockCard>()), Times.Once);
        }

        [Fact]
        public async Task DecreaseInventoryAsync_WhenStockBelowMin_ShouldTriggerNotification()
        {
            // Arrange
            int productId = 1;
            int warehouseId = 1;
            var product = new Product { Id = productId, Name = "Low Stock Product", SkuCode = "P02", MinStockLevel = 5 };
            var inventory = new Inventory { ProductId = productId, WarehouseId = warehouseId, QuantityOnHand = 10 };
            
            _mockUow.Setup(u => u.Products.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockUow.Setup(u => u.Inventories.FindAsync(It.IsAny<Expression<Func<Inventory, bool>>>()))
                    .ReturnsAsync(new List<Inventory> { inventory });
            _mockUow.Setup(u => u.StockCards.AddAsync(It.IsAny<StockCard>())).Returns(Task.CompletedTask);

            // Act
            await _inventoryService.DecreaseInventoryAsync(productId, warehouseId, "LOT01", 6, "OUT01"); // 10 - 6 = 4 (Below 5)

            // Assert
            _mockNotification.Verify(n => n.SendNotificationToAllAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DecreaseInventoryAsync_InsufficientStock_ShouldThrowException()
        {
            // Arrange
            int productId = 1;
            int warehouseId = 1;
            var inventory = new Inventory { ProductId = productId, WarehouseId = warehouseId, QuantityOnHand = 5 };
            
            _mockUow.Setup(u => u.Inventories.FindAsync(It.IsAny<Expression<Func<Inventory, bool>>>()))
                    .ReturnsAsync(new List<Inventory> { inventory });

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _inventoryService.DecreaseInventoryAsync(productId, warehouseId, "LOT01", 10, "OUT01"));
        }
    }
}
