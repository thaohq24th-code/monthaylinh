using Core.Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    /// <summary>
    /// Controller cho trang danh sách sản phẩm, chi tiết sản phẩm, tìm kiếm và lọc realtime qua Ajax.
    /// </summary>
    [AllowAnonymous]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // GET: /Product
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(MapToViewModels(products));
        }

        // GET: /Product/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Type = product.Type,
                Image = product.Image,
                Featured = product.Featured,
                Stock = product.Stock
            };

            return View(model);
        }

        /// <summary>
        /// Ajax endpoint: lọc sản phẩm theo loại (category). Dùng cho nút lọc realtime trên trang sản phẩm.
        /// GET: /Product/FilterByType?type=Gà rán
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> FilterByType(string type)
        {
            var products = await _productService.GetProductsByTypeAsync(type);
            return PartialView("_ProductGridPartial", MapToViewModels(products));
        }

        /// <summary>
        /// Ajax endpoint: tìm kiếm sản phẩm theo tên (tìm kiếm realtime khi gõ).
        /// GET: /Product/Search?keyword=ga
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            var products = await _productService.SearchProductsAsync(keyword);
            return PartialView("_ProductGridPartial", MapToViewModels(products));
        }

        private static List<ProductViewModel> MapToViewModels(List<Core.Database.Models.Product> products)
        {
            return products.Select(p => new ProductViewModel
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
        }
    }
}
