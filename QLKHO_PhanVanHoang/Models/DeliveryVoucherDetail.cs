using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Chi tiết phiếu xuất
    public class DeliveryVoucherDetail : BaseEntity
    {
        [Required]
        public int DeliveryVoucherId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [MaxLength(50)]
        public string? LotNumber { get; set; } // Hàng lấy từ lô nào (nếu quản lý theo lô)

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SellingPrice { get; set; } // Giá bán

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CostPrice { get; set; } // Giá vốn xuất kho (tính toán)

        public virtual DeliveryVoucher? DeliveryVoucher { get; set; }
        public virtual Product? Product { get; set; }
    }
}
