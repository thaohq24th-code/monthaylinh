using System;
using Core.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Database.Data
{
    /// <summary>
    /// DbContext chính của hệ thống, quản lý kết nối tới SQL Server và tất cả các Entity.
    /// Database: sellitemQuanLy
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>Bảng sản phẩm.</summary>
        public DbSet<Product> Products { get; set; } = null!;

        /// <summary>Bảng bài viết / tin tức.</summary>
        public DbSet<Article> Articles { get; set; } = null!;

        /// <summary>Bảng người dùng (khách hàng + admin).</summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>Bảng nhân viên.</summary>
        public DbSet<Staff> Staffs { get; set; } = null!;

        /// <summary>Bảng đơn hàng.</summary>
        public DbSet<Order> Orders { get; set; } = null!;

        /// <summary>Bảng chi tiết đơn hàng.</summary>
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

        /// <summary>Bảng phản hồi của khách hàng.</summary>
        public DbSet<Feedback> Feedbacks { get; set; } = null!;

        /// <summary>Bảng bài viết cộng đồng của người dùng.</summary>
        public DbSet<Post> Posts { get; set; } = null!;

        /// <summary>Bảng bình luận dưới bài viết.</summary>
        public DbSet<PostComment> PostComments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ----- Cấu hình ràng buộc duy nhất (Unique constraints) -----
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // ----- Cấu hình quan hệ giữa các bảng -----

            // Một User có nhiều Order. Khi xóa User, không cho xóa Order liên quan (Restrict)
            // để bảo toàn dữ liệu lịch sử đơn hàng.
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Một Order có nhiều OrderDetail. Khi xóa Order, xóa luôn các OrderDetail con (Cascade).
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Một Product có thể nằm trong nhiều OrderDetail. Không cho xóa Product nếu đã
            // từng được đặt hàng (Restrict) để bảo toàn lịch sử đơn hàng.
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Một User có nhiều Feedback. Khi xóa User, xóa luôn Feedback liên quan (Cascade).
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Một User có nhiều Post. Khi xóa User, xóa luôn Post (Cascade).
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Một Post có nhiều PostComment. Khi xóa Post, xóa luôn Comment (Cascade).
            modelBuilder.Entity<PostComment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Một User có nhiều PostComment. Khi xóa User, không xóa Comment theo (NoAction).
            modelBuilder.Entity<PostComment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // ----- Seed Data -----
            // Mật khẩu được băm sẵn bằng BCrypt cho tài khoản mặc định.
            // admin / 123456 và customer / 123456
            // Lưu ý: hash dưới đây được sinh cố định (fixed salt) để đảm bảo Migration
            // luôn tạo ra cùng một giá trị mỗi khi chạy lại (cần thiết cho HasData).
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "admin123" ,
                    FullName = "Quản trị viên",
                    Phone = "0817139878",
                    Email = "admin@garangion.vn",
                    Role = "Admin",
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new User
                {
                    Id = 2,
                    Username = "customer",
                    PasswordHash = "admin123",
                    FullName = "Khách hàng demo",
                    Phone = "0900000000",
                    Email = "customer@garangion.vn",
                    Role = "Customer",
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Gà Rán Giòn Cay (2 miếng)",
                    Description = "Gà rán áo bột giòn tan, tẩm sốt cay đặc trưng Hàn Quốc.",
                    Price = 75000,
                    Type = "Gà rán",
                    Image = "https://images.unsplash.com/photo-1626082927389-6cd097cdc6ec?q=80&w=1000&auto=format&fit=crop",
                    Featured = true,
                    Stock = 100,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Product
                {
                    Id = 2,
                    Name = "Gà Rán Truyền Thống (3 miếng)",
                    Description = "Phần gà nguyên vị giòn rụm bên ngoài, mọng nước bên trong.",
                    Price = 95000,
                    Type = "Gà rán",
                    Image = "https://images.unsplash.com/photo-1569058242253-92a9c755a0ec?q=80&w=1000&auto=format&fit=crop",
                    Featured = true,
                    Stock = 100,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Product
                {
                    Id = 3,
                    Name = "Burger Gà Phô Mai",
                    Description = "Vỏ bánh mềm, phi-lê gà giòn, sốt mayonnaise và phô mai chảy béo ngậy.",
                    Price = 55000,
                    Type = "Burger",
                    Image = "https://images.unsplash.com/photo-1606755962773-d324e0a13086?q=80&w=1000&auto=format&fit=crop",
                    Featured = true,
                    Stock = 50,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Product
                {
                    Id = 4,
                    Name = "Khoai Tây Chiên Lắc Phô Mai",
                    Description = "Khoai tây chiên vàng giòn rắc lớp phô mai thơm lừng.",
                    Price = 35000,
                    Type = "Ăn vặt",
                    Image = "https://images.unsplash.com/photo-1576107232684-1279f390859f?q=80&w=1000&auto=format&fit=crop",
                    Featured = false,
                    Stock = 80,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Product
                {
                    Id = 5,
                    Name = "Coca Cola (Ly lớn)",
                    Description = "Thức uống giải khát không thể thiếu khi ăn gà rán.",
                    Price = 20000,
                    Type = "Đồ uống",
                    Image = "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?q=80&w=1000&auto=format&fit=crop",
                    Featured = false,
                    Stock = 200,
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            modelBuilder.Entity<Article>().HasData(
                new Article
                {
                    Id = 1,
                    Title = "Bí quyết tạo nên lớp vỏ gà giòn tan chuẩn vị",
                    Category = "Khám phá",
                    Image = "https://images.unsplash.com/photo-1626082896492-766af4eb6501?q=80&w=1000&auto=format&fit=crop",
                    Content = "Để có một miếng gà chiên hoàn hảo, nhiệt độ dầu và công thức bột là hai yếu tố quyết định. Tại hệ thống của chúng tôi, gà luôn được tẩm ướp trong 12 tiếng và chiên ngập dầu ở nhiệt độ chuẩn 170 độ C.",
                    CreatedDate = new DateTime(2024, 3, 10)
                },
                new Article
                {
                    Id = 2,
                    Title = "Khai trương cửa hàng mới, tặng 100 combo Gà & Nước",
                    Category = "Tin tức",
                    Image = "https://images.unsplash.com/photo-1512152272829-e3139592d56f?q=80&w=1000&auto=format&fit=crop",
                    Content = "Nhân dịp mở cơ sở mới, 100 khách hàng đầu tiên đến check-in sẽ được nhận ngay combo ăn thử hoàn toàn miễn phí. Chương trình diễn ra duy nhất vào thứ 7 tuần này.",
                    CreatedDate = new DateTime(2024, 4, 2)
                },
                new Article
                {
                    Id = 3,
                    Title = "Ngày hội Gia đình: Mua 1 Gà nguyên con tặng 2 Burger",
                    Category = "Khuyến mãi",
                    Image = "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?q=80&w=1000&auto=format&fit=crop",
                    Content = "Thưởng thức bữa tối đầm ấm cùng gia đình với gói ưu đãi siêu khủng. Đặc biệt, có đồ chơi tặng kèm cho các bé nhỏ.",
                    CreatedDate = new DateTime(2024, 5, 15)
                }
            );
        }
    }
}
