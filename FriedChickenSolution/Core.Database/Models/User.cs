using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Database.Models
{
    /// <summary>
    /// Entity đại diện cho người dùng hệ thống (khách hàng hoặc quản trị viên).
    /// Bảng: Users
    /// </summary>
    [Table("Users")]
    public class User
    {
        /// <summary>
        /// Khóa chính, tự động tăng.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Tên đăng nhập, duy nhất trong hệ thống.
        /// </summary>
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu đã được băm (hash) bằng BCrypt. Không bao giờ lưu mật khẩu thô.
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Họ và tên đầy đủ của người dùng.
        /// </summary>
        [MaxLength(200)]
        public string? FullName { get; set; }

        /// <summary>
        /// Số điện thoại liên hệ.
        /// </summary>
        [MaxLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// Địa chỉ email.
        /// </summary>
        [MaxLength(200)]
        public string? Email { get; set; }

        /// <summary>
        /// Vai trò của người dùng: "Admin" hoặc "Customer".
        /// Dùng để phân quyền truy cập (role-based authorization).
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Customer";

        /// <summary>
        /// Ảnh đại diện (avatar) của người dùng.
        /// </summary>
        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Thời điểm tạo tài khoản.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Navigation property: danh sách đơn hàng của người dùng này.
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        /// <summary>
        /// Navigation property: danh sách phản hồi của người dùng này.
        /// </summary>
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
