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
        public System.DateTime? ExpiryDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
    }

    public class ReceivingVoucherDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public System.DateTime ReceivingDate { get; set; }
        public string? Notes { get; set; }
        public decimal TotalAmount { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public List<ReceivingVoucherDetailDto> Details { get; set; } = new List<ReceivingVoucherDetailDto>();
    }

    public class ReceivingVoucherDetailDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string? LotNumber { get; set; }
        public System.DateTime? ExpiryDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
    }
}
