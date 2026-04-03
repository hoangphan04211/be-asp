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
    public class CountingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICountingService _countingService;
        private readonly IMapper _mapper;

        public CountingController(IUnitOfWork unitOfWork, ICountingService countingService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _countingService = countingService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams @params)
        {
            var result = await _unitOfWork.CountingSheets.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                s => string.IsNullOrEmpty(@params.SearchTerm) || s.Code.Contains(@params.SearchTerm),
                null,
                "Warehouse");

            var dtos = _mapper.Map<IEnumerable<CountingSheetDto>>(result.Items);

            return Ok(ApiResponse<PagedResult<CountingSheetDto>>.SuccessResult(new PagedResult<CountingSheetDto>
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
            var result = await _unitOfWork.CountingSheets.GetPagedAsync(1, 1, s => s.Id == id, null, "Warehouse,Details.Product");
            var item = result.Items.FirstOrDefault();
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy phiếu kiểm."));
            return Ok(ApiResponse<CountingSheet>.SuccessResult(item));
        }

        [Authorize(Roles = "Admin,WarehouseManager,Employee")]
        [HttpPost("draft")]
        public async Task<IActionResult> CreateDraft(CreateCountingSheetDto dto)
        {
            var sheet = _mapper.Map<CountingSheet>(dto);
            sheet.Status = "Draft";

            await _unitOfWork.CountingSheets.AddAsync(sheet);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<object>.SuccessResult(new { SheetId = sheet.Id }, "Created draft counting sheet successfully"));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            await _countingService.ApproveCountingSheetAsync(id);
            return Ok(ApiResponse<object>.SuccessResult(null, "Counting sheet approved and inventory reconciled"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _unitOfWork.CountingSheets.GetPagedAsync(1, 1, s => s.Id == id);
            var item = result.Items.FirstOrDefault();
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy phiếu kiểm kê."));
            if (item.Status != "Draft") return BadRequest(ApiResponse<object>.FailureResult("Chỉ có thể xóa phiếu ở trạng thái Nháp."));

            _unitOfWork.CountingSheets.Delete(item);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<object>.SuccessResult(null, "Deleted draft counting sheet successfully"));
        }
    }
}
