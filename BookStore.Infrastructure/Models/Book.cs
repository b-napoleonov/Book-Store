using BookStore.Infrastructure.Common.SoftDeleteBaseClass;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Infrastructure.Models
{
    public class Book : IDeletableEntity
    {
        public Book()
        {
            this.Id = Guid.NewGuid();
            this.WarehouseBooks = new HashSet<WarehouseBook>();
            this.Categories = new HashSet<CategoryBook>();
            this.Ratings = new HashSet<Rating>();
            this.Reviews = new HashSet<Review>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(13)]
        public string ISBN { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = null!;

        [Required]
        public int Year { get; set; }

        //TODO: Try adding migration for that
        [Required]
        [Column(TypeName = "money")]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Required]
        public int Pages { get; set; }

        [Required]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public int AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public Author Author { get; set; }

        [Required]
        public int PublisherId { get; set; }

        [ForeignKey(nameof(PublisherId))]
        public Publisher Publisher { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public ICollection<CategoryBook> Categories { get; set; }

        public ICollection<WarehouseBook> WarehouseBooks { get; set; }

        public ICollection<Rating> Ratings { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
