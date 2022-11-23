using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Common.EntityConfiguration
{
    internal class BookOrderConfiguration : IEntityTypeConfiguration<BookOrder>
    {
        public void Configure(EntityTypeBuilder<BookOrder> builder)
        {
            builder.HasKey(k => new
            {
                k.BookId,
                k.OrderId
            });

            //builder.HasOne(e => e.Book)
            //    .WithMany(e => e.BookOrders)
            //    .HasForeignKey(e => e.BookId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
