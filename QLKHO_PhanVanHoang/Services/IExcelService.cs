using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using QLKHO_PhanVanHoang.Models;

namespace QLKHO_PhanVanHoang.Services
{
    public interface IExcelService
    {
        Task<byte[]> ExportInventoryReportAsync();
        Task<(int SuccessCount, List<string> Errors)> ImportProductsAsync(IFormFile file);
        Task<(int SuccessCount, List<string> Errors)> ImportInventoryAsync(IFormFile file);
    }
}
