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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams @params)
        {
            var result = await _unitOfWork.Categories.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                c => string.IsNullOrEmpty(@params.SearchTerm) || c.Name.Contains(@params.SearchTerm),
                q => q.OrderByDescending(c => c.CreatedAt));

            var dtos = _mapper.Map<IEnumerable<CategoryDto>>(result.Items);
            
            return Ok(ApiResponse<PagedResult<CategoryDto>>.SuccessResult(new PagedResult<CategoryDto>
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
            var item = await _unitOfWork.Categories.GetByIdAsync(id);
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy danh mục."));
            return Ok(ApiResponse<CategoryDto>.SuccessResult(_mapper.Map<CategoryDto>(item)));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<CategoryDto>.SuccessResult(_mapper.Map<CategoryDto>(category), "Tạo loại hàng thành công"));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateCategoryDto dto)
        {
            var item = await _unitOfWork.Categories.GetByIdAsync(id);
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy loại hàng."));

            _mapper.Map(dto, item);
            _unitOfWork.Categories.Update(item);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<CategoryDto>.SuccessResult(_mapper.Map<CategoryDto>(item), "Cập nhật loại hàng thành công"));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy danh mục."));

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<object>.SuccessResult(null, "Xóa danh mục thành công"));
        }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICodeGeneratorService _codeGenerator;

        public SuppliersController(IUnitOfWork unitOfWork, IMapper mapper, ICodeGeneratorService codeGenerator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _codeGenerator = codeGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams @params)
        {
            var result = await _unitOfWork.Suppliers.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                s => string.IsNullOrEmpty(@params.SearchTerm) || s.Name.Contains(@params.SearchTerm) || (s.Code != null && s.Code.Contains(@params.SearchTerm)),
                q => q.OrderByDescending(s => s.CreatedAt));

            var dtos = _mapper.Map<IEnumerable<SupplierDto>>(result.Items);
            
            return Ok(ApiResponse<PagedResult<SupplierDto>>.SuccessResult(new PagedResult<SupplierDto>
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
            var item = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy nhà cung cấp."));
            return Ok(ApiResponse<SupplierDto>.SuccessResult(_mapper.Map<SupplierDto>(item)));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateSupplierDto dto)
        {
            var supplier = _mapper.Map<Supplier>(dto);

            if (string.IsNullOrEmpty(supplier.Code))
            {
                supplier.Code = await _codeGenerator.GenerateSupplierCodeAsync();
            }

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<SupplierDto>.SuccessResult(_mapper.Map<SupplierDto>(supplier), "Tạo nhà cung cấp thành công"));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateSupplierDto dto)
        {
            var item = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy nhà cung cấp."));

            _mapper.Map(dto, item);
            _unitOfWork.Suppliers.Update(item);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<SupplierDto>.SuccessResult(_mapper.Map<SupplierDto>(item), "Cập nhật nhà cung cấp thành công"));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy nhà cung cấp."));
            _unitOfWork.Suppliers.Delete(item);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<object>.SuccessResult(null, "Xóa nhà cung cấp thành công"));
        }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICodeGeneratorService _codeGenerator;

        public CustomersController(IUnitOfWork unitOfWork, IMapper mapper, ICodeGeneratorService codeGenerator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _codeGenerator = codeGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams @params)
        {
            var result = await _unitOfWork.Customers.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                c => string.IsNullOrEmpty(@params.SearchTerm) || c.Name.Contains(@params.SearchTerm) || (c.Code != null && c.Code.Contains(@params.SearchTerm)),
                q => q.OrderByDescending(c => c.CreatedAt));

            var dtos = _mapper.Map<IEnumerable<CustomerDto>>(result.Items);
            
            return Ok(ApiResponse<PagedResult<CustomerDto>>.SuccessResult(new PagedResult<CustomerDto>
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
            var item = await _unitOfWork.Customers.GetByIdAsync(id);
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy khách hàng."));
            return Ok(ApiResponse<CustomerDto>.SuccessResult(_mapper.Map<CustomerDto>(item)));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerDto dto)
        {
            var customer = _mapper.Map<Customer>(dto);

            if (string.IsNullOrEmpty(customer.Code))
            {
                customer.Code = await _codeGenerator.GenerateCustomerCodeAsync();
            }

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<CustomerDto>.SuccessResult(_mapper.Map<CustomerDto>(customer), "Tạo khách hàng thành công"));
        }

        [Authorize(Roles = "Admin,WarehouseManager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateCustomerDto dto)
        {
            var item = await _unitOfWork.Customers.GetByIdAsync(id);
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy khách hàng."));

            _mapper.Map(dto, item);
            _unitOfWork.Customers.Update(item);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<CustomerDto>.SuccessResult(_mapper.Map<CustomerDto>(item), "Cập nhật khách hàng thành công"));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.Customers.GetByIdAsync(id);
            if (item == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy khách hàng."));
            _unitOfWork.Customers.Delete(item);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<object>.SuccessResult(null, "Xóa khách hàng thành công"));
        }
    }
}
