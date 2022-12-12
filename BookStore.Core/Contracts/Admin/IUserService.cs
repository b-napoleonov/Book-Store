using BookStore.Core.Models.Admin;

namespace BookStore.Core.Contracts.Admin
{
    /// <summary>
    /// Interface for managing Users by Admin
    /// </summary>
    public interface IUserService
    {
        Task<string> UserFullName(string userId);

        Task<IEnumerable<UserServiceModel>> All();
    }
}
