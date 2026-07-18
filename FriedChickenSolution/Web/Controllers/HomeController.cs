using System.Diagnostics;
using Core.Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    /// <summary>
    /// Controller cho trang chủ. Hiển thị sản phẩm nổi bật và thông tin giới thiệu.
    /// </summary>
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ProductService _productService;

        public HomeController(ProductService productService)
        {
            _productService = productService;
        }

        // GET: /
        public async Task<IActionResult> Index()
        {
            try
            {
                var featured = await _productService.GetFeaturedProductsAsync();

                var viewModels = featured.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Type = p.Type,
                    Image = p.Image,
                    Featured = p.Featured,
                    Stock = p.Stock
                }).ToList();

                return View(viewModels);
            }
            catch
            {
                // Nếu DB chưa sẵn sàng, trả về trang chủ với danh sách trống
                return View(new List<ProductViewModel>());
            }
        }

        // GET: /Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
