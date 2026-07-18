using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Database.Models
{
    /// <summary>
    /// Entity đại diện cho từng sản phẩm cụ thể trong một đơn hàng (chi tiết đơn hàng).
    /// Bảng: OrderDetails
    /// </summary>
    [Table("OrderDetails")]
    public class OrderDetail
    {
        /// <summary>
        /// Khóa chính, tự động tăng.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Khóa ngoại tới Order chứa chi tiết này.
        /// </summary>
        [Required]
        public int OrderId { get; set; }

        /// <summary>
        /// Khóa ngoại tới Product được đặt.
        /// </summary>
        [Required]
        public int ProductId { get; set; }

        /// <summary>
        /// Số lượng sản phẩm được đặt.
        /// </summary>
        [Required]
        public int Quantity { get; set; }

        /// <summary>
        /// Giá sản phẩm tại thời điểm đặt hàng (lưu lại để không bị ảnh hưởng
        /// nếu giá sản phẩm thay đổi sau này).
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Navigation property: đơn hàng chứa chi tiết này.
        /// </summary>
        [ForeignKey(nameof(OrderId))]
        public virtual Order? Order { get; set; }

        /// <summary>
        /// Navigation property: sản phẩm tương ứng.
        /// </summary>
        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }
    }
}
