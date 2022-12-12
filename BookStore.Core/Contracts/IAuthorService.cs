using BookStore.Core.Models.Book;

namespace BookStore.Core.Contracts
{
    /// <summary>
    /// Interface for managing Authors
    /// </summary>
    public interface IAuthorService
    {
        Task<IEnumerable<BookAuthorViewModel>> GetAllAuthorsAsync();
    }
}
