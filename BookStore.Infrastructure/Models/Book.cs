using BookStore.Common;
using BookStore.Infrastructure.Common.SoftDeleteBaseClass;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Infrastructure.Models
{
    /// <summary>
    /// Book DB Entity
    /// </summary>
    public class Book : IDeletableEntity
    {
        private const string MoneyTypeName = "money";
        private const int PricePrecision = 18;
        private const int PriceScale = 2;

        public Book()
        {
            this.Id = Guid.NewGuid();
            this.Categories = new HashSet<CategoryBook>();
            this.Ratings = new HashSet<Rating>();
            this.Reviews = new HashSet<Review>();
            this.BookOrders = new HashSet<BookOrder>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(GlobalConstants.ISBNMaxLength)]
        public string ISBN { get; set; } = null!;

        [Required]
        [MaxLength(GlobalConstants.TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(GlobalConstants.DescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        public int Year { get; set; }

        [Required]
        [Column(TypeName = MoneyTypeName)]
        [Precision(PricePrecision, PriceScale)]
        public decimal Price { get; set; }

        [Required]
        public int Pages { get; set; }

        [Required]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public Author Author { get; set; } = null!;

        [Required]
        public int PublisherId { get; set; }

        [ForeignKey(nameof(PublisherId))]
        public Publisher Publisher { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public ICollection<CategoryBook> Categories { get; set; }

        public ICollection<Rating> Ratings { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public ICollection<BookOrder> BookOrders { get; set; }
    }
}
