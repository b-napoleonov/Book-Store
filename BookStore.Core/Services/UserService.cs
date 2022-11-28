using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.User;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly ILogger<UserService> logger;

        public UserService(
            IDeletableEntityRepository<ApplicationUser> _userRepository,
            ILogger<UserService> _logger)
        {
            userRepository = _userRepository;
            logger = _logger;
        }

        public async Task<int> GetOrdersCountAsync(string userId)
        {
            ApplicationUser currentUser;
            int booksOrdered = 0;

            try
            {
                currentUser = await userRepository
                .AllAsNoTracking()
                .Include(x => x.Orders)
                .ThenInclude(u => u.BookOrders)
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetOrdersCountAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToFetch, ex);
            }

            if (currentUser == null)
            {
                throw new ArgumentException(GlobalExceptions.InvalidUser);
            }

            booksOrdered = currentUser.Orders.Select(o => o.BookOrders.Sum(bo => bo.Copies)).Sum();

            return booksOrdered;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            ApplicationUser user;

            try
            {
                user = await userRepository
                .AllAsNoTracking()
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetOrdersCountAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToFetch, ex);
            }

            return user;
        }

        public async Task<UserProfileViewModel> GetUserProfileDataAsync(string userId)
        {
            ApplicationUser user;

            try
            {
                user = await userRepository
                .AllAsNoTracking()
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetOrdersCountAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToFetch, ex);
            }

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
