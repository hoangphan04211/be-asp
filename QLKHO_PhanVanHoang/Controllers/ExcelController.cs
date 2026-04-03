using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Services;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelController : ControllerBase
    {
        private readonly IExcelService _excelService;

        public ExcelController(IExcelService excelService)
        {
            _excelService = excelService;
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpGet("export-inventory")]
        public async Task<IActionResult> ExportInventory()
        {
            var fileBytes = await _excelService.ExportInventoryReportAsync();
            
            // Trả về file định dạng Excel chuẩn
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCaoTonKho.xlsx");
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost("import-products")]
        public async Task<IActionResult> ImportProducts(IFormFile file)
        {
            var (successCount, errors) = await _excelService.ImportProductsAsync(file);
            
            if (errors.Count > 0 && successCount == 0)
            {
                return BadRequest(ApiResponse<List<string>>.FailureResult("Import lỗi hoàn toàn.", errors));
            }

            var result = new 
            { 
                Message = $"Import thành công {successCount} dòng.", 
                Errors = errors.Count > 0 ? errors : null 
            };

            return Ok(ApiResponse<object>.SuccessResult(result, "Xử lý import hoàn tất."));
        }
    }
}
