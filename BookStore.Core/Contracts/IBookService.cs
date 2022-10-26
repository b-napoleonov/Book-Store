using BookStore.Core.Models.Book;

namespace BookStore.Core.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<AllBooksViewModel>> GetAllBooksAsync();
    }
}
