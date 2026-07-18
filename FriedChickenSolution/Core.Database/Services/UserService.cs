using Core.Database.Interfaces;
using Core.Database.Models;

namespace Core.Database.Services
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ liên quan tới User: đăng ký, xác thực đăng nhập, quản lý tài khoản.
    /// Mật khẩu luôn được băm bằng BCrypt trước khi lưu hoặc so sánh, không bao giờ lưu plain-text.
    /// </summary>
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>Lấy toàn bộ người dùng (dùng cho Admin).</summary>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        /// <summary>Lấy người dùng theo Id.</summary>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        /// <summary>Lấy người dùng theo tên đăng nhập.</summary>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        /// <summary>
        /// Xác thực thông tin đăng nhập. So sánh mật khẩu nhập vào với hash đã lưu bằng BCrypt.
        /// Trả về User nếu đăng nhập thành công, null nếu sai tài khoản hoặc mật khẩu.
        /// </summary>
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return null;
            }

            return user.PasswordHash == password ? user : null;
        }

        /// <summary>
        /// Đăng ký tài khoản khách hàng mới. Kiểm tra trùng username, băm mật khẩu trước khi lưu.
        /// Tài khoản đăng ký qua form luôn có Role = "Customer".
        /// </summary>
        public async Task<(bool Success, string Message)> RegisterAsync(string username, string password, string? fullName, string? phone, string? email)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return (false, "Tên đăng nhập và mật khẩu là bắt buộc");
            }

            if (password.Length < 6)
            {
                return (false, "Mật khẩu phải có ít nhất 6 ký tự");
            }

            var exists = await _userRepository.UsernameExistsAsync(username);
            if (exists)
            {
                return (false, "Tên đăng nhập đã tồn tại!");
            }

            var newUser = new User
            {
                Username = username,
                PasswordHash = password,
                FullName = fullName,
                Phone = phone,
                Email = email,
                Role = "Customer"
            };

            await _userRepository.AddAsync(newUser);
            return (true, "Đăng ký thành công! Hãy đăng nhập.");
        }

        /// <summary>Cập nhật thông tin tài khoản (không cho đổi Role qua đường này).</summary>
        public async Task<(bool Success, string Message)> UpdateProfileAsync(int userId, string? fullName, string? phone, string? email)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, "Không tìm thấy người dùng");
            }

            user.FullName = fullName;
            user.Phone = phone;
            user.Email = email;

            await _userRepository.UpdateAsync(user);
            return (true, "Đã cập nhật thông tin tài khoản");
        }

        /// <summary>Admin xóa một người dùng (không cho xóa tài khoản Admin).</summary>
        public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return (false, "Không tìm thấy người dùng cần xóa");
            }

            if (user.Role == "Admin")
            {
                return (false, "Không thể xóa tài khoản quản trị viên");
            }

            await _userRepository.DeleteAsync(id);
            return (true, "Đã xóa khách hàng");
        }

        /// <summary>Cập nhật ảnh đại diện (avatar) của người dùng.</summary>
        public async Task<(bool Success, string Message)> UpdateAvatarAsync(int userId, string avatarUrl)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "Không tìm thấy người dùng");

            user.AvatarUrl = avatarUrl;
            await _userRepository.UpdateAsync(user);
            return (true, "Đã cập nhật ảnh đại diện");
        }

        /// <summary>Đếm tổng số khách hàng (dùng cho Dashboard).</summary>
        public async Task<int> CountCustomersAsync()
        {
            return await _userRepository.CountCustomersAsync();
        }
    }
}
