using Core.Database.Models;

namespace Core.Database.Interfaces
{
    /// <summary>
    /// Interface định nghĩa các phương thức truy xuất dữ liệu cho Article.
    /// </summary>
    public interface IArticleRepository
    {
        /// <summary>Lấy toàn bộ bài viết, sắp xếp mới nhất trước.</summary>
        Task<List<Article>> GetAllAsync();

        /// <summary>Lấy bài viết theo Id. Trả về null nếu không tìm thấy.</summary>
        Task<Article?> GetByIdAsync(int id);

        /// <summary>Lấy danh sách bài viết theo danh mục. "all" trả về toàn bộ.</summary>
        Task<List<Article>> GetByCategoryAsync(string category);

        /// <summary>Thêm bài viết mới.</summary>
        Task AddAsync(Article article);

        /// <summary>Cập nhật bài viết đã tồn tại.</summary>
        Task UpdateAsync(Article article);

        /// <summary>Xóa bài viết theo Id.</summary>
        Task<bool> DeleteAsync(int id);
    }
}
