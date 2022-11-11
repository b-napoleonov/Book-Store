namespace BookStore.Core.Models.Review
{
	public class DetailsReviewViewModel
	{
		public int ReviewId { get; set; }

        public string OwnerId { get; set; } = null!;

        public string UserReview { get; set; } = null!;

        public string UserEmail { get; set; } = null!;
    }
}
