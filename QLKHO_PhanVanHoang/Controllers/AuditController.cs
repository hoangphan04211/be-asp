using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuditController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams @params, string? entityName, string? action)
        {
            var query = _unitOfWork.AuditLogs.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                a => (string.IsNullOrEmpty(entityName) || a.EntityName == entityName) &&
                     (string.IsNullOrEmpty(action) || a.Action == action),
                q => q.OrderByDescending(a => a.ChangedAt));

            var result = await query;
            return Ok(ApiResponse<PagedResult<AuditLog>>.SuccessResult(result));
        }
    }
}
