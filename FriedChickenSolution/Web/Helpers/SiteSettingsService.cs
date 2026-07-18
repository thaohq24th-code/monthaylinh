using Web.Models;
using System.Text.Json;

namespace Web.Helpers
{
    public class SiteSettingsService
    {
        private readonly string _settingsPath;
        private SiteSettings? _cached;
        private DateTime _cacheTime;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        public SiteSettingsService(IWebHostEnvironment env)
        {
            // Lưu settings vào ContentRootPath để tránh lỗi quyền ghi trên Somee.com
            _settingsPath = Path.Combine(env.ContentRootPath, "site-settings.json");
        }

        public SiteSettings GetSettings()
        {
            // Cache 30 giây để tránh đọc file quá nhiều
            if (_cached != null && (DateTime.Now - _cacheTime).TotalSeconds < 30)
                return _cached;

            if (!File.Exists(_settingsPath))
            {
                _cached = new SiteSettings();
                _cacheTime = DateTime.Now;
                return _cached;
            }

            try
            {
                var json = File.ReadAllText(_settingsPath);
                _cached = JsonSerializer.Deserialize<SiteSettings>(json) ?? new SiteSettings();
                _cacheTime = DateTime.Now;
                return _cached;
            }
            catch
            {
                // Nếu file bị corrupt, dùng default settings
                _cached = new SiteSettings();
                _cacheTime = DateTime.Now;
                return _cached;
            }
        }

        public void SaveSettings(SiteSettings settings)
        {
            try
            {
                settings.LastUpdated = DateTime.Now;
                var json = JsonSerializer.Serialize(settings, _jsonOptions);
                File.WriteAllText(_settingsPath, json);
                _cached = settings;
                _cacheTime = DateTime.Now;
            }
            catch (UnauthorizedAccessException)
            {
                // Somee.com có thể giới hạn quyền ghi - cache trong memory
                _cached = settings;
                _cacheTime = DateTime.Now;
            }
            catch (IOException)
            {
                // Lỗi I/O - cache trong memory
                _cached = settings;
                _cacheTime = DateTime.Now;
            }
        }
    }
}
