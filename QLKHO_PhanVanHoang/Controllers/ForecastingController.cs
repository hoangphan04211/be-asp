using QLKHO_PhanVanHoang.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Attributes;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Services;
using System;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ForecastingController : ControllerBase
    {
        private readonly IForecastingService _forecastingService;

        public ForecastingController(IForecastingService forecastingService)
        {
            _forecastingService = forecastingService;
        }

        [HttpGet("predict/{productId}")]
        public IActionResult PredictDemand(int productId, [FromQuery] int horizon = 30)
        {
            try
            {
                var result = _forecastingService.PredictDemand(productId, horizon);
                return Ok(ApiResponse<object>.SuccessResult(result, "Dự báo thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailureResult($"Lỗi dự báo: {ex.Message}"));
            }
        }
    }
}


