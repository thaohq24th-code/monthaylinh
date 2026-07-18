using Core.Database.Interfaces;
using Core.Database.Models;

namespace Core.Database.Services
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ liên quan tới Article (bài viết / tin tức).
    /// </summary>
    public class ArticleService
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        /// <summary>Lấy toàn bộ bài viết.</summary>
        public async Task<List<Article>> GetAllArticlesAsync()
        {
            return await _articleRepository.GetAllAsync();
        }

        /// <summary>Lấy bài viết theo Id (dùng cho trang chi tiết tin tức).</summary>
        public async Task<Article?> GetArticleByIdAsync(int id)
        {
            return await _articleRepository.GetByIdAsync(id);
        }

        /// <summary>Lấy bài viết theo danh mục.</summary>
        public async Task<List<Article>> GetArticlesByCategoryAsync(string category)
        {
            return await _articleRepository.GetByCategoryAsync(category);
        }

        /// <summary>Tạo bài viết mới.</summary>
        public async Task<(bool Success, string Message)> CreateArticleAsync(Article article)
        {
            if (string.IsNullOrWhiteSpace(article.Title))
            {
                return (false, "Tiêu đề bài viết không được để trống");
            }

            await _articleRepository.AddAsync(article);
            return (true, "Đã thêm bài viết mới thành công");
        }

        /// <summary>Cập nhật bài viết đã tồn tại.</summary>
        public async Task<(bool Success, string Message)> UpdateArticleAsync(Article article)
        {
            var existing = await _articleRepository.GetByIdAsync(article.Id);
            if (existing == null)
            {
                return (false, "Không tìm thấy bài viết cần cập nhật");
            }

            existing.Title = article.Title;
            existing.Category = article.Category;
            existing.Content = article.Content;

            if (!string.IsNullOrWhiteSpace(article.Image))
            {
                existing.Image = article.Image;
            }

            await _articleRepository.UpdateAsync(existing);
            return (true, "Đã cập nhật bài viết thành công");
        }

        /// <summary>Xóa bài viết theo Id.</summary>
        public async Task<(bool Success, string Message)> DeleteArticleAsync(int id)
        {
            var deleted = await _articleRepository.DeleteAsync(id);
            return deleted
                ? (true, "Đã xóa bài viết thành công")
                : (false, "Không tìm thấy bài viết cần xóa");
        }
    }
}
