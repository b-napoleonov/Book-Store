using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Common.EntityConfiguration
{
	internal class OrderConfiguration : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.HasMany(o => o.BookOrders)
				.WithOne(o => o.Order)
				.HasForeignKey(o => o.OrderId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(o => o.Customer)
				.WithMany(o => o.Orders)
				.HasForeignKey(o => o.CustomerId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
