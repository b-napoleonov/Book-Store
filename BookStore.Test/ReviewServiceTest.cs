using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Review;
using BookStore.Core.Services;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace BookStore.Test
{
    /// <summary>
    /// Tests for Review Service
    /// </summary>
    public class ReviewServiceTest
    {
        private const int ExpectedReviewCount = 2;

        private ServiceProvider serviceProvider;
        private InMemoryDbContext dbContext;
        private Book book;
        private ApplicationUser user;
        private int reviewId;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IDeletableEntityRepository<Review>, DeletableEntityRepository<Review>>()
                .AddSingleton<IDeletableEntityRepository<Book>, DeletableEntityRepository<Book>>()
                .AddSingleton<IDeletableEntityRepository<ApplicationUser>, DeletableEntityRepository<ApplicationUser>>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<ILogger<UserService>, Logger<UserService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .AddSingleton<IBookService, BookService>()
                .AddSingleton<ILogger<BookService>, Logger<BookService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .AddSingleton<IReviewService, ReviewService>()
                .AddSingleton<ILogger<ReviewService>, Logger<ReviewService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .BuildServiceProvider();

            var ratingRepository = serviceProvider.GetService<IDeletableEntityRepository<Review>>();
            var userRepository = serviceProvider.GetService<IDeletableEntityRepository<ApplicationUser>>();
            var bookRepository = serviceProvider.GetService<IDeletableEntityRepository<Book>>();

            user = await SeedDbAsync(userRepository);
            book = await SeedDbAsync(bookRepository);
            await SeedDbAsync(ratingRepository, book.Id, user.Id);
        }

        [Test]
        public void AddReviewAsyncThrowsErrorIfDatabaseFailedToSave()
        {
            var repo = new Mock<IDeletableEntityRepository<Review>>();
            repo.Setup(r => r.SaveChangesAsync())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToSave));

            var model = new ReviewViewModel();

            var logger = new Mock<ILogger<ReviewService>>();
            var bookService = new Mock<IBookService>();
            bookService.Setup(s => s.GetBookByIdAsync(book.Id).Result)
                .Returns(book);

            var userService = new Mock<IUserService>();
            userService.Setup(s => s.GetUserByIdAsync(user.Id).Result)
                .Returns(user);

            IReviewService categoryService = new ReviewService(repo.Object, bookService.Object, userService.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await categoryService.AddReviewAsync(model, book.Id, user.Id), GlobalExceptions.DatabaseFailedToFetch);
        }

        [Test]
        public async Task AddReviewAsyncShouldCreateNewReview()
        {
            var model = new ReviewViewModel
            {
                UserReview = ""
            };

            var service = serviceProvider.GetService<IReviewService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Review>>();

            await service.AddReviewAsync(model, book.Id, user.Id);

            Assert.That(await repo.AllAsNoTracking().CountAsync(), Is.EqualTo(ExpectedReviewCount));
        }

        [Test]
        public void DeleteReviewAsyncThrowsErrorIfReviewIsNotFound()
        {
            var service = serviceProvider.GetService<IReviewService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.DeleteReviewAsync(111, "Random user"), GlobalExceptions.InvalidReviewId);
        }

        [Test]
        public void DeleteReviewAsyncThrowsErrorIfUserIsNotTheOwner()
        {
            var service = serviceProvider.GetService<IReviewService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.DeleteReviewAsync(reviewId, "Random user"), GlobalExceptions.NotReviewOwner);
        }

        [Test]
        public async Task DeleteReviewAsyncRemovesTheReview()
        {
            var service = serviceProvider.GetService<IReviewService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Review>>();

            await service.DeleteReviewAsync(reviewId, user.Id);

            Assert.That(await repo.AllAsNoTracking().CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public void DeleteReviewAsyncThrowsErrorIfDatabaseFailedToFetch()
        {
            var repo = new Mock<IDeletableEntityRepository<Review>>();
            repo.Setup(r => r.All())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToFetch));

            var logger = new Mock<ILogger<ReviewService>>();
            var userService = new Mock<IUserService>();
            var bookService = new Mock<IBookService>();

            IReviewService categoryService = new ReviewService(repo.Object, bookService.Object, userService.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await categoryService.DeleteReviewAsync(1, user.Id), GlobalExceptions.DatabaseFailedToFetch);
        }

        [Test]
        public void DeleteReviewAsyncThrowsErrorIfDatabaseFailedToSave()
        {
            var repo = new Mock<IDeletableEntityRepository<Review>>();
            repo.Setup(r => r.SaveChangesAsync())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToSave));

            var logger = new Mock<ILogger<ReviewService>>();
            var userService = new Mock<IUserService>();
            var bookService = new Mock<IBookService>();

            IReviewService categoryService = new ReviewService(repo.Object, bookService.Object, userService.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await categoryService.DeleteReviewAsync(1, user.Id), GlobalExceptions.DatabaseFailedToSave);
        }

        [Test]
        public async Task GetAllReviewsAsyncReturnCorrectData()
        {
            var service = serviceProvider.GetService<IReviewService>();

            var actual = await service.GetAllReviewsAsync();
            var expected = new List<Review>
            {
                new Review
                {
                    Id = 1,
                    BookId = book.Id,
                    UserId = user.Id,
                    UserReview = "Some test Review"
                }
            };

            var actualJson = JsonConvert.SerializeObject(actual);
            var expectedJson = JsonConvert.SerializeObject(expected);

            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public void GetAllReviewsAsyncThrowsErrorIfDatabaseFailedToSave()
        {
            var repo = new Mock<IDeletableEntityRepository<Review>>();
            repo.Setup(r => r.AllAsNoTracking())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToSave));

            var logger = new Mock<ILogger<ReviewService>>();
            var userService = new Mock<IUserService>();
            var bookService = new Mock<IBookService>();

            IReviewService categoryService = new ReviewService(repo.Object, bookService.Object, userService.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await categoryService.GetAllReviewsAsync(), GlobalExceptions.DatabaseFailedToFetch);
        }

        [Test]
        public async Task GetReviewByIdAsyncThrowsExceptionIfIdIsInvalid()
        {
            var service = serviceProvider.GetService<IReviewService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.GetReviewByIdAsync(111), GlobalExceptions.InvalidReviewId);
        }

        [Test]
        public void GetReviewByIdAsyncThrowsErrorIfDatabaseFailedToFetch()
        {
            var repo = new Mock<IDeletableEntityRepository<Review>>();
            repo.Setup(r => r.AllAsNoTracking())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToSave));

            var logger = new Mock<ILogger<ReviewService>>();
            var userService = new Mock<IUserService>();
            var bookService = new Mock<IBookService>();

            IReviewService categoryService = new ReviewService(repo.Object, bookService.Object, userService.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await categoryService.GetReviewByIdAsync(1), GlobalExceptions.DatabaseFailedToFetch);
        }

        [Test]
        public async Task GetReviewByIdAsyncReturnsCorrectData()
        {
            var service = serviceProvider.GetService<IReviewService>();

            var actual = await service.GetReviewByIdAsync(reviewId);
            var expected = new Review
            {
                Id = 1,
                BookId = book.Id,
                UserId = user.Id,
                UserReview = "Some test Review"
            };

            var actualJson = JsonConvert.SerializeObject(actual);
            var expectedJson = JsonConvert.SerializeObject(expected);

            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public async Task GetUserReviewsAsyncReturnsCorrectData()
        {
            var service = serviceProvider.GetService<IReviewService>();

            var actual = await service.GetUserReviewsAsync(user.Id);
            var expected = new List<Review>
            {
                new Review
                {
                    Id = 1,
                    BookId = book.Id,
                    UserId = user.Id,
                    UserReview = "Some test Review"
                }
            };

            var actualJson = JsonConvert.SerializeObject(actual);
            var expectedJson = JsonConvert.SerializeObject(expected);

            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public void GetUserReviewsAsyncThrowsErrorIfDatabaseFailedToSave()
        {
            var repo = new Mock<IDeletableEntityRepository<Review>>();
            repo.Setup(r => r.AllAsNoTracking())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToSave));

            var logger = new Mock<ILogger<ReviewService>>();
            var bookService = new Mock<IBookService>();
            bookService.Setup(s => s.GetBookByIdAsync(book.Id).Result)
                .Returns(book);

            var userService = new Mock<IUserService>();
            userService.Setup(s => s.GetUserByIdAsync(user.Id).Result)
                .Returns(user);

            IReviewService categoryService = new ReviewService(repo.Object, bookService.Object, userService.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await categoryService.GetUserReviewsAsync("peshoId"), GlobalExceptions.DatabaseFailedToFetch);
        }

        [Test]
        public async Task UpdateReviewAsyncThrowsErrorIfReviewIsNotFound()
        {
            var model = new ReviewViewModel
            {
                UserReview = ""
            };

            var service = serviceProvider.GetService<IReviewService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateReviewAsync(model, 111, "Random User"), GlobalExceptions.InvalidReviewId);
        }

        [Test]
        public async Task UpdateReviewAsyncThrowsErrorIfUserIsNotTheOwner()
        {
            var model = new ReviewViewModel
            {
                UserReview = ""
            };

            var service = serviceProvider.GetService<IReviewService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateReviewAsync(model, reviewId, "Random User"), GlobalExceptions.NotReviewOwner);
        }

        [Test]
        public async Task UpdateReviewAsyncModifyReviewCorrectly()
        {
            var model = new ReviewViewModel
            {
                UserReview = "New Review"
            };

            var service = serviceProvider.GetService<IReviewService>();
            var actual = await service.UpdateReviewAsync(model, reviewId, user.Id);

            Assert.That(actual.UserReview, Is.EqualTo(model.UserReview));
        }

        [Test]
        public void UpdateReviewAsyncThrowsErrorIfDatabaseFailedToFetch()
        {
            var repo = new Mock<IDeletableEntityRepository<Review>>();
            repo.Setup(r => r.AllAsNoTracking())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToSave));

            var logger = new Mock<ILogger<ReviewService>>();
            var userService = new Mock<IUserService>();
            var bookService = new Mock<IBookService>();

            IReviewService categoryService = new ReviewService(repo.Object, bookService.Object, userService.Object, logger.Object);

            var model = new ReviewViewModel();

            Assert.ThrowsAsync<ApplicationException>(async () => await categoryService.UpdateReviewAsync(model, 1, "peshoId"), GlobalExceptions.DatabaseFailedToFetch);
        }

        [Test]
        public void UpdateReviewAsyncThrowsErrorIfDatabaseFailedToSave()
        {
            var repo = new Mock<IDeletableEntityRepository<Review>>();
            repo.Setup(r => r.SaveChangesAsync())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToSave));

            var logger = new Mock<ILogger<ReviewService>>();
            var userService = new Mock<IUserService>();
            var bookService = new Mock<IBookService>();

            IReviewService categoryService = new ReviewService(repo.Object, bookService.Object, userService.Object, logger.Object);

            var model = new ReviewViewModel();

            Assert.ThrowsAsync<ApplicationException>(async () => await categoryService.UpdateReviewAsync(model, 1, "peshoId"), GlobalExceptions.DatabaseFailedToSave);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private async Task<ApplicationUser> SeedDbAsync(IDeletableEntityRepository<ApplicationUser> repo)
        {
            List<ApplicationUser> users = new List<ApplicationUser>
            {
                new ApplicationUser{ Id = "peshoId", UserName = "Pesho", Email = "pesho@abv.bg", FirstName = "Peter", LastName = "Petrov"},
                new ApplicationUser{ Id = "GoshoId", UserName = "Gosho", Email = "gosho@abv.bg"},
            };

            foreach (var user in users)
            {
                await repo.AddAsync(user);
            }

            await repo.SaveChangesAsync();

            return users[0];
        }

        private async Task SeedDbAsync(IDeletableEntityRepository<Review> repo, Guid bookId, string userId)
        {
            var review = new Review
            {
                BookId = bookId,
                UserId = userId,
                UserReview = "Some test Review"
            };

            await repo.AddAsync(review);
            await repo.SaveChangesAsync();

            reviewId = review.Id;
        }

        private async Task<Book> SeedDbAsync(IDeletableEntityRepository<Book> repo)
        {
            var book = new Book
            {
                ISBN = "",
                Title = "",
                Description = "",
                Year = 2021,
                Price = 10,
                Pages = 100,
                Quantity = 3,
                ImageUrl = "",
                Author = new Author
                {
                    Name = ""
                },
                Publisher = new Publisher
                {
                    Name = "",
                    Email = "",
                    Phone = "",
                    URL = ""
                }
            };

            await repo.AddAsync(book);
            await repo.SaveChangesAsync();

            return book;
        }
    }
}
