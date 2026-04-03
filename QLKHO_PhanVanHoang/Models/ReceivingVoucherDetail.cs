using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Chi tiết phiếu nhập
    public class ReceivingVoucherDetail : BaseEntity
    {
        [Required]
        public int ReceivingVoucherId { get; set; }  // Thuộc phiếu nhập nào

        [Required]
        public int ProductId { get; set; }  // Sản phẩm gì?

        [MaxLength(50)]
        public string? LotNumber { get; set; }  // Số lô

        public DateTime? ExpiryDate { get; set; }  // Hạn sử dụng

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }  // Số lượng nhập

        [Column(TypeName = "decimal(18,2)")]
        public decimal? UnitPrice { get; set; }  // Đơn giá

        // ===== NAVIGATION PROPERTIES =====

        // Chi tiết này thuộc phiếu nhập nào
        public virtual ReceivingVoucher? ReceivingVoucher { get; set; }

        // Chi tiết này là sản phẩm gì
        public virtual Product? Product { get; set; }
    }
}