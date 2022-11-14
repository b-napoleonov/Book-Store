using BookStore.Infrastructure.Common.SoftDeleteBaseClass;
using LearnFast.Common;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Infrastructure.Models
{
    public class Publisher : IDeletableEntity
    {
        public Publisher()
        {
            this.Books = new HashSet<Book>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(GlobalConstants.PublisherNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(GlobalConstants.PublisherPhoneMaxLength)]
        public string Phone { get; set; } = null!;

        [Required]
        [MaxLength(GlobalConstants.PublisherEmailMaxLength)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(GlobalConstants.PublisherURLMaxLength)]
        public string URL { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
