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
            Log.Information("Bắt đầu Job quét cảnh báo tồn kho...");

            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var thresholdDate = DateTime.UtcNow.AddDays(30);
            
            // 1. Quét hàng sắp hết hạn trong 30 ngày tới
            var expiringInventories = await unitOfWork.Inventories.FindAsync(i => i.ExpiryDate != null && i.ExpiryDate <= thresholdDate && i.QuantityOnHand > 0);
            
            // 2. Quét hàng tồn kho thấp (dưới <= 10)
            var lowStockInventories = await unitOfWork.Inventories.FindAsync(i => i.QuantityOnHand > 0 && i.QuantityOnHand <= 10);

            if (expiringInventories.Any() || lowStockInventories.Any())
            {
                string subject = "🔔 CẢNH BÁO TỒN KHO & HẠN SỬ DỤNG - HỆ THỐNG WMS";
                string body = $"<h3>Phát hiện {expiringInventories.Count()} lô hàng sắp hết hạn và {lowStockInventories.Count()} mặt hàng sắp cạn kho.</h3>";
                
                body += "<ul>";
                foreach(var item in expiringInventories) {
                    body += $"<li>CẢNH BÁO HẠN SỬ DỤNG: Sản phẩm ID {item.ProductId} (Lô: {item.LotNumber}) - Hết hạn vào: {item.ExpiryDate?.ToString("dd/MM/yyyy")}</li>";
                }
                foreach(var item in lowStockInventories) {
                    body += $"<li>CẢNH BÁO SẮP HẾT HÀNG: Sản phẩm ID {item.ProductId} chỉ còn {item.QuantityOnHand} chiếc/đơn vị.</li>";
                }
                body += "</ul>";

                await emailService.SendEmailAsync("phan21828@gmail.com", subject, body);
            }

            Log.Information("Job quét tồn kho hoàn tất!");
        }
    }
}
