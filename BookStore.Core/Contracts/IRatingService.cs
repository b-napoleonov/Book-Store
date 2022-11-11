using BookStore.Core.Models.Rating;

namespace BookStore.Core.Contracts
{
	public interface IRatingService
	{
		Task AddRating(RatingViewModel model, Guid bookId, string userId);
	}
}
