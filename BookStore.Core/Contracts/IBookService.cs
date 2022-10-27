using BookStore.Core.Models.Book;
using BookStore.Infrastructure.Models;

namespace BookStore.Core.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<AllBooksViewModel>> GetAllBooksAsync();

        Task<DetailsBookViewModel> GetBookAsync(Guid bookId);

        Task<IEnumerable<Author>> GetAllAuthorsAsync();

        Task<IEnumerable<Publisher>> GetAllPublishersAsync();

        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        Task<IEnumerable<Warehouse>> GetAllWarehousesAsync();
    }
}
