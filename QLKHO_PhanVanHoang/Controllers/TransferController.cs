using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Services;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _transferService;
        private readonly IUnitOfWork _unitOfWork;

        public TransferController(ITransferService transferService, IUnitOfWork unitOfWork)
        {
            _transferService = transferService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams @params)
        {
            var result = await _unitOfWork.TransferVouchers.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                v => string.IsNullOrEmpty(@params.SearchTerm) || v.Code.Contains(@params.SearchTerm),
                null,
                "FromWarehouse,ToWarehouse");
            
            return Ok(ApiResponse<PagedResult<TransferVoucher>>.SuccessResult(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _unitOfWork.TransferVouchers.GetPagedAsync(1, 1, v => v.Id == id, null, "FromWarehouse,ToWarehouse,Details.Product");
            var item = result.Items.FirstOrDefault();
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy phiếu chuyển."));
            return Ok(ApiResponse<TransferVoucher>.SuccessResult(item));
        }

        [Authorize(Roles = "Admin,WarehouseManager,Employee")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTransfer([FromBody] QLKHO_PhanVanHoang.DTOs.CreateTransferDto dto)
        {
            await _transferService.CreateTransferVoucherAsync(dto);
            return Ok(ApiResponse<object>.SuccessResult(null, "Created transfer voucher successfully"));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveTransfer(int id)
        {
            await _transferService.ApproveTransferVoucherAsync(id);
            return Ok(ApiResponse<object>.SuccessResult(null, "Transfer approved and inventory updated"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _unitOfWork.TransferVouchers.GetPagedAsync(1, 1, v => v.Id == id);
            var item = result.Items.FirstOrDefault();
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy phiếu chuyển."));
            if (item.Status != "Draft") return BadRequest(ApiResponse<object>.FailureResult("Chỉ có thể xóa phiếu ở trạng thái Nháp."));

            _unitOfWork.TransferVouchers.Delete(item);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<object>.SuccessResult(null, "Deleted draft transfer voucher successfully"));
        }
    }
}
