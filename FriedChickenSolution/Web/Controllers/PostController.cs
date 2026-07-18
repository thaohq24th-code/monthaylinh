using System.Security.Claims;
using Core.Database.Data;
using Core.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Helpers;

namespace Web.Controllers
{
    /// <summary>
    /// Controller quản lý bài viết cộng đồng của người dùng.
    /// Cho phép người dùng đăng bài, bình luận và xem bài của nhau.
    /// </summary>
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly FileUploadHelper _fileUploadHelper;

        public PostController(ApplicationDbContext db, FileUploadHelper fileUploadHelper)
        {
            _db = db;
            _fileUploadHelper = fileUploadHelper;
        }

        // GET /Post/Index - Danh sách bài viết cộng đồng
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 9;
            var posts = await _db.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int totalPosts = await _db.Posts.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(posts);
        }

        // GET /Post/Detail/5 - Chi tiết bài viết và bình luận
        public async Task<IActionResult> Detail(int id)
        {
            var post = await _db.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }

        // GET /Post/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST /Post/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title, string content, IFormFile? imageFile)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                ModelState.AddModelError("", "Tiêu đề và nội dung là bắt buộc");
                return View();
            }

            int userId = GetCurrentUserId();
            string? imageUrl = null;

            if (imageFile != null && imageFile.Length > 0)
            {
                var (success, message, path) = await _fileUploadHelper.SaveImageAsync(imageFile);
                if (success) imageUrl = path;
                else { TempData["ErrorMessage"] = message; return View(); }
            }

            var post = new Post
            {
                UserId = userId,
                Title = title.Trim(),
                Content = content.Trim(),
                ImageUrl = imageUrl,
                CreatedDate = DateTime.Now
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã đăng bài viết thành công!";
            return RedirectToAction("Detail", new { id = post.Id });
        }

        // POST /Post/AddComment
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Nội dung bình luận không được để trống";
                return RedirectToAction("Detail", new { id = postId });
            }

            int userId = GetCurrentUserId();
            var comment = new PostComment
            {
                PostId = postId,
                UserId = userId,
                Content = content.Trim(),
                CreatedDate = DateTime.Now
            };

            _db.PostComments.Add(comment);
            await _db.SaveChangesAsync();
            return RedirectToAction("Detail", new { id = postId });
        }

        // POST /Post/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetCurrentUserId();
            var post = await _db.Posts.FindAsync(id);
            if (post == null) return NotFound();

            // Chỉ tác giả hoặc Admin mới được xóa
            if (post.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            if (!string.IsNullOrEmpty(post.ImageUrl))
                _fileUploadHelper.DeleteImage(post.ImageUrl);

            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa bài viết";
            return RedirectToAction("Index");
        }

        // POST /Post/DeleteComment/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId, int postId)
        {
            int userId = GetCurrentUserId();
            var comment = await _db.PostComments.FindAsync(commentId);
            if (comment == null) return NotFound();

            if (comment.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            _db.PostComments.Remove(comment);
            await _db.SaveChangesAsync();
            return RedirectToAction("Detail", new { id = postId });
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
