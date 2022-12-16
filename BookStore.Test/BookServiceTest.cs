using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Core.Models.Rating;
using BookStore.Core.Models.Review;
using BookStore.Core.Services;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BookStore.Test
{
    /// <summary>
    /// Tests for Book Service
    /// </summary>
    public class BookServiceTest
    {
        private const int ExpectedBookCountAfterAdd = 3;
        private const int ExpectedAverageRating = 4;
        private const int ExpectedBookCountAfterRemove = 1;

        private ServiceProvider serviceProvider;
        private InMemoryDbContext dbContext;
        private Guid bookId;
        private Guid secondBookId;
        private string userId;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IDeletableEntityRepository<Book>, DeletableEntityRepository<Book>>()
                .AddSingleton<IDeletableEntityRepository<ApplicationUser>, DeletableEntityRepository<ApplicationUser>>()
                .AddSingleton<IDeletableEntityRepository<Category>, DeletableEntityRepository<Category>>()
                .AddSingleton<IBookService, BookService>()
                .AddSingleton<ILogger<BookService>, Logger<BookService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .BuildServiceProvider();

            var userRepository = serviceProvider.GetService<IDeletableEntityRepository<ApplicationUser>>();
            var categoryRepository = serviceProvider.GetService<IDeletableEntityRepository<Category>>();
            var bookRepository = serviceProvider.GetService<IDeletableEntityRepository<Book>>();

            await SeedDbAsync(userRepository);
            await SeedDbAsync(categoryRepository);
            await SeedDbAsync(bookRepository);
        }

        [Test]
        public async Task AddBookAsyncWorksRight()
        {
            var model = new AddBookViewModel
            {
                ISBN = "",
                Title = "",
                Description = "",
                Year = 2021,
                Price = 10,
                Pages = 100,
                Quantity = 3,
                ImageUrl = "",
                AuthorId = 1,
                PublisherId = 1,
                CategoryIds = new List<int>() { 1 }
            };

            var service = serviceProvider.GetService<IBookService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Book>>();

            await service.AddBookAsync(model);

            var actualBookCount = await repo.AllAsNoTracking().ToListAsync();

            Assert.That(actualBookCount.Count, Is.EqualTo(ExpectedBookCountAfterAdd));
        }

        [Test]
        public async Task GetAllBooksAsyncReturnsCorrectData()
        {
            var service = serviceProvider.GetService<IBookService>();

            var expected = new List<AllBooksViewModel>
            {
                new AllBooksViewModel
                {
                    Id = bookId,
                    Title = "First",
                    ImageUrl = "",
                    Author = "Test Author",
                    Price = 10,
                    Rating = 4
                },

                new AllBooksViewModel
                {
                    Id = secondBookId,
                    Title = "Second",
                    ImageUrl = "",
                    Author = "",
                    Price = 10,
                    Rating = 0
                },
            };

            var actual = await service.GetAllBooksAsync();

            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);

            Assert.That(expectedJson, Is.EqualTo(actualJson));
        }

        [Test]
        public void GetBookAsyncThrowsErrorIfBookIsNotFound()
        {
            var service = serviceProvider.GetService<IBookService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.GetBookAsync(Guid.NewGuid()), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public async Task GetBookAsyncReturnsCorrectData()
        {
            var service = serviceProvider.GetService<IBookService>();

            var expected = new DetailsBookViewModel
            {
                Id = bookId,
                Title = "First",
                Description = "",
                Year = 2021,
                Price = 10,
                Pages = 100,
                Quantity = 3,
                ImageUrl = "",
                Author = "Test Author",
                Publisher = "Test Publisher",
                Categories = new List<string>() { "Test" },
            };

            var actual = await service.GetBookAsync(bookId);

            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);

            Assert.That(expectedJson, Is.EqualTo(actualJson));
        }

        [Test]
        public void GetBooksByCategoryAsyncThrowsErrorIfNoBookIsFound()
        {
            var service = serviceProvider.GetService<IBookService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.GetBooksByCategoryAsync("Random Name"), GlobalExceptions.CategoryNotFound);
        }

        [Test]
        public async Task GetBooksByCategoryAsyncReturnsCorrectData()
        {
            var service = serviceProvider.GetService<IBookService>();

            var expected = new List<AllBooksViewModel>
            {
                new AllBooksViewModel
                {
                    Id = bookId,
                    Title = "First",
                    ImageUrl = "",
                    Author = "Test Author",
                    Price = 10,
                    Rating = 4
                },

                new AllBooksViewModel
                {
                    Id = secondBookId,
                    Title = "Second",
                    ImageUrl = "",
                    Author = "",
                    Price = 10,
                }
            };

            var actual = await service.GetBooksByCategoryAsync("Test");

            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);

            Assert.That(expectedJson, Is.EqualTo(actualJson));
        }

        [Test]
        public void GetBooksByAuthorAsyncThrowsErrorIfNoBookIsFound()
        {
            var service = serviceProvider.GetService<IBookService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.GetBooksByAuthorAsync("Random Name"), GlobalExceptions.CategoryNotFound);
        }

        [Test]
        public async Task GetBooksByAuthorAsyncReturnsCorrectData()
        {
            var service = serviceProvider.GetService<IBookService>();

            var expected = new List<AllBooksViewModel>
            {
                new AllBooksViewModel
                {
                    Id = bookId,
                    Title = "First",
                    ImageUrl = "",
                    Author = "Test Author",
                    Price = 10,
                    Rating = 4
                }
            };

            var actual = await service.GetBooksByAuthorAsync("Test Author");

            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);

            Assert.That(expectedJson, Is.EqualTo(actualJson));
        }

        [Test]
        public void GetBooksByPublisherAsyncThrowsErrorIfNoBookIsFound()
        {
            var service = serviceProvider.GetService<IBookService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.GetBooksByPublisherAsync("Random Name"), GlobalExceptions.CategoryNotFound);
        }

        [Test]
        public async Task GetBooksByPublisherAsyncReturnsCorrectData()
        {
            var service = serviceProvider.GetService<IBookService>();

            var expected = new List<AllBooksViewModel>
            {
                new AllBooksViewModel
                {
                    Id = bookId,
                    Title = "First",
                    ImageUrl = "",
                    Author = "Test Author",
                    Price = 10,
                    Rating = 4
                }
            };

            var actual = await service.GetBooksByPublisherAsync("Test Publisher");

            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);

            Assert.That(expectedJson, Is.EqualTo(actualJson));
        }

        [Test]
        public void GetBookRatingThrowsErrorIfNoBookIsFound()
        {
            var service = serviceProvider.GetService<IBookService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.GetBookRating(Guid.NewGuid()), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public async Task GetBookRatingReturnsRightData()
        {
            var service = serviceProvider.GetService<IBookService>();

            var actualRating = await service.GetBookRating(bookId);

            Assert.That(actualRating, Is.EqualTo(ExpectedAverageRating));
        }

        [Test]
        public void GetBookReviewsAsyncThrowsErrorIfNoBookIsFound()
        {
            var service = serviceProvider.GetService<IBookService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.GetBookReviewsAsync(Guid.NewGuid()), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public async Task GetBookReviewsAsyncReturnsRightData()
        {
            var service = serviceProvider.GetService<IBookService>();

            var expected = new List<DetailsReviewViewModel>
            {
                new DetailsReviewViewModel
                {
                    ReviewId = 1,
                    OwnerId = userId,
                    UserEmail = "pesho@abv.bg",
                    UserReview = "Test Review"
                }
            };

            var actual = await service.GetBookReviewsAsync(bookId);

            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);

            Assert.That(expectedJson, Is.EqualTo(actualJson));
        }

        [Test]
        public void GetBookRatingDetailsAsyncThrowsErrorIfNoBookIsFound()
        {
            var service = serviceProvider.GetService<IBookService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.GetBookRatingDetailsAsync(Guid.NewGuid()), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public async Task GetBookRatingDetailsAsyncReturnsRightData()
        {
            var service = serviceProvider.GetService<IBookService>();

            var expected = new DetailsRatingViewModel
            {
                Rating = 4,
                OneStarRating = 0,
                TwoStarRating = 0,
                ThreeStarRating = 0,
                FourStarRating = 1,
                FiveStarRating = 0,
                RatingsCount = 1
            };

            var actual = await service.GetBookRatingDetailsAsync(bookId);

            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);

            Assert.That(expectedJson, Is.EqualTo(actualJson));
        }

        [Test]
        public void RemoveBookThrowsErrorIfNoBookIsFound()
        {
            var service = serviceProvider.GetService<IBookService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.RemoveBook(Guid.NewGuid()), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public async Task RemoveBookWorksCorrect()
        {
            var service = serviceProvider.GetService<IBookService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Book>>();

            await service.RemoveBook(secondBookId);

            var actualBookCount = (await repo.AllAsNoTracking().ToListAsync()).Count;

            Assert.That(actualBookCount, Is.EqualTo(ExpectedBookCountAfterRemove));
        }

        [Test]
        public void EditBookAsyncThrowsErrorIfNoBookIsFound()
        {
            var service = serviceProvider.GetService<IBookService>();

            var model = new EditBookViewModel
            {

            };

            Assert.ThrowsAsync<ArgumentException>(async () => await service.EditBookAsync(model, Guid.NewGuid()), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public async Task EditBookAsyncWorksRight()
        {
            var model = new EditBookViewModel
            {
                ISBN = "12345",
                Title = "Test Title",
                Description = "Test Description",
                Year = 2000,
                Price = 15,
                Pages = 200,
                Quantity = 4,
                ImageUrl = "Test URL",
                AuthorId = 1,
                PublisherId = 1,
                CategoryIds = new List<int>() { 1 }
            };

            var service = serviceProvider.GetService<IBookService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Book>>();

            await service.EditBookAsync(model, secondBookId);

            var actual = await repo.AllAsNoTracking().Include(b => b.Categories).Where(b => b.Id == secondBookId).FirstOrDefaultAsync();

            var expected = new Book
            {
                Id = secondBookId,
                ISBN = "12345",
                Title = "Test Title",
                Description = "Test Description",
                Year = 2000,
                Price = 15,
                Pages = 200,
                Quantity = 4,
                ImageUrl = "Test URL",
                AuthorId = 1,
                PublisherId = 1,
                Categories = new List<CategoryBook>
                {
                    new CategoryBook
                    {
                        BookId = secondBookId,
                        CategoryId = 1
                    }
                }
            };

            var expectedJson = JsonConvert.SerializeObject(expected, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });

            var actualJson = JsonConvert.SerializeObject(actual, Formatting.None, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
            });

            Assert.That(expectedJson, Is.EqualTo(actualJson));
        }

        [Test]
        public void GetBookDataForEditAsyncThrowsErrorIfNoBookIsFound()
        {
            var service = serviceProvider.GetService<IBookService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.GetBookDataForEditAsync(Guid.NewGuid()), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public async Task GetBookDataForEditAsyncReturnsRightData()
        {
            var service = serviceProvider.GetService<IBookService>();

            var expected = new EditBookViewModel
            {
                Id = bookId,
                ISBN = "",
                Title = "First",
                Description = "",
                Year = 2021,
                Price = 10,
                Pages = 100,
                Quantity = 3,
                ImageUrl = "",
            };

            var actual = await service.GetBookDataForEditAsync(bookId);

            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);

            Assert.That(expectedJson, Is.EqualTo(actualJson));
        }

        [Test]
        public async Task GetLastThreeBooksAsyncReturnsRightData()
        {
            var service = serviceProvider.GetService<IBookService>();

            var expected = new List<HomeBookViewModel>
            {
                new HomeBookViewModel
                {
                    Id = bookId,
                    Title = "First",
                    ImageUrl = "",
                },

                new HomeBookViewModel
                {
                    Id = secondBookId,
                    Title = "Second",
                    ImageUrl = "",
                },
            };

            var actual = await service.GetLastThreeBooksAsync();

            var expectedJson = JsonConvert.SerializeObject(expected);
            var actualJson = JsonConvert.SerializeObject(actual);

            Assert.That(expectedJson, Is.EqualTo(actualJson));
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

        private async Task SeedDbAsync(IDeletableEntityRepository<Category> repo)
        {
            var categories = new List<Category>
            {
                 new Category
                {
                    Id = 1,
                    Name = "Test"
                },

                 new Category
                 {
                     Id = 2,
                     Name = "New Test",
                 }
            };

            foreach (var category in categories)
            {
                await repo.AddAsync(category);
            }

            await repo.SaveChangesAsync();
        }

        private async Task SeedDbAsync(IDeletableEntityRepository<Book> repo)
        {
            var book = new Book
            {
                ISBN = "",
                Title = "First",
                Description = "",
                Year = 2021,
                Price = 10,
                Pages = 100,
                Quantity = 3,
                ImageUrl = "",
                Author = new Author
                {
                    Name = "Test Author"
                },
                Publisher = new Publisher
                {
                    Name = "Test Publisher",
                    Email = "",
                    Phone = "",
                    URL = ""
                },
                Categories = new List<CategoryBook>()
                {
                    new CategoryBook
                    {
                        CategoryId = 1
                    }
                },
                Ratings = new List<Rating>()
                {
                    new Rating
                    {
                        UserId = userId,
                        UserRating = 4
                    }
                },
                Reviews = new List<Review>()
                {
                    new Review
                    {
                        UserId = userId,
                        UserReview = "Test Review",
                    }
                }
            };

            await repo.AddAsync(book);

            var secondBook = new Book
            {
                ISBN = "",
                Title = "Second",
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
                },
                Categories = new List<CategoryBook>()
                {
                    new CategoryBook
                    {
                        CategoryId = 1
                    }
                }
            };

            await repo.AddAsync(secondBook);

            await repo.SaveChangesAsync();

            bookId = book.Id;
            secondBookId = secondBook.Id;
        }
    }
}
