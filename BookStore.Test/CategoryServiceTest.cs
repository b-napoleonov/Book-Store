using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Core.Services;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BookStore.Test
{
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
        public async Task AddCategoryShouldAddCorrectly()
        {
            var service = serviceProvider.GetService<ICategoryService>();

            await service.AddCategoryAsync(new Core.Models.Category.AddCategoryViewModel { Name = "Test" });

            Assert.That((await service.GetAllCategoriesAsync()).Count(), Is.EqualTo(expectedCategoriesCount));
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
