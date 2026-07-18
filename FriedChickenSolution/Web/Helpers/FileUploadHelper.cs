namespace Web.Helpers
{
    /// <summary>
    /// Helper xử lý upload file ảnh vào wwwroot/uploads.
    /// Trả về đường dẫn tương đối (vd: /uploads/abc123.jpg) để lưu vào SQL Server.
    /// </summary>
    public class FileUploadHelper
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly string[] _allowedExtensions;
        private readonly long _maxFileSizeBytes;

        public FileUploadHelper(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
            _allowedExtensions = _configuration.GetSection("UploadSettings:AllowedExtensions").Get<string[]>()
                ?? new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var maxMb = _configuration.GetValue<int>("UploadSettings:MaxFileSizeMb", 5);
            _maxFileSizeBytes = maxMb * 1024L * 1024L;
        }

        /// <summary>
        /// Lưu file ảnh được upload vào wwwroot/uploads với tên file duy nhất (Guid).
        /// Trả về (Success, Message, RelativePath). RelativePath null nếu thất bại.
        /// </summary>
        public async Task<(bool Success, string Message, string? RelativePath)> SaveImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "Không có file nào được chọn", null);
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                return (false, $"Định dạng không hỗ trợ. Chỉ chấp nhận: {string.Join(", ", _allowedExtensions)}", null);
            }

            if (file.Length > _maxFileSizeBytes)
            {
                return (false, $"File quá lớn. Tối đa {_maxFileSizeBytes / (1024 * 1024)}MB", null);
            }

            try
            {
                // Xác định thư mục uploads
                string webRootPath = _env.WebRootPath;
                if (string.IsNullOrEmpty(webRootPath))
                {
                    // Fallback: dùng ContentRootPath/wwwroot nếu WebRootPath null
                    webRootPath = Path.Combine(_env.ContentRootPath, "wwwroot");
                }

                var uploadsFolder = Path.Combine(webRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/{uniqueFileName}";
                return (true, "Upload ảnh thành công", relativePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, $"Không có quyền ghi file. Vui lòng liên hệ hỗ trợ. ({ex.Message})", null);
            }
            catch (IOException ex)
            {
                return (false, $"Lỗi lưu file: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi không xác định khi upload: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Xóa file ảnh khỏi wwwroot/uploads dựa trên đường dẫn tương đối.
        /// Không báo lỗi nếu file không tồn tại (vd: ảnh là URL ngoài, không phải file local).
        /// </summary>
        public void DeleteImage(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || !relativePath.StartsWith("/uploads/"))
            {
                return;
            }

            try
            {
                var webRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                var fullPath = Path.Combine(webRootPath, relativePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch
            {
                // Bỏ qua lỗi khi xóa - không ảnh hưởng chức năng chính
            }
        }
    }
}
