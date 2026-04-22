using System;
using System.Collections.Generic;

namespace QLKHO_PhanVanHoang.DTOs
{
    public class TransferVoucherDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int FromWarehouseId { get; set; }
        public string FromWarehouseName { get; set; } = string.Empty;
        public int ToWarehouseId { get; set; }
        public string ToWarehouseName { get; set; } = string.Empty;
        public DateTime TransferDate { get; set; }
        public string? Notes { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<TransferVoucherDetailDto> Details { get; set; } = new List<TransferVoucherDetailDto>();
    }

    public class TransferVoucherDetailDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string? LotNumber { get; set; }
        public decimal Quantity { get; set; }
    }
}
