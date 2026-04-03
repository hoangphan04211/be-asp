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
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Lấy danh sách người dùng (phân trang)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams @params)
        {
            var result = await _unitOfWork.SystemUsers.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                u => string.IsNullOrEmpty(@params.SearchTerm) || 
                     u.Username.Contains(@params.SearchTerm) || 
                     u.FullName.Contains(@params.SearchTerm),
                null,
                "Role");

            var users = result.Items.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                RoleId = u.RoleId,
                RoleName = u.Role?.Name ?? "N/A",
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            });

            return Ok(ApiResponse<PagedResult<UserDto>>.SuccessResult(new PagedResult<UserDto>
            {
                Items = users,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            }));
        }

        /// <summary>
        /// Lấy chi tiết người dùng
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _unitOfWork.SystemUsers.GetPagedAsync(1, 1, u => u.Id == id, null, "Role");
            var user = result.Items.FirstOrDefault();
            if (user == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy người dùng."));

            return Ok(ApiResponse<UserDto>.SuccessResult(new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? "N/A",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }));
        }

        /// <summary>
        /// Tạo tài khoản nhân viên mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegisterRequestDto dto)
        {
            // Kiểm tra username đã tồn tại chưa
            var existing = await _unitOfWork.SystemUsers.FindAsync(u => u.Username == dto.Username);
            if (existing.Any())
                return BadRequest(ApiResponse<object>.FailureResult("Tên đăng nhập đã tồn tại."));

            var user = new SystemUser
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                Email = dto.Email,
                RoleId = dto.RoleId,
                IsActive = true
            };

            await _unitOfWork.SystemUsers.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<object>.SuccessResult(new { user.Id, user.Username }, "Tạo tài khoản thành công."));
        }

        /// <summary>
        /// Cập nhật thông tin người dùng (tên, email, role, trạng thái)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _unitOfWork.SystemUsers.GetByIdAsync(id);
            if (user == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy người dùng."));

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.RoleId = dto.RoleId;
            user.IsActive = dto.IsActive;

            _unitOfWork.SystemUsers.Update(user);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<object>.SuccessResult(null, "Cập nhật thông tin thành công."));
        }

        /// <summary>
        /// Admin reset mật khẩu cho nhân viên
        /// </summary>
        [HttpPost("reset-password/{id}")]
        public async Task<IActionResult> AdminResetPassword(int id, [FromBody] AdminResetPasswordDto dto)
        {
            var user = await _unitOfWork.SystemUsers.GetByIdAsync(id);
            if (user == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy người dùng."));

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            _unitOfWork.SystemUsers.Update(user);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<object>.SuccessResult(null, "Đặt lại mật khẩu thành công."));
        }

        /// <summary>
        /// Xóa mềm tài khoản (Soft Delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _unitOfWork.SystemUsers.GetByIdAsync(id);
            if (user == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy người dùng."));

            _unitOfWork.SystemUsers.Delete(user);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<object>.SuccessResult(null, "Xóa tài khoản thành công."));
        }

        /// <summary>
        /// Lấy danh sách Roles
        /// </summary>
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _unitOfWork.Roles.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<Role>>.SuccessResult(roles));
        }
    }
}
