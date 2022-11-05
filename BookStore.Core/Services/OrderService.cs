using BookStore.Core.Contracts;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;

namespace BookStore.Core.Services
{
	public class OrderService : IOrderService
	{
		private readonly IDeletableEntityRepository<Order> orderRepository;

		public OrderService(IDeletableEntityRepository<Order> _orderRepository)
		{
			orderRepository = _orderRepository;
		}

		public async Task AddOrderAsync(Guid bookId, string userId)
		{
			var order = new Order
			{

			};

			await orderRepository.AddAsync(order);
		}
	}
}
