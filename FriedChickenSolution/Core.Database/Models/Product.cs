using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Database.Models
{
    /// <summary>
    /// Entity đại diện cho sản phẩm (gà rán, burger, ...) được bán trên website.
    /// Bảng: Products
    /// </summary>
    [Table("Products")]
    public class Product
    {
        /// <summary>
        /// Khóa chính, tự động tăng.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Tên sản phẩm. Bắt buộc, tối đa 200 ký tự.
        /// </summary>
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả chi tiết sản phẩm (thành phần, hương vị, ...).
        /// Cho phép null vì không phải sản phẩm nào cũng có mô tả ngay khi tạo.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Giá bán, đơn vị VNĐ. Dùng decimal để tránh sai số khi tính toán tiền tệ.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Loại / danh mục sản phẩm (Gà rán, Gà quay, Burger, Nước uống, ...).
        /// Đặt tên là "Type" theo yêu cầu nghiệp vụ, tương ứng với "category" trong JS gốc.
        /// </summary>
        [Required(ErrorMessage = "Loại sản phẩm là bắt buộc")]
        [MaxLength(100)]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Đường dẫn ảnh sản phẩm. Khi upload file, lưu đường dẫn tương đối tới wwwroot/uploads.
        /// Khi dùng URL ngoài, lưu trực tiếp URL.
        /// </summary>
        [MaxLength(500)]
        public string? Image { get; set; }

        /// <summary>
        /// Đánh dấu sản phẩm nổi bật để hiển thị ở trang chủ / khu vực "Sản phẩm nổi bật".
        /// </summary>
        public bool Featured { get; set; } = false;

        /// <summary>
        /// Số lượng còn trong kho. Dùng để kiểm tra tồn kho khi đặt hàng.
        /// </summary>
        public int Stock { get; set; } = 0;

        /// <summary>
        /// Thời điểm tạo sản phẩm trong hệ thống.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Thời điểm cập nhật gần nhất. Null nếu chưa từng được sửa sau khi tạo.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Navigation property: danh sách chi tiết đơn hàng có chứa sản phẩm này.
        /// </summary>
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
