using BookStore.Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }

        public DbSet<AuthorBook> AuthorBooks { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<ShoppingBasket> ShoppingBaskets { get; set; }

        public DbSet<ShoppingBasketBook> ShoppingBasketBooks { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<WarehouseBook> WarehouseBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AuthorBook>()
                .HasKey(k => new
                {
                    k.AuthorId,
                    k.BookId
                });

            builder.Entity<ShoppingBasketBook>()
                .HasKey(k => new
                {
                    k.BookId,
                    k.ShoppingBasketId
                });

            builder.Entity<WarehouseBook>()
                .HasKey(k => new
                {
                    k.BookId,
                    k.WarehouseId
                });

            base.OnModelCreating(builder);
        }
    }
}