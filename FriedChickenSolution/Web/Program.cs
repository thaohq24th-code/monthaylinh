using Core.Database.Data;
using Core.Database.Interfaces;
using Core.Database.Repositories;
using Core.Database.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Web.Helpers;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. ĐĂNG KÝ DBCONTEXT (Entity Framework Core + SQL Server)
// ============================================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(15),
            errorNumbersToAdd: null)));

// ============================================================
// 2. ĐĂNG KÝ REPOSITORY PATTERN (Dependency Injection)
// ============================================================
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();

// ============================================================
// 3. ĐĂNG KÝ SERVICES (Business Logic Layer)
// ============================================================
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ArticleService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<StaffService>();
builder.Services.AddScoped<FeedbackService>();

// ============================================================
// 4. ĐĂNG KÝ HELPER (Upload ảnh, Site Settings)
// ============================================================
builder.Services.AddScoped<FileUploadHelper>();
builder.Services.AddSingleton<Web.Helpers.SiteSettingsService>();

// ============================================================
// 5. CẤU HÌNH COOKIE AUTHENTICATION + ROLE-BASED AUTHORIZATION
// ============================================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "FriedChicken.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        // SameAsRequest: hỗ trợ cả HTTP và HTTPS trên Somee.com
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// ============================================================
// 6. ĐĂNG KÝ MVC + SESSION
// ============================================================
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// ============================================================
// 7. ĐĂNG KÝ LOGGING
// ============================================================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
if (!builder.Environment.IsDevelopment())
{
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
    builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
}

var app = builder.Build();

// ============================================================
// 8. TỰ ĐỘNG MIGRATE DATABASE KHI KHỞI ĐỘNG
//    Đảm bảo các bảng đã tồn tại trên Somee.com trước khi nhận request.
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        logger.LogInformation("Database migration hoàn thành.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Lỗi khi migrate database lúc khởi động. Ứng dụng vẫn tiếp tục.");
        // Không throw exception - cho phép app vẫn khởi động được
    }
}

// ============================================================
// 9. PIPELINE CẤU HÌNH MIDDLEWARE
// ============================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

// KHÔNG dùng UseHttpsRedirection() - Somee.com free hosting dùng HTTP
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ============================================================
// 10. ĐỊNH NGHĨA ROUTE
// ============================================================
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
