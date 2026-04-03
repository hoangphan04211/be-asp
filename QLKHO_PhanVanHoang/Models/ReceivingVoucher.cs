using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Phiếu nhập kho
    public class ReceivingVoucher : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;  // Mã phiếu nhập: PN-20240319-001

        public int? SupplierId { get; set; }  // Nhà cung cấp

        [Required]
        public int WarehouseId { get; set; }  // Nhập vào kho nào

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Draft";  // Draft, Completed, Cancelled

        public DateTime ReceivingDate { get; set; } = DateTime.Now;  // Ngày nhập

        [MaxLength(500)]
        public string? Notes { get; set; }  // Ghi chú

        // ===== NAVIGATION PROPERTIES =====

        // Phiếu nhập thuộc về nhà cung cấp nào
        public virtual Supplier? Supplier { get; set; }

        // Phiếu nhập thuộc về kho nào
        public virtual Warehouse? Warehouse { get; set; }

        // 1 phiếu nhập có nhiều dòng chi tiết
        public virtual ICollection<ReceivingVoucherDetail> Details { get; set; } = new List<ReceivingVoucherDetail>();
    }
}