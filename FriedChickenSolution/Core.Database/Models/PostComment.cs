using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Database.Models
{
    /// <summary>
    /// Entity bình luận dưới bài viết của người dùng.
    /// Bảng: PostComments
    /// </summary>
    [Table("PostComments")]
    public class PostComment
    {
        [Key]
        public int Id { get; set; }

        public int PostId { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "Nội dung bình luận là bắt buộc")]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual Post? Post { get; set; }
        public virtual User? User { get; set; }
    }
}
