using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Database.Models
{
    /// <summary>
    /// Entity đại diện cho phản hồi / góp ý của khách hàng và phản hồi của admin.
    /// Bảng: Feedbacks
    /// </summary>
    [Table("Feedbacks")]
    public class Feedback
    {
        /// <summary>
        /// Khóa chính, tự động tăng.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Khóa ngoại tới User đã gửi phản hồi.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Chủ đề phản hồi (Đánh giá, Góp ý, Hỗ trợ, Khác, ...).
        /// </summary>
        [Required(ErrorMessage = "Chủ đề là bắt buộc")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Nội dung phản hồi của khách hàng.
        /// </summary>
        [Required(ErrorMessage = "Nội dung phản hồi là bắt buộc")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Nội dung admin trả lời phản hồi. Null nếu chưa được trả lời.
        /// </summary>
        public string? AdminReply { get; set; }

        /// <summary>
        /// Thời điểm khách hàng gửi phản hồi.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Thời điểm admin trả lời. Null nếu chưa được trả lời.
        /// </summary>
        public DateTime? ReplyDate { get; set; }

        /// <summary>
        /// Navigation property: người dùng đã gửi phản hồi này.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }
    }
}
