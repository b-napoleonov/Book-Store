using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IDeletableEntityRepository<Author> authorRepository;

        public AuthorService(IDeletableEntityRepository<Author> _authorRepository)
        {
            authorRepository = _authorRepository;
        }

        public async Task<IEnumerable<BookAuthorViewModel>> GetAllAuthorsAsync()
        {
            return await authorRepository
                .AllAsNoTracking()
                .OrderBy(a => a.Name)
                .Select(a => new BookAuthorViewModel
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .ToListAsync();
        }
    }
}
