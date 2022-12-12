using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Rating;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.Core.Services
{
    /// <summary>
    /// Main class who manages Ratings
    /// </summary>
    public class RatingService : IRatingService
    {
        private readonly IDeletableEntityRepository<Rating> ratingRepository;
        private readonly IUserService userService;
        private readonly IBookService bookService;
        private readonly ILogger<RatingService> logger;

        public RatingService(
            IDeletableEntityRepository<Rating> _ratingRepository,
            IUserService _userService,
            IBookService _bookService,
            ILogger<RatingService> _logger)
        {
            ratingRepository = _ratingRepository;
            userService = _userService;
            bookService = _bookService;
            logger = _logger;
        }

        /// <summary>
        /// Adds new rating with given model and saves it to the Db
        /// </summary>
        /// <param name="model">View model with data for creating the rating</param>
        /// <param name="bookId">ID of the book to be checked</param>
        /// <param name="userId">ID of the user to be checked</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task AddRating(AddRatingViewModel model, Guid bookId, string userId)
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

            var ratingExists = false;

            try
            {
                ratingExists = await ratingRepository
                    .AllAsNoTracking()
                    .AnyAsync(r => r.UserId == userId && r.BookId == bookId);
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddRating), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToFetch, ex);
            }

            if (ratingExists)
            {
                Rating existingRating;

                try
                {
                    existingRating = await ratingRepository
                    .All()
                    .Where(r => r.UserId == userId)
                    .FirstOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(nameof(AddRating), ex);

                    throw new ApplicationException(GlobalExceptions.DatabaseFailedToFetch, ex);
                }

                if (existingRating == null)
                {
                    throw new ArgumentException(GlobalExceptions.InvalidRating);
                }

                existingRating.UserRating = model.UserRating;
            }
            else
            {
                var rating = new Rating
                {
                    BookId = bookId,
                    UserId = userId,
                    UserRating = model.UserRating
                };

                await ratingRepository.AddAsync(rating);
            }

            try
            {
                await ratingRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddRating), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }
        }
    }
}
