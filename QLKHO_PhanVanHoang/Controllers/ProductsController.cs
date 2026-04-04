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
using QLKHO_PhanVanHoang.Services;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize(Roles = "Admin,WarehouseManager,Staff")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly ICodeGeneratorService _codeGenerator;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService, ICodeGeneratorService codeGenerator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
            _codeGenerator = codeGenerator;
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(ApiResponse<object>.FailureResult("File không hợp lệ."));
            
            try 
            {
                var url = await _fileService.UploadImageAsync(file);
                return Ok(ApiResponse<string>.SuccessResult(url, "Tải ảnh lên thành công."));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailureResult($"Lỗi upload: {ex.Message}"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams @params)
        {
            var result = await _unitOfWork.Products.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                p => string.IsNullOrEmpty(@params.SearchTerm) || p.Name.Contains(@params.SearchTerm) || p.SkuCode.Contains(@params.SearchTerm),
                q => q.OrderByDescending(p => p.CreatedAt),
                "Category");

            var dtos = _mapper.Map<IEnumerable<ProductDto>>(result.Items);

            return Ok(ApiResponse<PagedResult<ProductDto>>.SuccessResult(new PagedResult<ProductDto>
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
            var product = await _unitOfWork.Products.GetPagedAsync(1, 1, p => p.Id == id, null, "Category,Inventories");
            var item = product.Items.FirstOrDefault();
            
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy sản phẩm."));

            return Ok(ApiResponse<ProductDto>.SuccessResult(_mapper.Map<ProductDto>(item)));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            
            if (string.IsNullOrEmpty(product.SkuCode))
            {
                product.SkuCode = await _codeGenerator.GenerateProductCodeAsync();
            }

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<ProductDto>.SuccessResult(_mapper.Map<ProductDto>(product), "Created product successfully"));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateProductDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy sản phẩm."));

            // Nếu đổi ảnh mới, xóa ảnh cũ trên Cloudinary để nhẹ storage (Tùy chọn cho doanh nghiệp)
            if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl != dto.ImageUrl)
            {
                await _fileService.DeleteImageAsync(product.ImageUrl);
            }

            _mapper.Map(dto, product);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<ProductDto>.SuccessResult(_mapper.Map<ProductDto>(product), "Updated product successfully"));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy sản phẩm."));

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<object>.SuccessResult(null, "Product soft-deleted successfully"));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _unitOfWork.Products.GetPagedWithDeletedAsync(1, 1, p => p.Id == id);
            var product = result.Items.FirstOrDefault();
            
            if (product == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy sản phẩm để khôi phục."));

            _unitOfWork.Products.Restore(product);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<object>.SuccessResult(null, "Product restored successfully"));
        }
    }
}
