using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Services;
using AutoMapper;
using QLKHO_PhanVanHoang.DTOs;
using QLKHO_PhanVanHoang.Repositories;
using System.Linq;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;

        public InventoryController(IUnitOfWork unitOfWork, IMapper mapper, IExcelService excelService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _excelService = excelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventory([FromQuery] int? productId, [FromQuery] int? warehouseId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 15)
        {
            var result = await _unitOfWork.Inventories.GetPagedAsync(
                pageNumber,
                pageSize,
                i => (!productId.HasValue || i.ProductId == productId) && (!warehouseId.HasValue || i.WarehouseId == warehouseId),
                q => q.OrderByDescending(i => i.QuantityOnHand),
                "Product,Warehouse"
            );

            var dtos = _mapper.Map<IEnumerable<InventoryDto>>(result.Items);

            return Ok(ApiResponse<PagedResult<InventoryDto>>.SuccessResult(new PagedResult<InventoryDto>
            {
                Items = dtos,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            }));
        }

        [HttpGet("stock-cards")]
        public async Task<IActionResult> GetStockCards([FromQuery] int? productId, [FromQuery] int? warehouseId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 15)
        {
            // Mặc định 30 ngày nếu không truyền
            if (!fromDate.HasValue) fromDate = DateTime.Now.AddDays(-30);
            if (!toDate.HasValue) toDate = DateTime.Now;

            var result = await _unitOfWork.StockCards.GetPagedAsync(
                pageNumber,
                pageSize,
                sc => (!productId.HasValue || sc.ProductId == productId) && 
                      (!warehouseId.HasValue || sc.WarehouseId == warehouseId) &&
                      (sc.TransactionDate.Date >= fromDate.Value.Date && sc.TransactionDate.Date <= toDate.Value.Date),
                q => q.OrderByDescending(sc => sc.TransactionDate),
                "Product,Warehouse"
            );

            var dtos = _mapper.Map<IEnumerable<StockCardDto>>(result.Items);

            return Ok(ApiResponse<PagedResult<StockCardDto>>.SuccessResult(new PagedResult<StockCardDto>
            {
                Items = dtos,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            }));
        }

        [HttpGet("stock-check")]
        public async Task<IActionResult> GetProductStock([FromQuery] int productId, [FromQuery] int warehouseId)
        {
            var inventories = await _unitOfWork.Inventories.FindAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId);
            var total = inventories.Sum(i => i.QuantityOnHand);
            return Ok(ApiResponse<decimal>.SuccessResult(total));
        }

        [HttpPost("import")]
        [Authorize(Roles = "Admin,WarehouseManager")]
        public async Task<IActionResult> ImportInventory(IFormFile file)
        {
            var result = await _excelService.ImportInventoryAsync(file);
            if (result.Errors.Count > 0 && result.SuccessCount == 0)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Nhập tồn kho thất bại", result.Errors));
            }

            return Ok(ApiResponse<object>.SuccessResult(new { result.SuccessCount, result.Errors }, 
                $"Đã nhập thành công {result.SuccessCount} dòng tồn kho."));
        }
    }
}
