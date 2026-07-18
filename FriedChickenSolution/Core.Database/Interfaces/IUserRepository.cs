using Core.Database.Models;

namespace Core.Database.Interfaces
{
    /// <summary>
    /// Interface định nghĩa các phương thức truy xuất dữ liệu cho User.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>Lấy toàn bộ người dùng.</summary>
        Task<List<User>> GetAllAsync();

        /// <summary>Lấy người dùng theo Id.</summary>
        Task<User?> GetByIdAsync(int id);

        /// <summary>Lấy người dùng theo tên đăng nhập (dùng cho đăng nhập).</summary>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>Kiểm tra tên đăng nhập đã tồn tại trong hệ thống chưa.</summary>
        Task<bool> UsernameExistsAsync(string username);

        /// <summary>Thêm người dùng mới (đăng ký tài khoản).</summary>
        Task AddAsync(User user);

        /// <summary>Cập nhật thông tin người dùng.</summary>
        Task UpdateAsync(User user);

        /// <summary>Xóa người dùng theo Id.</summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>Đếm tổng số khách hàng (role = Customer).</summary>
        Task<int> CountCustomersAsync();
    }
}
