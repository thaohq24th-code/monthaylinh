using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models
{
    public class ArticleAdminListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class ArticleAdminEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Ảnh (upload mới)")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "URL ảnh hiện tại")]
        public string? ExistingImage { get; set; }
    }
}
