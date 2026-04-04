using System;

namespace QLKHO_PhanVanHoang.DTOs
{
    public class TrashItemDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // Product, Supplier, Customer, Warehouse, Category
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public DateTime DeletedAt { get; set; } // We can use UpdatedAt from BaseEntity as soft-delete mark
        public string? DeletedBy { get; set; }
    }
}
