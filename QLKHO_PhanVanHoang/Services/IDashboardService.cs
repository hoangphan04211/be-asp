using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLKHO_PhanVanHoang.Services
{
    public interface IDashboardService
    {
        Task<object> GetSummaryAsync();
        Task<object> GetTrendDataAsync(int days = 7);
        Task<object> GetLowStockAlertsAsync();
        Task<object> GetCategoryDistributionAsync();
    }
}
