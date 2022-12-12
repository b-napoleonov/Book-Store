using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Rating;
using BookStore.Core.Services;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookStore.Test
{
    /// <summary>
    /// Tests for Rating Service
    /// </summary>
    public class RatingServiceTest
    {
        private ServiceProvider serviceProvider;
        private InMemoryDbContext dbContext;
        private Book book;
        private ApplicationUser user;
        private int ratingId;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IDeletableEntityRepository<Rating>, DeletableEntityRepository<Rating>>()
                .AddSingleton<IDeletableEntityRepository<Book>, DeletableEntityRepository<Book>>()
                .AddSingleton<IDeletableEntityRepository<ApplicationUser>, DeletableEntityRepository<ApplicationUser>>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<ILogger<UserService>, Logger<UserService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .AddSingleton<IBookService, BookService>()
                .AddSingleton<ILogger<BookService>, Logger<BookService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .AddSingleton<IRatingService, RatingService>()
                .AddSingleton<ILogger<RatingService>, Logger<RatingService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .BuildServiceProvider();

            var ratingRepository = serviceProvider.GetService<IDeletableEntityRepository<Rating>>();
            var userRepository = serviceProvider.GetService<IDeletableEntityRepository<ApplicationUser>>();
            var bookRepository = serviceProvider.GetService<IDeletableEntityRepository<Book>>();

            user = await SeedDbAsync(userRepository);
            book = await SeedDbAsync(bookRepository);
            await SeedDbAsync(ratingRepository, book.Id, user.Id);
        }

        [Test]
        public void AddRatingShouldThrowExceptionWhenBookIsNotFound()
        {
            var model = new AddRatingViewModel
            {
                UserRating = 5
            };

            var service = serviceProvider.GetService<IRatingService>();

            Assert.ThrowsAsync<NullReferenceException>(async () => await service.AddRating(model, Guid.NewGuid(), "Random user"), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public void AddRatingShouldNotThrowExceptionWhenBookAndAuthorAreFound()
        {
            var model = new AddRatingViewModel
            {
                UserRating = 5
            };

            var service = serviceProvider.GetService<IRatingService>();

            Assert.DoesNotThrowAsync(async () => await service.AddRating(model, book.Id, user.Id));
        }

        [Test]
        public void AddRatingShouldThrowExceptionWhenUserIsNotFound()
        {
            var model = new AddRatingViewModel
            {
                UserRating = 5
            };

            var service = serviceProvider.GetService<IRatingService>();

            Assert.CatchAsync<NullReferenceException>(async () => await service.AddRating(model, book.Id, "RandomID"), GlobalExceptions.InvalidUser);
        }

        [Test]
        public void AddRatingThrowsErrorIfDatabaseFailedToFetch()
        {
            var repo = new Mock<IDeletableEntityRepository<Rating>>();
            repo.Setup(r => r.AllAsNoTracking())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToFetch));

            var logger = new Mock<ILogger<RatingService>>();
            var bookService = new Mock<IBookService>();
            bookService.Setup(s => s.GetBookByIdAsync(book.Id).Result)
                .Returns(book);

            var userService = new Mock<IUserService>();
            userService.Setup(s => s.GetUserByIdAsync(user.Id).Result)
                .Returns(user);

            var model = new AddRatingViewModel();

            IRatingService ratingService = new RatingService(repo.Object, userService.Object, bookService.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await ratingService.AddRating(model, book.Id, user.Id), GlobalExceptions.DatabaseFailedToFetch);
        }

        [Test]
        public async Task AddRatingShouldCreateNewRatingIfOneDoesNotExist()
        {
            var model = new AddRatingViewModel
            {
                UserRating = 5
            };

            var service = serviceProvider.GetService<IRatingService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Rating>>();

            await service.AddRating(model, book.Id, "GoshoId");

            Assert.That(await repo.AllAsNoTracking().CountAsync(), Is.EqualTo(2));
        }

        [Test]
        public async Task AddRatingShouldModifyTheRatingIfOneExist()
        {
            var model = new AddRatingViewModel
            {
                UserRating = 4
            };

            var service = serviceProvider.GetService<IRatingService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Rating>>();

            await service.AddRating(model, book.Id, user.Id);

            var actualUserRating = await repo
                .AllAsNoTracking()
                .Where(r => r.Id == ratingId)
                .FirstOrDefaultAsync();

            Assert.That(actualUserRating.UserRating, Is.EqualTo(model.UserRating));
        }

        //[Test]
        //public void AddRatingThrowsErrorIfDatabaseFailedToSave()
        //{
        //    var repo = new Mock<IDeletableEntityRepository<Rating>>();
        //    repo.Setup(r => r.SaveChangesAsync())
        //    .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToSave));

        //    repo.Setup(r => r.AllAsNoTracking())
        //        .Returns(true);
               

        //    var logger = new Mock<ILogger<RatingService>>();
        //    var bookService = new Mock<IBookService>();
        //    bookService.Setup(s => s.GetBookByIdAsync(book.Id).Result)
        //        .Returns(book);

        //    var userService = new Mock<IUserService>();
        //    userService.Setup(s => s.GetUserByIdAsync(user.Id).Result)
        //        .Returns(user);

        //    var model = new AddRatingViewModel();

        //    IRatingService ratingService = new RatingService(repo.Object, userService.Object, bookService.Object, logger.Object);

        //    Assert.ThrowsAsync<ApplicationException>(async () => await ratingService.AddRating(model, book.Id, user.Id), GlobalExceptions.DatabaseFailedToSave);
        //}

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

        private async Task SeedDbAsync(IDeletableEntityRepository<Rating> repo, Guid bookId, string userId)
        {
            var rating = new Rating
            {
                BookId = bookId,
                UserId = userId,
                UserRating = 5
            };

            await repo.AddAsync(rating);
            await repo.SaveChangesAsync();

            ratingId = rating.Id;
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
