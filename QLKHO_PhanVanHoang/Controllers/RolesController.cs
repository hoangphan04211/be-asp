using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.DTOs;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RolesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _unitOfWork.Roles.FindAsync(null, "Permissions");
            var result = roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                PermissionCodes = r.Permissions?.Select(p => p.Code).ToList() ?? new List<string>()
            });

            return Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResult(result));
        }

        [HttpPut("{id}/permissions")]
        public async Task<IActionResult> UpdatePermissions(int id, [FromBody] UpdateRolePermissionsDto dto)
        {
            var roles = await _unitOfWork.Roles.FindAsync(r => r.Id == id, "Permissions");
            var role = roles.FirstOrDefault();
            
            if (role == null) return NotFound(ApiResponse<object>.FailureResult("Không tìm thấy vai trò."));

            // Admin role should always have all permissions (optional check, but good for safety)
            if (role.Name == "Admin")
            {
                // return BadRequest(ApiResponse<object>.FailureResult("Không thể thay đổi quyền của Admin quản trị."));
            }

            // Get all valid permissions from DB
            var allPermissions = await _unitOfWork.Permissions.GetAllAsync();
            var validPermissions = allPermissions.Where(p => dto.PermissionCodes.Contains(p.Code)).ToList();

            // Update relationship
            role.Permissions.Clear();
            foreach (var p in validPermissions)
            {
                role.Permissions.Add(p);
            }

            _unitOfWork.Roles.Update(role);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<object>.SuccessResult(null, "Cập nhật quyền hạn thành công."));
        }
    }
}
