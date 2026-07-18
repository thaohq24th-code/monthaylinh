using Core.Database.Interfaces;
using Core.Database.Models;

namespace Core.Database.Services
{
    /// <summary>
    /// DTO đại diện cho một dòng sản phẩm trong giỏ hàng khi đặt hàng (đến từ client qua Ajax).
    /// </summary>
    public class CheckoutItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Service xử lý logic nghiệp vụ liên quan tới Order: đặt hàng, kiểm tra tồn kho,
    /// cập nhật trạng thái, thống kê doanh thu cho Dashboard.
    /// </summary>
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        /// <summary>Lấy toàn bộ đơn hàng (dùng cho Admin).</summary>
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        /// <summary>Lấy đơn hàng theo Id.</summary>
        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        /// <summary>Lấy lịch sử đơn hàng của một khách hàng cụ thể.</summary>
        public async Task<List<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _orderRepository.GetByUserIdAsync(userId);
        }

        /// <summary>Lấy đơn hàng theo trạng thái (dùng cho lọc trong Admin).</summary>
        public async Task<List<Order>> GetOrdersByStatusAsync(string status)
        {
            return await _orderRepository.GetByStatusAsync(status);
        }

        /// <summary>Lấy các đơn hàng gần nhất để hiển thị trên Dashboard.</summary>
        public async Task<List<Order>> GetRecentOrdersAsync(int count = 5)
        {
            return await _orderRepository.GetRecentAsync(count);
        }

        /// <summary>
        /// Xử lý đặt hàng (checkout) từ giỏ hàng của khách.
        /// Kiểm tra tồn kho từng sản phẩm, tạo Order + OrderDetail, trừ tồn kho.
        /// Đây là logic nghiệp vụ quan trọng nhất, được tách hoàn toàn ra khỏi Controller.
        /// </summary>
        public async Task<(bool Success, string Message, int? OrderId)> CheckoutAsync(
            int userId,
            string receiverName,
            string phone,
            string address,
            List<CheckoutItem> items)
        {
            if (items == null || items.Count == 0)
            {
                return (false, "Giỏ hàng đang trống", null);
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                return (false, "Vui lòng nhập địa chỉ giao hàng", null);
            }

            var orderDetails = new List<OrderDetail>();
            decimal total = 0;

            // Kiểm tra tồn kho và xây dựng chi tiết đơn hàng cho từng sản phẩm.
            foreach (var item in items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    return (false, $"Sản phẩm không tồn tại (Id: {item.ProductId})", null);
                }

                if (product.Stock < item.Quantity)
                {
                    return (false, $"Sản phẩm \"{product.Name}\" không đủ số lượng trong kho!", null);
                }

                orderDetails.Add(new OrderDetail
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.Price
                });

                total += product.Price * item.Quantity;
            }

            var order = new Order
            {
                UserId = userId,
                ReceiverName = receiverName,
                Phone = phone,
                Address = address,
                Status = "Đang xử lý",
                Total = total,
                OrderDetails = orderDetails
            };

            await _orderRepository.AddAsync(order);

            // Trừ tồn kho sau khi đơn hàng được tạo thành công.
            foreach (var item in items)
            {
                await _productRepository.ReduceStockAsync(item.ProductId, item.Quantity);
            }

            return (true, "Đặt hàng thành công! Cảm ơn bạn.", order.Id);
        }

        /// <summary>Admin cập nhật trạng thái đơn hàng (đang xử lý / đang giao / đã giao / đã hủy).</summary>
        public async Task<(bool Success, string Message)> UpdateOrderStatusAsync(int orderId, string status)
        {
            var validStatuses = new[] { "Đang xử lý", "Đang giao", "Đã giao", "Đã hủy" };
            if (!validStatuses.Contains(status))
            {
                return (false, "Trạng thái đơn hàng không hợp lệ");
            }

            var updated = await _orderRepository.UpdateStatusAsync(orderId, status);
            return updated
                ? (true, "Đã cập nhật trạng thái đơn hàng")
                : (false, "Không tìm thấy đơn hàng");
        }

        /// <summary>Xóa đơn hàng theo Id (Admin).</summary>
        public async Task<(bool Success, string Message)> DeleteOrderAsync(int id)
        {
            var deleted = await _orderRepository.DeleteAsync(id);
            return deleted
                ? (true, "Đã xóa đơn hàng")
                : (false, "Không tìm thấy đơn hàng cần xóa");
        }

        /// <summary>Tính tổng doanh thu từ các đơn hàng đã giao (dùng cho Dashboard).</summary>
        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _orderRepository.GetTotalRevenueAsync();
        }

        /// <summary>Đếm số đơn hàng đang chờ xử lý (dùng cho Dashboard).</summary>
        public async Task<int> CountPendingOrdersAsync()
        {
            return await _orderRepository.CountByStatusAsync("Đang xử lý");
        }

        /// <summary>Đếm tổng số đơn hàng trong hệ thống.</summary>
        public async Task<int> CountAllOrdersAsync()
        {
            return await _orderRepository.CountAllAsync();
        }
    }
}
