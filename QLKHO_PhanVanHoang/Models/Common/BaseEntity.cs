using System;
using System.ComponentModel.DataAnnotations;

namespace QLKHO_PhanVanHoang.Models.Common
{
    // Đây là class cha - tất cả các class khác sẽ kế thừa từ class này
    // Lợi ích: không phải viết lại các trường này cho mỗi bảng
    public abstract class BaseEntity
    {
        [Key]  // Đánh dấu đây là khóa chính (Primary Key)
        public int Id { get; set; }  // Id tự tăng, bắt đầu từ 1,2,3...

        [Required]  // Trường bắt buộc (NOT NULL)
        public DateTime CreatedAt { get; set; }  // Thời gian tạo bản ghi

        public DateTime? UpdatedAt { get; set; }  // Thời gian sửa (có thể null)

        [Required]
        [MaxLength(100)]  // Giới hạn độ dài tối đa
        public string CreatedBy { get; set; } = "system"; // Mặc định là system

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }  // Ai sửa

        public bool IsDeleted { get; set; } = false;  // Đánh dấu xóa mềm (false là chưa xóa)

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>(); // Chống ghi đè dữ liệu đồng thời
    }
}
