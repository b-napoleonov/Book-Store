using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Order;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly IDeletableEntityRepository<Order> orderRepository;
        private readonly IBookService bookService;
        private readonly IUserService userService;

        public OrderService(
            IDeletableEntityRepository<Order> _orderRepository,
            IBookService _bookService,
            IUserService _userService)
        {
            orderRepository = _orderRepository;
            bookService = _bookService;
            userService = _userService;
        }

        public async Task AddNewOrderAsync(Guid bookId, string userId)
        {
            var book = await bookService.GetBookByIdAsync(bookId);

            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            if (book.Quantity < 1)
            {
                throw new ArgumentException(GlobalExceptions.InsufficientQuantity);
            }

            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidUser);
            }

            var order = new Order
            {
                CustomerId = userId,
                OrderStatus = "Accepted",
                OrderDate = DateTime.UtcNow,
            };

            order.BookOrders.Add(new BookOrder
            {
                BookId = bookId,
                Order = order,
            });

            book.Quantity--;

            await orderRepository.AddAsync(order);
            await orderRepository.SaveChangesAsync();
        }

        public async Task AddCopiesToOrderAsync(Guid bookId, string userId)
        {
            var book = await bookService.GetBookByIdAsync(bookId);

            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            if (book.Quantity < 1)
            {
                throw new ArgumentException(GlobalExceptions.InsufficientQuantity);
            }

            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidUser);
            }

            var bookOrder = await orderRepository
                .All()
                .Include(o => o.BookOrders)
                .Where(o => o.CustomerId == userId)
                .SelectMany(o => o.BookOrders)
                .Where(bo => bo.BookId == bookId)
                .FirstOrDefaultAsync();

            if (bookOrder == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidOrder);
            }

            bookOrder.Copies++;
            book.Quantity--;
            await orderRepository.SaveChangesAsync();
        }

        public async Task<bool> CheckBookOrderAsync(Guid bookId)
        {
            return await orderRepository
                .AllAsNoTracking()
                .Include(o => o.BookOrders)
                .AnyAsync(o => o.BookOrders.Any(bo => bo.BookId == bookId));
        }

        public async Task<bool> CheckUserOrderAsync(string userId)
        {
            return await orderRepository
                .AllAsNoTracking()
                .AnyAsync(o => o.CustomerId == userId);
        }

        public async Task AddNewBookToOrderAsync(Guid bookId, string userId)
        {
            var book = await bookService.GetBookByIdAsync(bookId);

            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            if (book.Quantity < 1)
            {
                throw new ArgumentException(GlobalExceptions.InsufficientQuantity);
            }

            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidUser);
            }

            var customerOrder = await orderRepository
                .All()
                .Where(o => o.CustomerId == userId)
                .FirstOrDefaultAsync();

            if (customerOrder == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidOrder);
            }

            customerOrder.BookOrders.Add(new BookOrder
            {
                BookId = bookId,
                OrderId = customerOrder.Id,
                Copies = 1
            });

            book.Quantity--;

            await orderRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderViewModel>> GetUserOrdersAsync(string userId)
        {
            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidUser);
            }

            var customerOrders = await orderRepository
                .AllAsNoTracking()
                .Include(o => o.BookOrders)
                .ThenInclude(o => o.Book)
                .ThenInclude(o => o.Author)
                .Where(o => o.CustomerId == userId)
                .SelectMany(o => o.BookOrders)
                .ToListAsync();

            var result = new List<OrderViewModel>();

            foreach (var order in customerOrders)
            {
                if (order.Copies > 0)
                {
                    result.Add(new OrderViewModel
                    {
                        BookId = order.BookId,
                        Title = order.Book.Title,
                        Author = order.Book.Author.Name,
                        Price = order.Book.Price,
                        ImageUrl = order.Book.ImageUrl,
                        Copies = order.Copies
                    });
                }
            }

            return result;
        }

        public async Task RemoveUserOrdersAsync(Guid bookId, string userId)
        {
            var book = await bookService.GetBookByIdAsync(bookId);

            if (book == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidBookId);
            }

            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidUser);
            }

            var bookOrder = await orderRepository
                .All()
                .Include(o => o.BookOrders)
                .Where(o => o.CustomerId == userId)
                .SelectMany(o => o.BookOrders)
                .Where(bo => bo.BookId == bookId)
                .FirstOrDefaultAsync();

            if (bookOrder == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidOrder);
            }

            if (bookOrder.Copies > 0)
            {
                bookOrder.Copies--;
            }

            book.Quantity++;

            await orderRepository.SaveChangesAsync();
        }
    }
}
