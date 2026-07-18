using Core.Database.Models;
using Core.Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Models;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StaffController : Controller
    {
        private readonly StaffService _staffService;

        public StaffController(StaffService staffService)
        {
            _staffService = staffService;
        }

        // GET: /Admin/Staff
        public async Task<IActionResult> Index()
        {
            var staffs = await _staffService.GetAllStaffsAsync();
            var models = staffs.Select(s => new StaffAdminListViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Position = s.Position,
                Shift = s.Shift,
                Salary = s.Salary,
                CreatedDate = s.CreatedDate
            }).ToList();

            return View(models);
        }

        // GET: /Admin/Staff/Create
        public IActionResult Create()
        {
            return View(new StaffAdminEditViewModel());
        }

        // POST: /Admin/Staff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StaffAdminEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var staff = new Staff
            {
                Name = model.Name,
                Phone = model.Phone,
                Shift = model.Shift,
                Position = model.Position,
                Salary = model.Salary
            };

            var result = await _staffService.CreateStaffAsync(staff);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        // GET: /Admin/Staff/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            var model = new StaffAdminEditViewModel
            {
                Id = staff.Id,
                Name = staff.Name,
                Phone = staff.Phone,
                Shift = staff.Shift,
                Position = staff.Position,
                Salary = staff.Salary
            };

            return View(model);
        }

        // POST: /Admin/Staff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StaffAdminEditViewModel model)
        {
            if (id != model.Id || !ModelState.IsValid)
            {
                return View(model);
            }

            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            staff.Name = model.Name;
            staff.Phone = model.Phone;
            staff.Shift = model.Shift;
            staff.Position = model.Position;
            staff.Salary = model.Salary;

            var result = await _staffService.UpdateStaffAsync(staff);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        // POST: /Admin/Staff/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _staffService.DeleteStaffAsync(id);
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
