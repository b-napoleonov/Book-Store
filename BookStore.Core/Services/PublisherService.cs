using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IDeletableEntityRepository<Publisher> publisherRepository;

        public PublisherService(IDeletableEntityRepository<Publisher> _publisherRepository)
        {
            publisherRepository = _publisherRepository;
        }

        public async Task<IEnumerable<BookPublisherViewModel>> GetAllPublishersAsync()
        {
            return await publisherRepository
                .AllAsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new BookPublisherViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
        }
    }
}
