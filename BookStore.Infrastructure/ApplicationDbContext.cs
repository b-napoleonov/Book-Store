using BookStore.Infrastructure.Common.EntityConfiguration;
using BookStore.Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; } = null!;

        public DbSet<Book> Books { get; set; } = null!;

        public DbSet<BookOrder> BookOrders { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<CategoryBook> CategoryBooks { get; set; } = null!;

        public DbSet<Publisher> Publishers { get; set; } = null!;

        public DbSet<Rating> Ratings { get; set; } = null!;

        public DbSet<Review> Reviews { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;

        public DbSet<Warehouse> Warehouses { get; set; } = null!;

        public DbSet<WarehouseBook> WarehouseBooks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new BookOrderConfiguration());
            builder.ApplyConfiguration(new CategoryBookConfiguration());
            builder.ApplyConfiguration(new RatingConfiguration());
            builder.ApplyConfiguration(new WarehouseBookConfiguration());

            base.OnModelCreating(builder);
        }
    }
}