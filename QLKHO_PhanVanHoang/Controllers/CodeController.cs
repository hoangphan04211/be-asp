using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Services;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CodeController : ControllerBase
    {
        private readonly ICodeGeneratorService _codeGenerator;

        public CodeController(ICodeGeneratorService codeGenerator)
        {
            _codeGenerator = codeGenerator;
        }

        [HttpGet("product")]
        public async Task<IActionResult> GetNextProductCode()
        {
            var code = await _codeGenerator.GenerateProductCodeAsync();
            return Ok(ApiResponse<string>.SuccessResult(code));
        }

        [HttpGet("supplier")]
        public async Task<IActionResult> GetNextSupplierCode()
        {
            var code = await _codeGenerator.GenerateSupplierCodeAsync();
            return Ok(ApiResponse<string>.SuccessResult(code));
        }

        [HttpGet("customer")]
        public async Task<IActionResult> GetNextCustomerCode()
        {
            var code = await _codeGenerator.GenerateCustomerCodeAsync();
            return Ok(ApiResponse<string>.SuccessResult(code));
        }

        [HttpGet("inbound")]
        public async Task<IActionResult> GetNextInboundCode()
        {
            var code = await _codeGenerator.GenerateReceivingCodeAsync();
            return Ok(ApiResponse<string>.SuccessResult(code));
        }

        [HttpGet("outbound")]
        public async Task<IActionResult> GetNextOutboundCode()
        {
            var code = await _codeGenerator.GenerateDeliveryCodeAsync();
            return Ok(ApiResponse<string>.SuccessResult(code));
        }

        [HttpGet("transfer")]
        public async Task<IActionResult> GetNextTransferCode()
        {
            var code = await _codeGenerator.GenerateTransferCodeAsync();
            return Ok(ApiResponse<string>.SuccessResult(code));
        }

        [HttpGet("counting")]
        public async Task<IActionResult> GetNextCountingCode()
        {
            var code = await _codeGenerator.GenerateCountingCodeAsync();
            return Ok(ApiResponse<string>.SuccessResult(code));
        }
    }
}
