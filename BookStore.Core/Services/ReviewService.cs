using BookStore.Common;
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

		public async Task AddReviewAsync(ReviewViewModel model, Guid bookId, string userId)
		{
            try
            {
                var book = await bookService.GetBookByIdAsync(bookId);
                var user = await userService.GetUserByIdAsync(userId);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException(ae.Message);
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

		public async Task DeleteReviewAsync(int reviewId, string userId)
		{
			var review = await reviewRepository
				.All()
				.Where(r => r.Id == reviewId)
				.FirstOrDefaultAsync();

			if (review == null)
			{
				throw new ArgumentException(GlobalExceptions.InvalidReviewId);
			}

			if (review.UserId != userId)
			{
                throw new ArgumentException(GlobalExceptions.NotReviewOwner);
            }

			reviewRepository.Delete(review);
			await reviewRepository.SaveChangesAsync();
		}

		public async Task<IEnumerable<Review>> GetAllReviewsAsync()
		{
			return await reviewRepository
				.AllAsNoTracking()
				.ToListAsync();
		}

		public async Task<Review> GetReviewByIdAsync(int reviewId)
		{
			var review = await reviewRepository
				.AllAsNoTracking()
				.Where(r => r.Id == reviewId)
				.FirstOrDefaultAsync();

			if (review == null)
			{
                throw new ArgumentException(GlobalExceptions.InvalidReviewId);
            }

			return review;
		}

		public async Task<IEnumerable<Review>> GetUserReviewsAsync(string userId)
		{
            try
			{
                var user = await userService.GetUserByIdAsync(userId);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException(ae.Message);
            }

            return await reviewRepository
				.AllAsNoTracking()
				.Where(r => r.UserId == userId)
				.ToListAsync();
		}

		public async Task<Review> UpdateReviewAsync(ReviewViewModel model, int reviewId, string userId)
		{
            var review = await reviewRepository
                .All()
                .Where(r => r.Id == reviewId)
                .FirstOrDefaultAsync();

            if (review == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidReviewId);
            }

            if (review.UserId != userId)
            {
                throw new ArgumentException(GlobalExceptions.NotReviewOwner);
            }

            review.UserReview = model.UserReview;

			await reviewRepository.SaveChangesAsync();

			return review;
        }
	}
}
