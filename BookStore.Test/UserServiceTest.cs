using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Core.Models.User;
using BookStore.Core.Services;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace BookStore.Test
{
    public class UserServiceTest
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
                .AddSingleton<IDeletableEntityRepository<ApplicationUser>, DeletableEntityRepository<ApplicationUser>>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<ILogger<UserService>, Logger<UserService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IDeletableEntityRepository<ApplicationUser>>();
            await SeedDbAsync(repo);
        }

        [Test]
        public void GetUserByIdAsyncShouldNotThrowExceptionWhenUserIsFound()
        {
            var service = serviceProvider.GetService<IUserService>();

            Assert.DoesNotThrowAsync(async () => await service.GetUserByIdAsync("peshoId"));
        }

        [Test]
        public async Task GetUserByIdAsyncReturnsCorrectData()
        {
            var service = serviceProvider.GetService<IUserService>();

            var actualUser = await service.GetUserByIdAsync("peshoId");

            Assert.That(actualUser.UserName, Is.EqualTo("Pesho"));
            Assert.That(actualUser.Email, Is.EqualTo("pesho@abv.bg"));
        }

        [Test]
        public void GetUserByIdAsyncThrowsErrorIfDatabaseFailedToFetch()
        {
            var repo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            repo.Setup(r => r.AllAsNoTracking())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToFetch));

            var logger = new Mock<ILogger<UserService>>();

            IUserService userService = new UserService(repo.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await userService.GetUserByIdAsync("peshoId"), GlobalExceptions.DatabaseFailedToFetch);
        }

        [Test]
        public void GetUserProfileDataAsyncShouldThrowExceptionWhenUserIsNotFound()
        {
            var service = serviceProvider.GetService<IUserService>();

            Assert.CatchAsync<ArgumentException>(async () => await service.GetUserProfileDataAsync("RandomID"), GlobalExceptions.InvalidUser);
        }

        [Test]
        public void GetUserProfileDataAsyncShouldNotThrowExceptionWhenUserIsFound()
        {
            var service = serviceProvider.GetService<IUserService>();

            Assert.DoesNotThrowAsync(async () => await service.GetUserProfileDataAsync("peshoId"));
        }

        [Test]
        public async Task GetUserProfileDataAsyncShouldReturnCorrectData()
        {
            var service = serviceProvider.GetService<IUserService>();

            var testData = GetTestData();
            var resultData = await service.GetUserProfileDataAsync("peshoId");

            var actualJson = JsonConvert.SerializeObject(testData);
            var resultJson = JsonConvert.SerializeObject(resultData);

            Assert.That(actualJson, Is.EqualTo(resultJson));
        }

        [Test]
        public void GetUserProfileDataAsyncThrowsErrorIfDatabaseFailedToFetch()
        {
            var repo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            repo.Setup(r => r.AllAsNoTracking())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToFetch));

            var logger = new Mock<ILogger<UserService>>();

            IUserService userService = new UserService(repo.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await userService.GetUserProfileDataAsync("peshoId"), GlobalExceptions.DatabaseFailedToFetch);
        }

        [Test]
        public void GetOrdersCountAsyncShouldThrowExceptionWhenUserIsNotFound()
        {
            var service = serviceProvider.GetService<IUserService>();

            Assert.CatchAsync<ArgumentException>(async () => await service.GetOrdersCountAsync("RandomID"), GlobalExceptions.InvalidUser);
        }

        [Test]
        public void GetOrdersCountAsyncShouldNotThrowExceptionWhenUserIsFound()
        {
            var service = serviceProvider.GetService<IUserService>();

            Assert.DoesNotThrowAsync(async () => await service.GetOrdersCountAsync("peshoId"));
        }


        [Test]
        public async Task GetOrdersCountAsyncShouldReturnCorrectData()
        {
            var service = serviceProvider.GetService<IUserService>();

            var resultData = await service.GetOrdersCountAsync("peshoId");

            //TODO:Fix problem with BookOrders saving to context
            Assert.That(resultData, Is.EqualTo(0));
        }

        [Test]
        public void GetOrdersCountAsyncThrowsErrorIfDatabaseFailedToFetch()
        {
            var repo = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            repo.Setup(r => r.AllAsNoTracking())
            .Throws(new ApplicationException(GlobalExceptions.DatabaseFailedToFetch));

            var logger = new Mock<ILogger<UserService>>();

            IUserService userService = new UserService(repo.Object, logger.Object);

            Assert.ThrowsAsync<ApplicationException>(async () => await userService.GetOrdersCountAsync("peshoId"), GlobalExceptions.DatabaseFailedToFetch);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private UserProfileViewModel GetTestData()
        {
            return new UserProfileViewModel
            {
                UserName = "Pesho",
                Email = "pesho@abv.bg",
                FirstName = "Peter",
                LastName = "Petrov"
            };
        }

        private async Task SeedDbAsync(IDeletableEntityRepository<ApplicationUser> repo)
        {
            List<ApplicationUser> users = new List<ApplicationUser>
            {
                new ApplicationUser{ Id = "peshoId", UserName = "Pesho", Email = "pesho@abv.bg", FirstName = "Peter", LastName = "Petrov"},
                new ApplicationUser{ Id = "GoshoId", UserName = "Gosho", Email = "gosho@abv.bg"},
            };

            var order = new Order
            {
                CustomerId = users[0].Id,
                OrderDate = DateTime.Now,
                OrderStatus = "",
                BookOrders = new List<BookOrder>()
            };

            //var bookOrder = new BookOrder
            //{
            //    BookId = Guid.NewGuid(),
            //    OrderId = order.Id,
            //    Order = order,
            //    Copies = 3
            //};


            //order.BookOrders.Add(bookOrder);

            users[0].Orders = new List<Order>();
            users[0].Orders.Add(order);

            foreach (var user in users)
            {
                await repo.AddAsync(user);
            }

            await repo.SaveChangesAsync();
        }
    }
}
