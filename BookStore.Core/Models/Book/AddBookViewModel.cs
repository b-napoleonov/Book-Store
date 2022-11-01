using BookStore.Core.CustomAttributes;
using BookStore.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Book
{
    public class AddBookViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "{0} must be between exactly {1} characters.")]
        public string ISBN { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} must be between {2} and {1} characters.")]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "{0} must be between {2} and {1} characters.")]
        public string Description { get; set; } = null!;

        [Required]
        [CurrentYearValue(1900)]
        public int Year { get; set; }

        [Required]
        [Range(typeof(decimal), "0.0", "500.0", ConvertValueInInvariantCulture = true, ErrorMessage = "{0} must be between {2} and {1} characters.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 9999, ErrorMessage = "{0} must be between {2} and {1} characters.")]
        public int Pages { get; set; }

        [Required]
        public string ImageUrl { get; set; } = null!;

        public int AuthorId { get; set; }

        public IEnumerable<Author> Authors { get; set; } = null!;

        public int PublisherId { get; set; }

        public IEnumerable<Publisher> Publishers { get; set; } = null!;

        public int CategoryId { get; set; }

        public IEnumerable<Category> Categories { get; set; } = null!;

        public int WarehouseId { get; set; }

        public IEnumerable<Warehouse> Warehouses { get; set; } = null!;
    }
}
