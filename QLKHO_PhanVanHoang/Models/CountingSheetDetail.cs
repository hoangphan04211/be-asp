using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Chi tiết phiếu kiểm kê
    public class CountingSheetDetail : BaseEntity
    {
        [Required]
        public int CountingSheetId { get; set; }  // Thuộc phiếu kiểm nào

        [Required]
        public int ProductId { get; set; }  // Sản phẩm gì?

        [MaxLength(50)]
        public string? LotNumber { get; set; }  // Lô hàng nào?

        public DateTime? ExpiryDate { get; set; }  // Hạn sử dụng

        [Required]
        public int SystemQuantity { get; set; }  // Số lượng trên hệ thống

        public int? ActualQuantity { get; set; }  // Số lượng đếm thực tế

        public int? DamagedQuantity { get; set; }  // Số lượng hỏng

        [MaxLength(500)]
        public string? Note { get; set; }  // Ghi chú

        // ===== NAVIGATION PROPERTIES =====

        // Chi tiết này thuộc phiếu kiểm nào
        public virtual CountingSheet? CountingSheet { get; set; }

        // Chi tiết này là sản phẩm gì
        public virtual Product? Product { get; set; }
    }
}