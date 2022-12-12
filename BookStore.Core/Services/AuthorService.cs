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
    /// Main class who manages Authors
    /// </summary>
    public class AuthorService : IAuthorService
    {
        private readonly IDeletableEntityRepository<Author> authorRepository;
        private readonly ILogger<AuthorService> logger;

        public AuthorService(
            IDeletableEntityRepository<Author> _authorRepository,
            ILogger<AuthorService> _logger)
        {
            authorRepository = _authorRepository;
            logger = _logger;
        }

        /// <summary>
        /// Gets all non-deleted authors from the DB
        /// </summary>
        /// <returns>IEnumerable of BookAuthorViewModel</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<IEnumerable<BookAuthorViewModel>> GetAllAuthorsAsync()
        {
            var authors = new List<BookAuthorViewModel>();

            try
            {
                authors = await authorRepository
                .AllAsNoTracking()
                .OrderBy(a => a.Name)
                .Select(a => new BookAuthorViewModel
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetAllAuthorsAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToFetch, ex);
            }

            return authors;
        }
    }
}
