namespace BookStore.Core.Models.Rating
{
    /// <summary>
    /// Details about given rating
    /// </summary>
    public class DetailsRatingViewModel
    {
        public double Rating { get; set; }

        public int FiveStarRating { get; set; }

        public int FourStarRating { get; set; }

        public int ThreeStarRating { get; set; }

        public int TwoStarRating { get; set; }

        public int OneStarRating { get; set; }

        public int RatingsCount { get; set; }
    }
}
