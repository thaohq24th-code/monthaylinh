using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Database.Models
{
    /// <summary>
    /// Entity đại diện cho bài viết / tin tức của tiệm (công thức, sự kiện, khuyến mãi, ...).
    /// Bảng: Articles
    /// </summary>
    [Table("Articles")]
    public class Article
    {
        /// <summary>
        /// Khóa chính, tự động tăng.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Tiêu đề bài viết.
        /// </summary>
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Danh mục bài viết (Công thức, Khuyến mãi, Sự kiện, Tin tức, ...).
        /// </summary>
        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Ảnh đại diện cho bài viết.
        /// </summary>
        [MaxLength(500)]
        public string? Image { get; set; }

        /// <summary>
        /// Nội dung đầy đủ của bài viết (hỗ trợ HTML cơ bản).
        /// </summary>
        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Thời điểm tạo bài viết.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
