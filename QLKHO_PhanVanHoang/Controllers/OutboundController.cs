using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.DTOs;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Services;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OutboundController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutboundService _outboundService;
        private readonly IMapper _mapper;
        private readonly ICodeGeneratorService _codeGenerator;

        public OutboundController(IUnitOfWork unitOfWork, IOutboundService outboundService, IMapper mapper, ICodeGeneratorService codeGenerator)
        {
            _unitOfWork = unitOfWork;
            _outboundService = outboundService;
            _mapper = mapper;
            _codeGenerator = codeGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams @params)
        {
            var result = await _unitOfWork.DeliveryVouchers.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                v => string.IsNullOrEmpty(@params.SearchTerm) || v.Code.Contains(@params.SearchTerm),
                null,
                "Warehouse,Customer,Details");

            var dtos = _mapper.Map<IEnumerable<DeliveryVoucherDto>>(result.Items).ToList();

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

            return Ok(ApiResponse<PagedResult<DeliveryVoucherDto>>.SuccessResult(new PagedResult<DeliveryVoucherDto>
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
            var result = await _unitOfWork.DeliveryVouchers.GetPagedAsync(1, 1, v => v.Id == id, null, "Warehouse,Customer,Details.Product");
            var item = result.Items.FirstOrDefault();
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy phiếu xuất."));
            
            var dto = _mapper.Map<DeliveryVoucherDto>(item);
            
            // Resolve FullName
            var user = await _unitOfWork.Context.SystemUsers.FirstOrDefaultAsync(u => u.Username == dto.CreatedBy);
            if (user != null) dto.CreatedByName = user.FullName;

            return Ok(ApiResponse<DeliveryVoucherDto>.SuccessResult(dto));
        }

        [Authorize(Roles = "Admin,WarehouseManager,Employee")]
        [HttpPost("draft")]
        public async Task<IActionResult> CreateDraft(CreateDeliveryVoucherDto dto)
        {
            try
            {
                var voucher = _mapper.Map<DeliveryVoucher>(dto);
                voucher.Status = "Draft";

                if (voucher.DeliveryDate == default)
                {
                    voucher.DeliveryDate = DateTime.Now;
                }

                if (string.IsNullOrEmpty(voucher.Code))
                {
                    voucher.Code = await _codeGenerator.GenerateDeliveryCodeAsync();
                }

                await _unitOfWork.DeliveryVouchers.AddAsync(voucher);
                await _unitOfWork.CompleteAsync();

                return Ok(ApiResponse<object>.SuccessResult(new { VoucherId = voucher.Id, Code = voucher.Code }, "Created draft delivery voucher successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailureResult($"Lỗi khi lưu phiếu nháp: {ex.InnerException?.Message ?? ex.Message}"));
            }
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await _outboundService.ApproveDeliveryVoucherAsync(id);
                return Ok(ApiResponse<object>.SuccessResult(null, "Delivery voucher approved and stock updated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailureResult($"Lỗi khi duyệt phiếu xuất: {ex.InnerException?.Message ?? ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _unitOfWork.DeliveryVouchers.GetPagedAsync(1, 1, v => v.Id == id);
            var item = result.Items.FirstOrDefault();
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy phiếu xuất."));
            if (item.Status != "Draft") return BadRequest(ApiResponse<object>.FailureResult("Chỉ có thể xóa phiếu ở trạng thái Nháp."));

            _unitOfWork.DeliveryVouchers.Delete(item);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<object>.SuccessResult(null, "Deleted draft outbound voucher successfully"));
        }
    }
}
