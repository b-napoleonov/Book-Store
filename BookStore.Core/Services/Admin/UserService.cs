using BookStore.Core.Contracts.Admin;
using BookStore.Core.Models.Admin;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services.Admin
{
    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;

        public UserService(IDeletableEntityRepository<ApplicationUser> _userRepository)
        {
            userRepository = _userRepository;
        }

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
                    PhoneNumber = u.PhoneNumber
                })
                .ToListAsync();
        }

        public async Task<string> UserFullName(string userId)
        {
            var user = await userRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            return $"{user?.FirstName} {user?.LastName}".Trim();
        }
    }
}
