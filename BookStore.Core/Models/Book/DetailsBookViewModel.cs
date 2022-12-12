using BookStore.Core.Contracts;
using BookStore.Core.Models.Rating;
using BookStore.Core.Models.Review;

namespace BookStore.Core.Models.Book
{
    /// <summary>
    /// Details about the book for book details view
    /// </summary>
    public class DetailsBookViewModel : IBookModel
    {
        public DetailsBookViewModel()
        {
            this.Categories = new List<string>();
            this.Reviews = new List<DetailsReviewViewModel>();
            this.Ratings = new DetailsRatingViewModel();
        }

        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int Year { get; set; }

        public decimal Price { get; set; }

        public int Pages { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string Publisher { get; set; } = null!;

        public IEnumerable<string> Categories { get; set; }

        public IEnumerable<DetailsReviewViewModel> Reviews { get; set; }

        public DetailsRatingViewModel Ratings { get; set; }
    }
}
