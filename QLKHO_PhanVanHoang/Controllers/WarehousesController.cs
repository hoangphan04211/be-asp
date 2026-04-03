using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.DTOs;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WarehousesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WarehousesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams @params)
        {
            var result = await _unitOfWork.Warehouses.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                w => string.IsNullOrEmpty(@params.SearchTerm) || w.Name.Contains(@params.SearchTerm));

            var dtos = _mapper.Map<IEnumerable<WarehouseDto>>(result.Items);
            
            return Ok(ApiResponse<PagedResult<WarehouseDto>>.SuccessResult(new PagedResult<WarehouseDto>
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
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);
            if (warehouse == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy kho."));

            return Ok(ApiResponse<WarehouseDto>.SuccessResult(_mapper.Map<WarehouseDto>(warehouse)));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateWarehouseDto dto)
        {
            var warehouse = _mapper.Map<Warehouse>(dto);
            await _unitOfWork.Warehouses.AddAsync(warehouse);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<WarehouseDto>.SuccessResult(_mapper.Map<WarehouseDto>(warehouse), "Created warehouse successfully"));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateWarehouseDto dto)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);
            if (warehouse == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy kho."));

            _mapper.Map(dto, warehouse);
            _unitOfWork.Warehouses.Update(warehouse);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<WarehouseDto>.SuccessResult(_mapper.Map<WarehouseDto>(warehouse), "Updated warehouse successfully"));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);
            if (warehouse == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy kho."));

            _unitOfWork.Warehouses.Delete(warehouse);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<object>.SuccessResult(null, "Deleted warehouse successfully"));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _unitOfWork.Warehouses.GetPagedWithDeletedAsync(1, 1, w => w.Id == id);
            var item = result.Items.FirstOrDefault();
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy kho để khôi phục."));

            _unitOfWork.Warehouses.Restore(item);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<object>.SuccessResult(null, "Restored warehouse successfully"));
        }
    }
}
