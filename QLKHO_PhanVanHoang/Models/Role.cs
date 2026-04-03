using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Vai trò user
    public class Role : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty; // Admin, WarehouseManager, Employee...

        [MaxLength(500)]
        public string? Description { get; set; }

        public virtual ICollection<SystemUser> Users { get; set; } = new List<SystemUser>();
    }
}
