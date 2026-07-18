using System.Text.Json;

namespace Web.Models
{
    public class SiteSettings
    {
        public string SiteName { get; set; } = "Gà Rán Giòn Rụm";
        public string SiteLogoIcon { get; set; } = "fa-drumstick-bite";
        public string SiteLogoUrl { get; set; } = "";
        public string ThemePreset { get; set; } = "spicy";
        public string PrimaryColor { get; set; } = "#d32f2f";
        public string AccentColor { get; set; } = "#ff9800";
        public string BgWarm { get; set; } = "#fff9f0";
        public string TextDark { get; set; } = "#212121";
        public string SiteTagline { get; set; } = "Gà rán chuẩn vị Hàn Quốc, giòn rụm khó cưỡng";
        public string HeroImageUrl { get; set; } = "";
        public string FooterAddress { get; set; } = "123 Đường Gà Rán, Quận 1, TP.HCM";
        public string FooterPhone { get; set; } = "(028) 8888 9999";
        public string FooterEmail { get; set; } = "hello@garan.vn";
        public bool MaintenanceMode { get; set; } = false;
        public string SocialFacebook { get; set; } = "#";
        public string SocialInstagram { get; set; } = "#";
        public string SocialTiktok { get; set; } = "#";
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
