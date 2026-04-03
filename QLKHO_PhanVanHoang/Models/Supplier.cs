using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Nhà cung cấp
    public class Supplier : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;  // Tên công ty

        [MaxLength(50)]
        public string? Code { get; set; }  // Mã nhà cung cấp

        [MaxLength(20)]
        public string? TaxCode { get; set; }  // Mã số thuế

        [MaxLength(100)]
        public string? ContactPerson { get; set; }  // Người liên hệ

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }  // Số điện thoại

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }  // Địa chỉ

        // ===== NAVIGATION PROPERTIES =====

        // 1 nhà cung cấp có nhiều phiếu nhập
        public virtual ICollection<ReceivingVoucher> ReceivingVouchers { get; set; } = new List<ReceivingVoucher>();
    }
}