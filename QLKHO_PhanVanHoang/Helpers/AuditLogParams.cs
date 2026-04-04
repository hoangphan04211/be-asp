namespace QLKHO_PhanVanHoang.Helpers
{
    public class AuditLogParams : PaginationParams
    {
        public string? EntityName { get; set; }
        public string? Action { get; set; }
    }
}
