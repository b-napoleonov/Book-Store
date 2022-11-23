using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Core.Models.Rating;
using BookStore.Core.Services;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Test
{
    public class RatingServiceTest
    {
        private ServiceProvider serviceProvider;
        private InMemoryDbContext dbContext;
        private Guid bookId;
        private string userId;
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
                .AddSingleton<IBookService, BookService>()
                .AddSingleton<IRatingService, RatingService>()
                .BuildServiceProvider();

            var ratingRepository = serviceProvider.GetService<IDeletableEntityRepository<Rating>>();
            var userRepository = serviceProvider.GetService<IDeletableEntityRepository<ApplicationUser>>();
            var bookRepository = serviceProvider.GetService<IDeletableEntityRepository<Book>>();

            await SeedDbAsync(userRepository);
            await SeedDbAsync(bookRepository);
            await SeedDbAsync(ratingRepository, bookId, userId);
        }

        [Test]
        public void AddRatingShouldThrowExceptionWhenBookIsNotFound()
        {
            var model = new AddRatingViewModel
            {
                UserRating = 5
            };

            var service = serviceProvider.GetService<IRatingService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddRating(model, Guid.NewGuid(), "Random user"), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public void AddRatingShouldNotThrowExceptionWhenBookAndAuthorAreFound()
        {
            var model = new AddRatingViewModel
            {
                UserRating = 5
            };

            var service = serviceProvider.GetService<IRatingService>();

            Assert.DoesNotThrowAsync(async () => await service.AddRating(model, bookId, userId));
        }

        [Test]
        public void AddRatingShouldThrowExceptionWhenUserIsNotFound()
        {
            var model = new AddRatingViewModel
            {
                UserRating = 5
            };

            var service = serviceProvider.GetService<IRatingService>();

            Assert.CatchAsync<ArgumentException>(async () => await service.AddRating(model, bookId, "RandomID"), GlobalExceptions.InvalidUser);
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

            await service.AddRating(model, bookId, "GoshoId");

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

            await service.AddRating(model, bookId, userId);

            var actualUserRating = await repo
                .AllAsNoTracking()
                .Where(r => r.Id == ratingId)
                .FirstOrDefaultAsync();

            Assert.That(actualUserRating.UserRating, Is.EqualTo(model.UserRating));
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private async Task SeedDbAsync(IDeletableEntityRepository<ApplicationUser> repo)
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

            userId = users[0].Id;
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

        private async Task SeedDbAsync(IDeletableEntityRepository<Book> repo)
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
            bookId = book.Id;
        }
    }
}
