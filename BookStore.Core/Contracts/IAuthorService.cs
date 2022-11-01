using BookStore.Infrastructure.Models;

namespace BookStore.Core.Contracts
{
    public interface IAuthorService
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
    }
}
