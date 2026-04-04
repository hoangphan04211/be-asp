using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.DTOs;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize(Roles = "Admin,WarehouseManager")]
    [ApiController]
    [Route("api/[controller]")]
    public class TrashController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrashController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetDeletedItems()
        {
            var trashItems = new List<TrashItemDto>();

            // 1. Products
            var products = await _unitOfWork.Products.GetPagedWithDeletedAsync(1, 100, p => p.IsDeleted);
            trashItems.AddRange(products.Items.Select(x => new TrashItemDto
            {
                Id = x.Id,
                Type = "Product",
                Name = x.Name,
                Code = x.SkuCode,
                DeletedAt = x.UpdatedAt ?? x.CreatedAt,
                DeletedBy = x.UpdatedBy ?? x.CreatedBy
            }));

            // 2. Suppliers
            var suppliers = await _unitOfWork.Suppliers.GetPagedWithDeletedAsync(1, 100, s => s.IsDeleted);
            trashItems.AddRange(suppliers.Items.Select(x => new TrashItemDto
            {
                Id = x.Id,
                Type = "Supplier",
                Name = x.Name,
                Code = x.Code,
                DeletedAt = x.UpdatedAt ?? x.CreatedAt,
                DeletedBy = x.UpdatedBy ?? x.CreatedBy
            }));

            // 3. Customers
            var customers = await _unitOfWork.Customers.GetPagedWithDeletedAsync(1, 100, c => c.IsDeleted);
            trashItems.AddRange(customers.Items.Select(x => new TrashItemDto
            {
                Id = x.Id,
                Type = "Customer",
                Name = x.Name,
                Code = x.Code,
                DeletedAt = x.UpdatedAt ?? x.CreatedAt,
                DeletedBy = x.UpdatedBy ?? x.CreatedBy
            }));

            // 4. Warehouses
            var warehouses = await _unitOfWork.Warehouses.GetPagedWithDeletedAsync(1, 100, w => w.IsDeleted);
            trashItems.AddRange(warehouses.Items.Select(x => new TrashItemDto
            {
                Id = x.Id,
                Type = "Warehouse",
                Name = x.Name,
                Code = null,
                DeletedAt = x.UpdatedAt ?? x.CreatedAt,
                DeletedBy = x.UpdatedBy ?? x.CreatedBy
            }));

            // 5. Categories
            var categories = await _unitOfWork.Categories.GetPagedWithDeletedAsync(1, 100, c => c.IsDeleted);
            trashItems.AddRange(categories.Items.Select(x => new TrashItemDto
            {
                Id = x.Id,
                Type = "Category",
                Name = x.Name,
                Code = null,
                DeletedAt = x.UpdatedAt ?? x.CreatedAt,
                DeletedBy = x.UpdatedBy ?? x.CreatedBy
            }));

            return Ok(ApiResponse<IEnumerable<TrashItemDto>>.SuccessResult(trashItems.OrderByDescending(t => t.DeletedAt)));
        }

        [HttpPost("restore/{type}/{id}")]
        public async Task<IActionResult> Restore(string type, int id)
        {
            switch (type.ToLower())
            {
                case "product":
                    var prodRes = await _unitOfWork.Products.GetPagedWithDeletedAsync(1, 1, p => p.Id == id);
                    var prod = prodRes.Items.FirstOrDefault();
                    if (prod != null) 
                    {
                        // Luôn khôi phục loại hàng nếu nó đang bị xóa (Theo yêu cầu)
                        var categoryRes = await _unitOfWork.Categories.GetPagedWithDeletedAsync(1, 1, c => c.Id == prod.CategoryId);
                        var category = categoryRes.Items.FirstOrDefault();
                        if (category != null && category.IsDeleted)
                        {
                            _unitOfWork.Categories.Restore(category);
                        }
                        
                        _unitOfWork.Products.Restore(prod);
                    }
                    else return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy sản phẩm."));
                    break;

                case "supplier":
                    var supRes = await _unitOfWork.Suppliers.GetPagedWithDeletedAsync(1, 1, s => s.Id == id);
                    var sup = supRes.Items.FirstOrDefault();
                    if (sup != null) _unitOfWork.Suppliers.Restore(sup);
                    else return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy nhà cung cấp."));
                    break;

                case "customer":
                    var cusRes = await _unitOfWork.Customers.GetPagedWithDeletedAsync(1, 1, s => s.Id == id);
                    var cus = cusRes.Items.FirstOrDefault();
                    if (cus != null) _unitOfWork.Customers.Restore(cus);
                    else return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy khách hàng."));
                    break;

                case "warehouse":
                    var warRes = await _unitOfWork.Warehouses.GetPagedWithDeletedAsync(1, 1, s => s.Id == id);
                    var war = warRes.Items.FirstOrDefault();
                    if (war != null) _unitOfWork.Warehouses.Restore(war);
                    else return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy kho."));
                    break;

                case "category":
                    var catRes = await _unitOfWork.Categories.GetPagedWithDeletedAsync(1, 1, s => s.Id == id);
                    var cat = catRes.Items.FirstOrDefault();
                    if (cat != null) _unitOfWork.Categories.Restore(cat);
                    else return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy loại hàng."));
                    break;

                default:
                    return BadRequest(ApiResponse<object>.FailureResult("Loại thực thể không hợp lệ."));
            }

            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<object>.SuccessResult(null, $"Khôi phục {type} thành công."));
        }

        [HttpDelete("hard-delete/{type}/{id}")]
        [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền xóa vĩnh viễn
        public async Task<IActionResult> DeletePermanently(string type, int id)
        {
            try
            {
                switch (type.ToLower())
                {
                    case "product":
                        var prodRes = await _unitOfWork.Products.GetPagedWithDeletedAsync(1, 1, p => p.Id == id);
                        var prod = prodRes.Items.FirstOrDefault();
                        if (prod != null) _unitOfWork.Products.Delete(prod);
                        else return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy sản phẩm."));
                        break;
                    case "supplier":
                        var supRes = await _unitOfWork.Suppliers.GetPagedWithDeletedAsync(1, 1, s => s.Id == id);
                        var sup = supRes.Items.FirstOrDefault();
                        if (sup != null) _unitOfWork.Suppliers.Delete(sup);
                        else return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy nhà cung cấp."));
                        break;
                    case "customer":
                        var cusRes = await _unitOfWork.Customers.GetPagedWithDeletedAsync(1, 1, s => s.Id == id);
                        var cus = cusRes.Items.FirstOrDefault();
                        if (cus != null) _unitOfWork.Customers.Delete(cus);
                        else return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy khách hàng."));
                        break;
                    case "warehouse":
                        var warRes = await _unitOfWork.Warehouses.GetPagedWithDeletedAsync(1, 1, s => s.Id == id);
                        var war = warRes.Items.FirstOrDefault();
                        if (war != null) _unitOfWork.Warehouses.Delete(war);
                        else return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy kho."));
                        break;
                    case "category":
                        var catRes = await _unitOfWork.Categories.GetPagedWithDeletedAsync(1, 1, s => s.Id == id);
                        var cat = catRes.Items.FirstOrDefault();
                        if (cat != null) _unitOfWork.Categories.Delete(cat);
                        else return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy loại hàng."));
                        break;
                }

                await _unitOfWork.CompleteAsync();
                return Ok(ApiResponse<object>.SuccessResult(null, "Đã xóa vĩnh viễn dữ liệu."));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.FailureResult("Không thể xóa vĩnh viễn do dữ liệu này đang được sử dụng trong các nghiệp vụ vận hành kho!"));
            }
        }
    }
}
