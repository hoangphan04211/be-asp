using System.Collections.Generic;

namespace QLKHO_PhanVanHoang.DTOs
{
    public class CreateDeliveryVoucherDto
    {
        public string Code { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public int? CustomerId { get; set; }
        public string? Notes { get; set; }
        public List<CreateDeliveryVoucherDetailDto> Details { get; set; } = new List<CreateDeliveryVoucherDetailDto>();
    }

    public class CreateDeliveryVoucherDetailDto
    {
        public int ProductId { get; set; }
        public string? LotNumber { get; set; }
        public decimal Quantity { get; set; }
    }

    public class DeliveryVoucherDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public System.DateTime DeliveryDate { get; set; }
    }
}
