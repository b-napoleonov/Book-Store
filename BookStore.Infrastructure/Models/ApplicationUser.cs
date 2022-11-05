using BookStore.Infrastructure.Common.SoftDeleteBaseClass;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Infrastructure.Models
{
    public class ApplicationUser : IdentityUser, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Reviews = new HashSet<Review>();
            this.Orders = new HashSet<Order>();
        }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public int? RatingId { get; set; }

        [ForeignKey(nameof(RatingId))]
        public Rating? Rating { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
