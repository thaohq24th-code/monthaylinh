using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Database.Models
{
    /// <summary>
    /// Entity bài viết/đăng bài của người dùng trong cộng đồng.
    /// Bảng: Posts
    /// </summary>
    [Table("Posts")]
    public class Post
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string Content { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual User? User { get; set; }
        public virtual ICollection<PostComment> Comments { get; set; } = new List<PostComment>();
    }
}
