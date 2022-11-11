using BookStore.Core.Models.Book;
using BookStore.Infrastructure.Models;

namespace BookStore.Core.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<AllBooksViewModel>> GetAllBooksAsync();

        Task<DetailsBookViewModel> GetBookAsync(Guid bookId);

        Task AddBookAsync(AddBookViewModel model);

        Task<IEnumerable<AllBooksViewModel>> GetBooksByCategoryAsync(string categoryName);

        Task<IEnumerable<AllBooksViewModel>> GetBooksByAuthorAsync(string authorName);

        Task<IEnumerable<AllBooksViewModel>> GetBooksByPublisherAsync(string publisherName);

        Task<Book> GetBookByIdAsync(Guid bookId);

        Task<double> GetBookRating(Guid bookId);
    }
}
