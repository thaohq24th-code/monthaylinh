using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    /// <summary>
    /// ViewModel hiển thị một đơn hàng trên trang "Đơn hàng của tôi" hoặc trong Admin.
    /// </summary>
    public class OrderViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? CustomerUsername { get; set; }
        public DateTime OrderDate { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new();

        public string FormattedTotal => Total.ToString("#,##0") + "đ";
    }

    /// <summary>
    /// ViewModel hiển thị từng dòng sản phẩm trong một đơn hàng.
    /// </summary>
    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal => Price * Quantity;
    }

    /// <summary>
    /// ViewModel dùng để nhận dữ liệu checkout (đặt hàng) gửi lên từ Ajax phía Client.
    /// </summary>
    public class CheckoutRequestViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
        public string ReceiverName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        public string Address { get; set; } = string.Empty;

        [Required]
        public List<CheckoutItemViewModel> Items { get; set; } = new();
    }

    /// <summary>
    /// ViewModel cho từng dòng sản phẩm gửi lên khi đặt hàng qua Ajax.
    /// </summary>
    public class CheckoutItemViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
