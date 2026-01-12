using BestProductManager.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BestProductManager.Api.Data
{
    /// <summary>
    /// DbContext trung tâm dùng cho ASP.NET Core Identity và các bảng nghiệp vụ (Products).
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Khởi tạo ApplicationDbContext với cấu hình DbContextOptions.
        /// </summary>
        /// <param name="options">Tùy chọn cấu hình DbContext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Tập thực thể Product được ánh xạ sang bảng Products trong cơ sở dữ liệu.
        /// </summary>
        public DbSet<Product> Products { get; set; } = null!;

        /// <summary>
        /// Cấu hình mô hình EF Core, bao gồm tùy biến tên bảng cho ASP.NET Core Identity và các bảng nghiệp vụ.
        /// </summary>
        /// <param name="builder">Đối tượng ModelBuilder dùng để cấu hình mapping.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Gọi base để Identity thiết lập cấu hình mặc định (khóa, chỉ mục, quan hệ,...).
            base.OnModelCreating(builder);

            // Cấu hình tên bảng cho các thực thể Identity theo yêu cầu.
            builder.Entity<ApplicationUser>()
                .ToTable("Users");

            builder.Entity<IdentityRole>()
                .ToTable("SecRole");

            builder.Entity<IdentityUserRole<string>>()
                .ToTable("SecUserRoles");

            builder.Entity<IdentityUserClaim<string>>()
                .ToTable("SecUserClaims");

            builder.Entity<IdentityUserLogin<string>>()
                .ToTable("SecUserLogins");

            builder.Entity<IdentityRoleClaim<string>>()
                .ToTable("SecRoleClaims");

            builder.Entity<IdentityUserToken<string>>()
                .ToTable("SecUserTokens");

            // Cấu hình explicit để bảng sản phẩm có tên Products.
            builder.Entity<Product>()
                .ToTable("Products");
        }
    }
}
