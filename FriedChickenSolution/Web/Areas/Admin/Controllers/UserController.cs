using Core.Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Models;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // GET: /Admin/User
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var models = users.Select(u => new UserAdminListViewModel
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                CreatedDate = u.CreatedDate
            }).ToList();

            return View(models);
        }

        // GET: /Admin/User/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserAdminEditViewModel
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Phone = user.Phone,
                Email = user.Email,
                Role = user.Role
            };

            return View(model);
        }

        // POST: /Admin/User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserAdminEditViewModel model)
        {
            if (id != model.Id || !ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Không cho phép thay đổi username hoặc role qua Admin UI để tránh xung đột dữ liệu.
            // Dùng service để cập nhật thông tin (tránh gọi trùng lặp)
            await _userService.UpdateProfileAsync(id, model.FullName, model.Phone, model.Email);

            TempData["SuccessMessage"] = "Đã cập nhật thông tin người dùng";
            return RedirectToAction("Index");
        }

        // POST: /Admin/User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
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
