using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services
{
    public class BookService : IBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;
        private readonly IDeletableEntityRepository<Author> authorRepository;
        private readonly IDeletableEntityRepository<Publisher> publisherRepository;
        private readonly IDeletableEntityRepository<Category> categoryService;
        private readonly IDeletableEntityRepository<Warehouse> warehouseService;

        public BookService(
            IDeletableEntityRepository<Book> _bookRepository,
            IDeletableEntityRepository<Author> _authorRepository,
            IDeletableEntityRepository<Publisher> _publisherRepository,
            IDeletableEntityRepository<Category> _categoryService,
            IDeletableEntityRepository<Warehouse> _warehouseService)
        {
            bookRepository = _bookRepository;
            authorRepository = _authorRepository;
            publisherRepository = _publisherRepository;
            categoryService = _categoryService;
            warehouseService = _warehouseService;
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            return await authorRepository.AllAsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<AllBooksViewModel>> GetAllBooksAsync()
        {
            return await bookRepository.AllAsNoTracking()
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

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await categoryService.AllAsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Publisher>> GetAllPublishersAsync()
        {
            return await publisherRepository.AllAsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync()
        {
            return await warehouseService.AllAsNoTracking().ToListAsync();
        }

        public async Task<DetailsBookViewModel> GetBookAsync(Guid bookId)
        {
            var book = await bookRepository
                .AllAsNoTracking()
                .Where(x => x.Id == bookId)
                .FirstOrDefaultAsync();

            if (book == null)
            {
                throw new ArgumentException("Invalid Book ID");
            }

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
    }
}
