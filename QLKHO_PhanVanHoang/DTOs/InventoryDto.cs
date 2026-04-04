using System;

namespace QLKHO_PhanVanHoang.DTOs
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? SkuCode { get; set; }
        public int WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public string? LotNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal QuantityOnHand { get; set; }
        public decimal ReservedQuantity { get; set; }
        public string? LocationInWarehouse { get; set; }
        public decimal AvailableQuantity { get; set; }
    }

    public class StockCardDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? SkuCode { get; set; }
        public int WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public string? LotNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string? ReferenceCode { get; set; }
        public decimal BeforeQuantity { get; set; }
        public decimal ChangeQuantity { get; set; }
        public decimal AfterQuantity { get; set; }
        public string? Notes { get; set; }
    }
}
