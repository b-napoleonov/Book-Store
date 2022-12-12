using BookStore.Core.Models.Book;
using BookStore.Core.Models.Category;

namespace BookStore.Core.Contracts
{
    /// <summary>
    /// Interface for managing Categories
    /// </summary>
    public interface ICategoryService
    {
        Task<IEnumerable<BookCategoryViewModel>> GetAllCategoriesAsync();

        Task AddCategoryAsync(AddCategoryViewModel model);
    }
}
