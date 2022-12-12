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
    /// Tests for Publisher Service
    /// </summary>
    public class PublisherServiceTest
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
                .AddSingleton<IDeletableEntityRepository<Publisher>, DeletableEntityRepository<Publisher>>()
                .AddSingleton<IPublisherService, PublisherService>()
                .AddSingleton<ILogger<PublisherService>, Logger<PublisherService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IDeletableEntityRepository<Publisher>>();
            await SeedDbAsync(repo);
        }

        [Test]
        public async Task GetAllPublishersAsyncShouldReturnCorrectData()
        {
            var service = serviceProvider.GetService<IPublisherService>();

            var testData = GetTestData();
            var resultData = await service.GetAllPublishersAsync();

            var actualJson = JsonConvert.SerializeObject(testData);
            var resultJson = JsonConvert.SerializeObject(resultData);

            Assert.That(actualJson, Is.EqualTo(resultJson));
        }

        [Test]
        public void GetAllPublishersAsyncThrowsErrorIfDatabaseFailedToFetch()
        {
            var repo = new Mock<IDeletableEntityRepository<Publisher>>();
            repo.Setup(r => r.AllAsNoTracking())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToFetch));

            var logger = new Mock<ILogger<PublisherService>>();

            IPublisherService authorService = new PublisherService(repo.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await authorService.GetAllPublishersAsync(), GlobalExceptions.DatabaseFailedToFetch);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private List<BookPublisherViewModel> GetTestData()
        {
            return new List<BookPublisherViewModel>
            {
                new BookPublisherViewModel{ Id = 1, Name = "Bard"},
                new BookPublisherViewModel{ Id = 2, Name = "Ciela"},
                new BookPublisherViewModel{ Id = 3, Name = "Obsidian" },
            };
        }

        private async Task SeedDbAsync(IDeletableEntityRepository<Publisher> repo)
        {
            List<Publisher> publishers = new List<Publisher>
            {
                new Publisher{ Id = 1, Name = "Bard", Email = "", Phone = "", URL = ""},
                new Publisher{ Id = 2, Name = "Ciela", Email = "", Phone = "", URL = ""},
                new Publisher{ Id = 3, Name = "Obsidian", Email = "", Phone = "", URL = ""},
            };

            foreach (var publisher in publishers)
            {
                await repo.AddAsync(publisher);
            }

            await repo.SaveChangesAsync();
        }
    }
}
