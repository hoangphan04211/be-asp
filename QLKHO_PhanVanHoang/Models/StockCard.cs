using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Thẻ kho - Lịch sử biến động tồn kho
    public class StockCard : BaseEntity
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [MaxLength(50)]
        public string? LotNumber { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(50)]
        public string TransactionType { get; set; } = string.Empty; // Inbound, Outbound, TransferIn, TransferOut, Adjustment

        [MaxLength(50)]
        public string? ReferenceCode { get; set; } // Mã chứng từ VD: PN-..., PX-..., PK-...

        [Column(TypeName = "decimal(18,2)")]
        public decimal BeforeQuantity { get; set; } // Tồn hiện tại trước giao dịch

        [Column(TypeName = "decimal(18,2)")]
        public decimal ChangeQuantity { get; set; } // Mức thay đổi, dương = nhập, âm = xuất

        [Column(TypeName = "decimal(18,2)")]
        public decimal AfterQuantity { get; set; } // Tồn sau giao dịch

        [MaxLength(500)]
        public string? Notes { get; set; }

        public virtual Product? Product { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
    }
}
