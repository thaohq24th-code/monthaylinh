using Core.Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Models;

namespace Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Dashboard admin: hiển thị thống kê tổng quan về doanh thu, đơn hàng, sản phẩm.
    /// Chỉ cho phép Admin truy cập ([Authorize(Roles = "Admin")]).
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly OrderService _orderService;
        private readonly ProductService _productService;
        private readonly UserService _userService;

        public DashboardController(OrderService orderService, ProductService productService, UserService userService)
        {
            _orderService = orderService;
            _productService = productService;
            _userService = userService;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Index()
        {
            var totalRevenue = await _orderService.GetTotalRevenueAsync();
            var totalOrders = await _orderService.CountAllOrdersAsync();
            var pendingOrders = await _orderService.CountPendingOrdersAsync();
            var totalCustomers = await _userService.CountCustomersAsync();
            var allProducts = await _productService.GetAllProductsAsync();
            var recentOrders = await _orderService.GetRecentOrdersAsync(5);

            var model = new DashboardViewModel
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                TotalCustomers = totalCustomers,
                TotalProducts = allProducts.Count,
                RecentOrders = recentOrders.Select(o => new OrderAdminListViewModel
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    CustomerUsername = o.User?.Username,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    Total = o.Total
                }).ToList()
            };

            return View(model);
        }
    }
}
