using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Rating;
using BookStore.Core.Models.Review;
using BookStore.Core.Services;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BookStore.Test
{
    public class ReviewServiceTest
    {
        private const int ExpectedReviewCount = 2;

        private ServiceProvider serviceProvider;
        private InMemoryDbContext dbContext;
        private Guid bookId;
        private string userId;
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
                .AddSingleton<IBookService, BookService>()
                .AddSingleton<IReviewService, ReviewService>()
                .BuildServiceProvider();

            var ratingRepository = serviceProvider.GetService<IDeletableEntityRepository<Review>>();
            var userRepository = serviceProvider.GetService<IDeletableEntityRepository<ApplicationUser>>();
            var bookRepository = serviceProvider.GetService<IDeletableEntityRepository<Book>>();

            await SeedDbAsync(userRepository);
            await SeedDbAsync(bookRepository);
            await SeedDbAsync(ratingRepository, bookId, userId);
        }

        [Test]
        public void AddReviewAsyncShouldThrowExceptionWhenBookIsNotFound()
        {
            var model = new ReviewViewModel
            {
                UserReview = ""
            };

            var service = serviceProvider.GetService<IReviewService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddReviewAsync(model, Guid.NewGuid(), "Random user"), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public void AddReviewAsyncShouldNotThrowExceptionWhenBookAndAuthorAreFound()
        {
            var model = new ReviewViewModel
            {
                UserReview = ""
            };

            var service = serviceProvider.GetService<IReviewService>();

            Assert.DoesNotThrowAsync(async () => await service.AddReviewAsync(model, bookId, userId));
        }

        [Test]
        public void AddReviewAsyncShouldThrowExceptionWhenUserIsNotFound()
        {
            var model = new ReviewViewModel
            {
                UserReview = ""
            };

            var service = serviceProvider.GetService<IReviewService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddReviewAsync(model, bookId, "Random user"), GlobalExceptions.InvalidBookId);
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

            await service.AddReviewAsync(model, bookId, userId);

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

            await service.DeleteReviewAsync(reviewId, userId);

            Assert.That(await repo.AllAsNoTracking().CountAsync(), Is.EqualTo(0));
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
                    BookId = bookId,
                    UserId = userId,
                    UserReview = "Some test Review"
                }
            };

            var actualJson = JsonConvert.SerializeObject(actual);
            var expectedJson = JsonConvert.SerializeObject(expected);

            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public async Task GetReviewByIdAsyncThrowsExceptionIfIdIsInvalid()
        {
            var service = serviceProvider.GetService<IReviewService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.GetReviewByIdAsync(111), GlobalExceptions.InvalidReviewId);
        }

        [Test]
        public async Task GetReviewByIdAsyncReturnsCorrectData()
        {
            var service = serviceProvider.GetService<IReviewService>();

            var actual = await service.GetReviewByIdAsync(reviewId);
            var expected = new Review
            {
                Id = 1,
                BookId = bookId,
                UserId = userId,
                UserReview = "Some test Review"
            };

            var actualJson = JsonConvert.SerializeObject(actual);
            var expectedJson = JsonConvert.SerializeObject(expected);

            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        [Test]
        public async Task GetUserReviewsAsyncThrowsErrorIfUserIsNotFound()
        {
            var service = serviceProvider.GetService<IReviewService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.GetUserReviewsAsync("Random User"), GlobalExceptions.InvalidUser);
        }

        [Test]
        public async Task GetUserReviewsAsyncReturnsCorrectData()
        {
            var service = serviceProvider.GetService<IReviewService>();

            var actual = await service.GetUserReviewsAsync(userId);
            var expected = new List<Review>
            {
                new Review
                {
                    Id = 1,
                    BookId = bookId,
                    UserId = userId,
                    UserReview = "Some test Review"
                }
            };

            var actualJson = JsonConvert.SerializeObject(actual);
            var expectedJson = JsonConvert.SerializeObject(expected);

            Assert.That(actualJson, Is.EqualTo(expectedJson));
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
            var actual = await service.UpdateReviewAsync(model, reviewId, userId);

            Assert.That(actual.UserReview, Is.EqualTo(model.UserReview));
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
