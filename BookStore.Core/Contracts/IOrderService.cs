namespace BookStore.Core.Contracts
{
	public interface IOrderService
	{
		Task AddOrderAsync(Guid bookId, string userId);
	}
}
