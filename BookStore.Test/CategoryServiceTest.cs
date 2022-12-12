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
    /// Tests for Category Service
    /// </summary>
    public class CategoryServiceTest
    {
        private ServiceProvider serviceProvider;
        private InMemoryDbContext dbContext;
        private const int expectedCategoriesCount = 4;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IDeletableEntityRepository<Category>, DeletableEntityRepository<Category>>()
                .AddSingleton<ICategoryService, CategoryService>()
                .AddSingleton<ILogger<CategoryService>, Logger<CategoryService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IDeletableEntityRepository<Category>>();
            await SeedDbAsync(repo);
        }

        [Test]
        public async Task GetAllCategoriesAsyncShouldReturnCorrectData()
        {
            var service = serviceProvider.GetService<ICategoryService>();

            var testData = GetTestData();
            var resultData = await service.GetAllCategoriesAsync();

            var actualJson = JsonConvert.SerializeObject(testData);
            var resultJson = JsonConvert.SerializeObject(resultData);

            Assert.That(actualJson, Is.EqualTo(resultJson));
        }

        [Test]
        public void GetAllCategoriesAsyncThrowsErrorIfDatabaseFailedToFetch()
        {
            var repo = new Mock<IDeletableEntityRepository<Category>>();
            repo.Setup(r => r.AllAsNoTracking())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToFetch));

            var logger = new Mock<ILogger<CategoryService>>();

            ICategoryService authorService = new CategoryService(repo.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await authorService.GetAllCategoriesAsync(), GlobalExceptions.DatabaseFailedToFetch);
        }

        [Test]
        public async Task AddCategoryShouldAddCorrectly()
        {
            var service = serviceProvider.GetService<ICategoryService>();

            await service.AddCategoryAsync(new Core.Models.Category.AddCategoryViewModel { Name = "Test" });

            Assert.That((await service.GetAllCategoriesAsync()).Count(), Is.EqualTo(expectedCategoriesCount));
        }

        [Test]
        public void AddCategoryThrowsErrorIfDatabaseFailedToSave()
        {
            var repo = new Mock<IDeletableEntityRepository<Category>>();
            repo.Setup(r => r.SaveChangesAsync())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToSave));

            var logger = new Mock<ILogger<CategoryService>>();

            ICategoryService categoryService = new CategoryService(repo.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await categoryService.AddCategoryAsync(new Core.Models.Category.AddCategoryViewModel { Name = "Test" }), GlobalExceptions.DatabaseFailedToFetch);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private List<BookCategoryViewModel> GetTestData()
        {
            return new List<BookCategoryViewModel>
            {
                new BookCategoryViewModel{ Id = 1, Name = "Action"},
                new BookCategoryViewModel{ Id = 2, Name = "Drama"},
                new BookCategoryViewModel{ Id = 3, Name = "Romance" },
            };
        }

        private async Task SeedDbAsync(IDeletableEntityRepository<Category> repo)
        {
            List<Category> categories = new List<Category>
            {
                new Category{ Id = 1, Name = "Action"},
                new Category{ Id = 2, Name = "Drama"},
                new Category{ Id = 3, Name = "Romance"},
            };

            foreach (var category in categories)
            {
                await repo.AddAsync(category);
            }

            await repo.SaveChangesAsync();
        }
    }
}
