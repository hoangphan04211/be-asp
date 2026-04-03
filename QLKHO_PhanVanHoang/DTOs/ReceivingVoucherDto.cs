using System.Collections.Generic;

namespace QLKHO_PhanVanHoang.DTOs
{
    public class CreateReceivingVoucherDto
    {
        public string Code { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public int? SupplierId { get; set; }
        public string? Notes { get; set; }
        public List<CreateReceivingVoucherDetailDto> Details { get; set; } = new List<CreateReceivingVoucherDetailDto>();
    }

    public class CreateReceivingVoucherDetailDto
    {
        public int ProductId { get; set; }
        public string? LotNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
    }
}
