using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Tồn kho chi tiết theo lô
    public class Inventory : BaseEntity
    {
        [Required]
        public int ProductId { get; set; }  // Sản phẩm gì?

        [Required]
        public int WarehouseId { get; set; }  // Ở kho nào?

        [MaxLength(50)]
        public string? LotNumber { get; set; }  // Số lô

        public DateTime? ExpiryDate { get; set; }  // Hạn sử dụng

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityOnHand { get; set; } = 0;  // Số lượng tồn thực tế

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReservedQuantity { get; set; } = 0;  // Số lượng đã đặt trước

        [MaxLength(50)]
        public string? LocationInWarehouse { get; set; }  // Vị trí trong kho

        [NotMapped]
        public decimal AvailableQuantity => QuantityOnHand - ReservedQuantity;  // Số lượng có thể bán

        // ===== NAVIGATION PROPERTIES =====

        // Lô hàng này là sản phẩm gì
        public virtual Product? Product { get; set; }

        // Lô hàng này nằm ở kho nào
        public virtual Warehouse? Warehouse { get; set; }
    }
}