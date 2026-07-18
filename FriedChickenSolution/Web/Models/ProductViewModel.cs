namespace Web.Models
{
    /// <summary>
    /// ViewModel hiển thị sản phẩm trên các trang khách hàng (trang chủ, danh sách, chi tiết).
    /// Không truyền Entity Product trực tiếp xuống View.
    /// </summary>
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Image { get; set; }
        public bool Featured { get; set; }
        public int Stock { get; set; }

        /// <summary>Giá đã định dạng theo VNĐ, dùng để hiển thị trực tiếp lên View.</summary>
        public string FormattedPrice => Price.ToString("#,##0") + "đ";
    }
}
