using BookStore.Core.Models.Book;

namespace BookStore.Core.Contracts
{
    /// <summary>
    /// Interface for managing Publishers
    /// </summary>
    public interface IPublisherService
    {
        Task<IEnumerable<BookPublisherViewModel>> GetAllPublishersAsync();
    }
}
