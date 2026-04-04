using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Services;
using QLKHO_PhanVanHoang.DTOs;
using QLKHO_PhanVanHoang.Helpers;
using System.Collections.Generic;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InboundController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInboundService _inboundService;
        private readonly IMapper _mapper;
        private readonly ICodeGeneratorService _codeGenerator;

        public InboundController(IUnitOfWork unitOfWork, IInboundService inboundService, IMapper mapper, ICodeGeneratorService codeGenerator)
        {
            _unitOfWork = unitOfWork;
            _inboundService = inboundService;
            _mapper = mapper;
            _codeGenerator = codeGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams @params)
        {
            var result = await _unitOfWork.ReceivingVouchers.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                v => string.IsNullOrEmpty(@params.SearchTerm) || v.Code.Contains(@params.SearchTerm),
                v => v.OrderByDescending(x => x.ReceivingDate),
                "Warehouse,Supplier,Details");

            var dtos = _mapper.Map<IEnumerable<ReceivingVoucherDto>>(result.Items);
            
            return Ok(ApiResponse<PagedResult<ReceivingVoucherDto>>.SuccessResult(new PagedResult<ReceivingVoucherDto>
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
            var res = await _unitOfWork.ReceivingVouchers.GetPagedAsync(1, 1, v => v.Id == id, null, "Warehouse,Supplier,Details.Product");
            var item = res.Items.FirstOrDefault();
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy phiếu nhập."));

            var dto = _mapper.Map<ReceivingVoucherDto>(item);
            return Ok(ApiResponse<ReceivingVoucherDto>.SuccessResult(dto));
        }

        [Authorize(Roles = "Admin,WarehouseManager,Employee")]
        [HttpPost("draft")]
        public async Task<IActionResult> CreateDraftVoucher(CreateReceivingVoucherDto dto)
        {
            var voucher = _mapper.Map<ReceivingVoucher>(dto);
            voucher.Status = "Draft";

            if (string.IsNullOrEmpty(voucher.Code))
            {
                voucher.Code = await _codeGenerator.GenerateReceivingCodeAsync();
            }
            
            await _unitOfWork.ReceivingVouchers.AddAsync(voucher);
            await _unitOfWork.CompleteAsync(); 
            
            return Ok(ApiResponse<object>.SuccessResult(new { VoucherId = voucher.Id, Code = voucher.Code }, "Created draft receiving voucher successfully"));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveVoucher(int id)
        {
            await _inboundService.ApproveReceivingVoucherAsync(id);
            return Ok(ApiResponse<object>.SuccessResult(null, "Voucher approved successfully. Inventory has been updated."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _unitOfWork.ReceivingVouchers.GetPagedAsync(1, 1, v => v.Id == id);
            var item = result.Items.FirstOrDefault();
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy phiếu nhập."));
            if (item.Status != "Draft") return BadRequest(ApiResponse<object>.FailureResult("Chỉ có thể xóa phiếu ở trạng thái Nháp."));

            _unitOfWork.ReceivingVouchers.Delete(item);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<object>.SuccessResult(null, "Deleted draft inbound voucher successfully"));
        }
    }
}
