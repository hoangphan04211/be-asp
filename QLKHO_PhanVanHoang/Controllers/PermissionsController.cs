using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PermissionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await _unitOfWork.Permissions.GetAllAsync();
            
            // Group permissions by their Group property
            var grouped = permissions
                .GroupBy(p => p.Group)
                .Select(g => new
                {
                    GroupName = g.Key,
                    Permissions = g.Select(p => new
                    {
                        p.Id,
                        p.Code,
                        p.Name,
                        p.Description
                    })
                });

            return Ok(ApiResponse<object>.SuccessResult(grouped));
        }

        [HttpGet("my-permissions")]
        public async Task<IActionResult> GetMyPermissions()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var users = await _unitOfWork.SystemUsers.FindAsync(u => u.Username == username, "Role.Permissions");
            var user = users.FirstOrDefault();
            
            if (user?.Role == null) return Ok(ApiResponse<IEnumerable<string>>.SuccessResult(new List<string>()));

            var permissionCodes = user.Role.Permissions?.Select(p => p.Code) ?? new List<string>();
            return Ok(ApiResponse<IEnumerable<string>>.SuccessResult(permissionCodes));
        }
    }
}
