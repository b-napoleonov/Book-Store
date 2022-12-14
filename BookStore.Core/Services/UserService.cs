using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.User;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.Core.Services
{
    /// <summary>
    /// Main class who manages Users
    /// </summary>
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

        /// <summary>
        /// Gets the total count of all of given user's orders
        /// </summary>
        /// <param name="userId">ID of the user to be searched</param>
        /// <returns>Integer</returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Gets user by ID
        /// </summary>
        /// <param name="userId">ID of the user to be searched</param>
        /// <returns>ApplicationUser</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            ApplicationUser user;

            try
            {
                user = await userRepository
                .All()
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

        /// <summary>
        /// Gets data for the user profile page
        /// </summary>
        /// <param name="userId">ID of the user to be searched</param>
        /// <returns>UserProfileViewModel</returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentException"></exception>
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
                LastName = user.LastName,
                Phone = user.PhoneNumber
            };

            return model;
        }
    }
}
