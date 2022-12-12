using BookStore.Core.Models.Review;
using BookStore.Infrastructure.Models;

namespace BookStore.Core.Contracts
{
    /// <summary>
    /// Interface for managing Reviews
    /// </summary>
    public interface IReviewService
	{
		Task<IEnumerable<Review>> GetUserReviewsAsync(string userId);

		Task<IEnumerable<Review>> GetAllReviewsAsync();

		Task<Review> GetReviewByIdAsync(int reviewId);

		Task AddReviewAsync(ReviewViewModel model, Guid bookId, string userId);

		Task DeleteReviewAsync(int reviewId, string userId);

		Task<Review> UpdateReviewAsync(ReviewViewModel model, int reviewId, string userId);
	}
}
