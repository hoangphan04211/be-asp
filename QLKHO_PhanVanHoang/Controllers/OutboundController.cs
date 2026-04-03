using System;
using System.Collections.Generic;
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

        public OutboundController(IUnitOfWork unitOfWork, IOutboundService outboundService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _outboundService = outboundService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams @params)
        {
            var result = await _unitOfWork.DeliveryVouchers.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                v => string.IsNullOrEmpty(@params.SearchTerm) || v.Code.Contains(@params.SearchTerm),
                null,
                "Warehouse,Customer");

            var dtos = _mapper.Map<IEnumerable<DeliveryVoucherDto>>(result.Items);

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
            return Ok(ApiResponse<DeliveryVoucher>.SuccessResult(item));
        }

        [Authorize(Roles = "Admin,WarehouseManager,Employee")]
        [HttpPost("draft")]
        public async Task<IActionResult> CreateDraft(CreateDeliveryVoucherDto dto)
        {
            var voucher = _mapper.Map<DeliveryVoucher>(dto);
            voucher.Status = "Draft";

            await _unitOfWork.DeliveryVouchers.AddAsync(voucher);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<object>.SuccessResult(new { VoucherId = voucher.Id }, "Created draft delivery voucher successfully"));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            await _outboundService.ApproveDeliveryVoucherAsync(id);
            return Ok(ApiResponse<object>.SuccessResult(null, "Delivery voucher approved and stock updated"));
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
