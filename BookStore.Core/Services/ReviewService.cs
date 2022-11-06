using BookStore.Core.Contracts;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services
{
	public class ReviewService : IReviewService
	{
		private readonly IDeletableEntityRepository<Review> reviewRepository;

		public ReviewService(IDeletableEntityRepository<Review> _reviewRepository)
		{
			reviewRepository = _reviewRepository;
		}

		public async Task<IEnumerable<Review>> GetAllReviewsAsync()
		{
			return await reviewRepository
				.AllAsNoTracking()
				.ToListAsync();
		}

		public async Task<IEnumerable<Review>> GetUserReviewsAsync(string userId)
		{
			return await reviewRepository
				.AllAsNoTracking()
				.Where(r => r.UserId == userId)
				.ToListAsync();
		}
	}
}
