using BookStore.Infrastructure.Models;

namespace BookStore.Core.Contracts
{
    public interface IPublisherService
    {
        Task<IEnumerable<Publisher>> GetAllPublishersAsync();
    }
}
