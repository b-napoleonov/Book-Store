using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Review;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.Core.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IDeletableEntityRepository<Review> reviewRepository;
        private readonly IBookService bookService;
        private readonly IUserService userService;
        private readonly ILogger<ReviewService> logger;

        public ReviewService(
            IDeletableEntityRepository<Review> _reviewRepository,
            IBookService _bookService,
            IUserService _userService,
            ILogger<ReviewService> _logger)
        {
            reviewRepository = _reviewRepository;
            bookService = _bookService;
            userService = _userService;
            logger = _logger;
        }

        public async Task AddReviewAsync(ReviewViewModel model, Guid bookId, string userId)
        {
            var book = await bookService.GetBookByIdAsync(bookId);

            if (book == null)
            {
                throw new NullReferenceException(GlobalExceptions.InvalidBookId);
            }

            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new NullReferenceException(GlobalExceptions.InvalidUser);
            }

            var review = new Review
            {
                BookId = bookId,
                UserId = userId,
                UserReview = model.UserReview
            };

            try
            {
                await reviewRepository.AddAsync(review);
                await reviewRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddReviewAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }
        }

        public async Task DeleteReviewAsync(int reviewId, string userId)
        {
            Review review;

            try
            {
                review = await reviewRepository
                .All()
                .Where(r => r.Id == reviewId)
                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddReviewAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToFetch, ex);
            }

            if (review == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidReviewId);
            }

            if (review.UserId != userId)
            {
                throw new ArgumentException(GlobalExceptions.NotReviewOwner);
            }

            try
            {
                reviewRepository.Delete(review);
                await reviewRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddReviewAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            var reviews = new List<Review>();

            try
            {
                reviews = await reviewRepository
                .AllAsNoTracking()
                .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddReviewAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }

            return reviews;
        }

        public async Task<Review> GetReviewByIdAsync(int reviewId)
        {
            Review review;

            try
            {
                review = await reviewRepository
                .AllAsNoTracking()
                .Where(r => r.Id == reviewId)
                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddReviewAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }

            if (review == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidReviewId);
            }

            return review;
        }

        public async Task<IEnumerable<Review>> GetUserReviewsAsync(string userId)
        {
            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new NullReferenceException(GlobalExceptions.InvalidUser);
            }

            var reviews = new List<Review>();

            try
            {
                reviews = await reviewRepository
                .AllAsNoTracking()
                .Where(r => r.UserId == userId)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddReviewAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }

            return reviews;
        }

        public async Task<Review> UpdateReviewAsync(ReviewViewModel model, int reviewId, string userId)
        {
            Review review;

            try
            {
                review = await reviewRepository
                .All()
                .Where(r => r.Id == reviewId)
                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddReviewAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToFetch, ex);
            }

            if (review == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidReviewId);
            }

            if (review.UserId != userId)
            {
                throw new ArgumentException(GlobalExceptions.NotReviewOwner);
            }

            review.UserReview = model.UserReview;

            try
            {
                await reviewRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddReviewAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }

            return review;
        }
    }
}
