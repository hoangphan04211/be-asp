using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Lịch sử điều chỉnh tồn kho
    public class InventoryAdjustment : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string AdjustmentType { get; set; } = string.Empty;  // Counting, Damage, Return, Manual

        public int? ReferenceId { get; set; }  // ID của phiếu tham chiếu

        [MaxLength(50)]
        public string? ReferenceNumber { get; set; }  // Mã số của phiếu tham chiếu

        [Required]
        public int ProductId { get; set; }  // Sản phẩm bị điều chỉnh

        [Required]
        public int WarehouseId { get; set; }  // Ở kho nào

        [MaxLength(50)]
        public string? LotNumber { get; set; }  // Lô hàng nào

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OldQuantity { get; set; }  // Số lượng cũ

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal NewQuantity { get; set; }  // Số lượng mới

        [MaxLength(500)]
        public string? Reason { get; set; }  // Lý do điều chỉnh

        public int? ApprovedBy { get; set; }  // Ai phê duyệt

        public DateTime? ApprovedAt { get; set; }  // Thời gian phê duyệt

        [MaxLength(20)]
        public string Status { get; set; } = "Pending";  // Pending, Approved, Rejected

        // ===== NAVIGATION PROPERTIES =====

        // Điều chỉnh này liên quan đến sản phẩm nào
        public virtual Product? Product { get; set; }

        // Điều chỉnh này liên quan đến kho nào
        public virtual Warehouse? Warehouse { get; set; }
    }
}