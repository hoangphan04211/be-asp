using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Chi tiết phiếu chuyển kho
    public class TransferVoucherDetail : BaseEntity
    {
        [Required]
        public int TransferVoucherId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [MaxLength(50)]
        public string? LotNumber { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        public virtual TransferVoucher? TransferVoucher { get; set; }
        public virtual Product? Product { get; set; }
    }
}
