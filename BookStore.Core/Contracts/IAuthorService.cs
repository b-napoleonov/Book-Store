using BookStore.Core.Models.Book;

namespace BookStore.Core.Contracts
{
    public interface IAuthorService
    {
        Task<IEnumerable<BookAuthorViewModel>> GetAllAuthorsAsync();
    }
}
