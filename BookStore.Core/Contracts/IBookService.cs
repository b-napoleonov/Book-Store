using BookStore.Core.Models.Book;
using BookStore.Infrastructure.Models;

namespace BookStore.Core.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<AllBooksViewModel>> GetAllBooksAsync();

        Task<DetailsBookViewModel> GetBookAsync(Guid bookId);

        Task AddBookAsync(AddBookViewModel model);
    }
}
