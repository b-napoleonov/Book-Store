using BookStore.Core.Contracts;
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
				.Where(u => u.Id == userId)
				.FirstOrDefaultAsync();

			if (currentUser == null)
			{
                throw new ArgumentException("Invalid user.");
            }

			return currentUser.Orders.Count;
		}

		public async Task<ApplicationUser> GetUserByIdAsync(string userId)
		{
			var user = await userRepository
				.AllAsNoTracking()
				.Where(u => u.Id == userId)
				.FirstOrDefaultAsync();

			if (user == null)
			{
				throw new ArgumentException("Invalid User Id");
			}

			return user;
		}
	}
}
