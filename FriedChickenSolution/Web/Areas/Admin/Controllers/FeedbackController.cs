using Core.Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Models;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FeedbackController : Controller
    {
        private readonly FeedbackService _feedbackService;

        public FeedbackController(FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        // GET: /Admin/Feedback
        public async Task<IActionResult> Index()
        {
            var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
            var models = feedbacks.Select(f => new FeedbackAdminListViewModel
            {
                Id = f.Id,
                Username = f.User?.Username,
                Title = f.Title,
                CreatedDate = f.CreatedDate,
                AdminReply = f.AdminReply
            }).ToList();

            return View(models);
        }

        // GET: /Admin/Feedback/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            var model = new FeedbackAdminDetailViewModel
            {
                Id = feedback.Id,
                Username = feedback.User?.Username,
                Title = feedback.Title,
                Content = feedback.Content,
                CreatedDate = feedback.CreatedDate,
                AdminReply = feedback.AdminReply,
                ReplyDate = feedback.ReplyDate
            };

            return View(model);
        }

        // GET: /Admin/Feedback/Reply/5
        public async Task<IActionResult> Reply(int id)
        {
            var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            var model = new FeedbackAdminReplyViewModel
            {
                FeedbackId = id,
                AdminReply = feedback.AdminReply ?? string.Empty
            };

            return View(model);
        }

        // POST: /Admin/Feedback/Reply/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplyConfirmed(int id, FeedbackAdminReplyViewModel model)
        {
            if (id != model.FeedbackId || !ModelState.IsValid)
            {
                return View(nameof(Reply), model);
            }

            var result = await _feedbackService.ReplyFeedbackAsync(id, model.AdminReply);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Detail", new { id });
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(nameof(Reply), model);
        }

        // POST: /Admin/Feedback/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _feedbackService.DeleteFeedbackAsync(id);
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
