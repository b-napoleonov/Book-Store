using BookStore.Core.Models.Book;
using BookStore.Core.Models.Category;

namespace BookStore.Core.Contracts
{
    public interface ICategoryService
    {
        Task<IEnumerable<BookCategoryViewModel>> GetAllCategoriesAsync();

        Task AddCategoryAsync(AddCategoryViewModel model);
    }
}
