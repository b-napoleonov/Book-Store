using BookStore.Core.Contracts;
using BookStore.Core.Models.Review;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services
{
	public class ReviewService : IReviewService
	{
		private readonly IDeletableEntityRepository<Review> reviewRepository;
		private readonly IBookService bookService;
		private readonly IUserService userService;

		public ReviewService(
			IDeletableEntityRepository<Review> _reviewRepository,
			IBookService _bookService,
			IUserService _userService)
		{
			reviewRepository = _reviewRepository;
			bookService = _bookService;
			userService = _userService;
		}

		public async Task AddReviewAsync(AddReviewViewModel model, Guid bookId, string userId)
		{
			var book = await bookService.GetBookByIdAsync(bookId);

			if (book == null)
			{
                throw new ArgumentException("Invalid Book.");
            }

			var user = await userService.GetUserByIdAsync(userId);

			if (user == null)
			{
                throw new ArgumentException("Invalid User.");
            }

			var review = new Review
			{
				BookId = bookId,
				UserId = userId,
				UserReview = model.UserReview
			};

			await reviewRepository.AddAsync(review);
			await reviewRepository.SaveChangesAsync();
		}

		public async Task<IEnumerable<Review>> GetAllReviewsAsync()
		{
			return await reviewRepository
				.AllAsNoTracking()
				.ToListAsync();
		}

		public async Task<IEnumerable<Review>> GetUserReviewsAsync(string userId)
		{
            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException("Invalid User.");
            }

            return await reviewRepository
				.AllAsNoTracking()
				.Where(r => r.UserId == userId)
				.ToListAsync();
		}
	}
}
