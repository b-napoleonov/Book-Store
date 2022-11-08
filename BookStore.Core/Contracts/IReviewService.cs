using BookStore.Core.Models.Review;
using BookStore.Infrastructure.Models;

namespace BookStore.Core.Contracts
{
	public interface IReviewService
	{
		Task<IEnumerable<Review>> GetUserReviewsAsync(string userId);

		Task<IEnumerable<Review>> GetAllReviewsAsync();

		Task AddReviewAsync(AddReviewViewModel model, Guid bookId, string userId);
	}
}
