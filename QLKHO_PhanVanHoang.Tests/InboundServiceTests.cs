using Moq;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Services;
using System.Linq.Expressions;
using Xunit;

namespace QLKHO_PhanVanHoang.Tests
{
    public class InboundServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IInventoryService> _mockInventoryService;
        private readonly Mock<INotificationService> _mockNotification;
        private readonly InboundService _inboundService;

        public InboundServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockInventoryService = new Mock<IInventoryService>();
            _mockNotification = new Mock<INotificationService>();
            _inboundService = new InboundService(_mockUow.Object, _mockInventoryService.Object, _mockNotification.Object);
        }

        [Fact]
        public async Task ApproveReceivingVoucherAsync_ValidVoucher_ShouldCompleteAndNotify()
        {
            // Arrange
            int voucherId = 1;
            var voucher = new ReceivingVoucher { Id = voucherId, Code = "IM001", Status = "Draft", WarehouseId = 1 };
            var details = new List<ReceivingVoucherDetail>
            {
                new ReceivingVoucherDetail { ProductId = 1, Quantity = 10, UnitPrice = 100 }
            };

            _mockUow.Setup(u => u.ReceivingVouchers.GetByIdAsync(voucherId)).ReturnsAsync(voucher);
            _mockUow.Setup(u => u.ReceivingVoucherDetails.FindAsync(It.IsAny<Expression<Func<ReceivingVoucherDetail, bool>>>()))
                    .ReturnsAsync(details);
            _mockUow.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            await _inboundService.ApproveReceivingVoucherAsync(voucherId);

            // Assert
            Assert.Equal("Completed", voucher.Status);
            _mockInventoryService.Verify(s => s.IncreaseInventoryAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<string>()), Times.Once);
            _mockUow.Verify(u => u.ReceivingVouchers.Update(voucher), Times.Once);
            _mockNotification.Verify(n => n.SendNotificationToAllAsync(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ApproveReceivingVoucherAsync_AlreadyCompleted_ShouldThrowException()
        {
            // Arrange
            int voucherId = 1;
            var voucher = new ReceivingVoucher { Id = voucherId, Status = "Completed" };
            _mockUow.Setup(u => u.ReceivingVouchers.GetByIdAsync(voucherId)).ReturnsAsync(voucher);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _inboundService.ApproveReceivingVoucherAsync(voucherId));
        }
    }
}
