using BookStore.Core.Models.Book;

namespace BookStore.Core.Contracts
{
    public interface ICategoryService
    {
        Task<IEnumerable<BookCategoryViewModel>> GetAllCategoriesAsync();
    }
}
