using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Services;
using Serilog;

namespace QLKHO_PhanVanHoang.Jobs
{
    public class InventoryAlertJob
    {
        private readonly IServiceProvider _serviceProvider;

        public InventoryAlertJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task CheckExpiryAndLowStockAsync()
        {
            Log.Information(">>> [JOB] Bắt đầu quét cảnh báo tồn kho...");

            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var config = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();

            var thresholdDate = DateTime.UtcNow.AddDays(30);
            var recipientEmail = config["Smtp:SenderEmail"] ?? "phan21828@gmail.com";
            
            // 1. Quét hàng sắp hết hạn
            var expiringInventories = await unitOfWork.Inventories.FindAsync(i => i.ExpiryDate != null && i.ExpiryDate <= thresholdDate && i.QuantityOnHand > 0);
            
            // 2. Quét hàng tồn kho thấp (dưới <= 10)
            var lowStockInventories = await unitOfWork.Inventories.FindAsync(i => i.QuantityOnHand > 0 && i.QuantityOnHand <= 10);

            if (!expiringInventories.Any() && !lowStockInventories.Any())
            {
                Log.Information(">>> [JOB] Không phát hiện vấn đề về tồn kho. Không gửi mail.");
                return;
            }

            string subject = "🔔 CẢNH BÁO HÀNG HÓA - HỆ THỐNG WMS";
            string body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                    <h2 style='color: #ef4444;'>Cảnh báo từ Hệ thống Quản lý Kho</h2>
                    <p>Chào Ban Quản trị, hệ thống phát hiện các vấn đề cần lưu ý sau đây:</p>";

            if (expiringInventories.Any())
            {
                body += "<h3 style='color: #f59e0b;'>⚠️ Lô hàng sắp hết hạn (trong 30 ngày)</h3><ul>";
                foreach (var item in expiringInventories)
                {
                    var product = await unitOfWork.Products.GetByIdAsync(item.ProductId);
                    body += $"<li><b>{product?.Name}</b> (Mã: {product?.SkuCode}) - Lô: {item.LotNumber} - Hết hạn: <b>{item.ExpiryDate?.ToString("dd/MM/yyyy")}</b></li>";
                }
                body += "</ul>";
            }

            if (lowStockInventories.Any())
            {
                body += "<h3 style='color: #0061ff;'>📉 Sản phẩm sắp cạn kho (<= 10 chiếc)</h3><ul>";
                foreach (var item in lowStockInventories)
                {
                    var product = await unitOfWork.Products.GetByIdAsync(item.ProductId);
                    body += $"<li><b>{product?.Name}</b> (Mã: {product?.SkuCode}) - Còn lại: <b style='color:red;'>{item.QuantityOnHand}</b> đơn vị</li>";
                }
                body += "</ul>";
            }

            body += @"
                    <p style='margin-top: 20px; font-size: 12px; color: #666;'>Đây là thông báo tự động từ hệ thống. Vui lòng kiểm tra kho hàng thực tế.</p>
                </div>";

            await emailService.SendEmailAsync(recipientEmail, subject, body);
            Log.Information($">>> [JOB] Đã gửi mail cảnh báo tới {recipientEmail}.");
        }
    }
}
