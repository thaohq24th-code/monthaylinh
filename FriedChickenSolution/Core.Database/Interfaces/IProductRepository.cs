using Core.Database.Models;

namespace Core.Database.Interfaces
{
    /// <summary>
    /// Interface định nghĩa các phương thức truy xuất dữ liệu cho Product.
    /// Tuân thủ Repository Pattern: Controller/Service không truy cập DbContext trực tiếp.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>Lấy toàn bộ sản phẩm.</summary>
        Task<List<Product>> GetAllAsync();

        /// <summary>Lấy sản phẩm theo Id. Trả về null nếu không tìm thấy.</summary>
        Task<Product?> GetByIdAsync(int id);

        /// <summary>Lấy danh sách sản phẩm theo loại (category). "all" trả về toàn bộ.</summary>
        Task<List<Product>> GetByTypeAsync(string type);

        /// <summary>Lấy danh sách sản phẩm được đánh dấu nổi bật.</summary>
        Task<List<Product>> GetFeaturedAsync();

        /// <summary>Tìm kiếm sản phẩm theo tên (tìm kiếm tương đối, không phân biệt hoa thường).</summary>
        Task<List<Product>> SearchAsync(string keyword);

        /// <summary>Thêm sản phẩm mới vào database.</summary>
        Task AddAsync(Product product);

        /// <summary>Cập nhật thông tin sản phẩm đã tồn tại.</summary>
        Task UpdateAsync(Product product);

        /// <summary>Xóa sản phẩm theo Id. Trả về false nếu không tìm thấy sản phẩm.</summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>Kiểm tra sản phẩm có tồn tại không.</summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>Trừ số lượng tồn kho khi đặt hàng thành công.</summary>
        Task ReduceStockAsync(int productId, int quantity);

        /// <summary>Lưu thay đổi xuống database (dùng khi cần gộp nhiều thao tác trong 1 transaction).</summary>
        Task SaveChangesAsync();
    }
}
