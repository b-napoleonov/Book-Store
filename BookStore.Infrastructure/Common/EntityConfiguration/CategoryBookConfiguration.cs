using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Common.EntityConfiguration
{
    internal class CategoryBookConfiguration : IEntityTypeConfiguration<CategoryBook>
    {
        public void Configure(EntityTypeBuilder<CategoryBook> builder)
        {
            builder.HasKey(k => new
                {
                    k.BookId,
                    k.CategoryId
                });
        }
    }
}
