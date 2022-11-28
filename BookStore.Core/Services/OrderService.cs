﻿using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Order;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BookStore.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly IDeletableEntityRepository<Order> orderRepository;
        private readonly IBookService bookService;
        private readonly IUserService userService;
        private readonly ILogger<OrderService> logger;

        public OrderService(
            IDeletableEntityRepository<Order> _orderRepository,
            IBookService _bookService,
            IUserService _userService,
            ILogger<OrderService> _logger)
        {
            orderRepository = _orderRepository;
            bookService = _bookService;
            userService = _userService;
            logger = _logger;
        }

        public async Task AddNewOrderAsync(Guid bookId, string userId)
        {
            var book = await bookService.GetBookByIdAsync(bookId);

            if (book == null)
            {
                throw new NullReferenceException(GlobalExceptions.InvalidBookId);
            }

            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new NullReferenceException(GlobalExceptions.InvalidUser);
            }

            if (book.Quantity < 1)
            {
                throw new ArgumentException(GlobalExceptions.InsufficientQuantity);
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

            try
            {
                await orderRepository.AddAsync(order);
                await orderRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddNewOrderAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }
        }

        public async Task AddCopiesToOrderAsync(Guid bookId, string userId)
        {
            var book = await bookService.GetBookByIdAsync(bookId);

            if (book == null)
            {
                throw new NullReferenceException(GlobalExceptions.InvalidBookId);
            }

            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new NullReferenceException(GlobalExceptions.InvalidUser);
            }

            if (book.Quantity < 1)
            {
                throw new ArgumentException(GlobalExceptions.InsufficientQuantity);
            }

            BookOrder bookOrder;

            try
            {
                bookOrder = await orderRepository
                    .All()
                    .Include(o => o.BookOrders)
                    .Where(o => o.CustomerId == userId)
                    .SelectMany(o => o.BookOrders)
                    .Where(bo => bo.BookId == bookId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddCopiesToOrderAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }

            if (bookOrder == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidOrder);
            }

            bookOrder.Copies++;
            book.Quantity--;

            try
            {
                await orderRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddCopiesToOrderAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }
        }

        public async Task<bool> CheckBookOrderAsync(Guid bookId)
        {
            var result = false;

            try
            {
                result = await orderRepository
                    .AllAsNoTracking()
                    .Include(o => o.BookOrders)
                    .AnyAsync(o => o.BookOrders.Any(bo => bo.BookId == bookId));
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(CheckBookOrderAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }

            return result;
        }

        public async Task<bool> CheckUserOrderAsync(string userId)
        {
            var result = false;

            try
            {
                result = await orderRepository
                    .AllAsNoTracking()
                    .AnyAsync(o => o.CustomerId == userId);
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(CheckUserOrderAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }

            return result;

        }

        public async Task AddNewBookToOrderAsync(Guid bookId, string userId)
        {
            var book = await bookService.GetBookByIdAsync(bookId);

            if (book == null)
            {
                throw new NullReferenceException(GlobalExceptions.InvalidBookId);
            }

            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new NullReferenceException(GlobalExceptions.InvalidUser);
            }

            if (book.Quantity < 1)
            {
                throw new ArgumentException(GlobalExceptions.InsufficientQuantity);
            }

            Order customerOrder;

            try
            {
                customerOrder = await orderRepository
                    .All()
                    .Where(o => o.CustomerId == userId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddNewBookToOrderAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


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

            try
            {
                await orderRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddNewBookToOrderAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }
        }

        public async Task<IEnumerable<OrderViewModel>> GetUserOrdersAsync(string userId)
        {
            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new NullReferenceException(GlobalExceptions.InvalidUser);
            }

            List<BookOrder> customerOrders = new List<BookOrder>();

            try
            {
                customerOrders = await orderRepository
                    .AllAsNoTracking()
                    .Include(o => o.BookOrders)
                    .ThenInclude(o => o.Book)
                    .ThenInclude(o => o.Author)
                    .Where(o => o.CustomerId == userId)
                    .SelectMany(o => o.BookOrders)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetUserOrdersAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


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
                throw new NullReferenceException(GlobalExceptions.InvalidBookId);
            }

            var user = await userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new NullReferenceException(GlobalExceptions.InvalidUser);
            }

            BookOrder bookOrder;

            try
            {
                bookOrder = await orderRepository
                    .All()
                    .Include(o => o.BookOrders)
                    .Where(o => o.CustomerId == userId)
                    .SelectMany(o => o.BookOrders)
                    .Where(bo => bo.BookId == bookId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(RemoveUserOrdersAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }


            if (bookOrder == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidOrder);
            }

            if (bookOrder.Copies > 0)
            {
                bookOrder.Copies--;
            }

            book.Quantity++;

            try
            {
                await orderRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(RemoveUserOrdersAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }
        }
    }
}
