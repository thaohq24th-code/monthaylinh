using Core.Database.Models;
using Core.Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Models;
using Web.Helpers;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ArticleController : Controller
    {
        private readonly ArticleService _articleService;
        private readonly FileUploadHelper _uploadHelper;

        public ArticleController(ArticleService articleService, FileUploadHelper uploadHelper)
        {
            _articleService = articleService;
            _uploadHelper = uploadHelper;
        }

        // GET: /Admin/Article
        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetAllArticlesAsync();
            var models = articles.Select(a => new ArticleAdminListViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Category = a.Category,
                CreatedDate = a.CreatedDate
            }).ToList();

            return View(models);
        }

        // GET: /Admin/Article/Create
        public IActionResult Create()
        {
            return View(new ArticleAdminEditViewModel());
        }

        // POST: /Admin/Article/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleAdminEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? imagePath = null;
            if (model.ImageFile != null)
            {
                var uploadResult = await _uploadHelper.SaveImageAsync(model.ImageFile);
                if (!uploadResult.Success)
                {
                    ModelState.AddModelError(nameof(model.ImageFile), uploadResult.Message);
                    return View(model);
                }
                imagePath = uploadResult.RelativePath;
            }

            var article = new Article
            {
                Title = model.Title,
                Category = model.Category,
                Content = model.Content,
                Image = imagePath
            };

            var result = await _articleService.CreateArticleAsync(article);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        // GET: /Admin/Article/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var model = new ArticleAdminEditViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Category = article.Category,
                Content = article.Content,
                ExistingImage = article.Image
            };

            return View(model);
        }

        // POST: /Admin/Article/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArticleAdminEditViewModel model)
        {
            if (id != model.Id || !ModelState.IsValid)
            {
                return View(model);
            }

            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            string? imagePath = model.ExistingImage;
            if (model.ImageFile != null)
            {
                var uploadResult = await _uploadHelper.SaveImageAsync(model.ImageFile);
                if (!uploadResult.Success)
                {
                    ModelState.AddModelError(nameof(model.ImageFile), uploadResult.Message);
                    return View(model);
                }
                imagePath = uploadResult.RelativePath;
            }

            article.Title = model.Title;
            article.Category = model.Category;
            article.Content = model.Content;
            article.Image = imagePath;

            var result = await _articleService.UpdateArticleAsync(article);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        // GET: /Admin/Article/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var model = new ArticleAdminListViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Category = article.Category,
                CreatedDate = article.CreatedDate
            };

            return View(model);
        }

        // POST: /Admin/Article/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _articleService.DeleteArticleAsync(id);
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
