namespace Web.Models
{
    /// <summary>
    /// ViewModel hiển thị bài viết / tin tức trên các trang khách hàng.
    /// </summary>
    public class ArticleViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        /// <summary>Tóm tắt ngắn nội dung bài viết để hiển thị trong danh sách (card).</summary>
        public string Summary => Content.Length > 150 ? Content.Substring(0, 150) + "..." : Content;
    }
}
