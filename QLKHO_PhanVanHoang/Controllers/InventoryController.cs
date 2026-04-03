using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("stock-cards")]
        public async Task<IActionResult> GetStockCards([FromQuery] PaginationParams @params, int? productId, int? warehouseId)
        {
            var result = await _unitOfWork.StockCards.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                s => (!productId.HasValue || s.ProductId == productId) && (!warehouseId.HasValue || s.WarehouseId == warehouseId),
                q => q.OrderByDescending(s => s.CreatedAt),
                "Product,Warehouse");
            
            return Ok(ApiResponse<PagedResult<StockCard>>.SuccessResult(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetInventory([FromQuery] PaginationParams @params, int? productId, int? warehouseId)
        {
            var result = await _unitOfWork.Inventories.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                i => (!productId.HasValue || i.ProductId == productId) && (!warehouseId.HasValue || i.WarehouseId == warehouseId),
                q => q.OrderBy(i => i.ProductId),
                "Product,Warehouse");

            return Ok(ApiResponse<PagedResult<Inventory>>.SuccessResult(result));
        }
    }
}
