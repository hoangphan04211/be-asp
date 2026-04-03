namespace QLKHO_PhanVanHoang.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string SkuCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal? CostPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int MinStockLevel { get; set; }
        public bool IsLotManaged { get; set; }
    }

    public class CreateProductDto
    {
        public string SkuCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal? CostPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int MinStockLevel { get; set; }
        public bool IsLotManaged { get; set; }
    }
}
