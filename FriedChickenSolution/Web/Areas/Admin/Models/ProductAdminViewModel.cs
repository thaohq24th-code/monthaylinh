using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models
{
    /// <summary>
    /// ViewModel cho trang danh sách sản phẩm (Admin).
    /// </summary>
    public class ProductAdminListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Stock { get; set; }
        public bool Featured { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// ViewModel cho form tạo/sửa sản phẩm (Admin).
    /// </summary>
    public class ProductAdminEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Loại sản phẩm là bắt buộc")]
        [MaxLength(100)]
        public string Type { get; set; } = string.Empty;

        [Display(Name = "Tồn kho")]
        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không được âm")]
        public int Stock { get; set; }

        [Display(Name = "Nổi bật")]
        public bool Featured { get; set; }

        [Display(Name = "Ảnh (upload mới)")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "URL ảnh hiện tại")]
        public string? ExistingImage { get; set; }
    }
}
