using QLKHO_PhanVanHoang.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Attributes;
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

        [Authorize(Roles = AppRoles.AdminOrManager)]
        [HttpGet("export-inventory")]
        public async Task<IActionResult> ExportInventory()
        {
            var fileBytes = await _excelService.ExportInventoryReportAsync();
            
            // Trả về file định dạng Excel chuẩn
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCaoTonKho.xlsx");
        }

        [Authorize(Roles = AppRoles.AdminOrManager)]
        [HttpPost("import-products")]
        public async Task<IActionResult> ImportProducts(IFormFile file)
        {
            var (successCount, errors) = await _excelService.ImportProductsAsync(file);
            if (errors.Count > 0 && successCount == 0)
                return BadRequest(ApiResponse<List<string>>.FailureResult("Import lỗi hoàn toàn.", errors));

            var result = new { SuccessCount = successCount, Errors = errors.Count > 0 ? errors : null };
            return Ok(ApiResponse<object>.SuccessResult(result, $"Import sản phẩm hoàn tất. Thành công: {successCount} dòng."));
        }

        [Authorize(Roles = AppRoles.AdminOrManager)]
        [HttpGet("download-template")]
        public async Task<IActionResult> DownloadTemplate()
        {
            var fileBytes = await _excelService.GetInventoryTemplateAsync();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Mau_Nhap_Ton_Kho.xlsx");
        }

        [Authorize(Roles = AppRoles.AdminOrManager)]
        [HttpPost("import-inventory")]
        public async Task<IActionResult> ImportInventory(IFormFile file)
        {
            var (successCount, errors) = await _excelService.ImportInventoryAsync(file);
            if (errors.Count > 0 && successCount == 0)
                return BadRequest(ApiResponse<List<string>>.FailureResult("Import tồn kho lỗi hoàn toàn.", errors));

            var result = new { SuccessCount = successCount, Errors = errors.Count > 0 ? errors : null };
            return Ok(ApiResponse<object>.SuccessResult(result, $"Import tồn kho hoàn tất. Thành công: {successCount} dòng."));
        }
    }
}


