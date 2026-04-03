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
        public int SystemQuantity { get; set; }
        public int? ActualQuantity { get; set; }
        public string? Note { get; set; }
    }

    public class CountingSheetDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CountingDate { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
    }
}
