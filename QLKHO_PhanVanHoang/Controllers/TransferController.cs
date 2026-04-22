using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Services;
using QLKHO_PhanVanHoang.DTOs;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _transferService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransferController(ITransferService transferService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _transferService = transferService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams @params)
        {
            var result = await _unitOfWork.TransferVouchers.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                v => string.IsNullOrEmpty(@params.SearchTerm) || v.Code.Contains(@params.SearchTerm),
                q => q.OrderByDescending(v => v.CreatedAt),
                "FromWarehouse,ToWarehouse");
            
            var dtos = _mapper.Map<IEnumerable<TransferVoucherDto>>(result.Items).ToList();

            // Resolve FullNames
            var usernames = dtos.Select(d => d.CreatedBy).Distinct().ToList();
            var userMap = await _unitOfWork.Context.SystemUsers
                .Where(u => usernames.Contains(u.Username))
                .ToDictionaryAsync(u => u.Username, u => u.FullName);

            foreach (var dto in dtos)
            {
                if (userMap.TryGetValue(dto.CreatedBy, out var fullName))
                {
                    dto.CreatedByName = fullName;
                }
            }

            return Ok(ApiResponse<PagedResult<TransferVoucherDto>>.SuccessResult(new PagedResult<TransferVoucherDto>
            {
                Items = dtos,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _unitOfWork.TransferVouchers.GetPagedAsync(1, 1, v => v.Id == id, null, "FromWarehouse,ToWarehouse,Details.Product");
            var item = result.Items.FirstOrDefault();
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy phiếu chuyển."));
            
            var dto = _mapper.Map<TransferVoucherDto>(item);

            // Resolve FullName
            var user = await _unitOfWork.Context.SystemUsers.FirstOrDefaultAsync(u => u.Username == dto.CreatedBy);
            if (user != null) dto.CreatedByName = user.FullName;

            return Ok(ApiResponse<TransferVoucherDto>.SuccessResult(dto));
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
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await _transferService.ApproveTransferVoucherAsync(id);
                return Ok(ApiResponse<object>.SuccessResult(null, "Transfer voucher approved and stock updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailureResult($"Lỗi khi duyệt phiếu chuyển: {ex.InnerException?.Message ?? ex.Message}"));
            }
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
