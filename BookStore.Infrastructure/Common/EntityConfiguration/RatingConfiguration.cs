using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Common.EntityConfiguration
{
    /// <summary>
    /// Configuration for Rating Entity
    /// </summary>
    internal class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.HasOne(r => r.User)
                .WithOne(au => au.Rating)
                .HasForeignKey<ApplicationUser>(au => au.RatingId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
