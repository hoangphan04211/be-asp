using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<object> GetSummaryAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var inventories = await _unitOfWork.Inventories.GetAllAsync();
            var warehouses = await _unitOfWork.Warehouses.GetAllAsync();

            return new
            {
                TotalProducts = products.Count(),
                TotalWarehouses = warehouses.Count(),
                TotalStockValue = inventories.Sum(i => i.QuantityOnHand * (i.Product?.CostPrice ?? 0)),
                InventoryItems = inventories.Count()
            };
        }

        public async Task<object> GetTrendDataAsync(int days = 7)
        {
            var startDate = DateTime.Today.AddDays(-days + 1);
            
            // Lấy dữ liệu 7 ngày qua
            var inbounds = await _unitOfWork.ReceivingVouchers.FindAsync(v => v.ReceivingDate >= startDate && v.Status == "Completed");
            var outbounds = await _unitOfWork.DeliveryVouchers.FindAsync(v => v.DeliveryDate >= startDate && v.Status == "Dispatched");
            var transfers = await _unitOfWork.TransferVouchers.FindAsync(v => v.TransferDate >= startDate && v.Status == "Completed");

            var trend = new List<object>();

            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                var dayName = date.ToString("dd/MM");

                var inboundCount = inbounds.Where(v => v.ReceivingDate.Date == date.Date).Count();
                var outboundCount = outbounds.Where(v => v.DeliveryDate.Date == date.Date).Count();
                var transferCount = transfers.Where(v => v.TransferDate.Date == date.Date).Count();

                trend.Add(new
                {
                    Date = dayName,
                    Inbound = inboundCount,
                    Outbound = outboundCount,
                    Transfer = transferCount
                });
            }

            return trend;
        }

        public async Task<object> GetLowStockAlertsAsync()
        {
            var lowStockProducts = await _unitOfWork.Products.FindAsync(p => 
                p.MinStockLevel > 0 && 
                p.Inventories.Sum(i => i.QuantityOnHand) < p.MinStockLevel);

            return lowStockProducts.Select(p => new 
            { 
                p.Id, 
                p.Name, 
                p.SkuCode, 
                CurrentStock = p.Inventories.Sum(i => i.QuantityOnHand), 
                p.MinStockLevel 
            });
        }

        public async Task<object> GetCategoryDistributionAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var products = await _unitOfWork.Products.GetAllAsync();

            var distribution = categories.Select(c => new
            {
                Name = c.Name,
                Value = products.Count(p => p.CategoryId == c.Id)
            }).Where(d => d.Value > 0).ToList();

            return distribution;
        }
    }
}
