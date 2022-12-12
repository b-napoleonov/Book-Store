using BookStore.Core.Models.Rating;

namespace BookStore.Core.Contracts
{
    /// <summary>
    /// Interface for managing Ratings
    /// </summary>
    public interface IRatingService
	{
		Task AddRating(AddRatingViewModel model, Guid bookId, string userId);
	}
}
