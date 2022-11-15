using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.User;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;

        public UserService(IDeletableEntityRepository<ApplicationUser> _userRepository)
        {
            userRepository = _userRepository;
        }

        public async Task<int> GetOrdersCountAsync(string userId)
        {
            var currentUser = await userRepository
                .AllAsNoTracking()
                .Include(x => x.Orders)
                .ThenInclude(u => u.BookOrders)
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (currentUser == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidUser);
            }

            var booksOrdered = currentUser.Orders.Select(o => o.BookOrders.Sum(bo => bo.Copies)).Sum();

            return booksOrdered;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            var user = await userRepository
                .AllAsNoTracking()
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidUser);
            }

            return user;
        }

        public async Task<UserProfileViewModel> GetUserProfileDataAsync(string userId)
        {
            var user = await userRepository
                .AllAsNoTracking()
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidUser);
            }

            var model = new UserProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return model;
        }
    }
}
