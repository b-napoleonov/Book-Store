using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Core.Services;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace BookStore.Test
{
    /// <summary>
    /// Tests for Author Service
    /// </summary>
    public class AuthorServiceTest
    {
        private ServiceProvider serviceProvider;
        private InMemoryDbContext dbContext;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IDeletableEntityRepository<Author>, DeletableEntityRepository<Author>>()
                .AddSingleton<IAuthorService, AuthorService>()
                .AddSingleton<ILogger<AuthorService>, Logger<AuthorService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IDeletableEntityRepository<Author>>();
            await SeedDbAsync(repo);
        }

        [Test]
        public async Task GetAllAuthorsAsyncShouldReturnCorrectData()
        {
            var service = serviceProvider.GetService<IAuthorService>();

            var testData = GetTestData();
            var resultData = await service.GetAllAuthorsAsync();

            var actualJson = JsonConvert.SerializeObject(testData);
            var resultJson = JsonConvert.SerializeObject(resultData);

            Assert.That(actualJson, Is.EqualTo(resultJson));
        }

        [Test]
        public void GetAllAuthorsAsyncThrowsErrorIfDatabaseFailedToFetch()
        {
            var repo = new Mock<IDeletableEntityRepository<Author>>();
                repo.Setup(r => r.AllAsNoTracking())
                .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToFetch));

            var logger = new Mock<ILogger<AuthorService>>();

            IAuthorService authorService = new AuthorService(repo.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await authorService.GetAllAuthorsAsync(), GlobalExceptions.DatabaseFailedToFetch);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private List<BookAuthorViewModel> GetTestData()
        {
            return new List<BookAuthorViewModel>
            {
                new BookAuthorViewModel{ Id = 2, Name = "Gosho"},
                new BookAuthorViewModel{ Id = 3, Name = "Misho"},
                new BookAuthorViewModel{ Id = 1, Name = "Pesho" },
            };
        }

        private async Task SeedDbAsync(IDeletableEntityRepository<Author> repo)
        {
            List<Author> authors = new List<Author>
            {
                new Author{ Id = 1, Name = "Pesho"},
                new Author{ Id = 2, Name = "Gosho"},
                new Author{ Id = 3, Name = "Misho"},
            };

            foreach (var author in authors)
            {
                await repo.AddAsync(author);
            }

            await repo.SaveChangesAsync();
        }
    }
}