namespace BookStore.Core.Contracts
{
    public interface IUserService
	{
        Task<int> GetOrdersCountAsync(string userId);
    }
}
