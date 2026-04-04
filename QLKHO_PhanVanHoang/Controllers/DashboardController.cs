using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Services;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _dashboardService.GetSummaryAsync();
            return Ok(ApiResponse<object>.SuccessResult(summary));
        }

        [HttpGet("trend")]
        public async Task<IActionResult> GetTrend([FromQuery] int days = 7)
        {
            var trend = await _dashboardService.GetTrendDataAsync(days);
            return Ok(ApiResponse<object>.SuccessResult(trend));
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var distribution = await _dashboardService.GetCategoryDistributionAsync();
            return Ok(ApiResponse<object>.SuccessResult(distribution));
        }

        [HttpGet("alerts")]
        public async Task<IActionResult> GetAlerts()
        {
            var lowStock = await _dashboardService.GetLowStockAlertsAsync();
            return Ok(ApiResponse<object>.SuccessResult(new { LowStock = lowStock }));
        }
    }
}
