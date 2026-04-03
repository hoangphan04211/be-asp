using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IUnitOfWork _unitOfWork;

        public FileController(IWebHostEnvironment env, IUnitOfWork unitOfWork)
        {
            _env = env;
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost("upload-product-image/{productId}")]
        public async Task<IActionResult> UploadProductImage(int productId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.FailureResult("File không hợp lệ."));

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
                return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy sản phẩm."));

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{productId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Cập nhật ImageUrl vào DB
            product.ImageUrl = $"/uploads/products/{fileName}";
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<string>.SuccessResult(product.ImageUrl, "Tải ảnh lên thành công."));
        }
    }
}
