using Core.Database.Models;
using Core.Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Models;
using Web.Helpers;

namespace Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Admin controller quản lý sản phẩm (CRUD): tạo, sửa, xóa, xem danh sách.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly FileUploadHelper _uploadHelper;

        public ProductController(ProductService productService, FileUploadHelper uploadHelper)
        {
            _productService = productService;
            _uploadHelper = uploadHelper;
        }

        // GET: /Admin/Product
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            var models = products.Select(p => new ProductAdminListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Type = p.Type,
                Stock = p.Stock,
                Featured = p.Featured,
                CreatedDate = p.CreatedDate
            }).ToList();

            return View(models);
        }

        // GET: /Admin/Product/Create
        public IActionResult Create()
        {
            return View(new ProductAdminEditViewModel());
        }

        // POST: /Admin/Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductAdminEditViewModel model)
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

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Type = model.Type,
                Stock = model.Stock,
                Featured = model.Featured,
                Image = imagePath
            };

            var result = await _productService.CreateProductAsync(product);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        // GET: /Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductAdminEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Type = product.Type,
                Stock = product.Stock,
                Featured = product.Featured,
                ExistingImage = product.Image
            };

            return View(model);
        }

        // POST: /Admin/Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductAdminEditViewModel model)
        {
            if (id != model.Id || !ModelState.IsValid)
            {
                return View(model);
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
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

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Type = model.Type;
            product.Stock = model.Stock;
            product.Featured = model.Featured;
            product.Image = imagePath;

            var result = await _productService.UpdateProductAsync(product);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        // GET: /Admin/Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductAdminListViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Type = product.Type,
                Stock = product.Stock,
                Featured = product.Featured,
                CreatedDate = product.CreatedDate
            };

            return View(model);
        }

        // POST: /Admin/Product/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
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
