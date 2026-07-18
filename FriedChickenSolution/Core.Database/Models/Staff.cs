using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Database.Models
{
    /// <summary>
    /// Entity đại diện cho nhân viên làm việc tại tiệm (không phải tài khoản đăng nhập hệ thống).
    /// Bảng: Staffs
    /// </summary>
    [Table("Staffs")]
    public class Staff
    {
        /// <summary>
        /// Khóa chính, tự động tăng.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Họ và tên nhân viên.
        /// </summary>
        [Required(ErrorMessage = "Tên nhân viên là bắt buộc")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Số điện thoại liên hệ của nhân viên.
        /// </summary>
        [MaxLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// Ca làm việc (Sáng, Chiều, Tối, Toàn thời gian, ...).
        /// </summary>
        [MaxLength(100)]
        public string? Shift { get; set; }

        /// <summary>
        /// Vị trí / chức vụ công việc (Đầu bếp, Nhân viên bán hàng, Quản lý, ...).
        /// </summary>
        [MaxLength(100)]
        public string? Position { get; set; }

        /// <summary>
        /// Mức lương, đơn vị VNĐ.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        /// <summary>
        /// Thời điểm thêm nhân viên vào hệ thống.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
