using BookStore.Infrastructure.Models;

namespace BookStore.Core.Contracts
{
	public interface IReviewService
	{
		Task<IEnumerable<Review>> GetUserReviewsAsync(string userId);

		Task<IEnumerable<Review>> GetAllReviewsAsync();
	}
}
