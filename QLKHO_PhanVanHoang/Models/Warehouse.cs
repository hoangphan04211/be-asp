using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Kho hàng
    public class Warehouse : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;  // Tên kho

        [MaxLength(200)]
        public string? Location { get; set; }  // Địa chỉ kho

        public int? ManagerId { get; set; }  // ID người quản lý

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }  // Số điện thoại

        // ===== NAVIGATION PROPERTIES (QUAN HỆ) =====

        // 1 kho có nhiều lô hàng tồn
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

        // 1 kho có nhiều phiếu nhập
        public virtual ICollection<ReceivingVoucher> ReceivingVouchers { get; set; } = new List<ReceivingVoucher>();

        // 1 kho có nhiều phiếu kiểm
        public virtual ICollection<CountingSheet> CountingSheets { get; set; } = new List<CountingSheet>();

        // 1 kho có nhiều lịch sử điều chỉnh
        public virtual ICollection<InventoryAdjustment> InventoryAdjustments { get; set; } = new List<InventoryAdjustment>();
    }
}