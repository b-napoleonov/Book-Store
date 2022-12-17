using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Core.Models.Rating;
using BookStore.Core.Models.Review;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.Core.Services
{
    /// <summary>
    /// Main class who manages Books
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly ILogger<BookService> logger;

        public BookService(
            IDeletableEntityRepository<Book> _bookRepository,
            ILogger<BookService> _logger)
        {
            bookRepository = _bookRepository;
            logger = _logger;
        }

        /// <summary>
        /// Creates new book by given model and saves it to the database
        /// </summary>
        /// <param name="model">Contains information for the new book properties</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task AddBookAsync(AddBookViewModel model)
        {
            var book = new Book()
            {
                ISBN = model.ISBN,
                Title = model.Title,
                Description = model.Description,
                Year = model.Year,
                Price = model.Price,
                Pages = model.Pages,
                Quantity = model.Quantity,
                ImageUrl = model.ImageUrl,
                AuthorId = model.AuthorId,
                PublisherId = model.PublisherId
            };

            foreach (var categoryId in model.CategoryIds)
            {
                var categoryBook = new CategoryBook()
                {
                    Book = book,
                    CategoryId = categoryId,
                };

                book.Categories.Add(categoryBook);
            }

            try
            {
                await bookRepository.AddAsync(book);
                await bookRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddBookAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }

        }

        /// <summary>
        /// Gets all non-deleted books from the DB
        /// </summary>
        /// <returns>IEnumerable of AllBooksViewModel</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<IEnumerable<AllBooksViewModel>> GetAllBooksAsync()
        {
            List<AllBooksViewModel> result = new List<AllBooksViewModel>();

            try
            {
                result = await bookRepository
                    .AllAsNoTracking()
                    .Include(b => b.Ratings)
                    .OrderBy(b => b.Title)
                    .Select(b => new AllBooksViewModel
                    {
                        Id = b.Id,
                        Title = b.Title,
                        ImageUrl = b.ImageUrl,
                        Author = b.Author.Name,
                        Price = b.Price,
                        Rating = b.Ratings.Count > 0 ? b.Ratings.Average(r => r.UserRating) : 0
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetAllBooksAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }

            return result;
        }

        /// <summary>
        /// Gets single non-deleted book from the DB by bookId
        /// </summary>
        /// <param name="bookId">ID by which the book is searched</param>
        /// <returns>DetailsBookViewModel</returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<DetailsBookViewModel> GetBookAsync(Guid bookId)
        {
            Book book;

            try
            {
                book = await bookRepository
                    .AllAsNoTracking()
                    .Include(b => b.Author)
                    .Include(b => b.Publisher)
                    .Include(b => b.Categories)
                    .ThenInclude(b => b.Category)
                    .Where(x => x.Id == bookId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetBookAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            return new DetailsBookViewModel()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Year = book.Year,
                Price = book.Price,
                Pages = book.Pages,
                Quantity = book.Quantity,
                ImageUrl = book.ImageUrl,
                Author = book.Author.Name,
                Publisher = book.Publisher.Name,
                Categories = book.Categories.Select(c => c.Category.Name).ToList()
            };
        }

        /// <summary>
        /// Get non-deleted book by Category name
        /// </summary>
        /// <param name="categoryName">Category name by which the book is searched</param>
        /// <returns>IEnumerable of AllBooksViewModel</returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IEnumerable<AllBooksViewModel>> GetBooksByCategoryAsync(string categoryName)
        {
            var books = new List<Book>();

            try
            {
                books = await bookRepository
                    .AllAsNoTracking()
                    .Include(b => b.Categories)
                    .ThenInclude(cb => cb.Category)
                    .Include(b => b.Author)
                    .Include(b => b.Publisher)
                    .Include(b => b.Ratings)
                    .Where(b => b.Categories.Any(cb => cb.Category.Name == categoryName))
                    .OrderBy(b => b.Title)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetBooksByCategoryAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


            var models = new List<AllBooksViewModel>();

            foreach (var book in books)
            {
                var model = new AllBooksViewModel()
                {
                    Id = book.Id,
                    Title = book.Title,
                    ImageUrl = book.ImageUrl,
                    Author = book.Author.Name,
                    Price = book.Price,
                    Rating = book.Ratings.Count > 0 ? book.Ratings.Average(r => r.UserRating) : 0,
                };

                models.Add(model);
            }

            if (books.Count <= 0)
            {
                throw new ArgumentException(GlobalExceptions.CategoryNotFound);
            }

            return models;
        }

        /// <summary>
        /// Get non-deleted book by Author name
        /// </summary>
        /// <param name="authorName">Author name by which the book is searched</param>
        /// <returns>IEnumerable of AllBooksViewModel</returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IEnumerable<AllBooksViewModel>> GetBooksByAuthorAsync(string authorName)
        {
            var books = new List<Book>();

            try
            {
                books = await bookRepository
                    .AllAsNoTracking()
                    .Include(b => b.Categories)
                    .ThenInclude(cb => cb.Category)
                    .Include(b => b.Author)
                    .Include(b => b.Publisher)
                    .Include(b => b.Ratings)
                    .Where(b => b.Author.Name == authorName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetBooksByAuthorAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }

            var models = new List<AllBooksViewModel>();

            foreach (var book in books)
            {
                var model = new AllBooksViewModel()
                {
                    Id = book.Id,
                    Title = book.Title,
                    ImageUrl = book.ImageUrl,
                    Author = book.Author.Name,
                    Price = book.Price,
                    Rating = book.Ratings.Count > 0 ? book.Ratings.Average(r => r.UserRating) : 0,
                };

                models.Add(model);
            }

            if (books.Count <= 0)
            {
                throw new ArgumentException(GlobalExceptions.AuthrorNotFound);
            }

            return models;
        }

        /// <summary>
        /// Get non-deleted book by Publisher name
        /// </summary>
        /// <param name="publisherName">Publisher name by which the book is searched</param>
        /// <returns>IEnumerable of AllBooksViewModel</returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IEnumerable<AllBooksViewModel>> GetBooksByPublisherAsync(string publisherName)
        {
            var books = new List<Book>();

            try
            {
                books = await bookRepository
                    .AllAsNoTracking()
                    .Include(b => b.Categories)
                    .ThenInclude(cb => cb.Category)
                    .Include(b => b.Author)
                    .Include(b => b.Publisher)
                    .Include(b => b.Ratings)
                    .Where(b => b.Publisher.Name == publisherName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetBooksByPublisherAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


            if (books.Count <= 0)
            {
                throw new ArgumentException(GlobalExceptions.PublisherNotFound);
            }

            var models = new List<AllBooksViewModel>();

            foreach (var book in books)
            {
                var model = new AllBooksViewModel()
                {
                    Id = book.Id,
                    Title = book.Title,
                    ImageUrl = book.ImageUrl,
                    Author = book.Author.Name,
                    Price = book.Price,
                    Rating = book.Ratings.Count > 0 ? book.Ratings.Average(r => r.UserRating) : 0,
                };

                models.Add(model);
            }

            return models;
        }

        public async Task<Book> GetBookByIdAsync(Guid bookId)
        {
            Book book;

            try
            {
                book = await bookRepository
                    .All()
                    .Where(b => b.Id == bookId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetBookByIdAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


            return book;
        }

        public async Task<double> GetBookRating(Guid bookId)
        {
            Book book;

            try
            {
                book = await bookRepository
                    .AllAsNoTracking()
                    .Include(b => b.Ratings)
                    .Where(b => b.Id == bookId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetBookRating), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            return book.Ratings.Average(r => r.UserRating);
        }

        public async Task<IEnumerable<DetailsReviewViewModel>> GetBookReviewsAsync(Guid bookId)
        {
            Book book;

            try
            {
                book = await bookRepository
                    .AllAsNoTracking()
                    .Include(b => b.Reviews)
                    .ThenInclude(b => b.User)
                    .Where(b => b.Id == bookId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetBookReviewsAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            var reviews = new List<DetailsReviewViewModel>();

            foreach (var review in book.Reviews.Where(r => !r.IsDeleted))
            {
                var model = new DetailsReviewViewModel
                {
                    ReviewId = review.Id,
                    OwnerId = review.UserId,
                    UserReview = review.UserReview,
                    UserEmail = review.User.Email
                };

                reviews.Add(model);
            }

            return reviews;
        }

        public async Task<DetailsRatingViewModel> GetBookRatingDetailsAsync(Guid bookId)
        {
            Book book;

            try
            {
                book = await bookRepository
                    .AllAsNoTracking()
                    .Include(b => b.Ratings)
                    .Where(b => b.Id == bookId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetBookRatingDetailsAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            var rating = new DetailsRatingViewModel
            {
                Rating = book.Ratings.Count > 0 ? book.Ratings.Average(r => r.UserRating) : 0,
                FiveStarRating = book.Ratings.Count(r => r.UserRating == 5),
                FourStarRating = book.Ratings.Count(r => r.UserRating >= 4 && r.UserRating < 5),
                ThreeStarRating = book.Ratings.Count(r => r.UserRating >= 3 && r.UserRating < 4),
                TwoStarRating = book.Ratings.Count(r => r.UserRating >= 2 && r.UserRating < 3),
                OneStarRating = book.Ratings.Count(r => r.UserRating >= 1 && r.UserRating < 2),
                RatingsCount = book.Ratings.Count,
            };

            return rating;
        }

        public async Task RemoveBook(Guid bookId)
        {
            Book book;

            try
            {
                book = await bookRepository
                    .All()
                    .Where(b => b.Id == bookId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(RemoveBook), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            bookRepository.Delete(book);

            try
            {
                await bookRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(RemoveBook), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }
        }

        public async Task EditBookAsync(EditBookViewModel model, Guid bookId)
        {
            Book book;

            try
            {
                book = await bookRepository
                    .All()
                    .Include(b => b.Categories)
                    .Where(b => b.Id == bookId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(EditBookAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            book.ISBN = model.ISBN;
            book.Title = model.Title;
            book.Description = model.Description;
            book.Year = model.Year;
            book.Price = model.Price;
            book.Pages = model.Pages;
            book.Quantity = model.Quantity;
            book.ImageUrl = model.ImageUrl;
            book.AuthorId = model.AuthorId;
            book.PublisherId = model.PublisherId;

            //foreach (var category in book.Categories)
            //{
            //    book.Categories.Remove(category);
            //}

            //Works with clear, throws error with foreach
            book.Categories.Clear();

            foreach (var categoryId in model.CategoryIds)
            {
                var categoryBook = new CategoryBook()
                {
                    BookId = bookId,
                    CategoryId = categoryId,
                };

                book.Categories.Add(categoryBook);
            }

            try
            {
                await bookRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(EditBookAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }
        }

        public async Task<EditBookViewModel> GetBookDataForEditAsync(Guid bookId)
        {
            Book book;

            try
            {
                book = await bookRepository
                    .AllAsNoTracking()
                    .Where(b => b.Id == bookId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetBookDataForEditAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            return new EditBookViewModel
            {
                Id = book.Id,
                ISBN = book.ISBN,
                Title = book.Title,
                Description = book.Description,
                Year = book.Year,
                Price = book.Price,
                Pages = book.Pages,
                Quantity = book.Quantity,
                ImageUrl = book.ImageUrl,
            };
        }

        public async Task<IEnumerable<HomeBookViewModel>> GetLastThreeBooksAsync()
        {
            var books = new List<Book>();

            try
            {
                books = await bookRepository
                    .AllAsNoTracking()
                    .OrderBy(b => b.Title)
                    .Take(3)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetLastThreeBooksAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


            var model = new List<HomeBookViewModel>();

            foreach (var book in books)
            {
                model.Add(new HomeBookViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    ImageUrl = book.ImageUrl
                });
            }

            return model;
        }
    }
}
