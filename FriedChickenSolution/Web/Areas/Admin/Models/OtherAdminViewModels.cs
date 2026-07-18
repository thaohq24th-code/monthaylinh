using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models
{
    // ========== ORDER ==========
    public class OrderAdminListViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? CustomerUsername { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string FormattedTotal => Total.ToString("#,##0") + "đ";
    }

    public class OrderAdminDetailViewModel
    {
        public int Id { get; set; }
        public string? CustomerUsername { get; set; }
        public DateTime OrderDate { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new();
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderStatusUpdateViewModel
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public string Status { get; set; } = string.Empty;
    }

    // ========== STAFF ==========
    public class StaffAdminListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Position { get; set; }
        public string? Shift { get; set; }
        public decimal Salary { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class StaffAdminEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên nhân viên là bắt buộc")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Shift { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Lương không được âm")]
        public decimal Salary { get; set; }
    }

    // ========== USER ==========
    public class UserAdminListViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class UserAdminEditViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? FullName { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public string? Email { get; set; }

        [Required]
        public string Role { get; set; } = "Customer";
    }

    // ========== FEEDBACK ==========
    public class FeedbackAdminListViewModel
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool HasReply => !string.IsNullOrWhiteSpace(AdminReply);
        public string? AdminReply { get; set; }
    }

    public class FeedbackAdminDetailViewModel
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? AdminReply { get; set; }
        public DateTime? ReplyDate { get; set; }
    }

    public class FeedbackAdminReplyViewModel
    {
        public int FeedbackId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung phản hồi")]
        public string AdminReply { get; set; } = string.Empty;
    }

    // ========== DASHBOARD ==========
    public class DashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public List<OrderAdminListViewModel> RecentOrders { get; set; } = new();
    }
}
