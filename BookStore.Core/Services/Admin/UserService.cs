using BookStore.Core.Contracts.Admin;
using BookStore.Core.Models.Admin;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services.Admin
{
    /// <summary>
    /// Main class who manages Users by Admin
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;

        public UserService(IDeletableEntityRepository<ApplicationUser> _userRepository)
        {
            userRepository = _userRepository;
        }

        /// <summary>
        /// Gets all non-deleted users from the DB
        /// </summary>
        /// <returns>IEnumerable of UserServiceModel</returns>
        public async Task<IEnumerable<UserServiceModel>> All()
        {
            return await userRepository
                .AllAsNoTracking()
                .Select(u => new UserServiceModel
                {
                    UserId = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                })
                .ToListAsync();
        }

        /// <summary>
        /// Returns user's First Name + Last Name
        /// </summary>
        /// <param name="userId">ID of the user to be searched</param>
        /// <returns>First Name + Last Name as String</returns>
        public async Task<string> UserFullName(string userId)
        {
            var user = await userRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            return $"{user?.FirstName} {user?.LastName}".Trim();
        }
    }
}
