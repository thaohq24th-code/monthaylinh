using Core.Database.Data;
using Core.Database.Interfaces;
using Core.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Database.Repositories
{
    /// <summary>
    /// Triển khai IStaffRepository, sử dụng ApplicationDbContext (EF Core) để truy xuất SQL Server.
    /// </summary>
    public class StaffRepository : IStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public StaffRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Staff>> GetAllAsync()
        {
            return await _context.Staffs
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();
        }

        public async Task<Staff?> GetByIdAsync(int id)
        {
            return await _context.Staffs
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Staff staff)
        {
            staff.CreatedDate = DateTime.Now;
            await _context.Staffs.AddAsync(staff);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Staff staff)
        {
            _context.Staffs.Update(staff);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return false;
            }

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
