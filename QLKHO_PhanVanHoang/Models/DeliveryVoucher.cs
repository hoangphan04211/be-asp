using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Phiếu Xuất kho
    public class DeliveryVoucher : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty; // Mã phiếu xuất PX-2024...

        public int? CustomerId { get; set; }

        [Required]
        public int WarehouseId { get; set; } // Xuất từ kho nào

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Draft"; // Draft, Approved, Dispatched, Cancelled

        public DateTime DeliveryDate { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
        public virtual ICollection<DeliveryVoucherDetail> Details { get; set; } = new List<DeliveryVoucherDetail>();
    }
}
