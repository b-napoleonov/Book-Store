using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Review;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.Core.Services
{
    /// <summary>
    /// Main class who manages Reviews
    /// </summary>
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

        /// <summary>
        /// Add new review with given data and saves it to the DB
        /// </summary>
        /// <param name="model">View model with data for creating the review</param>
        /// <param name="bookId">ID of the book to be checked</param>
        /// <param name="userId">ID of the user to be checked</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ApplicationException"></exception>
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

        /// <summary>
        /// Deletes review by given ID
        /// </summary>
        /// <param name="reviewId">ID of the review to be checked</param>
        /// <param name="userId">ID of the user to be checked</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Gets all non-deleted reviews from the DB
        /// </summary>
        /// <returns>IEnumerable of Review</returns>
        /// <exception cref="ApplicationException"></exception>
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

        /// <summary>
        /// Gets review by given ID
        /// </summary>
        /// <param name="reviewId">ID of the review to be searched</param>
        /// <returns>Review</returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Gets all non-deleted reviews by given user
        /// </summary>
        /// <param name="userId">ID of the user to be searched</param>
        /// <returns>IEnumerable of Review</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ApplicationException"></exception>
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

        /// <summary>
        /// Updates given review
        /// </summary>
        /// <param name="model">View model with data to update the review</param>
        /// <param name="reviewId">ID of the review to be searched</param>
        /// <param name="userId">ID of the user to be searched</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentException"></exception>
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
