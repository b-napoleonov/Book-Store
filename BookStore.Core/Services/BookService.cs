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
                    Author = b.Author.Name,
                    Price = b.Price,
                })
                .ToListAsync();
        }

        public async Task<DetailsBookViewModel> GetBookAsync(Guid bookId)
        {
            var book = await repo.GetByIdAsync(bookId);

            if (book != null)
            {
                return new DetailsBookViewModel()
                {
                    Id = book.Id,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    Description = book.Description,
                    Year = book.Year,
                    Price = book.Price,
                    Pages = book.Pages,
                    ImageUrl = book.ImageUrl,
                    Author = book.Author.Name,
                    Publisher = book.Publisher.Name,
                    Categories = book.Categories.Select(c => c.Category.Name).ToList()
                };
            }

            throw new ArgumentException();
        }
    }
}
