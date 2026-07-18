using Core.Database.Models;

namespace Core.Database.Interfaces
{
    /// <summary>
    /// Interface định nghĩa các phương thức truy xuất dữ liệu cho Feedback.
    /// </summary>
    public interface IFeedbackRepository
    {
        /// <summary>Lấy toàn bộ phản hồi, sắp xếp mới nhất trước.</summary>
        Task<List<Feedback>> GetAllAsync();

        /// <summary>Lấy phản hồi theo Id.</summary>
        Task<Feedback?> GetByIdAsync(int id);

        /// <summary>Lấy danh sách phản hồi của một người dùng cụ thể.</summary>
        Task<List<Feedback>> GetByUserIdAsync(int userId);

        /// <summary>Thêm phản hồi mới từ khách hàng.</summary>
        Task AddAsync(Feedback feedback);

        /// <summary>Admin trả lời một phản hồi.</summary>
        Task<bool> ReplyAsync(int feedbackId, string replyContent);

        /// <summary>Xóa phản hồi theo Id.</summary>
        Task<bool> DeleteAsync(int id);
    }
}
