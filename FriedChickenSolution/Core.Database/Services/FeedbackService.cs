using Core.Database.Interfaces;
using Core.Database.Models;

namespace Core.Database.Services
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ liên quan tới Feedback: gửi phản hồi, admin trả lời.
    /// </summary>
    public class FeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        /// <summary>Lấy toàn bộ phản hồi (dùng cho Admin).</summary>
        public async Task<List<Feedback>> GetAllFeedbacksAsync()
        {
            return await _feedbackRepository.GetAllAsync();
        }

        /// <summary>Lấy phản hồi theo Id.</summary>
        public async Task<Feedback?> GetFeedbackByIdAsync(int id)
        {
            return await _feedbackRepository.GetByIdAsync(id);
        }

        /// <summary>Lấy lịch sử phản hồi của một khách hàng cụ thể.</summary>
        public async Task<List<Feedback>> GetFeedbacksByUserAsync(int userId)
        {
            return await _feedbackRepository.GetByUserIdAsync(userId);
        }

        /// <summary>Khách hàng gửi phản hồi mới.</summary>
        public async Task<(bool Success, string Message)> SubmitFeedbackAsync(int userId, string title, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return (false, "Nội dung phản hồi không được để trống");
            }

            var feedback = new Feedback
            {
                UserId = userId,
                Title = title,
                Content = content
            };

            await _feedbackRepository.AddAsync(feedback);
            return (true, "Cảm ơn bạn đã gửi phản hồi!");
        }

        /// <summary>Admin trả lời một phản hồi của khách hàng.</summary>
        public async Task<(bool Success, string Message)> ReplyFeedbackAsync(int feedbackId, string replyContent)
        {
            if (string.IsNullOrWhiteSpace(replyContent))
            {
                return (false, "Vui lòng nhập nội dung phản hồi");
            }

            var replied = await _feedbackRepository.ReplyAsync(feedbackId, replyContent);
            return replied
                ? (true, "Đã gửi phản hồi thành công")
                : (false, "Không tìm thấy phản hồi cần trả lời");
        }

        /// <summary>Xóa phản hồi theo Id.</summary>
        public async Task<(bool Success, string Message)> DeleteFeedbackAsync(int id)
        {
            var deleted = await _feedbackRepository.DeleteAsync(id);
            return deleted
                ? (true, "Đã xóa phản hồi")
                : (false, "Không tìm thấy phản hồi cần xóa");
        }
    }
}
