using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Database.Models
{
    /// <summary>
    /// Entity đại diện cho một đơn hàng do khách hàng đặt.
    /// Bảng: Orders
    /// </summary>
    [Table("Orders")]
    public class Order
    {
        /// <summary>
        /// Khóa chính, tự động tăng.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Khóa ngoại tới User đã đặt đơn hàng này.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Thời điểm đặt hàng.
        /// </summary>
        public DateTime OrderDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Tên người nhận hàng (có thể khác tên tài khoản đặt hàng).
        /// </summary>
        [Required(ErrorMessage = "Tên người nhận là bắt buộc")]
        [MaxLength(200)]
        public string ReceiverName { get; set; } = string.Empty;

        /// <summary>
        /// Số điện thoại người nhận hàng.
        /// </summary>
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Địa chỉ giao hàng đầy đủ.
        /// </summary>
        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái đơn hàng: "Đang xử lý", "Đang giao", "Đã giao", "Đã hủy".
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Đang xử lý";

        /// <summary>
        /// Tổng tiền của đơn hàng (tổng các OrderDetail.Price * Quantity).
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        /// <summary>
        /// Thời điểm tạo bản ghi đơn hàng trong hệ thống.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Navigation property: người dùng đã đặt đơn hàng này.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

        /// <summary>
        /// Navigation property: danh sách chi tiết sản phẩm trong đơn hàng.
        /// </summary>
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
