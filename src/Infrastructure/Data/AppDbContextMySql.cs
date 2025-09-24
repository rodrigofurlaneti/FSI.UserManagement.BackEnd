using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Data
{
    public class AppDbContextMySql : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public AppDbContextMySql(DbContextOptions<AppDbContextMySql> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(u => u.Email).IsUnique();
                b.Property(u => u.Name).IsRequired().HasMaxLength(100);
                b.Property(u => u.Email).IsRequired().HasMaxLength(150);
                b.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);

                b.Property(u => u.Id)
                    .HasColumnType("char(36)")
                    .ValueGeneratedOnAdd();

                b.Property(u => u.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("NOW()");

                b.Property(u => u.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("NOW() ON UPDATE NOW()");

                b.ToTable("Users");
            });
        }
    }
}