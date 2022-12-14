using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Common.EntityConfiguration
{
	internal class BookConfiguration : IEntityTypeConfiguration<Book>
	{
		public void Configure(EntityTypeBuilder<Book> builder)
		{
			builder.HasMany(b => b.Categories)
				.WithOne(b => b.Book)
				.HasForeignKey(b => b.BookId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(b => b.Author)
				.WithMany(b => b.Books)
				.HasForeignKey(b => b.AuthorId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(b => b.Publisher)
				.WithMany(b => b.Books)
				.HasForeignKey(b => b.PublisherId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
