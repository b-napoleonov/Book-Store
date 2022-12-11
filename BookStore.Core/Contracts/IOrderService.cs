using BookStore.Core.Models.Order;

namespace BookStore.Core.Contracts
{
	public interface IOrderService
	{
		Task<bool> CheckUserOrderAsync(string userId);

		Task<bool> CheckBookOrderAsync(Guid bookId, string userId);

		Task AddNewOrderAsync(Guid bookId, string userId);

		Task AddCopiesToOrderAsync(Guid bookId, string userId);

		Task AddNewBookToOrderAsync(Guid bookId, string userId);

        Task<IEnumerable<OrderViewModel>> GetUserOrdersAsync(string userId);

		Task RemoveUserOrdersAsync(Guid bookId, string userId);
    }
}
