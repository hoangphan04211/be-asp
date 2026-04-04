using System.Collections.Generic;
using System.Threading.Tasks;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Models;

namespace QLKHO_PhanVanHoang.Services
{
    public interface IAuditService
    {
        Task<PagedResult<AuditLogDto>> GetPagedLogsAsync(AuditLogParams @params);
    }

    public class AuditLogDto
    {
        public int Id { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string FriendlyDescription { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
    }
}
