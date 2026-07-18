using System.Security.Claims;
using Core.Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    /// <summary>
    /// Controller xử lý trang Liên hệ: gửi phản hồi và xem lịch sử phản hồi của khách hàng.
    /// </summary>
    public class FeedbackController : Controller
    {
        private readonly FeedbackService _feedbackService;

        public FeedbackController(FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        // GET: /Feedback/Contact
        [AllowAnonymous]
        public async Task<IActionResult> Contact()
        {
            var model = new List<FeedbackViewModel>();

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = GetCurrentUserId();
                var feedbacks = await _feedbackService.GetFeedbacksByUserAsync(userId);
                model = feedbacks.Select(MapToViewModel).ToList();
            }

            return View(model);
        }

        /// <summary>
        /// Ajax endpoint: khách hàng gửi phản hồi mới từ trang Liên hệ.
        /// POST: /Feedback/Submit
        /// </summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit([FromBody] FeedbackCreateViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Content))
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung phản hồi" });
            }

            var userId = GetCurrentUserId();
            var result = await _feedbackService.SubmitFeedbackAsync(userId, model.Title, model.Content);

            return Json(new { success = result.Success, message = result.Message });
        }

        /// <summary>
        /// Ajax endpoint: lấy lại lịch sử phản hồi của khách hàng hiện tại dưới dạng partial view.
        /// GET: /Feedback/MyFeedbackPartial
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyFeedbackPartial()
        {
            var userId = GetCurrentUserId();
            var feedbacks = await _feedbackService.GetFeedbacksByUserAsync(userId);
            var model = feedbacks.Select(MapToViewModel).ToList();
            return PartialView("_FeedbackHistoryPartial", model);
        }

        private static FeedbackViewModel MapToViewModel(Core.Database.Models.Feedback f)
        {
            return new FeedbackViewModel
            {
                Id = f.Id,
                Username = f.User?.Username,
                Title = f.Title,
                Content = f.Content,
                AdminReply = f.AdminReply,
                CreatedDate = f.CreatedDate,
                ReplyDate = f.ReplyDate
            };
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
