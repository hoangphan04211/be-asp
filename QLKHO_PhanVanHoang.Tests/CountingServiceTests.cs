using Moq;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Services;
using System.Linq.Expressions;
using Xunit;

namespace QLKHO_PhanVanHoang.Tests
{
    public class CountingServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IInventoryService> _mockInventoryService;
        private readonly CountingService _countingService;

        public CountingServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockInventoryService = new Mock<IInventoryService>();
            _countingService = new CountingService(_mockUow.Object, _mockInventoryService.Object);
        }

        [Fact]
        public async Task ApproveCountingSheetAsync_QuantityIncreased_ShouldIncreaseInventoryAndAddAdjustment()
        {
            // Arrange
            int sheetId = 1;
            var sheet = new CountingSheet 
            { 
                Id = sheetId, 
                Code = "CC001", 
                Status = "Draft", 
                WarehouseId = 1 
            };
            var pagedResult = new PagedResult<CountingSheet>
            {
                Items = new List<CountingSheet> { sheet }
            };

            var details = new List<CountingSheetDetail>
            {
                new CountingSheetDetail { ProductId = 1, SystemQuantity = 10, ActualQuantity = 15 } // diff = +5
            };
            sheet.Details = details;

            var product = new Product { Id = 1, CostPrice = 100 };

            _mockUow.Setup(u => u.CountingSheets.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<CountingSheet, bool>>>(), null, "Details"))
                    .ReturnsAsync(pagedResult);
            _mockUow.Setup(u => u.Products.GetByIdAsync(1)).ReturnsAsync(product);
            _mockUow.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.InventoryAdjustments.AddAsync(It.IsAny<InventoryAdjustment>())).Returns(Task.CompletedTask);

            // Act
            await _countingService.ApproveCountingSheetAsync(sheetId);

            // Assert
            Assert.Equal("Approved", sheet.Status);
            _mockUow.Verify(u => u.InventoryAdjustments.AddAsync(It.Is<InventoryAdjustment>(adj => adj.NewQuantity == 15 && adj.OldQuantity == 10)), Times.Once);
            _mockInventoryService.Verify(s => s.IncreaseInventoryAsync(1, 1, It.IsAny<string>(), 5m, 100, "CC001"), Times.Once);
            _mockUow.Verify(u => u.CountingSheets.Update(sheet), Times.Once);
        }

        [Fact]
        public async Task ApproveCountingSheetAsync_QuantityDecreased_ShouldDecreaseInventoryAndAddAdjustment()
        {
            // Arrange
            int sheetId = 2;
            var sheet = new CountingSheet 
            { 
                Id = sheetId, 
                Code = "CC002", 
                Status = "Draft", 
                WarehouseId = 1 
            };
            var pagedResult = new PagedResult<CountingSheet>
            {
                Items = new List<CountingSheet> { sheet }
            };

            var details = new List<CountingSheetDetail>
            {
                new CountingSheetDetail { ProductId = 1, SystemQuantity = 10, ActualQuantity = 7 } // diff = -3
            };
            sheet.Details = details;

            _mockUow.Setup(u => u.CountingSheets.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<CountingSheet, bool>>>(), null, "Details"))
                    .ReturnsAsync(pagedResult);
            _mockUow.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.InventoryAdjustments.AddAsync(It.IsAny<InventoryAdjustment>())).Returns(Task.CompletedTask);

            // Act
            await _countingService.ApproveCountingSheetAsync(sheetId);

            // Assert
            Assert.Equal("Approved", sheet.Status);
            _mockUow.Verify(u => u.InventoryAdjustments.AddAsync(It.Is<InventoryAdjustment>(adj => adj.NewQuantity == 7 && adj.OldQuantity == 10)), Times.Once);
            _mockInventoryService.Verify(s => s.DecreaseInventoryAsync(1, 1, It.IsAny<string>(), 3m, "CC002"), Times.Once);
            _mockUow.Verify(u => u.CountingSheets.Update(sheet), Times.Once);
        }
    }
}
