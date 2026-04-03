using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var inventories = await _unitOfWork.Inventories.GetAllAsync();
            var warehouses = await _unitOfWork.Warehouses.GetAllAsync();

            var summary = new
            {
                TotalProducts = products.Count(),
                TotalWarehouses = warehouses.Count(),
                TotalStockValue = inventories.Sum(i => i.QuantityOnHand * (i.Product?.CostPrice ?? 0)),
                InventoryItems = inventories.Count()
            };

            return Ok(ApiResponse<object>.SuccessResult(summary));
        }

        [HttpGet("alerts")]
        public async Task<IActionResult> GetAlerts()
        {
            // Low stock alerts
            var lowStockProducts = await _unitOfWork.Products.FindAsync(p => 
                p.MinStockLevel > 0 && 
                p.Inventories.Sum(i => i.QuantityOnHand) < p.MinStockLevel);

            return Ok(ApiResponse<object>.SuccessResult(new
            {
                LowStock = lowStockProducts.Select(p => new { p.Id, p.Name, p.SkuCode, CurrentStock = p.Inventories.Sum(i => i.QuantityOnHand), p.MinStockLevel })
            }));
        }
    }
}
