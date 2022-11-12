using BookStore.Core.Models.Rating;

namespace BookStore.Core.Contracts
{
	public interface IRatingService
	{
		Task AddRating(AddRatingViewModel model, Guid bookId, string userId);
	}
}
