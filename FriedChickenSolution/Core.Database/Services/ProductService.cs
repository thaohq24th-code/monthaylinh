using Core.Database.Interfaces;
using Core.Database.Models;

namespace Core.Database.Services
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ liên quan tới Product.
    /// Controller chỉ gọi Service, không truy cập Repository hoặc DbContext trực tiếp.
    /// </summary>
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>Lấy toàn bộ sản phẩm.</summary>
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        /// <summary>Lấy sản phẩm theo Id.</summary>
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        /// <summary>Lấy sản phẩm theo loại, hỗ trợ lọc realtime trên trang sản phẩm.</summary>
        public async Task<List<Product>> GetProductsByTypeAsync(string type)
        {
            return await _productRepository.GetByTypeAsync(type);
        }

        /// <summary>Lấy danh sách sản phẩm nổi bật để hiển thị ở trang chủ.</summary>
        public async Task<List<Product>> GetFeaturedProductsAsync()
        {
            return await _productRepository.GetFeaturedAsync();
        }

        /// <summary>Tìm kiếm sản phẩm theo tên, hỗ trợ tìm kiếm realtime.</summary>
        public async Task<List<Product>> SearchProductsAsync(string keyword)
        {
            return await _productRepository.SearchAsync(keyword);
        }

        /// <summary>
        /// Tạo sản phẩm mới. Áp dụng quy tắc nghiệp vụ:
        /// - Giá phải lớn hơn 0.
        /// - Tồn kho không được âm.
        /// </summary>
        public async Task<(bool Success, string Message)> CreateProductAsync(Product product)
        {
            if (product.Price <= 0)
            {
                return (false, "Giá sản phẩm phải lớn hơn 0");
            }

            if (product.Stock < 0)
            {
                product.Stock = 0;
            }

            await _productRepository.AddAsync(product);
            return (true, "Đã thêm sản phẩm mới thành công");
        }

        /// <summary>Cập nhật sản phẩm đã tồn tại.</summary>
        public async Task<(bool Success, string Message)> UpdateProductAsync(Product product)
        {
            var existing = await _productRepository.GetByIdAsync(product.Id);
            if (existing == null)
            {
                return (false, "Không tìm thấy sản phẩm cần cập nhật");
            }

            if (product.Price <= 0)
            {
                return (false, "Giá sản phẩm phải lớn hơn 0");
            }

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Type = product.Type;
            existing.Stock = product.Stock < 0 ? 0 : product.Stock;
            existing.Featured = product.Featured;

            // Chỉ cập nhật ảnh nếu có ảnh mới được truyền vào (tránh mất ảnh cũ khi không upload lại).
            if (!string.IsNullOrWhiteSpace(product.Image))
            {
                existing.Image = product.Image;
            }

            await _productRepository.UpdateAsync(existing);
            return (true, "Đã cập nhật sản phẩm thành công");
        }

        /// <summary>Xóa sản phẩm theo Id.</summary>
        public async Task<(bool Success, string Message)> DeleteProductAsync(int id)
        {
            var deleted = await _productRepository.DeleteAsync(id);
            return deleted
                ? (true, "Đã xóa sản phẩm thành công")
                : (false, "Không tìm thấy sản phẩm cần xóa");
        }

        /// <summary>Kiểm tra một sản phẩm còn đủ hàng để đáp ứng số lượng đặt mua không.</summary>
        public async Task<bool> HasEnoughStockAsync(int productId, int requestedQuantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            return product != null && product.Stock >= requestedQuantity;
        }
    }
}
