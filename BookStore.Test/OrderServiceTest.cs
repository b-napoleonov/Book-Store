using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Order;
using BookStore.Core.Models.Review;
using BookStore.Core.Services;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace BookStore.Test
{
    public class OrderServiceTest
    {
        private const int ExpectedOrderCount = 2;
        private const int ExpectedBookOrderCount = 1;
        private const int ExpectedNewBookOrderCount = 2;
        private const int ExpectedBookQuantityAfterOrder = 2;
        private const int ExpectedBookCopiesCount = 2;
        private const int ExpectedReducedBookQuantity = 2;
        private const int ExpectedIncreasedBookQuantity = 3;

        private ServiceProvider serviceProvider;
        private InMemoryDbContext dbContext;
        private Guid bookId;
        private Guid lowQuantityBookId;
        private Guid secondBookId;
        private string userId;
        private Guid orderId;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IDeletableEntityRepository<Order>, DeletableEntityRepository<Order>>()
                .AddSingleton<IDeletableEntityRepository<Book>, DeletableEntityRepository<Book>>()
                .AddSingleton<IDeletableEntityRepository<ApplicationUser>, DeletableEntityRepository<ApplicationUser>>()
                .AddSingleton<IDeletableEntityRepository<BookOrder>, DeletableEntityRepository<BookOrder>>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<ILogger<UserService>, Logger<UserService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .AddSingleton<IBookService, BookService>()
                .AddSingleton<ILogger<BookService>, Logger<BookService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .AddSingleton<IOrderService, OrderService>()
                .AddSingleton<ILogger<OrderService>, Logger<OrderService>>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .BuildServiceProvider();

            var orderRepository = serviceProvider.GetService<IDeletableEntityRepository<Order>>();
            var userRepository = serviceProvider.GetService<IDeletableEntityRepository<ApplicationUser>>();
            var bookRepository = serviceProvider.GetService<IDeletableEntityRepository<Book>>();
            var bookOrderRepository = serviceProvider.GetService<IDeletableEntityRepository<BookOrder>>();

            await SeedDbAsync(userRepository);
            await SeedDbAsync(bookRepository);
            await SeedDbAsync(orderRepository, bookId, userId);
            await SeedDbAsync(bookOrderRepository, bookId, orderId);
        }

        [Test]
        public void AddNewOrderAsyncThrowsErrorIfBookIsNotFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<NullReferenceException>(async () => await service.AddNewOrderAsync(Guid.NewGuid(), userId), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public void AddNewOrderAsyncThrowsExceptionIfBookQuantityIsLow()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddNewOrderAsync(lowQuantityBookId, userId), GlobalExceptions.InsufficientQuantity);
        }

        [Test]
        public void AddNewOrderAsyncThrowsErrorIfUserIsNotFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<NullReferenceException>(async () => await service.AddNewOrderAsync(bookId, "Random User"), GlobalExceptions.InvalidUser);
        }

        [Test]
        public void AddNewOrderAsyncShouldNotThrowExceptionWhenBookAndUserAreFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.DoesNotThrowAsync(async () => await service.AddNewOrderAsync(bookId, userId));
        }

        [Test]
        public async Task AddNewOrderAsyncShouldCreateNewOrder()
        {
            var service = serviceProvider.GetService<IOrderService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Order>>();

            await service.AddNewOrderAsync(bookId, userId);

            Assert.That(await repo.AllAsNoTracking().CountAsync(), Is.EqualTo(ExpectedOrderCount));
        }

        [Test]
        public async Task AddNewOrderAsyncShouldAddNewBookOrderToTheOrder()
        {
            var service = serviceProvider.GetService<IOrderService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Order>>();

            await service.AddNewOrderAsync(bookId, userId);

            var actual = await repo.AllAsNoTracking().Include(o => o.BookOrders).OrderBy(o => o.OrderDate).FirstOrDefaultAsync();
            
            var actualCount = actual.BookOrders.Count;

            Assert.That(actualCount, Is.EqualTo(ExpectedBookOrderCount));
        }

        [Test]
        public async Task AddNewOrderAsyncShouldReduceBookQuantity()
        {
            var service = serviceProvider.GetService<IOrderService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Book>>();

            await service.AddNewOrderAsync(bookId, userId);

            var book = await repo.AllAsNoTracking().Where(b => b.Id == bookId).FirstOrDefaultAsync();

            Assert.That(book.Quantity, Is.EqualTo(ExpectedBookQuantityAfterOrder));
        }

        [Test]
        public void AddCopiesToOrderAsyncThrowsErrorIfBookIsNotFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<NullReferenceException>(async () => await service.AddCopiesToOrderAsync(Guid.NewGuid(), userId), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public void AddCopiesToOrderAsyncThrowsExceptionIfBookQuantityIsLow()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddCopiesToOrderAsync(lowQuantityBookId, userId), GlobalExceptions.InsufficientQuantity);
        }

        [Test]
        public void AddCopiesToOrderAsyncThrowsErrorIfUserIsNotFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<NullReferenceException>(async () => await service.AddCopiesToOrderAsync(bookId, "Random User"), GlobalExceptions.InvalidUser);
        }

        [Test]
        public void AddCopiesToOrderAsyncShouldNotThrowExceptionWhenBookAndUserAreFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.DoesNotThrowAsync(async () => await service.AddCopiesToOrderAsync(bookId, userId));
        }

        [Test]
        public void AddCopiesToOrderAsyncThrowsErrorIfBookOrderIsNotFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddCopiesToOrderAsync(bookId, "GoshoId"), GlobalExceptions.InvalidOrder);
        }

        [Test]
        public async Task AddCopiesToOrderAsyncShouldAddCopiesToOrder()
        {
            var service = serviceProvider.GetService<IOrderService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Order>>();

            await service.AddCopiesToOrderAsync(bookId, userId);

            var actual = await repo.AllAsNoTracking()
                .Include(o => o.BookOrders)
                .Where(o => o.CustomerId == userId)
                .SelectMany(o => o.BookOrders)
                .Where(bo => bo.BookId == bookId)
                .FirstOrDefaultAsync();

            Assert.That(actual.Copies, Is.EqualTo(ExpectedBookCopiesCount));
        }

        [Test]
        public async Task AddCopiesToOrderAsyncShouldReduceBookCopies()
        {
            var service = serviceProvider.GetService<IOrderService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Book>>();

            await service.AddCopiesToOrderAsync(bookId, userId);

            var actual = await repo.AllAsNoTracking()
                .Where(b => b.Id == bookId)
                .FirstOrDefaultAsync();

            Assert.That(actual.Quantity, Is.EqualTo(ExpectedReducedBookQuantity));
        }

        [Test]
        public async Task CheckBookOrderAsyncReturnsTrueIfOrderIsFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.That(await service.CheckBookOrderAsync(bookId, userId), Is.True);
        }

        [Test]
        public async Task CheckBookOrderAsyncReturnsFalseIfOrderIsFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.That(await service.CheckBookOrderAsync(Guid.NewGuid(), userId), Is.False);
        }

        [Test]
        public async Task CheckUserOrderAsyncReturnsTrueIfOrderIsFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.That(await service.CheckUserOrderAsync(userId), Is.True);
        }

        [Test]
        public async Task CheckUserOrderAsyncReturnsFalseIfOrderIsFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.That(await service.CheckUserOrderAsync("Random User"), Is.False);
        }

        [Test]
        public void AddNewBookToOrderAsyncThrowsErrorIfBookIsNotFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<NullReferenceException>(async () => await service.AddNewBookToOrderAsync(Guid.NewGuid(), userId), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public void AddNewBookToOrderAsyncThrowsExceptionIfBookQuantityIsLow()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddNewBookToOrderAsync(lowQuantityBookId, userId), GlobalExceptions.InsufficientQuantity);
        }

        [Test]
        public void AddNewBookToOrderAsyncThrowsErrorIfUserIsNotFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<NullReferenceException>(async () => await service.AddNewBookToOrderAsync(bookId, "Random User"), GlobalExceptions.InvalidUser);
        }

        [Test]
        public void AddNewBookToOrderAsyncShouldNotThrowExceptionWhenBookAndUserAreFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.DoesNotThrowAsync(async () => await service.AddNewBookToOrderAsync(secondBookId, userId));
        }

        [Test]
        public void AddNewBookToOrderAsyncThrowsErrorIfOrderIsNotFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddNewBookToOrderAsync(secondBookId, "GoshoId"), GlobalExceptions.InvalidOrder);
        }

        [Test]
        public async Task AddNewBookToOrderAsyncShouldCreateNewBookOrder()
        {
            var service = serviceProvider.GetService<IOrderService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Order>>();

            await service.AddNewBookToOrderAsync(secondBookId, userId);

            var actual = await repo.AllAsNoTracking()
                .Include(o => o.BookOrders)
                .Where(o => o.CustomerId == userId)
                .FirstOrDefaultAsync();

            Assert.That(actual.BookOrders.Count, Is.EqualTo(ExpectedNewBookOrderCount));
        }

        [Test]
        public async Task AddNewBookToOrderAsyncShouldReduceBookCopies()
        {
            var service = serviceProvider.GetService<IOrderService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Book>>();

            await service.AddNewBookToOrderAsync(secondBookId, userId);

            var actual = await repo.AllAsNoTracking()
                .Where(b => b.Id == secondBookId)
                .FirstOrDefaultAsync();

            Assert.That(actual.Quantity, Is.EqualTo(ExpectedReducedBookQuantity));
        }

        [Test]
        public void GetUserOrdersAsyncThrowsErrorIfUserIsNotFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetUserOrdersAsync("Random User"), GlobalExceptions.InvalidUser);
        }

        [Test]
        public void GetUserOrdersAsyncShouldNotThrowExceptionWhenUserIsFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.DoesNotThrowAsync(async () => await service.GetUserOrdersAsync(userId));
        }

        [Test]
        public async Task GetUserOrdersAsyncReturnsCorrectData()
        {
            var service = serviceProvider.GetService<IOrderService>();

            var expectedModel = new List<OrderViewModel>()
            {
                new OrderViewModel
                {
                    BookId = bookId,
                    Title = "",
                    Author = "",
                    Price = 10,
                    ImageUrl = "",
                    Copies = 1
                }
            };

            var actualModel = await service.GetUserOrdersAsync(userId);

            var expectedJson = JsonConvert.SerializeObject(expectedModel);
            var actualJson = JsonConvert.SerializeObject(actualModel);

            Assert.That(expectedJson, Is.EqualTo(actualJson));
        }

        [Test]
        public void RemoveUserOrdersAsyncThrowsErrorIfBookIsNotFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<NullReferenceException>(async () => await service.RemoveUserOrdersAsync(Guid.NewGuid(), userId), GlobalExceptions.InvalidBookId);
        }

        [Test]
        public void RemoveUserOrdersAsyncThrowsErrorIfUserIsNotFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<NullReferenceException>(async () => await service.RemoveUserOrdersAsync(bookId, "Random User"), GlobalExceptions.InvalidUser);
        }

        [Test]
        public void RemoveUserOrdersAsyncShouldNotThrowExceptionWhenBookAndUserAreFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.DoesNotThrowAsync(async () => await service.RemoveUserOrdersAsync(bookId, userId));
        }

        [Test]
        public void RemoveUserOrdersAsyncThrowsErrorIfOrderIsNotFound()
        {
            var service = serviceProvider.GetService<IOrderService>();

            Assert.ThrowsAsync<ArgumentException>(async () => await service.RemoveUserOrdersAsync(secondBookId, "GoshoId"), GlobalExceptions.InvalidOrder);
        }

        [Test]
        public async Task RemoveUserOrdersAsyncShouldIncreaseBookCopies()
        {
            var service = serviceProvider.GetService<IOrderService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Book>>();

            await service.RemoveUserOrdersAsync(bookId, userId);

            var actual = await repo.AllAsNoTracking()
                .Where(b => b.Id == secondBookId)
                .FirstOrDefaultAsync();

            Assert.That(actual.Quantity, Is.EqualTo(ExpectedIncreasedBookQuantity));
        }

        [Test]
        public async Task RemoveUserOrdersAsyncShouldReduceBookOrders()
        {
            var service = serviceProvider.GetService<IOrderService>();
            var repo = serviceProvider.GetService<IDeletableEntityRepository<Order>>();

            await service.RemoveUserOrdersAsync(bookId, userId);

            var actual = await repo.AllAsNoTracking()
                .Include(o => o.BookOrders)
                .Where(o => o.CustomerId == userId)
                .FirstOrDefaultAsync();

            Assert.That(actual.BookOrders.Count, Is.EqualTo(ExpectedBookOrderCount));
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

        private async Task SeedDbAsync(IDeletableEntityRepository<Order> repo, Guid bookId, string userId)
        {
            var order = new Order
            {
                CustomerId = userId,
                OrderStatus = "",
                OrderDate = DateTime.Now,
            };

            await repo.AddAsync(order);
            await repo.SaveChangesAsync();

            orderId = order.Id;
        }

        private async Task SeedDbAsync(IDeletableEntityRepository<BookOrder> repo, Guid bookId, Guid orderId)
        {
            var bookOrder = new BookOrder
            {
                BookId = bookId,
                OrderId = orderId,
                Copies = 1
            };

            await repo.AddAsync(bookOrder);
            await repo.SaveChangesAsync();
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

            var lowQuantityBook = new Book
            {
                ISBN = "",
                Title = "",
                Description = "",
                Year = 2021,
                Price = 10,
                Pages = 100,
                Quantity = 0,
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

            await repo.AddAsync(lowQuantityBook);

            var secondBook = new Book
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

            await repo.AddAsync(secondBook);

            await repo.SaveChangesAsync();

            bookId = book.Id;
            lowQuantityBookId = lowQuantityBook.Id;
            secondBookId = secondBook.Id;
        }
    }
}
