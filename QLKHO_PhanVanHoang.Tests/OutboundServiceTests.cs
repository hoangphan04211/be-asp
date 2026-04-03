using Moq;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Services;
using System.Linq.Expressions;
using Xunit;

namespace QLKHO_PhanVanHoang.Tests
{
    public class OutboundServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IInventoryService> _mockInventoryService;
        private readonly OutboundService _outboundService;

        public OutboundServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockInventoryService = new Mock<IInventoryService>();
            _outboundService = new OutboundService(_mockUow.Object, _mockInventoryService.Object);
        }

        [Fact]
        public async Task ApproveDeliveryVoucherAsync_ValidVoucher_ShouldDispatchAndDecreaseInventory()
        {
            // Arrange
            int voucherId = 1;
            var voucher = new DeliveryVoucher 
            { 
                Id = voucherId, 
                Code = "EX001", 
                Status = "Draft", 
                WarehouseId = 1 
            };

            var details = new List<DeliveryVoucherDetail>
            {
                new DeliveryVoucherDetail { Id = 1, DeliveryVoucherId = voucherId, ProductId = 1, Quantity = 5, SellingPrice = 150 }
            };
            var product = new Product { Id = 1, CostPrice = 100 };

            var mockDetailRepo = new Mock<IGenericRepository<DeliveryVoucherDetail>>();
            mockDetailRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<DeliveryVoucherDetail, bool>>>()))
                          .ReturnsAsync(details);
            mockDetailRepo.Setup(r => r.Update(It.IsAny<DeliveryVoucherDetail>()));

            _mockUow.Setup(u => u.DeliveryVouchers.GetByIdAsync(voucherId)).ReturnsAsync(voucher);
            _mockUow.Setup(u => u.DeliveryVoucherDetails).Returns(mockDetailRepo.Object);
            _mockUow.Setup(u => u.Products.GetByIdAsync(1)).ReturnsAsync(product);
            _mockUow.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            await _outboundService.ApproveDeliveryVoucherAsync(voucherId);

            // Assert
            Assert.Equal("Dispatched", voucher.Status);
            _mockInventoryService.Verify(
                s => s.DecreaseInventoryAsync(1, 1, It.IsAny<string>(), 5m, "EX001"),
                Times.Once);
            _mockUow.Verify(u => u.DeliveryVouchers.Update(voucher), Times.Once);
        }

        [Fact]
        public async Task ApproveDeliveryVoucherAsync_AlreadyDispatched_ShouldThrowException()
        {
            // Arrange
            int voucherId = 2;
            var voucher = new DeliveryVoucher { Id = voucherId, Status = "Dispatched" };

            _mockUow.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.DeliveryVouchers.GetByIdAsync(voucherId)).ReturnsAsync(voucher);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _outboundService.ApproveDeliveryVoucherAsync(voucherId));
        }
    }
}
