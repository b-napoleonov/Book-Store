using BookStore.Core.Models.Rating;
using BookStore.Core.Models.Review;

namespace BookStore.Core.Models.Book
{
    public class DetailsBookViewModel
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

        public string ImageUrl { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string Publisher { get; set; } = null!;

        public IEnumerable<string> Categories { get; set; } = null!;

        public IEnumerable<DetailsReviewViewModel> Reviews { get; set; }

        public DetailsRatingViewModel Ratings { get; set; }
    }
}
