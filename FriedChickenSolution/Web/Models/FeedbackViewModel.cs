using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    /// <summary>
    /// ViewModel hiển thị phản hồi của khách hàng trên trang Liên hệ.
    /// </summary>
    public class FeedbackViewModel
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? AdminReply { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReplyDate { get; set; }
    }

    /// <summary>
    /// ViewModel cho form gửi phản hồi từ khách hàng.
    /// </summary>
    public class FeedbackCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn chủ đề")]
        [Display(Name = "Chủ đề")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập nội dung phản hồi")]
        [Display(Name = "Nội dung")]
        public string Content { get; set; } = string.Empty;
    }
}
