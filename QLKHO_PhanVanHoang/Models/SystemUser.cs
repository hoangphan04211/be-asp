using System.ComponentModel.DataAnnotations;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    // Bảng Người dùng hệ thống
    public class SystemUser : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Email { get; set; }

        [Required]
        public int RoleId { get; set; } // Vai trò

        public bool IsActive { get; set; } = true;

        public virtual Role? Role { get; set; }

        // === FOR REFRESH TOKEN ===
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // === FOR PASSWORD RESET (6-digit code) ===
        [MaxLength(6)]
        public string? ResetPasswordCode { get; set; }
        public DateTime? ResetPasswordExpiry { get; set; }
    }
}
