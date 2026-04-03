using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Danh mục sản phẩm
    public class Category : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;  // Tên danh mục

        [MaxLength(500)]
        public string? Description { get; set; }  // Mô tả

        // ===== NAVIGATION PROPERTIES =====

        // 1 danh mục có nhiều sản phẩm
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}