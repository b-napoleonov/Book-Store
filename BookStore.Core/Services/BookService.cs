using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Core.Models.Rating;
using BookStore.Core.Models.Review;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services
{
    public class BookService : IBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public BookService(IDeletableEntityRepository<Book> _bookRepository)
        {
            bookRepository = _bookRepository;
        }

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

            await bookRepository.AddAsync(book);
            await bookRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<AllBooksViewModel>> GetAllBooksAsync()
        {
            return await bookRepository
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

        public async Task<DetailsBookViewModel> GetBookAsync(Guid bookId)
        {
            var book = await bookRepository
                .AllAsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Categories)
                .ThenInclude(b => b.Category)
                .Where(x => x.Id == bookId)
                .FirstOrDefaultAsync();

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

        public async Task<IEnumerable<AllBooksViewModel>> GetBooksByCategoryAsync(string categoryName)
        {
            var books = await bookRepository
                .AllAsNoTracking()
                .Include(b => b.Categories)
                .ThenInclude(cb => cb.Category)
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Ratings)
                .Where(b => b.Categories.Select(cb => cb.Category.Name == categoryName).FirstOrDefault())
                .ToListAsync();

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

        public async Task<IEnumerable<AllBooksViewModel>> GetBooksByAuthorAsync(string authorName)
        {
            var books = await bookRepository
                .AllAsNoTracking()
                .Include(b => b.Categories)
                .ThenInclude(cb => cb.Category)
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Ratings)
                .Where(b => b.Author.Name == authorName)
                .ToListAsync();

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

        public async Task<IEnumerable<AllBooksViewModel>> GetBooksByPublisherAsync(string publisherName)
        {
            var books = await bookRepository
                .AllAsNoTracking()
                .Include(b => b.Categories)
                .ThenInclude(cb => cb.Category)
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Ratings)
                .Where(b => b.Publisher.Name == publisherName)
                .ToListAsync();

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
            var book = await bookRepository
                .All()
                .Where(b => b.Id == bookId)
                .FirstOrDefaultAsync();

            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            return book;
        }

        public async Task<double> GetBookRating(Guid bookId)
        {
            var book = await bookRepository
                .AllAsNoTracking()
                .Include(b => b.Ratings)
                .Where(b => b.Id == bookId)
                .FirstOrDefaultAsync();

            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            return book.Ratings.Average(r => r.UserRating);
        }

        public async Task<IEnumerable<DetailsReviewViewModel>> GetBookReviewsAsync(Guid bookId)
        {
            var book = await bookRepository
                .AllAsNoTracking()
                .Include(b => b.Reviews)
                .ThenInclude(b => b.User)
                .Where(b => b.Id == bookId)
                .FirstOrDefaultAsync();

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
            var book = await bookRepository
                .AllAsNoTracking()
                .Include(b => b.Ratings)
                .Where(b => b.Id == bookId)
                .FirstOrDefaultAsync();

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
            var book = await bookRepository
                .All()
                .Where(b => b.Id == bookId)
                .FirstOrDefaultAsync();

            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            bookRepository.Delete(book);
            await bookRepository.SaveChangesAsync();
        }

        public async Task EditBookAsync(EditBookViewModel model, Guid bookId)
        {
            var book = await bookRepository
                .All()
                .Include(b => b.Categories)
                .Where(b => b.Id == bookId)
                .FirstOrDefaultAsync();

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

            bookRepository.Update(book);

            await bookRepository.SaveChangesAsync();
        }

        public async Task<EditBookViewModel> GetBookDataForEditAsync(Guid bookId)
        {
            var book = await bookRepository
                .AllAsNoTracking()
                .Where(b => b.Id == bookId)
                .FirstOrDefaultAsync();

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
            var books = await bookRepository
                .AllAsNoTracking()
                .OrderBy(b => b.Title)
                .Take(3)
                .ToListAsync();

            var model = new List<HomeBookViewModel>();

            foreach (var book in books)
            {
                model.Add(new HomeBookViewModel
                {
                    Title = book.Title,
                    ImageUrl = book.ImageUrl
                });
            }

            return model;
        }
    }
}
