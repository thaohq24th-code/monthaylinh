using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Helpers;
using Web.Models;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly SiteSettingsService _settingsService;
        private readonly FileUploadHelper _fileUploadHelper;

        public SettingsController(SiteSettingsService settingsService, FileUploadHelper fileUploadHelper)
        {
            _settingsService = settingsService;
            _fileUploadHelper = fileUploadHelper;
        }

        // GET /Admin/Settings
        public IActionResult Index()
        {
            var settings = _settingsService.GetSettings();
            return View(settings);
        }

        // POST /Admin/Settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SiteSettings settings, IFormFile? logoFile)
        {
            // Giữ logo URL cũ nếu không upload mới
            var current = _settingsService.GetSettings();

            // Xử lý upload logo
            if (logoFile != null && logoFile.Length > 0)
            {
                var (success, message, path) = await _fileUploadHelper.SaveImageAsync(logoFile);
                if (success && path != null)
                {
                    // Xóa logo cũ nếu có
                    if (!string.IsNullOrEmpty(current.SiteLogoUrl))
                        _fileUploadHelper.DeleteImage(current.SiteLogoUrl);

                    settings.SiteLogoUrl = path;
                    settings.SiteLogoIcon = "";
                }
                else
                {
                    TempData["ErrorMessage"] = "Lỗi upload logo: " + message;
                    settings.SiteLogoUrl = current.SiteLogoUrl;
                }
            }
            else
            {
                settings.SiteLogoUrl = current.SiteLogoUrl;
            }

            _settingsService.SaveSettings(settings);
            TempData["SuccessMessage"] = "✅ Cài đặt đã được lưu thành công!";
            return RedirectToAction("Index");
        }

        // POST /Admin/Settings/ResetLogo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetLogo()
        {
            var settings = _settingsService.GetSettings();
            if (!string.IsNullOrEmpty(settings.SiteLogoUrl))
                _fileUploadHelper.DeleteImage(settings.SiteLogoUrl);

            settings.SiteLogoUrl = "";
            settings.SiteLogoIcon = "fa-utensils";
            _settingsService.SaveSettings(settings);
            TempData["SuccessMessage"] = "Logo đã được đặt lại về mặc định.";
            return RedirectToAction("Index");
        }
    }
}
