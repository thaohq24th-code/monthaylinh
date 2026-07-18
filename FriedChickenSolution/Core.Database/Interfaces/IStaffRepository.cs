using Core.Database.Models;

namespace Core.Database.Interfaces
{
    /// <summary>
    /// Interface định nghĩa các phương thức truy xuất dữ liệu cho Staff.
    /// </summary>
    public interface IStaffRepository
    {
        /// <summary>Lấy toàn bộ nhân viên.</summary>
        Task<List<Staff>> GetAllAsync();

        /// <summary>Lấy nhân viên theo Id.</summary>
        Task<Staff?> GetByIdAsync(int id);

        /// <summary>Thêm nhân viên mới.</summary>
        Task AddAsync(Staff staff);

        /// <summary>Cập nhật thông tin nhân viên.</summary>
        Task UpdateAsync(Staff staff);

        /// <summary>Xóa nhân viên theo Id.</summary>
        Task<bool> DeleteAsync(int id);
    }
}
