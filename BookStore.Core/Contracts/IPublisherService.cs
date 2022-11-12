using BookStore.Core.Models.Book;

namespace BookStore.Core.Contracts
{
    public interface IPublisherService
    {
        Task<IEnumerable<BookPublisherViewModel>> GetAllPublishersAsync();
    }
}
