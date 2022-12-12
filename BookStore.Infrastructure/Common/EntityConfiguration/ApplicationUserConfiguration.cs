using BookStore.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Common.EntityConfiguration
{
    /// <summary>
    /// Configuration for ApplicationUser Entity
    /// </summary>
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.UserName)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(60)
                .IsRequired();

            builder.HasOne(au => au.Rating)
                .WithOne(r => r.User)
                .HasForeignKey<Rating>(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            //builder.HasData(CreateAdmin());
        }

        //private List<ApplicationUser> CreateAdmin()
        //{
        //    var users = new List<ApplicationUser>();
        //    var hasher = new PasswordHasher<ApplicationUser>();

        //    var user = new ApplicationUser()
        //    {
        //        Id = "e8bcd2d8-a517-423b-8adc-ac938e532312",
        //        UserName = "Admin",
        //        NormalizedUserName = "ADMIN",
        //        Email = "admin@mail.com",
        //        NormalizedEmail = "ADMIN@MAIL.COM"
        //    };

        //    user.PasswordHash =
        //         hasher.HashPassword(user, "Admin123!");

        //    users.Add(user);

        //    return users;
        //}
    }
}
