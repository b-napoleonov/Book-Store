using BookStore.Core.Models.Review;

namespace BookStore.Core.Models.Book
{
    public class DetailsBookViewModel
    {
        public DetailsBookViewModel()
        {
            this.Categories = new List<string>();
        }

        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int Year { get; set; }

        public decimal Price { get; set; }

        public int Pages { get; set; }

        public double Rating { get; set; }

        public int FiveStarRating { get; set; }

        public int FourStarRating { get; set; }

        public int ThreeStarRating { get; set; }

        public int TwoStarRating { get; set; }

        public int OneStarRating { get; set; }

        public int RatingsCount { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string Publisher { get; set; } = null!;

        public IEnumerable<string> Categories { get; set; } = null!;

        public IEnumerable<DetailsReviewViewModel> Reviews { get; set; } = null!;
    }
}
