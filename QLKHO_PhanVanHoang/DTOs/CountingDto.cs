using System;
using System.Collections.Generic;

namespace QLKHO_PhanVanHoang.DTOs
{
    public class CreateCountingSheetDto
    {
        public string Code { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string? Notes { get; set; }
        public List<CreateCountingSheetDetailDto> Details { get; set; } = new List<CreateCountingSheetDetailDto>();
    }

    public class CreateCountingSheetDetailDto
    {
        public int ProductId { get; set; }
        public string? LotNumber { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public string? Note { get; set; }
    }

    public class CountingSheetDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CountingDate { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public List<CountingSheetDetailDto> Details { get; set; } = new List<CountingSheetDetailDto>();
    }

    public class CountingSheetDetailDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public decimal SystemQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public string? Note { get; set; }
    }
}
