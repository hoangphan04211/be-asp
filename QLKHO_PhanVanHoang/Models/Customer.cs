using System.ComponentModel.DataAnnotations;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Khách hàng
    public class Customer : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Address { get; set; }
        
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
        
        [MaxLength(200)]
        public string? Email { get; set; }
        
        [MaxLength(100)]
        public string? ContactPerson { get; set; }
        
        [MaxLength(1000)]
        public string? Description { get; set; }
    }
}
