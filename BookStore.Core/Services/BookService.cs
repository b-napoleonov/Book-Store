using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services
{
    public class BookService : IBookService
    {
        private readonly IDeletableEntityRepository<Book> repo;

        public BookService(IDeletableEntityRepository<Book> _repo)
        {
            repo = _repo;
        }

        public async Task<IEnumerable<AllBooksViewModel>> GetAllBooksAsync()
        {
            return await repo.AllAsNoTracking()
                .Select(b => new AllBooksViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    ImageUrl = b.ImageUrl,
                    Author = b.Author.FirstName + b.Author.LastName,
                    Price = b.Price,
                })
                .ToListAsync();
        }
    }
}
