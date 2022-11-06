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

		public OrderService(IDeletableEntityRepository<Order> _orderRepository)
		{
			orderRepository = _orderRepository;
		}

		public async Task AddNewOrderAsync(Guid bookId, string userId)
		{
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

            await orderRepository.AddAsync(order);
            await orderRepository.SaveChangesAsync();
        }

		public async Task AddCopiesToOrderAsync(Guid bookId, string userId)
		{
			var bookOrder = await orderRepository
				.All()
				.Include(o => o.BookOrders)
				.Where(o => o.CustomerId == userId)
				.SelectMany(o => o.BookOrders)
				.Where(bo => bo.BookId == bookId)
				.FirstOrDefaultAsync();

			if (bookOrder == null)
			{
				throw new ArgumentException("Invalid order.");
			}

			bookOrder.Copies++;

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
			var customerOrder = await orderRepository
				.All()
				.Where(o => o.CustomerId == userId)
				.FirstOrDefaultAsync();

			if (customerOrder == null)
			{
                throw new ArgumentException("Invalid order.");
            }

			customerOrder.BookOrders.Add(new BookOrder
			{
				BookId = bookId,
				OrderId = customerOrder.Id,
				Copies = 1
			});

            await orderRepository.SaveChangesAsync();
        }

		public async Task<IEnumerable<OrderViewModel>> GetUserOrdersAsync(string userId)
		{
			return await orderRepository
				.AllAsNoTracking()
				.Include(o => o.BookOrders)
				.ThenInclude(o => o.Book)
				.Where(o => o.CustomerId == userId)
				.Select(o => new OrderViewModel
				{
					Title = o.BookOrders.Select(bo => bo.Book.Title).First(),
					Price = o.BookOrders.Select(bo => bo.Book.Price).First(),
					ImageUrl = o.BookOrders.Select(bo => bo.Book.ImageUrl).First(),
					Copies = o.BookOrders.Select(bo => bo.Copies).First()
				})
				.ToListAsync();
		}
	}
}
