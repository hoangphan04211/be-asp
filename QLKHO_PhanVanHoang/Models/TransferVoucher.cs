using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Phiếu chuyển kho
    public class TransferVoucher : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public int FromWarehouseId { get; set; } // Chuyển từ kho

        [Required]
        public int ToWarehouseId { get; set; } // Chuyển tới kho

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Draft"; // Draft, InTransit, Completed, Cancelled

        public DateTime TransferDate { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public virtual Warehouse? FromWarehouse { get; set; }
        public virtual Warehouse? ToWarehouse { get; set; }
        public virtual ICollection<TransferVoucherDetail> Details { get; set; } = new List<TransferVoucherDetail>();
    }
}
