using Core.Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    /// <summary>
    /// Controller cho trang tin tức (danh sách bài viết) và chi tiết bài viết.
    /// </summary>
    [AllowAnonymous]
    public class ArticleController : Controller
    {
        private readonly ArticleService _articleService;

        public ArticleController(ArticleService articleService)
        {
            _articleService = articleService;
        }

        // GET: /Article
        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetAllArticlesAsync();

            var model = articles.Select(a => new ArticleViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Category = a.Category,
                Image = a.Image,
                Content = a.Content,
                CreatedDate = a.CreatedDate
            }).ToList();

            return View(model);
        }

        // GET: /Article/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var model = new ArticleViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Category = article.Category,
                Image = article.Image,
                Content = article.Content,
                CreatedDate = article.CreatedDate
            };

            return View(model);
        }
    }
}
