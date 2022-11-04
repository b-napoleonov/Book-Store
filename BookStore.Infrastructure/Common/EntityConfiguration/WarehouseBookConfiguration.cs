using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Common.EntityConfiguration
{
    internal class WarehouseBookConfiguration : IEntityTypeConfiguration<WarehouseBook>
    {
        public void Configure(EntityTypeBuilder<WarehouseBook> builder)
        {
            builder.HasKey(k => new
                 {
                     k.BookId,
                     k.WarehouseId
                 });
        }
    }
}
