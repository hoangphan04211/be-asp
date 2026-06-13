using QLKHO_PhanVanHoang.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Attributes;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;
using QLKHO_PhanVanHoang.Services;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [HasPermission("SYSTEM_LOGS")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] AuditLogParams @params)
        {
            var result = await _auditService.GetPagedLogsAsync(@params);
            return Ok(ApiResponse<PagedResult<AuditLogDto>>.SuccessResult(result));
        }
    }
}


