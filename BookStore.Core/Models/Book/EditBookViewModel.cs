using BookStore.Common;
using BookStore.Core.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Book
{
    public class EditBookViewModel
    {
        private const string ImageURLDisplayName = "Image URL";
        private const string AuthorDisplayName = "Author";
        private const string PublisherDisplayName = "Publisher";
        private const string CategoryDisplayName = "Category";

        public EditBookViewModel()
        {
            this.Authors = new List<BookAuthorViewModel>();
            this.Publishers = new List<BookPublisherViewModel>();
            this.Categories = new List<BookCategoryViewModel>();
            this.CategoryIds = new List<int>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(GlobalConstants.ISBNMaxLength, MinimumLength = GlobalConstants.ISBNMinLength, ErrorMessage = GlobalExceptions.StringFieldsErrorMessage)]
        public string ISBN { get; set; } = null!;

        [Required]
        [StringLength(GlobalConstants.TitleMaxLength, MinimumLength = GlobalConstants.TitleMinLength, ErrorMessage = GlobalExceptions.StringFieldsErrorMessage)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(GlobalConstants.DescriptionMaxLength, MinimumLength = GlobalConstants.DescriptionMinLength, ErrorMessage = GlobalExceptions.StringFieldsErrorMessage)]
        public string Description { get; set; } = null!;

        [Required]
        [CurrentYearValue(GlobalConstants.CurrentYearValue)]
        public int Year { get; set; }

        [Required]
        [Range(typeof(decimal), GlobalConstants.PriceMinRange, GlobalConstants.PriceMaxRange, ConvertValueInInvariantCulture = true, ErrorMessage = GlobalExceptions.NumberFieldsErrorMessage)]
        public decimal Price { get; set; }

        [Required]
        [Range(GlobalConstants.PagesMinRange, GlobalConstants.PagesMaxRange, ErrorMessage = GlobalExceptions.NumberFieldsErrorMessage)]
        public int Pages { get; set; }

        [Required]
        [Range(GlobalConstants.QuantityMinRange, GlobalConstants.QuantityMaxRange, ErrorMessage = GlobalExceptions.NumberFieldsErrorMessage)]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = ImageURLDisplayName)]
        public string ImageUrl { get; set; } = null!;

        [Display(Name = AuthorDisplayName)]
        public int AuthorId { get; set; }

        public IEnumerable<BookAuthorViewModel> Authors { get; set; }

        [Display(Name = PublisherDisplayName)]
        public int PublisherId { get; set; }

        public IEnumerable<BookPublisherViewModel> Publishers { get; set; }

        [Display(Name = CategoryDisplayName)]
        public IEnumerable<int> CategoryIds { get; set; }

        public IEnumerable<BookCategoryViewModel> Categories { get; set; }
    }
}
