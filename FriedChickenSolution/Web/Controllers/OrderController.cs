using System.Security.Claims;
using Core.Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    /// <summary>
    /// Controller xử lý đặt hàng (checkout) qua Ajax và hiển thị lịch sử đơn hàng của khách hàng.
    /// Toàn bộ logic kiểm tra tồn kho, tính tổng tiền được xử lý trong OrderService.
    /// </summary>
    [Authorize]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: /Order/MyOrders
        public async Task<IActionResult> MyOrders()
        {
            var userId = GetCurrentUserId();
            var orders = await _orderService.GetOrdersByUserAsync(userId);

            var model = orders.Select(MapToViewModel).ToList();
            return View(model);
        }

        /// <summary>
        /// Ajax endpoint: xử lý đặt hàng từ giỏ hàng phía Client.
        /// POST: /Order/Checkout
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestViewModel request)
        {
            if (request == null || request.Items == null || request.Items.Count == 0)
            {
                return Json(new { success = false, message = "Giỏ hàng đang trống!" });
            }

            var userId = GetCurrentUserId();
            var checkoutItems = request.Items.Select(i => new CheckoutItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList();

            var result = await _orderService.CheckoutAsync(
                userId,
                request.ReceiverName,
                request.Phone,
                request.Address,
                checkoutItems);

            return Json(new { success = result.Success, message = result.Message, orderId = result.OrderId });
        }

        /// <summary>
        /// Ajax endpoint: lấy lại danh sách đơn hàng của khách dưới dạng partial view (dùng để
        /// refresh giao diện "Đơn hàng của tôi" sau khi đặt hàng mà không cần reload toàn trang).
        /// GET: /Order/MyOrdersPartial
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MyOrdersPartial()
        {
            var userId = GetCurrentUserId();
            var orders = await _orderService.GetOrdersByUserAsync(userId);
            var model = orders.Select(MapToViewModel).ToList();
            return PartialView("_MyOrdersPartial", model);
        }

        private static OrderViewModel MapToViewModel(Core.Database.Models.Order order)
        {
            return new OrderViewModel
            {
                Id = order.Id,
                UserId = order.UserId,
                CustomerUsername = order.User?.Username,
                OrderDate = order.OrderDate,
                ReceiverName = order.ReceiverName,
                Phone = order.Phone,
                Address = order.Address,
                Status = order.Status,
                Total = order.Total,
                Items = order.OrderDetails.Select(od => new OrderItemViewModel
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product?.Name ?? "Sản phẩm đã bị xóa",
                    ProductImage = od.Product?.Image,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
