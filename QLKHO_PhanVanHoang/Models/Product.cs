using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Sản phẩm
    public class Product : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string SkuCode { get; set; } = string.Empty;
        // SKU = Stock Keeping Unit - Mã hàng hóa, ví dụ: "BIA001"

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;  // Tên sản phẩm

        [Required]
        public int CategoryId { get; set; }  // Khóa ngoại, tham chiếu đến bảng Category

        [Required]
        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;  // Đơn vị tính

        public int MinStockLevel { get; set; } = 0;  // Ngưỡng tồn tối thiểu

        public bool IsLotManaged { get; set; } = true;  // Có quản lý theo lô/hạn không?

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CostPrice { get; set; }  // Giá vốn

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SellingPrice { get; set; }  // Giá bán

        [MaxLength(1000)]
        public string? Description { get; set; }  // Mô tả

        [MaxLength(500)]
        public string? ImageUrl { get; set; }  // Đường dẫn ảnh

        // ===== NAVIGATION PROPERTIES (QUAN HỆ) =====

        // 1 sản phẩm thuộc 1 danh mục
        public virtual Category? Category { get; set; }

        // 1 sản phẩm có nhiều lô hàng trong kho
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

        // 1 sản phẩm có nhiều dòng trong phiếu nhập
        public virtual ICollection<ReceivingVoucherDetail> ReceivingVoucherDetails { get; set; } = new List<ReceivingVoucherDetail>();

        // 1 sản phẩm có nhiều dòng trong phiếu kiểm
        public virtual ICollection<CountingSheetDetail> CountingSheetDetails { get; set; } = new List<CountingSheetDetail>();

        // 1 sản phẩm có nhiều lịch sử điều chỉnh
        public virtual ICollection<InventoryAdjustment> InventoryAdjustments { get; set; } = new List<InventoryAdjustment>();
    }
}