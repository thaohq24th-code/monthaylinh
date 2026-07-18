using Core.Database.Models;

namespace Core.Database.Interfaces
{
    /// <summary>
    /// Interface định nghĩa các phương thức truy xuất dữ liệu cho Order và OrderDetail.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>Lấy toàn bộ đơn hàng, bao gồm chi tiết đơn hàng và sản phẩm liên quan, sắp xếp mới nhất trước.</summary>
        Task<List<Order>> GetAllAsync();

        /// <summary>Lấy đơn hàng theo Id, bao gồm chi tiết đơn hàng.</summary>
        Task<Order?> GetByIdAsync(int id);

        /// <summary>Lấy danh sách đơn hàng của một người dùng cụ thể.</summary>
        Task<List<Order>> GetByUserIdAsync(int userId);

        /// <summary>Lấy danh sách đơn hàng theo trạng thái. "all" trả về toàn bộ.</summary>
        Task<List<Order>> GetByStatusAsync(string status);

        /// <summary>Lấy N đơn hàng gần nhất (dùng cho Dashboard).</summary>
        Task<List<Order>> GetRecentAsync(int count);

        /// <summary>Thêm đơn hàng mới kèm chi tiết đơn hàng.</summary>
        Task AddAsync(Order order);

        /// <summary>Cập nhật trạng thái đơn hàng.</summary>
        Task<bool> UpdateStatusAsync(int orderId, string status);

        /// <summary>Xóa đơn hàng theo Id.</summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>Tính tổng doanh thu từ các đơn hàng đã giao thành công.</summary>
        Task<decimal> GetTotalRevenueAsync();

        /// <summary>Đếm số đơn hàng theo trạng thái cụ thể.</summary>
        Task<int> CountByStatusAsync(string status);

        /// <summary>Đếm tổng số đơn hàng trong hệ thống.</summary>
        Task<int> CountAllAsync();
    }
}
