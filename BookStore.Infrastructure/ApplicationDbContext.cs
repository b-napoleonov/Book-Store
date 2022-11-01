using BookStore.Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace BookStore.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<CategoryBook> CategoryBooks { get; set; }

        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<Rating> Ratings { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<WarehouseBook> WarehouseBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CategoryBook>()
                .HasKey(k => new
                {
                    k.BookId,
                    k.CategoryId
                });

            builder.Entity<WarehouseBook>()
                .HasKey(k => new
                {
                    k.BookId,
                    k.WarehouseId
                });

            builder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithOne(au => au.Rating)
                .HasForeignKey<ApplicationUser>(au => au.RatingId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ApplicationUser>()
                .HasOne(au => au.Rating)
                .WithOne(r => r.User)
                .HasForeignKey<Rating>(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);
        }
    }
}