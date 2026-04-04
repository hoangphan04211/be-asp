using System.Collections.Generic;

namespace QLKHO_PhanVanHoang.DTOs
{
    public class TransferDetailDto
    {
        public int ProductId { get; set; }
        public string? LotNumber { get; set; }
        public decimal Quantity { get; set; }
    }

    public class CreateTransferDto
    {
        public int FromWarehouseId { get; set; }
        public int ToWarehouseId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public List<TransferDetailDto> Details { get; set; } = new List<TransferDetailDto>();
    }
}
