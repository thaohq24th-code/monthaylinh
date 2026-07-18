using Core.Database.Data;
using Core.Database.Interfaces;
using Core.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Database.Repositories
{
    /// <summary>
    /// Triển khai IArticleRepository, sử dụng ApplicationDbContext (EF Core) để truy xuất SQL Server.
    /// </summary>
    public class ArticleRepository : IArticleRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Article>> GetAllAsync()
        {
            return await _context.Articles
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<Article?> GetByIdAsync(int id)
        {
            return await _context.Articles
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Article>> GetByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category) || category.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                return await GetAllAsync();
            }

            return await _context.Articles
                .Where(a => a.Category == category)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task AddAsync(Article article)
        {
            article.CreatedDate = DateTime.Now;
            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Article article)
        {
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return false;
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
