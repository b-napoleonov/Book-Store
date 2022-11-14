using BookStore.Core.Contracts;
using BookStore.Core.Models.Rating;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using LearnFast.Common;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services
{
    public class RatingService : IRatingService
    {
        private readonly IDeletableEntityRepository<Rating> ratingRepository;
        private readonly IUserService userService;
        private readonly IBookService bookService;

        public RatingService(
            IDeletableEntityRepository<Rating> _ratingRepository,
            IUserService _userService,
            IBookService _bookService)
        {
            ratingRepository = _ratingRepository;
            userService = _userService;
            bookService = _bookService;
        }

        public async Task AddRating(AddRatingViewModel model, Guid bookId, string userId)
        {
            var book = await bookService.GetBookByIdAsync(bookId);

            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidUser);
            }

            var ratingExists = await ratingRepository.AllAsNoTracking().AnyAsync(r => r.UserId == userId && r.BookId == bookId);

            if (ratingExists)
            {
                var existingRating = await ratingRepository
                    .All()
                    .Where(r => r.UserId == userId)
                    .FirstOrDefaultAsync();

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

            await ratingRepository.SaveChangesAsync();
        }
    }
}
