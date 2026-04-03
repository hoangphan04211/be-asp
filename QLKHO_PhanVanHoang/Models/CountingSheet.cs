using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Phiếu kiểm kê
    public class CountingSheet : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;  // Mã phiếu kiểm: PK-20240319-001

        [Required]
        public int WarehouseId { get; set; }  // Kiểm kho nào

        [MaxLength(200)]
        public string? Scope { get; set; }  // Phạm vi kiểm

        public DateTime CountingDate { get; set; } = DateTime.Now;  // Ngày kiểm

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Draft";  // Draft, PendingApproval, Approved, Rejected

        [MaxLength(500)]
        public string? Notes { get; set; }  // Ghi chú

        // ===== NAVIGATION PROPERTIES =====

        // Phiếu kiểm thuộc về kho nào
        public virtual Warehouse? Warehouse { get; set; }

        // 1 phiếu kiểm có nhiều dòng chi tiết
        public virtual ICollection<CountingSheetDetail> Details { get; set; } = new List<CountingSheetDetail>();
    }
}