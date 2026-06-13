using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Models
{
    public class UserSession : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string RefreshToken { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? DeviceName { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; } = false;

        [ForeignKey("UserId")]
        public virtual SystemUser? User { get; set; }
    }
}
