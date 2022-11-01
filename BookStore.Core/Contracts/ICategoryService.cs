using BookStore.Infrastructure.Models;

namespace BookStore.Core.Contracts
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}
