using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.Core.Services
{
    /// <summary>
    /// Main class who manages Publishers
    /// </summary>
    public class PublisherService : IPublisherService
    {
        private readonly IDeletableEntityRepository<Publisher> publisherRepository;
        private readonly ILogger<PublisherService> logger;

        public PublisherService(
            IDeletableEntityRepository<Publisher> _publisherRepository,
            ILogger<PublisherService> _logger)
        {
            publisherRepository = _publisherRepository;
            logger = _logger;
        }

        /// <summary>
        /// Gets all non-deleted publishers from the DB
        /// </summary>
        /// <returns>IEnumerable of BookPublisherViewModel</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<IEnumerable<BookPublisherViewModel>> GetAllPublishersAsync()
        {
            var publishers = new List<BookPublisherViewModel>();

            try
            {
                publishers = await publisherRepository
                .AllAsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new BookPublisherViewModel
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetAllPublishersAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToFetch, ex);
            }

            return publishers;
        }
    }
}
