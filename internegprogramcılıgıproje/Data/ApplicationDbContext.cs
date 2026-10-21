using Microsoft.EntityFrameworkCore;
using internegprogramcılıgıproje.Models;

namespace internegprogramcılıgıproje.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Answer> Answers => Set<Answer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Soru - Kategori İlişkisi
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Category)
                .WithMany(c => c.Questions)
                .HasForeignKey(q => q.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Soru - Kullanıcı İlişkisi
            modelBuilder.Entity<Question>()
                .HasOne(q => q.User)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cevap - Soru İlişkisi
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cevap - Kullanıcı İlişkisi
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.User)
                .WithMany(u => u.Answers)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Başlangıç Admin Hesabı (Seed Data)
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Id = 1,
                    UserName = "admin",
                    Email = "admin@sorucevap.com",
                    Password = "AdminPassword123!",
                    FullName = "Sistem Yöneticisi",
                    Role = "Admin",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Başlangıç Kategorileri (Seed Data)
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Yazılım & Programlama",
                    Description = "Kodlama, diller ve kütüphaneler",
                    IsActive = true
                },
                new Category
                {
                    Id = 2,
                    Name = "Veritabanı",
                    Description = "SQL, NoSQL, ORM araçları",
                    IsActive = true
                },
                new Category
                {
                    Id = 3,
                    Name = "Web Geliştirme",
                    Description = "Frontend, Backend ve API mimarileri",
                    IsActive = true
                }
            );
        }
    }
}