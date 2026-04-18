using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Quyền hạn chi tiết
    public class Permission : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty; // e.g., "PRODUCTS_VIEW", "INBOUND_CREATE"

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; // e.g., "Xem danh sách sản phẩm"

        [MaxLength(50)]
        public string Group { get; set; } = string.Empty; // e.g., "Sản phẩm", "Vận hành", "Hệ thống"

        [MaxLength(200)]
        public string? Description { get; set; }

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
