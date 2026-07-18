using Core.Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Models;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: /Admin/Order
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var models = orders.Select(o => new OrderAdminListViewModel
            {
                Id = o.Id,
                UserId = o.UserId,
                CustomerUsername = o.User?.Username,
                OrderDate = o.OrderDate,
                Status = o.Status,
                Total = o.Total
            }).ToList();

            return View(models);
        }

        // GET: /Admin/Order/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderAdminDetailViewModel
            {
                Id = order.Id,
                CustomerUsername = order.User?.Username,
                OrderDate = order.OrderDate,
                ReceiverName = order.ReceiverName,
                Phone = order.Phone,
                Address = order.Address,
                Status = order.Status,
                Total = order.Total,
                Items = order.OrderDetails.Select(od => new OrderItemViewModel
                {
                    ProductName = od.Product?.Name ?? "Sản phẩm đã bị xóa",
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };

            return View(model);
        }

        // GET: /Admin/Order/UpdateStatus/5
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderStatusUpdateViewModel
            {
                OrderId = id,
                Status = order.Status
            };

            return View(model);
        }

        // POST: /Admin/Order/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatusConfirmed(int id, OrderStatusUpdateViewModel model)
        {
            if (id != model.OrderId || !ModelState.IsValid)
            {
                return View(nameof(UpdateStatus), model);
            }

            var result = await _orderService.UpdateOrderStatusAsync(id, model.Status);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Detail", new { id });
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(nameof(UpdateStatus), model);
        }

        // POST: /Admin/Order/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
