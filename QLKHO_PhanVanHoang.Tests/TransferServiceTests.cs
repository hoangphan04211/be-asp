using Moq;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Services;
using QLKHO_PhanVanHoang.DTOs;
using System.Linq.Expressions;
using Xunit;

namespace QLKHO_PhanVanHoang.Tests
{
    public class TransferServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IInventoryService> _mockInventoryService;
        private readonly TransferService _transferService;

        public TransferServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockInventoryService = new Mock<IInventoryService>();
            _transferService = new TransferService(_mockUow.Object, _mockInventoryService.Object);
        }

        [Fact]
        public async Task CreateTransferVoucherAsync_SameWarehouses_ShouldThrowArgumentException()
        {
            // Arrange
            var dto = new CreateTransferDto
            {
                FromWarehouseId = 1,
                ToWarehouseId = 1,
                Code = "TR001",
                Notes = "Notes"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _transferService.CreateTransferVoucherAsync(dto));
        }

        [Fact]
        public async Task ApproveTransferVoucherAsync_ValidVoucher_ShouldTransferInventory()
        {
            // Arrange
            int voucherId = 1;
            var voucher = new TransferVoucher 
            { 
                Id = voucherId, 
                Code = "TR001", 
                Status = "Draft", 
                FromWarehouseId = 1, 
                ToWarehouseId = 2 
            };
            var pagedResult = new PagedResult<TransferVoucher>
            {
                Items = new List<TransferVoucher> { voucher }
            };

            var details = new List<TransferVoucherDetail>
            {
                new TransferVoucherDetail { ProductId = 1, Quantity = 5 }
            };
            voucher.Details = details;

            var product = new Product { Id = 1, CostPrice = 100 };

            _mockUow.Setup(u => u.TransferVouchers.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<TransferVoucher, bool>>>(), null, "Details"))
                    .ReturnsAsync(pagedResult);
            _mockUow.Setup(u => u.Products.GetByIdAsync(1)).ReturnsAsync(product);
            _mockUow.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            await _transferService.ApproveTransferVoucherAsync(voucherId);

            // Assert
            Assert.Equal("Completed", voucher.Status);
            // Verify decrease at fromWarehouse
            _mockInventoryService.Verify(s => s.DecreaseInventoryAsync(1, 1, It.IsAny<string>(), 5, "TR001"), Times.Once);
            // Verify increase at toWarehouse
            _mockInventoryService.Verify(s => s.IncreaseInventoryAsync(1, 2, It.IsAny<string>(), 5, 100, "TR001"), Times.Once);

            _mockUow.Verify(u => u.TransferVouchers.Update(voucher), Times.Once);
        }
    }
}
