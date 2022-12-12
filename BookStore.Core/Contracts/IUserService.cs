using BookStore.Core.Models.User;
using BookStore.Infrastructure.Models;

namespace BookStore.Core.Contracts
{
    /// <summary>
    /// Interface for managing Users
    /// </summary>
    public interface IUserService
	{
        Task<int> GetOrdersCountAsync(string userId);

        Task<ApplicationUser> GetUserByIdAsync(string userId);

        Task<UserProfileViewModel> GetUserProfileDataAsync(string userId);
    }
}
